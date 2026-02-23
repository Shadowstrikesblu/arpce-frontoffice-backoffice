using BackOffice.Domain.Entities;

public interface IFeeCalculatorService
{
    decimal CalculateTotal(CategorieEquipement cat, int quantity);
}