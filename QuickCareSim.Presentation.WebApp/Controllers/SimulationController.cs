using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickCareSim.Application.Interfaces.Repositories;
using QuickCareSim.Application.Interfaces.Services;
using QuickCareSim.Application.Interfaces.Services.Core;
using QuickCareSim.Application.Interfaces.Services.Executors;
using QuickCareSim.Application.ViewModels.UrgencyRoom;
using QuickCareSim.Domain.Entities;
using QuickCareSim.Infrastructure.Identity.Entities;

namespace QuickCareSim.Presentation.WebApp.Controllers;

public class SimulationController : Controller
{
    private readonly IParallelSimulationExecutor _parallelExecutor;
    private readonly ISequentialSimulationExecutor _sequentialExecutor;
    private readonly ISimulationRetryHandler _retryHandler;
    private readonly ISpeedupCalculator _speedupCalculator;
    private readonly ISimulationInfoService _info;
    private readonly IDoctorService _doctorService;
    private readonly IExportMetricsService _exportService;
    private readonly UserManager<ApplicationUser> _userManager;

    public SimulationController(
        IParallelSimulationExecutor parallelExecutor,
        ISequentialSimulationExecutor sequentialExecutor,
        ISimulationRetryHandler retryHandler,
        ISpeedupCalculator speedupCalculator,
        ISimulationInfoService info,
        IDoctorService doctorService,
        IExportMetricsService exportService,
        UserManager<ApplicationUser> userManager)
    {
        _parallelExecutor = parallelExecutor;
        _sequentialExecutor = sequentialExecutor;
        _retryHandler = retryHandler;
        _speedupCalculator = speedupCalculator;
        _info = info;
        _doctorService = doctorService;
        _exportService = exportService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var availableDoctors = await _doctorService.GetAvailableDoctorsAsync();
        ViewBag.AvailableDoctorsCount = availableDoctors.Count;

        return View(new SimulationParametersViewModel
        {
            TotalPatients = 10,
            DoctorsToUse = availableDoctors.Count
        });
    }

    [HttpPost]
    public async Task<IActionResult> Index(SimulationParametersViewModel vm)
    {
        var availableDoctors = await _doctorService.GetAvailableDoctorsAsync();
        ViewBag.AvailableDoctorsCount = availableDoctors.Count;

        if (!ModelState.IsValid)
            return View(vm);

        if (vm.ExecutionMode == "Parallel" && vm.DoctorsToUse > availableDoctors.Count)
        {
            ModelState.AddModelError(nameof(vm.DoctorsToUse),
                $"Solo hay {availableDoctors.Count} doctores disponibles.");
            return View(vm);
        }

        int simulationId;
        var parameters = new SimulationParametersViewModel
        {
            TotalPatients = vm.TotalPatients,
            Strategy = vm.Strategy,
            ExecutionMode = vm.ExecutionMode,
            DoctorsToUse = vm.DoctorsToUse
        };

        var stopwatch = Stopwatch.StartNew();
        var cancellationToken = new CancellationTokenSource().Token;

        if (parameters.ExecutionMode == "Sequential")
            simulationId = await _sequentialExecutor.ExecuteNewAsync(parameters, cancellationToken);
        else
            simulationId = await _parallelExecutor.ExecuteNewAsync(parameters, cancellationToken);

        stopwatch.Stop();

        var runRepo = HttpContext.RequestServices.GetRequiredService<IGenericRepository<SimulationRun>>();
        var run = await runRepo.GetByIdAsync(simulationId);
        if (run != null)
        {
            run.RealExecutionTimeSeconds = stopwatch.Elapsed.TotalSeconds;
            await runRepo.UpdateAsync(run);
        }

        return RedirectToAction("Result", new { id = simulationId });
    }

    [HttpPost]
    public async Task<IActionResult> CalculateMetrics(int id)
    {
        var runRepo = HttpContext.RequestServices.GetRequiredService<IGenericRepository<SimulationRun>>();
        var run = await runRepo.GetByIdAsync(id);

        if (run == null || run.ProcessorsUsed == 1 || run.Speedup != null)
            return RedirectToAction("Result", new { id });

        var parameters = new SimulationParametersViewModel
        {
            TotalPatients = run.TotalPatients,
            Strategy = run.StrategyUsed,
            ExecutionMode = "Sequential",
            DoctorsToUse = run.TotalDoctors
        };

        await _speedupCalculator.CalculateAsync(run, parameters, new CancellationToken());

        return RedirectToAction("Result", new { id });
    }


    [HttpGet]
    public async Task<IActionResult> Result(int id)
    {
        var result = await _info.GetSimulationResultAsync(id);
        if (result == null)
            return NotFound();

        var metrics = await _info.GetUrgencyMetricsAsync(id);
        var perfMetrics = await _info.GetPerformanceMetricsAsync(id);
        var users = await _userManager.Users.ToListAsync();
        var doctorNames = users.ToDictionary(u => u.Id, u => $"{u.Name} {u.LastName}");

        ViewBag.Metrics = metrics;
        ViewBag.PerformanceMetrics = perfMetrics;
        ViewBag.DoctorNames = doctorNames;

        return View(result);
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
    public async Task<IActionResult> History()
    {
        var list = await _info.GetAllSimulationsAsync();
        return View(list);
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RetrySimulation(int id)
    {
        var token = new CancellationTokenSource().Token;

        try
        {
            await _retryHandler.RetryAsync(id, token);
            return Ok(id);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSimulation(int id)
    {
        var runRepo = HttpContext.RequestServices.GetRequiredService<IGenericRepository<SimulationRun>>();
        await runRepo.DeleteAsync(id);
        return RedirectToAction("History");
    }
}