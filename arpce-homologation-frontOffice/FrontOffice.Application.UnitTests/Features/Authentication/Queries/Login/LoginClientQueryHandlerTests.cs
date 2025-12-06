using FluentAssertions;
using FrontOffice.Application.Common.Exceptions;
using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Application.Features.Authentication.Queries.Login;
using FrontOffice.Domain.Entities;
using MockQueryable.Moq; 
using Moq;

namespace FrontOffice.Application.UnitTests.Features.Authentication.Queries.Login;

public class LoginClientQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly Mock<IJwtTokenGenerator> _mockJwtGenerator;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly LoginClientQueryHandler _handler;

    public LoginClientQueryHandlerTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _mockJwtGenerator = new Mock<IJwtTokenGenerator>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();

        _handler = new LoginClientQueryHandler(
            _mockContext.Object,
            _mockJwtGenerator.Object,
            _mockPasswordHasher.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCredentialsAreValidAndAccountIsFullyValidated()
    {
        // ARRANGE
        var query = new LoginClientQuery { Email = "test@valide.com", Password = "goodpassword" };
        var client = new Client
        {
            Id = Guid.NewGuid(),
            Email = query.Email,
            MotPasse = "hashed_password",
            IsVerified = true,
            NiveauValidation = 2
        };

        // Crée une liste contenant notre client de test
        var clientsList = new List<Client> { client };
        // Transforme cette liste en un DbSet mocké qui supporte les requêtes asynchrones
        var mockClientDbSet = clientsList.AsQueryable().BuildMockDbSet();

        // Configure le DbContext pour qu'il retourne ce DbSet mocké
        _mockContext.Setup(c => c.Clients).Returns(mockClientDbSet.Object);

        _mockPasswordHasher.Setup(p => p.Verify(query.Password, client.MotPasse)).Returns(true);
        _mockJwtGenerator.Setup(g => g.GenerateToken(client.Id, client.Email!)).Returns("fake_jwt_token");

        // ACT
        var result = await _handler.Handle(query, CancellationToken.None);

        // ASSERT
        result.Should().NotBeNull();
        result.Message.Should().Be("Connexion réussie.");
        result.Token.Should().Be("fake_jwt_token");
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenPasswordIsInvalid()
    {
        // ARRANGE
        var query = new LoginClientQuery { Email = "test@test.com", Password = "wrong_password" };
        var client = new Client { Id = Guid.NewGuid(), Email = query.Email, MotPasse = "hashed_password", NiveauValidation = 2 };

        var clientsList = new List<Client> { client };
        var mockClientDbSet = clientsList.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(c => c.Clients).Returns(mockClientDbSet.Object);

        _mockPasswordHasher.Setup(p => p.Verify(query.Password, client.MotPasse)).Returns(false);

        // ACT & ASSERT
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenAccountIsNotVerifiedByOtp()
    {
        // ARRANGE
        var query = new LoginClientQuery { Email = "test@test.com", Password = "goodpassword" };
        var client = new Client { Id = Guid.NewGuid(), Email = query.Email, MotPasse = "hashed_password", NiveauValidation = 0 }; // Niveau 0

        var clientsList = new List<Client> { client };
        var mockClientDbSet = clientsList.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(c => c.Clients).Returns(mockClientDbSet.Object);

        _mockPasswordHasher.Setup(p => p.Verify(query.Password, client.MotPasse)).Returns(true);

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(query, CancellationToken.None));
        exception.Message.Should().Contain("n'a pas encore été vérifiée");
    }

    [Fact]
    public async Task Handle_ShouldThrowAccountPendingValidationException_WhenAccountIsPendingArpceValidation()
    {
        // ARRANGE
        var query = new LoginClientQuery { Email = "test@test.com", Password = "goodpassword" };
        var client = new Client { Id = Guid.NewGuid(), Email = query.Email, MotPasse = "hashed_password", NiveauValidation = 1 }; // Niveau 1

        var clientsList = new List<Client> { client };
        var mockClientDbSet = clientsList.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(c => c.Clients).Returns(mockClientDbSet.Object);

        _mockPasswordHasher.Setup(p => p.Verify(query.Password, client.MotPasse)).Returns(true);

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<AccountPendingValidationException>(() => _handler.Handle(query, CancellationToken.None));
        exception.Message.Should().Contain("en attente de validation");
    }
}