using BackOffice.Domain.Entities;
using System;

namespace BackOffice.Application.Common.DTOs;

public class CategorieEquipementDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string TypeEquipement { get; set; } = string.Empty;
    public string TypeClient { get; set; } = string.Empty;

    // Frais financiers
    public decimal? FraisEtude { get; set; }
    public decimal? FraisHomologation { get; set; }
    public decimal? FraisControle { get; set; }

    // Champs de configuration 
    public string? FormuleHomologation { get; set; }
    public int? QuantiteReference { get; set; }
    public string? Remarques { get; set; }

    // Moteur de calcul Loi de Finance
    public ModeCalcul ModeCalcul { get; set; }
    public int? BlockSize { get; set; }
    public int? QtyMin { get; set; }
    public int? QtyMax { get; set; }
    public string? ReferenceLoiFinance { get; set; }
}