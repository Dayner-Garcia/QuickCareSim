namespace QuickCareSim.Application.Interfaces.Services.Core
{
    public interface ISimulationRecoveryService
    {
        Task ResetBusyDoctorsAsync();
    }
}