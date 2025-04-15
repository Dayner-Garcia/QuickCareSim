

namespace QuickCareSim.Domain.Entities
{
    public class SimulationPerformance
    {
        public int Id { get; set; }
        public DateTime SimulationDate { get; set; }
        public int TotalPatientsAttended { get; set; }
        public double TotalSimulationTimeSeconds { get; set; }
        public int TotalDoctorsUsed { get; set; }
        public double PatientsPerMinute => TotalPatientsAttended / (TotalSimulationTimeSeconds / 60);
        public int ProcessorsUsed { get; set; } // Para evaluar la escalabilidad de la simulacion.
    }
}
