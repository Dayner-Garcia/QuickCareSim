using QuickCareSim.Domain.Enums;

namespace QuickCareSim.Application.Utils
{
    public static class UrgencyUtils
    {
        private static readonly ThreadLocal<Random> _random = new(() => new Random());

        public static int GetSimulatedAttentionTime(UrgencyLevel level)
        {
            return level switch
            {
                UrgencyLevel.CRITICAL => RandomBetween(6, 10),
                UrgencyLevel.HIGH => RandomBetween(8, 14),
                UrgencyLevel.MEDIUM => RandomBetween(10, 18),
                UrgencyLevel.LOW => RandomBetween(12, 25),
                _ => 10
            };
        }

        private static int RandomBetween(int min, int max)
            => _random.Value!.Next(min, max);
    }
}