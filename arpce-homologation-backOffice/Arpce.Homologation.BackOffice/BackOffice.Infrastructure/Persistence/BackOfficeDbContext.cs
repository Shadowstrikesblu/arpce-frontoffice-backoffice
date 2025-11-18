using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BackOffice.Infrastructure.Persistence;

public class BackOfficeDbContext : DbContext, IApplicationDbContext
{
    public BackOfficeDbContext(DbContextOptions<BackOfficeDbContext> options)
        : base(options)
    {
    }

    public DbSet<AdminUtilisateur> AdminUtilisateurs => Set<AdminUtilisateur>();
    public DbSet<AdminProfil> AdminProfils => Set<AdminProfil>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Applique toutes les configurations d'entités depuis l'assembly actuel
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}