using QuickCareSim.Application.Interfaces.Repositories;
using QuickCareSim.Domain.Entities;
using OfficeOpenXml;
using QuickCareSim.Application.Interfaces.Services;
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace QuickCareSim.Application.Services.Core
{
    public class ExportMetricsService : IExportMetricsService
    {
        private readonly IGenericRepository<SimulationRun> _simulationRepo;

        public ExportMetricsService(IGenericRepository<SimulationRun> simulationRepo)
        {
            _simulationRepo = simulationRepo;
        }

        private async Task<SimulationRun?> GetSimulationWithMetricsAsync(int simulationId)
        {
            return await _simulationRepo.Query()
                .Include(r => r.PerformanceMetrics).ThenInclude(pm => pm.Doctor)
                .Include(r => r.UrgencyWaitMetrics)
                .FirstOrDefaultAsync(r => r.Id == simulationId);
        }

        public async Task ExportSummaryExcelAsync(int simulationId, string outputPath)
        {
            var run = await _simulationRepo.GetByIdAsync(simulationId)
                      ?? throw new Exception("Simulación no encontrada.");

            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Resumen");

            sheet.Cells[1, 1].Value = "ID Simulación";
            sheet.Cells[1, 2].Value = "Estrategia";
            sheet.Cells[1, 3].Value = "Speedup";
            sheet.Cells[1, 4].Value = "Eficiencia";
            sheet.Cells[1, 5].Value = "Pacientes/minuto";

            var isSequential = run.ProcessorsUsed <= 1;

            sheet.Cells[2, 1].Value = run.Id;
            sheet.Cells[2, 2].Value = run.StrategyUsed.ToString();
            sheet.Cells[2, 3].Value = isSequential || run.Speedup == null ? "N/D" : run.Speedup.Value.ToString("0.00");
            sheet.Cells[2, 4].Value =
                isSequential || run.Efficiency == null ? "N/D" : run.Efficiency.Value.ToString("0.00");
            sheet.Cells[2, 5].Value =
                run.RealExecutionTimeSeconds == 0 ? "N/D" : run.PatientsPerMinute.ToString("0.00");

            sheet.Cells.AutoFitColumns();
            package.SaveAs(new FileInfo(outputPath));
        }

        public async Task ExportPerformanceCsvAsync(int simulationId, string outputPath, Dictionary<string, string> doctorNames)
        {
            var run = await GetSimulationWithMetricsAsync(simulationId)
                      ?? throw new Exception("Simulación no encontrada.");

            var sb = new StringBuilder();
            sb.AppendLine("Nombre del Doctor;Pacientes Atendidos;Promedio Atención (s)");

            foreach (var m in run.PerformanceMetrics)
            {
                var doctorName = m.Doctor != null && doctorNames.TryGetValue(m.Doctor.UserId, out var name)
                    ? name
                    : m.DoctorId;

                sb.AppendLine($"{doctorName};{m.PatientsAttended};{m.AverageAttentionTimeSeconds.ToString(CultureInfo.InvariantCulture)}");
            }

            await File.WriteAllTextAsync(outputPath, sb.ToString(), Encoding.UTF8);
        }



        public async Task ExportUrgencyCsvAsync(int simulationId, string outputPath)
        {
            var run = await GetSimulationWithMetricsAsync(simulationId)
                      ?? throw new Exception("Simulación no encontrada.");

            var sb = new StringBuilder();
            sb.AppendLine("Nivel de Urgencia;Promedio de Espera (s);Total de Pacientes");

            foreach (var m in run.UrgencyWaitMetrics)
            {
                sb.AppendLine(
                    $"{m.UrgencyLevel};{m.AverageWaitSeconds.ToString(CultureInfo.InvariantCulture)};{m.TotalPatients}");
            }

            await File.WriteAllTextAsync(outputPath, sb.ToString(), Encoding.UTF8);
        }

        public async Task ExportGlobalMetricsExcelAsync(string outputPath)
        {
            var runs = await _simulationRepo.Query()
                .Include(r => r.PerformanceMetrics)
                .Include(r => r.UrgencyWaitMetrics)
                .ToListAsync();

            using var package = new ExcelPackage();

            // Hoja 1 - Promedios por doctor
            var doctorSheet = package.Workbook.Worksheets.Add("Promedios por Doctor");
            doctorSheet.Cells[1, 1].Value = "DoctorId";
            doctorSheet.Cells[1, 2].Value = "Promedio Global Atención (s)";
            doctorSheet.Cells[1, 3].Value = "Total Pacientes";

            var doctorData = runs
                .SelectMany(r => r.PerformanceMetrics)
                .GroupBy(m => m.DoctorId)
                .Select(g => new
                {
                    DoctorId = g.Key,
                    Promedio = g.Average(x => x.AverageAttentionTimeSeconds),
                    Total = g.Sum(x => x.PatientsAttended)
                })
                .ToList();

            for (int i = 0; i < doctorData.Count; i++)
            {
                doctorSheet.Cells[i + 2, 1].Value = doctorData[i].DoctorId;
                doctorSheet.Cells[i + 2, 2].Value = Math.Round(doctorData[i].Promedio, 2);
                doctorSheet.Cells[i + 2, 3].Value = doctorData[i].Total;
            }

            // Hoja 2 - Promedios por urgencia
            var urgencySheet = package.Workbook.Worksheets.Add("Promedios por Urgencia");
            urgencySheet.Cells[1, 1].Value = "Nivel de Urgencia";
            urgencySheet.Cells[1, 2].Value = "Promedio Espera (s)";
            urgencySheet.Cells[1, 3].Value = "Total Pacientes";

            var urgencyData = runs
                .SelectMany(r => r.UrgencyWaitMetrics)
                .GroupBy(m => m.UrgencyLevel)
                .Select(g => new
                {
                    Nivel = g.Key.ToString(),
                    Promedio = g.Average(x => x.AverageWaitSeconds),
                    Total = g.Sum(x => x.TotalPatients)
                })
                .ToList();

            for (int i = 0; i < urgencyData.Count; i++)
            {
                urgencySheet.Cells[i + 2, 1].Value = urgencyData[i].Nivel;
                urgencySheet.Cells[i + 2, 2].Value = Math.Round(urgencyData[i].Promedio, 2);
                urgencySheet.Cells[i + 2, 3].Value = urgencyData[i].Total;
            }

            // Hoja 3 - Total
            var totalsSheet = package.Workbook.Worksheets.Add("Totales");
            totalsSheet.Cells[1, 1].Value = "Total Simulaciones";
            totalsSheet.Cells[1, 2].Value = runs.Count;

            totalsSheet.Cells[2, 1].Value = "Total Pacientes Atendidos";
            totalsSheet.Cells[2, 2].Value = runs.Sum(r => r.TotalPatientsAttended);

            package.SaveAs(new FileInfo(outputPath));
        }

        public async Task ExportStrategyComparisonExcelAsync(string outputPath)
        {
            var runs = await _simulationRepo.Query()
                .Where(r => r.Speedup != null && r.Efficiency != null && r.RealExecutionTimeSeconds > 0)
                .ToListAsync();

            var grouped = runs
                .GroupBy(r => r.StrategyUsed)
                .Select(g => new
                {
                    Estrategia = g.Key.ToString(),
                    Simulaciones = g.Count(),
                    AvgSpeedup = g.Average(r => r.Speedup ?? 0),
                    AvgEfficiency = g.Average(r => r.Efficiency ?? 0),
                    AvgPatientsPerMin = g.Average(r => r.PatientsPerMinute)
                })
                .OrderByDescending(g => g.AvgSpeedup)
                .ToList();

            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Comparativa Estrategias");

            // Encabezados
            sheet.Cells[1, 1].Value = "Estrategia";
            sheet.Cells[1, 2].Value = "Simulaciones";
            sheet.Cells[1, 3].Value = "Prom. Speedup";
            sheet.Cells[1, 4].Value = "Prom. Eficiencia";
            sheet.Cells[1, 5].Value = "Prom. Pacientes/minuto";

            // Datos
            for (int i = 0; i < grouped.Count; i++)
            {
                var g = grouped[i];
                sheet.Cells[i + 2, 1].Value = g.Estrategia;
                sheet.Cells[i + 2, 2].Value = g.Simulaciones;
                sheet.Cells[i + 2, 3].Value = Math.Round(g.AvgSpeedup, 2);
                sheet.Cells[i + 2, 4].Value = Math.Round(g.AvgEfficiency, 2);
                sheet.Cells[i + 2, 5].Value = Math.Round(g.AvgPatientsPerMin, 2);
            }

            sheet.Cells.AutoFitColumns();
            package.SaveAs(new FileInfo(outputPath));
        }
    }
}