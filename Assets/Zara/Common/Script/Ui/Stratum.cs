namespace Zara.Common.Ui
{
    public class Stratum
    {
        public StratumSetting Setting { get; }
        public int MinOrder { get; }
        public int ExclusiveMaxOrder { get; }

        public Stratum(StratumSetting setting, int minOrder)
        {
            this.Setting = setting;
            this.MinOrder = minOrder;
            this.ExclusiveMaxOrder = minOrder + setting.OrderInterval * setting.PlannedMaxCanvasCount;
        }
    }

    public class StratumSetting
    {
        public int Id { get; }
        public int OrderInterval { get; }
        public int PlannedMaxCanvasCount { get; }

        public StratumSetting(
            int id,
            int orderInterval,
            int plannedMaxCanvasCount)
        {
            this.Id = id;
            this.OrderInterval = orderInterval;
            this.PlannedMaxCanvasCount = plannedMaxCanvasCount;
        }
    }
}
