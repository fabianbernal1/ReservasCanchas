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
using Xceed.Words.NET;
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

        // GET api/reports/reservas/por-cancha/pdf
        [HttpGet("reservas/por-cancha/pdf")]

        public async Task<IActionResult> GetReservasPorCanchaPdf()
        {
            var reservas = (await _reservaService.GetAllAsync()).ToList();
            var groups = reservas.GroupBy(r => r.Cancha?.Nombre ?? "Sin cancha")
                                 .OrderBy(g => g.Key)
                                 .ToList();

            using var ms = new MemoryStream();
            QuestPDF.Settings.License = LicenseType.Community;

            // Generar PDF con QuestPDF
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text($"Reporte de reservas por cancha - {DateTime.Now:dd/MM/yyyy}")
                        .SemiBold().FontSize(16).AlignCenter();

                    page.Content().Column(col =>
                    {
                        foreach (var g in groups)
                        {
                            col.Item().PaddingTop(10).Text($"{g.Key} ({g.Count()} reservas)").Bold();

                            col.Item().Table(table =>
                            {
                                // Definir columnas
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(1); // ID
                                    columns.RelativeColumn(2); // Usuario
                                    columns.RelativeColumn(2); // Fecha
                                    columns.RelativeColumn(1); // Hora inicio
                                    columns.RelativeColumn(1); // Hora fin
                                    columns.RelativeColumn(1); // Estado
                                });

                                // Header
                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("ID");
                                    header.Cell().Element(CellStyle).Text("Usuario");
                                    header.Cell().Element(CellStyle).Text("Fecha");
                                    header.Cell().Element(CellStyle).Text("Hora inicio");
                                    header.Cell().Element(CellStyle).Text("Hora fin");
                                    header.Cell().Element(CellStyle).Text("Estado");
                                });

                                foreach (var r in g.OrderBy(r => r.FechaReserva).ThenBy(r => r.HoraInicio))
                                {
                                    table.Cell().Element(CellStyle).Text(r.ReservaId.ToString());
                                    table.Cell().Element(CellStyle).Text(r.Usuario != null ? $"{r.Usuario.Nombre} {r.Usuario.PrimerApellido}" : r.UsuarioId.ToString());
                                    table.Cell().Element(CellStyle).Text(r.FechaReserva.ToString("dd/MM/yyyy"));
                                    table.Cell().Element(CellStyle).Text(r.HoraInicio.ToString(@"hh\:mm"));
                                    table.Cell().Element(CellStyle).Text(r.HoraFin.ToString(@"hh\:mm"));
                                    table.Cell().Element(CellStyle).Text(r.Estado?.NombreEstado ?? r.EstadoId.ToString());
                                }

                                // estilo de celda
                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.PaddingVertical(3).PaddingHorizontal(5);
                                }
                            });
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });
                });
            }).GeneratePdf(ms);

            ms.Position = 0;
            var fileName = $"reservas_por_cancha_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
            return File(ms.ToArray(), "application/pdf", fileName);
        }

        // GET api/reports/reservas/por-cancha/excel
        [HttpGet("reservas/por-cancha/excel")]
        public async Task<IActionResult> GetReservasPorCanchaExcel()
        {
            var reservas = (await _reservaService.GetAllAsync()).ToList();
            var groups = reservas.GroupBy(r => r.Cancha?.Nombre ?? "Sin cancha")
                                 .OrderBy(g => g.Key)
                                 .ToList();

            using var wb = new XLWorkbook();
            foreach (var g in groups)
            {
                // Generar un nombre de hoja único y válido (máx 31 caracteres)
                string MakeSheetName(string key, int? id)
                {
                    var name = string.IsNullOrWhiteSpace(key) ? "Sin_cancha" : key;
                    // caracteres no permitidos en nombres de hoja: \ / ? * [ ]
                    var invalid = new[] { '\\', '/', '?', '*', '[', ']' };
                    var cleaned = new string(name.Select(ch => invalid.Contains(ch) ? '_' : ch).ToArray());
                    // añadir id para garantizar unicidad
                    var baseName = (cleaned.Length > 20) ? cleaned.Substring(0, 20) : cleaned;
                    var candidate = id.HasValue ? $"{baseName}_{id.Value}" : baseName;
                    if (candidate.Length > 31) candidate = candidate.Substring(0, 31);
                    return candidate;
                }

                var firstReserva = g.FirstOrDefault();
                var sheetName = MakeSheetName(g.Key, firstReserva?.CanchaId);

                // Si por alguna razón existe la hoja (muy improbable) añadimos sufijo incremental
                var finalSheetName = sheetName;
                var suffix = 1;
                while (wb.Worksheets.Any(ws => ws.Name.Equals(finalSheetName, StringComparison.OrdinalIgnoreCase)))
                {
                    var suffixStr = $"_{suffix++}";
                    var allowedLen = 31 - suffixStr.Length;
                    var basePart = sheetName.Length > allowedLen ? sheetName.Substring(0, allowedLen) : sheetName;
                    finalSheetName = basePart + suffixStr;
                }

                var ws = wb.Worksheets.Add(finalSheetName);

                // Headers
                ws.Cell(1, 1).Value = "ID";
                ws.Cell(1, 2).Value = "Usuario";
                ws.Cell(1, 3).Value = "Fecha";
                ws.Cell(1, 4).Value = "Hora inicio";
                ws.Cell(1, 5).Value = "Hora fin";
                ws.Cell(1, 6).Value = "Estado";

                var row = 2;
                foreach (var r in g.OrderBy(r => r.FechaReserva).ThenBy(r => r.HoraInicio))
                {
                    ws.Cell(row, 1).Value = r.ReservaId;
                    ws.Cell(row, 2).Value = r.Usuario != null ? $"{r.Usuario.Nombre} {r.Usuario.PrimerApellido}" : r.UsuarioId.ToString();
                    ws.Cell(row, 3).Value = r.FechaReserva;
                    ws.Cell(row, 3).Style.DateFormat.Format = "dd/MM/yyyy";
                    ws.Cell(row, 4).Value = r.HoraInicio.ToString(@"hh\:mm");
                    ws.Cell(row, 5).Value = r.HoraFin.ToString(@"hh\:mm");
                    ws.Cell(row, 6).Value = r.Estado?.NombreEstado ?? r.EstadoId.ToString();
                    row++;
                }

                // Formato autoajuste
                ws.Columns().AdjustToContents();
            }

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            ms.Position = 0;
            var fileName = $"reservas_por_cancha_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        // GET api/reports/reservas/por-cancha/word
        [HttpGet("reservas/por-cancha/word")]
        public async Task<IActionResult> GetReservasPorCanchaWord()
        {
            var reservas = (await _reservaService.GetAllAsync()).ToList();
            var groups = reservas.GroupBy(r => r.Cancha?.Nombre ?? "Sin cancha")
                                 .OrderBy(g => g.Key)
                                 .ToList();

            var sb = new StringBuilder();
            sb.Append("<!doctype html><html><head><meta charset=\"utf-8\" /><style>");
            sb.Append("body{font-family: Arial, Helvetica, sans-serif; font-size:12px;} ");
            sb.Append("table{border-collapse:collapse; width:100%; margin-bottom:12px;} ");
            sb.Append("th,td{border:1px solid #ccc; padding:6px; text-align:left;} ");
            sb.Append("th{background:#f2f2f2;} ");
            sb.Append("</style></head><body>");
            sb.AppendFormat("<h1>Reporte de reservas por cancha - {0}</h1>", DateTime.Now.ToString("dd/MM/yyyy"));

            foreach (var g in groups)
            {
                sb.AppendFormat("<h2>{0} ({1} reservas)</h2>", WebUtility.HtmlEncode(g.Key), g.Count());
                sb.Append("<table>");
                sb.Append("<thead><tr><th>ID</th><th>Usuario</th><th>Fecha</th><th>Hora inicio</th><th>Hora fin</th><th>Estado</th></tr></thead>");
                sb.Append("<tbody>");
                foreach (var r in g.OrderBy(r => r.FechaReserva).ThenBy(r => r.HoraInicio))
                {
                    var usuario = r.Usuario != null ? $"{r.Usuario.Nombre} {r.Usuario.PrimerApellido}" : r.UsuarioId.ToString();
                    sb.Append("<tr>");
                    sb.AppendFormat("<td>{0}</td>", WebUtility.HtmlEncode(r.ReservaId.ToString()));
                    sb.AppendFormat("<td>{0}</td>", WebUtility.HtmlEncode(usuario));
                    sb.AppendFormat("<td>{0}</td>", WebUtility.HtmlEncode(r.FechaReserva.ToString("dd/MM/yyyy")));
                    sb.AppendFormat("<td>{0}</td>", WebUtility.HtmlEncode(r.HoraInicio.ToString(@"hh\:mm")));
                    sb.AppendFormat("<td>{0}</td>", WebUtility.HtmlEncode(r.HoraFin.ToString(@"hh\:mm")));
                    sb.AppendFormat("<td>{0}</td>", WebUtility.HtmlEncode(r.Estado?.NombreEstado ?? r.EstadoId.ToString()));
                    sb.Append("</tr>");
                }
                sb.Append("</tbody></table>");
            }

            sb.Append("</body></html>");

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            var fileName = $"reservas_por_cancha_{DateTime.Now:yyyyMMdd_HHmm}.doc";
            // application/msword funciona bien para que Word abra el HTML como documento
            return File(bytes, "application/msword; charset=utf-8", fileName);
        }
    }
}