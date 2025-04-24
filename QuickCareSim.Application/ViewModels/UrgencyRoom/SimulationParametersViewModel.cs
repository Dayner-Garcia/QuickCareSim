using QuickCareSim.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace QuickCareSim.Application.ViewModels.UrgencyRoom
{
    public class SimulationParametersViewModel
    {
        [Display(Name = "Cantidad de Pacientes")]
        [Range(1, 10000)]
        public int TotalPatients { get; set; }

        [Display(Name = "Estrategia")] public StrategyType Strategy { get; set; }

        public string ExecutionMode { get; set; } = "Parallel";

        [Display(Name = "Doctores a Utilizar")]
        [Range(1, 100, ErrorMessage = "Debe seleccionar al menos un doctor.")]
        public int DoctorsToUse { get; set; }
    }
}