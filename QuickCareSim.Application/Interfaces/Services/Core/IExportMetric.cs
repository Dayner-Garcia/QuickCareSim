using System;
using System.Collections.Generic;


    namespace QuickCareSim.Application.Interfaces.Services
    {
        public interface IExportMetricsService
        {
            Task ExportSummaryExcelAsync(int simulationId, string outputPath);
           // Task ExportPerformanceCsvAsync(int simulationId, string outputPath);
           Task ExportPerformanceCsvAsync(int simulationId, string outputPath, Dictionary<string, string> doctorNames);
            Task ExportUrgencyCsvAsync(int simulationId, string outputPath);
            Task ExportGlobalMetricsExcelAsync(string outputPath);
            Task ExportStrategyComparisonExcelAsync(string outputPath);
        }
    }
