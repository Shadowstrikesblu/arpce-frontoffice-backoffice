//using FrontOffice.Application.Common; // Pour FromUnixTimeMilliseconds
//using FrontOffice.Application.Common.Interfaces;
//using FrontOffice.Domain.Entities;
//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using QuestPDF.Fluent;
//using QuestPDF.Helpers;
//using QuestPDF.Infrastructure;
//using System;
//using System.IO;
//using System.Threading;
//using System.Threading.Tasks;

//namespace FrontOffice.Application.Features.Devis.Queries.DownloadDevisPdf;

//// DTO pour le retour
//public class DevisPdfResult
//{
//    public byte[] FileContents { get; set; } = Array.Empty<byte>();
//    public string FileName { get; set; } = "devis.pdf";
//}

//// La Requête
//public class DownloadDevisPdfQuery : IRequest<DevisPdfResult>
//{
//    public Guid DevisId { get; set; }
//}

//// Le Handler
//public class DownloadDevisPdfQueryHandler : IRequestHandler<DownloadDevisPdfQuery, DevisPdfResult>
//{
//    private readonly IApplicationDbContext _context;
//    private readonly ICurrentUserService _currentUserService;

//    public DownloadDevisPdfQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
//    {
//        _context = context;
//        _currentUserService = currentUserService;
//    }

//    public async Task<DevisPdfResult> Handle(DownloadDevisPdfQuery request, CancellationToken cancellationToken)
//    {
//        var userId = _currentUserService.UserId;
//        if (!userId.HasValue) throw new UnauthorizedAccessException();

//        var devis = await _context.Devis
//            .AsNoTracking()
//            .Include(d => d.Dossier)
//            .FirstOrDefaultAsync(d => d.Id == request.DevisId, cancellationToken);

//        if (devis == null)
//        {
//            throw new FileNotFoundException("Devis introuvable.");
//        }

//        if (devis.Dossier.IdClient != userId.Value)
//        {
//            throw new UnauthorizedAccessException("Vous n'êtes pas autorisé à télécharger ce devis.");
//        }

//        QuestPDF.Settings.License = LicenseType.Community;
//        var pdfBytes = Document.Create(container =>
//        {
//            container.Page(page =>
//            {
//                page.Size(PageSizes.A4);
//                page.Margin(2, Unit.Centimetre);
//                page.DefaultTextStyle(x => x.FontSize(12));

//                page.Header().AlignCenter().Text("DEVIS D'HOMOLOGATION").Bold().FontSize(20).PaddingBottom(1, Unit.Centimetre);

//                page.Content().Column(col =>
//                {
//                    col.Item().Text($"Devis N° : {devis.Id.ToString().Substring(0, 8)}");
//                    col.Item().Text($"Date : {devis.Date.FromUnixTimeMilliseconds():dd/MM/yyyy}");
//                    col.Item().Text($"Dossier : {devis.Dossier.Numero} - {devis.Dossier.Libelle}");

//                    col.Item().PaddingTop(1, Unit.Centimetre).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

//                    col.Item().PaddingTop(1, Unit.Centimetre).Table(table =>
//                    {
//                        table.ColumnsDefinition(columns => { columns.RelativeColumn(); columns.ConstantColumn(100); });
//                        table.Header(header => { header.Cell().Text("Description"); header.Cell().AlignRight().Text("Montant (XAF)"); });
//                        table.Cell().Text("Frais d'étude de dossier");
//                        table.Cell().AlignRight().Text($"{devis.MontantEtude:N0}");
//                        table.Cell().Text("Frais d'homologation");
//                        table.Cell().AlignRight().Text($"{devis.MontantHomologation:N0}");
//                        table.Cell().Text("Frais de contrôle");
//                        table.Cell().AlignRight().Text($"{devis.MontantControle:N0}");
//                        var total = devis.MontantEtude + (devis.MontantHomologation ?? 0) + (devis.MontantControle ?? 0);
//                        table.Cell().ColumnSpan(2).PaddingTop(10).LineHorizontal(1);
//                        table.Cell().Bold().Text("TOTAL À PAYER");
//                        table.Cell().AlignRight().Bold().Text($"{total:N0}");
//                    });
//                });
//            });
//        }).GeneratePdf();

//        return new DevisPdfResult
//        {
//            FileContents = pdfBytes,
//            FileName = $"Devis_{devis.Dossier.Numero}.pdf"
//        };
//    }
//}