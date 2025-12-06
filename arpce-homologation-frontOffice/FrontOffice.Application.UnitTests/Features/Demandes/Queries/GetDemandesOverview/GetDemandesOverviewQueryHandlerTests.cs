using FluentAssertions;
using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Application.Features.Demandes.Queries.GetDemandesOverview;
using FrontOffice.Domain.Entities;
using MockQueryable.Moq; 
using Moq;

namespace FrontOffice.Application.UnitTests.Features.Demandes.Queries.GetDemandesOverview;

public class GetDemandesOverviewQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly GetDemandesOverviewQueryHandler _handler;

    public GetDemandesOverviewQueryHandlerTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();

        _handler = new GetDemandesOverviewQueryHandler(_mockContext.Object, _mockCurrentUserService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectCounts_OnlyForCurrentUserDossiers()
    {
        // ARRANGE
        var query = new GetDemandesOverviewQuery();
        var currentUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid(); // Un autre utilisateur pour s'assurer que ses dossiers sont ignorés

        // Définir les statuts dont on a besoin pour le test
        var statutSigned = new Statut { Id = Guid.NewGuid(), Code = "DossierSigne" };
        var statutDenied = new Statut { Id = Guid.NewGuid(), Code = "RefusDossier" }; // Ou Denied/Rejetee selon votre enum final
        var statutReview = new Statut { Id = Guid.NewGuid(), Code = "Instruction" };
        var statutNew = new Statut { Id = Guid.NewGuid(), Code = "NouveauDossier" };
        var statutUnpaid = new Statut { Id = Guid.NewGuid(), Code = "DevisPaiement" };

        // Créer une liste de dossiers
        var dossiersList = new List<Dossier>
        {
            // Dossiers de l'utilisateur connecté
            new Dossier { Id = Guid.NewGuid(), IdClient = currentUserId, Statut = statutSigned, Devis = new List<Devis>() },
            new Dossier { Id = Guid.NewGuid(), IdClient = currentUserId, Statut = statutSigned, Devis = new List<Devis>() },
            new Dossier { Id = Guid.NewGuid(), IdClient = currentUserId, Statut = statutDenied, Devis = new List<Devis>() },
            new Dossier { Id = Guid.NewGuid(), IdClient = currentUserId, Statut = statutReview, Devis = new List<Devis>() },
            new Dossier { Id = Guid.NewGuid(), IdClient = currentUserId, Statut = statutNew, Devis = new List<Devis>() },
            new Dossier { Id = Guid.NewGuid(), IdClient = currentUserId, Statut = statutUnpaid, Devis = new List<Devis> { new Devis { PaiementOk = 0 } } }, // En attente paiement

            // Dossier d'un autre utilisateur (doit être ignoré)
            new Dossier { Id = Guid.NewGuid(), IdClient = otherUserId, Statut = statutSigned, Devis = new List<Devis>() }
        };

        // Créer le DbSet mocké
        var mockDossierDbSet = dossiersList.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(c => c.Dossiers).Returns(mockDossierDbSet.Object);

        // Simuler l'utilisateur connecté
        _mockCurrentUserService.Setup(s => s.UserId).Returns(currentUserId);

        // ACT
        var result = await _handler.Handle(query, CancellationToken.None);

        // ASSERT
        result.Should().NotBeNull();

        // L'utilisateur courant a 6 dossiers au total
        result.Total.Should().Be(6);

        // 2 dossiers avec le statut "Signed"
        result.Success.Should().Be(2);

        // 1 dossier avec le statut de refus
        result.Failed.Should().Be(1);

        // 2 dossiers avec des statuts "en cours" (Review, New)
        result.InProgress.Should().Be(2);

        // 1 dossier en attente de paiement
        result.PendingPayments.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthenticated()
    {
        // ARRANGE
        var query = new GetDemandesOverviewQuery();
        // L'utilisateur n'est pas connecté
        _mockCurrentUserService.Setup(s => s.UserId).Returns((Guid?)null);

        // ACT & ASSERT
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(query, CancellationToken.None));
    }
}