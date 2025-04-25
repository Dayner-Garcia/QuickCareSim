using Microsoft.AspNetCore.Mvc;
using QuickCareSim.Application.Interfaces.Services;
using QuickCareSim.Application.Interfaces.Services.Core;

namespace QuickCareSim.Presentation.WebApp.Controllers;

public class SimulationController : Controller
{

    private readonly ISimulationInfoService _info;
    private readonly IExportMetricsService _exportService;
    public SimulationController(
        ISimulationInfoService info,
        IExportMetricsService exportService)
    {
        _info = info;
        _exportService = exportService;
    }
    
    [HttpGet]
    public async Task<IActionResult> History()
    {
        var list = await _info.GetAllSimulationsAsync();
        return View(list);
    }
    
    [HttpGet]
        public async Task<IActionResult> ExportSummaryExcel(int id)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), $"summary-{id}.xlsx");
            await _exportService.ExportSummaryExcelAsync(id, tempPath);
            return File(System.IO.File.ReadAllBytes(tempPath),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Resumen De Simulacion-{id}.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> ExportPerformanceCsv(int id)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), $"doctores-{id}.csv");
            await _exportService.ExportPerformanceCsvAsync(id, tempPath);
            return File(System.IO.File.ReadAllBytes(tempPath),
                "text/csv",
                $"tiempo-promedio-doctores-{id}.csv");
        }

        [HttpGet]
        public async Task<IActionResult> ExportUrgencyCsv(int id)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), $"urgencia-{id}.csv");
            await _exportService.ExportUrgencyCsvAsync(id, tempPath);
            return File(System.IO.File.ReadAllBytes(tempPath),
                "text/csv",
                $"tiempos-espera-por-urgencia-{id}.csv");
        }
        
        [HttpGet]
        public async Task<IActionResult> ExportGlobalMetricsExcel()
        {
            var tempPath = Path.Combine(Path.GetTempPath(), $"global-metrics-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            await _exportService.ExportGlobalMetricsExcelAsync(tempPath);

            return File(System.IO.File.ReadAllBytes(tempPath),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"informe-global-simulaciones.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> ExportStrategyComparisonExcel()
        {
            var tempPath = Path.Combine(Path.GetTempPath(), $"estrategias-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            await _exportService.ExportStrategyComparisonExcelAsync(tempPath);

            return File(System.IO.File.ReadAllBytes(tempPath),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "comparativa-estrategias.xlsx");
        }
}