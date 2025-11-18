using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BackOffice.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<AdminUtilisateur> AdminUtilisateurs { get; }
    DbSet<AdminProfil> AdminProfils { get; }

    // Nous ajouterons les autres DbSet (Dossier, Demande, etc.) plus tard

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}