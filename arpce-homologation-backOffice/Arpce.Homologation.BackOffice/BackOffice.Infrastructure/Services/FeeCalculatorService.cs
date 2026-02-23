using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using System;

namespace BackOffice.Infrastructure.Services;

public class FeeCalculatorService : IFeeCalculatorService
{
    public decimal CalculateTotal(CategorieEquipement cat, int quantity)
    {
        decimal study = cat.FraisEtude ?? 0m;
        decimal control = cat.FraisControle ?? 0m;
        decimal homologation = 0m;

        switch (cat.ModeCalcul)
        {
            case ModeCalcul.PER_BLOCK:
                int blockSize = cat.BlockSize ?? 50;
                decimal blocks = Math.Ceiling((decimal)quantity / blockSize);
                homologation = (cat.FraisHomologation ?? 0m) * blocks;
                break;

            case ModeCalcul.PER_UNIT:
                homologation = (cat.FraisHomologation ?? 0m) * quantity;
                break;

            case ModeCalcul.FIXED:
            default:
                homologation = cat.FraisHomologation ?? 0m;
                break;
        }

        return study + homologation + control;
    }
}