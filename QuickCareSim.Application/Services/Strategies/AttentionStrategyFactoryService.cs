using QuickCareSim.Application.Interfaces.Services.Strategies;
using QuickCareSim.Domain.Enums;

namespace QuickCareSim.Application.Services.Strategys
{
    public class AttentionStrategyFactoryService : IAttentionStrategyFactoryService
    {
        private readonly RoundRobinStrategyService _roundRobin;
        private readonly PriorityStrategyService _priority;
        private readonly EmergencyTypeStrategyService _emergency;

        public AttentionStrategyFactoryService(
            RoundRobinStrategyService roundRobin,
            PriorityStrategyService priority,
            EmergencyTypeStrategyService emergency)
        {
            _roundRobin = roundRobin;
            _priority = priority;
            _emergency = emergency;
        }

        // Metodo reutilizable para seleccionar los tipos de estrategia.
        public IAttentionStrategyService GetStrategy(StrategyType strategy) => strategy switch
        {
            StrategyType.RoundRobin => _roundRobin,
            StrategyType.Priority => _priority,
            StrategyType.EmergencyType => _emergency,
            _ => _roundRobin
        };
    }
}
