using FluentAssertions;
using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Application.Features.Authentication.Commands.ConfirmAccount;
using FrontOffice.Domain.Entities;
using Moq;

namespace FrontOffice.Application.UnitTests.Features.Authentication.Commands.ConfirmAccount;

public class ConfirmAccountCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;

    private readonly ConfirmAccountCommandHandler _handler;

    public ConfirmAccountCommandHandlerTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();

        _handler = new ConfirmAccountCommandHandler(
            _mockContext.Object,
            _mockCurrentUserService.Object);
    }

    [Fact]
    public async Task Handle_ShouldSetNiveauValidationTo1_WhenCodeIsValidAndTokenNotExpired()
    {
        // ARRANGE
        var command = new ConfirmAccountCommand { Code = "123456" };
        var userId = Guid.NewGuid();

        var client = new Client
        {
            Id = userId,
            IsVerified = false,
            NiveauValidation = 0,
            VerificationCode = "123456",
            VerificationTokenExpiry = DateTime.UtcNow.AddMinutes(10) // Le token est valide
        };

        // Simule la récupération de l'utilisateur depuis le token
        _mockCurrentUserService.Setup(s => s.UserId).Returns(userId);
        // Simule la récupération de l'entité depuis la base de données
        _mockContext.Setup(c => c.Clients.FindAsync(new object[] { userId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        client.NiveauValidation.Should().Be(1); // Le niveau doit passer à 1
        client.IsVerified.Should().Be(true);
        client.VerificationCode.Should().BeNull(); 
        result.Token.Should().BeEmpty(); 
        result.Message.Should().Contain("en attente de validation administrative");

        // Vérifie que SaveChanges a été appelé
        _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenCodeIsIncorrect()
    {
        // ARRANGE
        var command = new ConfirmAccountCommand { Code = "654321" }; 
        var userId = Guid.NewGuid();

        var client = new Client
        {
            Id = userId,
            NiveauValidation = 0,
            VerificationCode = "123456",
            VerificationTokenExpiry = DateTime.UtcNow.AddMinutes(10)
        };

        _mockCurrentUserService.Setup(s => s.UserId).Returns(userId);
        _mockContext.Setup(c => c.Clients.FindAsync(new object[] { userId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Be("Le code de vérification est incorrect.");
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenTokenIsExpired()
    {
        // ARRANGE
        var command = new ConfirmAccountCommand { Code = "123456" };
        var userId = Guid.NewGuid();

        var client = new Client
        {
            Id = userId,
            NiveauValidation = 0,
            VerificationCode = "123456",
            VerificationTokenExpiry = DateTime.UtcNow.AddMinutes(-10) 
        };

        _mockCurrentUserService.Setup(s => s.UserId).Returns(userId);
        _mockContext.Setup(c => c.Clients.FindAsync(new object[] { userId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Contain("expiré");
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenAccountIsAlreadyVerified()
    {
        // ARRANGE
        var command = new ConfirmAccountCommand { Code = "123456" };
        var userId = Guid.NewGuid();

        var client = new Client
        {
            Id = userId,
            NiveauValidation = 1, 
            IsVerified = true,
            VerificationCode = "123456",
            VerificationTokenExpiry = DateTime.UtcNow.AddMinutes(10)
        };

        _mockCurrentUserService.Setup(s => s.UserId).Returns(userId);
        _mockContext.Setup(c => c.Clients.FindAsync(new object[] { userId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Be("Ce compte a déjà été vérifié par e-mail.");
    }
}