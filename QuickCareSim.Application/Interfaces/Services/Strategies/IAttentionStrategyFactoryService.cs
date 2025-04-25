using QuickCareSim.Domain.Enums;

namespace QuickCareSim.Application.Interfaces.Services.Strategies
{
    public interface IAttentionStrategyFactoryService
    {
        public IAttentionStrategyService GetStrategy(StrategyType strategy);
    }
}
