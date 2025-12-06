using FluentAssertions;
using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Application.Features.Authentication.Commands.Register;
using FrontOffice.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace FrontOffice.Application.UnitTests.Features.Authentication.Commands.Register;

public class RegisterClientCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly Mock<IJwtTokenGenerator> _mockJwtGenerator;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<Microsoft.Extensions.Configuration.IConfiguration> _mockConfiguration;
    private readonly Mock<Microsoft.Extensions.Logging.ILogger<RegisterClientCommandHandler>> _mockLogger;

    private readonly RegisterClientCommandHandler _handler;

    public RegisterClientCommandHandlerTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _mockJwtGenerator = new Mock<IJwtTokenGenerator>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockEmailService = new Mock<IEmailService>();
        _mockConfiguration = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<RegisterClientCommandHandler>>();

        _handler = new RegisterClientCommandHandler(
            _mockContext.Object,
            _mockJwtGenerator.Object,
            _mockPasswordHasher.Object,
            _mockEmailService.Object,
            _mockConfiguration.Object,
            _mockLogger.Object,
            new Mock<ICaptchaValidator>().Object // On peut ignorer le captcha pour ces tests
        );
    }

    [Fact]
    public async Task Handle_ShouldCreateNewUser_WhenEmailDoesNotExist()
    {
        // ARRANGE
        var command = new RegisterClientCommand { Email = "newuser@test.com", Password = "password" };
        var clientsList = new List<Client>(); // La liste de clients est vide
        var mockClientDbSet = clientsList.AsQueryable().BuildMockDbSet();

        _mockContext.Setup(c => c.Clients).Returns(mockClientDbSet.Object);
        _mockPasswordHasher.Setup(p => p.Hash(command.Password)).Returns("hashed_password");
        _mockJwtGenerator.Setup(g => g.GenerateToken(It.IsAny<Guid>(), command.Email)).Returns("fake_verification_token");

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        result.Should().NotBeNull();
        result.Token.Should().Be("fake_verification_token");
        result.Message.Should().Contain("Inscription enregistrée");

        // Vérifie que le client a été ajouté à la DB et qu'un e-mail a été envoyé
        _mockContext.Verify(c => c.Clients.Add(It.IsAny<Client>()), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        _mockEmailService.Verify(e => e.SendEmailAsync(command.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenVerifiedUserExists()
    {
        // ARRANGE
        var command = new RegisterClientCommand { Email = "existing@test.com", Password = "password" };
        var existingClient = new Client { Email = command.Email, IsVerified = true, NiveauValidation = 1 };

        var clientsList = new List<Client> { existingClient };
        var mockClientDbSet = clientsList.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(c => c.Clients).Returns(mockClientDbSet.Object);

        // ACT & ASSERT
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldResendOtp_WhenUnverifiedUserExists()
    {
        // ARRANGE
        var command = new RegisterClientCommand { Email = "unverified@test.com", Password = "password" };
        var unverifiedClient = new Client { Id = Guid.NewGuid(), Email = command.Email, IsVerified = false, NiveauValidation = 0 };

        var clientsList = new List<Client> { unverifiedClient };
        var mockClientDbSet = clientsList.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(c => c.Clients).Returns(mockClientDbSet.Object);

        _mockJwtGenerator.Setup(g => g.GenerateToken(unverifiedClient.Id, unverifiedClient.Email!)).Returns("new_verification_token");

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        result.Should().NotBeNull();
        result.Token.Should().Be("new_verification_token");
        result.Message.Should().Contain("Un nouveau code de confirmation a été envoyé");

        // Vérifie que la sauvegarde a été appelée (pour le nouveau code) et que l'email a été renvoyé
        _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        _mockEmailService.Verify(e => e.SendEmailAsync(unverifiedClient.Email!, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
}