using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ReservasCanchas_Web.Servicios.Interfaces;
using ReservasCanchas_Web.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using ClosedXML.Excel;
using System.Text;
using System.Net;

namespace ReservasCanchas_Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReservaService _reservaService;

        public ReportsController(IReservaService reservaService)
        {
            _reservaService = reservaService;
        }

        // ---------------------------------------------------------
        // 1. REPORTE PDF (QuestPDF)
        // ---------------------------------------------------------
        [HttpGet("reservas/por-cancha/pdf")]
        public async Task<IActionResult> GetReservasPorCanchaPdf()
        {
            var reservas = (await _reservaService.GetAllAsync()).ToList();
            var groups = reservas.GroupBy(r => r.Cancha?.Nombre ?? "Sin cancha")
                                 .OrderBy(g => g.Key)
                                 .ToList();

            using var ms = new MemoryStream();
            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(25);
                    page.PageColor(Colors.White);
                    // CORRECCIÓN 1: Usamos Fonts.Arial en lugar de Helvetica para evitar error CS0117
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                    // --- HEADER ---
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Sistema de Reservas").Bold().FontSize(20).FontColor(Colors.Blue.Medium);
                            col.Item().Text($"Reporte General por Cancha").FontSize(14).FontColor(Colors.Grey.Darken2);
                            col.Item().Text($"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10).FontColor(Colors.Grey.Medium);
                        });
                    });

                    // --- CONTENT ---
                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        foreach (var g in groups)
                        {
                            col.Item().PaddingTop(15).PaddingBottom(5).Background(Colors.Grey.Lighten3).Padding(5)
                               .Text($"{g.Key} ({g.Count()} reservas)").FontSize(12).Bold().FontColor(Colors.Black);

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(40);
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyleHeader).Text("ID");
                                    header.Cell().Element(CellStyleHeader).Text("Usuario");
                                    header.Cell().Element(CellStyleHeader).Text("Fecha");
                                    header.Cell().Element(CellStyleHeader).Text("Horario");
                                    header.Cell().Element(CellStyleHeader).Text("Estado");
                                });

                                uint rowIndex = 0;
                                foreach (var r in g.OrderBy(r => r.FechaReserva).ThenBy(r => r.HoraInicio))
                                {
                                    var style = (rowIndex % 2 == 0) ? CellStyleOdd : (Func<IContainer, IContainer>)CellStyleEven;

                                    table.Cell().Element(style).Text(r.ReservaId.ToString());
                                    table.Cell().Element(style).Text(r.Usuario != null ? $"{r.Usuario.Nombre} {r.Usuario.PrimerApellido}" : "N/A");
                                    table.Cell().Element(style).Text(r.FechaReserva.ToString("dd/MM/yyyy"));
                                    table.Cell().Element(style).Text($"{r.HoraInicio:hh\\:mm} - {r.HoraFin:hh\\:mm}");

                                    string estado = r.Estado?.NombreEstado ?? "N/A";
                                    table.Cell().Element(style).Text(estado).FontColor(estado == "Cancelada" ? Colors.Red.Medium : Colors.Black);

                                    rowIndex++;
                                }
                            });
                        }
                    });

                    page.Footer().PaddingTop(10).Row(row =>
                    {
                        row.RelativeItem().Text("Reservas Canchas Web").FontSize(9).FontColor(Colors.Grey.Medium);
                        row.RelativeItem().AlignRight().Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                            x.Span(" de ");
                            x.TotalPages();
                        });
                    });
                });
            }).GeneratePdf(ms);

            ms.Position = 0;
            return File(ms.ToArray(), "application/pdf", $"reporte_reservas_{DateTime.Now:yyyyMMdd}.pdf");
        }

        static IContainer CellStyleHeader(IContainer container) =>
            container.Background(Colors.Blue.Medium).Padding(5).AlignMiddle().DefaultTextStyle(x => x.FontColor(Colors.White));

        static IContainer CellStyleOdd(IContainer container) =>
            container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5);

        static IContainer CellStyleEven(IContainer container) =>
            container.Background(Colors.Grey.Lighten4).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5);


        // ---------------------------------------------------------
        // 2. REPORTE EXCEL (ClosedXML)
        // ---------------------------------------------------------
        [HttpGet("reservas/por-cancha/excel")]
        public async Task<IActionResult> GetReservasPorCanchaExcel()
        {
            var reservas = (await _reservaService.GetAllAsync()).ToList();
            var groups = reservas.GroupBy(r => r.Cancha?.Nombre ?? "Sin cancha")
                                 .OrderBy(g => g.Key)
                                 .ToList();

            using var wb = new XLWorkbook();
            var headerColor = XLColor.FromHtml("#007bff");

            foreach (var g in groups)
            {
                string sheetName = CleanSheetName(g.Key, g.FirstOrDefault()?.CanchaId, wb);
                var ws = wb.Worksheets.Add(sheetName);

                // --- TÍTULO PRINCIPAL ---
                var titleRange = ws.Range(1, 1, 1, 6);
                titleRange.Merge().Value = $"Reservas: {g.Key}";

                // CORRECCIÓN 2: Separar las asignaciones de estilo línea por línea (evita CS1061)
                var titleStyle = titleRange.Style;
                titleStyle.Font.Bold = true;
                titleStyle.Font.FontSize = 14;
                titleStyle.Font.FontColor = XLColor.White;
                titleStyle.Fill.BackgroundColor = headerColor;
                titleStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // --- ENCABEZADOS ---
                var headers = new[] { "ID", "Usuario", "Fecha", "Hora Inicio", "Hora Fin", "Estado" };
                for (int i = 0; i < headers.Length; i++)
                {
                    ws.Cell(2, i + 1).Value = headers[i];
                }

                var headerRange = ws.Range(2, 1, 2, 6);
                // CORRECCIÓN 3: Separar asignaciones
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                // --- DATOS ---
                var row = 3;
                foreach (var r in g.OrderBy(r => r.FechaReserva).ThenBy(r => r.HoraInicio))
                {
                    ws.Cell(row, 1).Value = r.ReservaId;
                    ws.Cell(row, 2).Value = r.Usuario != null ? $"{r.Usuario.Nombre} {r.Usuario.PrimerApellido}" : "N/A";
                    ws.Cell(row, 3).Value = r.FechaReserva;
                    ws.Cell(row, 3).Style.DateFormat.Format = "dd/MM/yyyy";
                    ws.Cell(row, 4).Value = r.HoraInicio.ToString(@"hh\:mm");
                    ws.Cell(row, 5).Value = r.HoraFin.ToString(@"hh\:mm");
                    ws.Cell(row, 6).Value = r.Estado?.NombreEstado ?? "N/A";

                    ws.Range(row, 1, row, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    row++;
                }

                ws.Columns().AdjustToContents();
                ws.SheetView.FreezeRows(2);
            }

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            ms.Position = 0;
            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"reservas_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        private string CleanSheetName(string key, int? id, XLWorkbook wb)
        {
            var invalid = new[] { '\\', '/', '?', '*', '[', ']' };
            var cleaned = new string(key.Select(ch => invalid.Contains(ch) ? '_' : ch).ToArray());
            var baseName = (cleaned.Length > 20) ? cleaned.Substring(0, 20) : cleaned;
            var candidate = id.HasValue ? $"{baseName}_{id.Value}" : baseName;
            if (candidate.Length > 31) candidate = candidate.Substring(0, 31);

            var finalName = candidate;
            int suffix = 1;
            while (wb.Worksheets.Any(ws => ws.Name.Equals(finalName, StringComparison.OrdinalIgnoreCase)))
            {
                finalName = $"{candidate.Substring(0, Math.Min(candidate.Length, 28))}_{suffix++}";
            }
            return finalName;
        }


        // ---------------------------------------------------------
        // 3. REPORTE WORD (HTML a DOC)
        // ---------------------------------------------------------
        [HttpGet("reservas/por-cancha/word")]
        public async Task<IActionResult> GetReservasPorCanchaWord()
        {
            var reservas = (await _reservaService.GetAllAsync()).ToList();
            var groups = reservas.GroupBy(r => r.Cancha?.Nombre ?? "Sin cancha")
                                 .OrderBy(g => g.Key)
                                 .ToList();

            var sb = new StringBuilder();
            sb.Append("<!doctype html><html><head><meta charset='utf-8' />");

            sb.Append("<style>");
            sb.Append("body { font-family: 'Calibri', 'Arial', sans-serif; font-size: 11pt; }");
            sb.Append("h1 { color: #007bff; border-bottom: 2px solid #007bff; padding-bottom: 10px; }");
            sb.Append("h2 { background-color: #f2f2f2; padding: 5px; border-left: 5px solid #007bff; color: #333; margin-top: 20px; }");
            sb.Append("table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }");
            sb.Append("th { background-color: #007bff; color: white; padding: 8px; text-align: left; border: 1px solid #ddd; }");
            sb.Append("td { padding: 8px; border: 1px solid #ddd; }");
            sb.Append("tr:nth-child(even) { background-color: #f9f9f9; }");
            sb.Append("</style>");

            sb.Append("</head><body>");
            sb.Append($"<h1>Reporte de Reservas - {DateTime.Now:dd/MM/yyyy}</h1>");

            foreach (var g in groups)
            {
                sb.Append($"<h2>{WebUtility.HtmlEncode(g.Key)} ({g.Count()} reservas)</h2>");
                sb.Append("<table>");
                sb.Append("<thead><tr><th>ID</th><th>Usuario</th><th>Fecha</th><th>Hora inicio</th><th>Hora fin</th><th>Estado</th></tr></thead>");
                sb.Append("<tbody>");

                foreach (var r in g.OrderBy(r => r.FechaReserva).ThenBy(r => r.HoraInicio))
                {
                    var usuario = r.Usuario != null ? $"{r.Usuario.Nombre} {r.Usuario.PrimerApellido}" : "N/A";

                    sb.Append("<tr>");
                    sb.AppendFormat("<td>{0}</td>", r.ReservaId);
                    sb.AppendFormat("<td>{0}</td>", WebUtility.HtmlEncode(usuario));
                    sb.AppendFormat("<td>{0}</td>", r.FechaReserva.ToString("dd/MM/yyyy"));
                    sb.AppendFormat("<td>{0}</td>", r.HoraInicio.ToString(@"hh\:mm"));
                    sb.AppendFormat("<td>{0}</td>", r.HoraFin.ToString(@"hh\:mm"));
                    sb.AppendFormat("<td>{0}</td>", WebUtility.HtmlEncode(r.Estado?.NombreEstado ?? "N/A"));
                    sb.Append("</tr>");
                }
                sb.Append("</tbody></table>");
            }

            sb.Append("</body></html>");

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "application/msword; charset=utf-8", $"reporte_reservas_{DateTime.Now:yyyyMMdd}.doc");
        }
    }
}