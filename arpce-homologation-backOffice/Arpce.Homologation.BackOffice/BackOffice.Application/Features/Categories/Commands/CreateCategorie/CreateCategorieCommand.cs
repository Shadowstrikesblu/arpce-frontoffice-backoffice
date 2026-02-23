using BackOffice.Application.Common.DTOs;
using BackOffice.Domain.Entities;
using MediatR;
using System;

namespace BackOffice.Application.Features.Categories.Commands.CreateCategorie;

public class CreateCategorieCommand : IRequest<CategorieEquipementDto>
{
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string TypeClient { get; set; } = string.Empty;
    public string TypeEquipement { get; set; } = string.Empty;

    public decimal FraisEtude { get; set; }
    public decimal FraisHomologation { get; set; }
    public decimal? FraisControle { get; set; }

    public ModeCalcul ModeCalcul { get; set; }
    public int? BlockSize { get; set; }
    public int? QtyMin { get; set; }
    public int? QtyMax { get; set; }
    public string? ReferenceLoiFinance { get; set; }
    public string? Remarques { get; set; }
}