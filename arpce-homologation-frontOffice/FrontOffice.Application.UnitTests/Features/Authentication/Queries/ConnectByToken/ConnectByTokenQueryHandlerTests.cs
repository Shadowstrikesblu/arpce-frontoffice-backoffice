using FluentAssertions;
using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Application.Features.Authentication.Queries.ConnectByToken;
using FrontOffice.Domain.Entities;
using MockQueryable.Moq; 
using Moq;

namespace FrontOffice.Application.UnitTests.Features.Authentication.Queries.ConnectByToken;

public class ConnectByTokenQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly ConnectByTokenQueryHandler _handler;

    public ConnectByTokenQueryHandlerTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();

        _handler = new ConnectByTokenQueryHandler(
            _mockContext.Object,
            _mockCurrentUserService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserInfo_WhenTokenIsValidAndUserExists()
    {
        // ARRANGE
        var query = new ConnectByTokenQuery();
        var userId = Guid.NewGuid();

        var client = new Client
        {
            Id = userId,
            Email = "connected@user.com",
            RaisonSociale = "Connected Corp",
            ContactNom = "Jane Doe"
        };

        // Simule la récupération de l'ID depuis le token
        _mockCurrentUserService.Setup(s => s.UserId).Returns(userId);

        // Simule la recherche de l'utilisateur en base de données
        var clientsList = new List<Client> { client };
        var mockClientDbSet = clientsList.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(c => c.Clients).Returns(mockClientDbSet.Object);

        // ACT
        var result = await _handler.Handle(query, CancellationToken.None);

        // ASSERT
        result.Should().NotBeNull();
        result.Message.Should().Be("Session validée avec succès.");
        result.UserId.Should().Be(userId);
        result.Email.Should().Be(client.Email);
        result.RaisonSociale.Should().Be(client.RaisonSociale);
        result.ContactNom.Should().Be(client.ContactNom);
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenUserFromTokenDoesNotExist()
    {
        // ARRANGE
        var query = new ConnectByTokenQuery();
        var userIdFromToken = Guid.NewGuid();

        // Simuler la récupération de l'ID depuis le token
        _mockCurrentUserService.Setup(s => s.UserId).Returns(userIdFromToken);

        // Simuler que l'utilisateur n'est PAS trouvé en base de données
        var clientsList = new List<Client>(); // Liste vide
        var mockClientDbSet = clientsList.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(c => c.Clients).Returns(mockClientDbSet.Object);

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(query, CancellationToken.None));
        exception.Message.Should().Contain("n'existe plus");
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenTokenIsInvalid()
    {
        // ARRANGE
        var query = new ConnectByTokenQuery();

        // Simule un token invalide (pas d'ID utilisateur)
        _mockCurrentUserService.Setup(s => s.UserId).Returns((Guid?)null);

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(query, CancellationToken.None));
        exception.Message.Should().Contain("Token invalide");
    }
}