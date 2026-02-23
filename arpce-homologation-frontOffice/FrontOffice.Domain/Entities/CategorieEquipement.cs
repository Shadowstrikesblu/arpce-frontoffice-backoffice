using FrontOffice.Domain.Common;

namespace FrontOffice.Domain.Entities;

public enum ModeCalcul { FIXED, PER_UNIT, PER_BLOCK }

public class CategorieEquipement : AuditableEntity
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string TypeEquipement { get; set; } = string.Empty;

    public string TypeClient { get; set; } = string.Empty;

    public string? FormuleHomologation { get; set; }

    public decimal? FraisEtude { get; set; }
    public decimal? FraisHomologation { get; set; }
    public decimal? FraisControle { get; set; }

    public byte? FraisHomologationParLot { get; set; }
    public int? FraisHomologationQuantiteParLot { get; set; }
    public int? QuantiteReference { get; set; }

    public decimal CoutUnitaire { get; set; }
    public bool EstCalculeParQuantite { get; set; }
    public string TypeCalcul { get; set; } = "FORFAIT";
    public string? ReferenceLoiFinance { get; set; }

    public int? QtyMin { get; set; }
    public int? QtyMax { get; set; }
    public ModeCalcul ModeCalcul { get; set; } = ModeCalcul.FIXED;
    public int? BlockSize { get; set; }

    public string? Remarques { get; set; }
}