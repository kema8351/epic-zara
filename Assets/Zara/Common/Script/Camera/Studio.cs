namespace Zara.Common.ExCamera
{
    public class Studio
    {
        public StudioSetting Setting { get; }
        public int MinDepth { get; }
        public int ExclusiveMaxDepth { get; }

        public Studio(StudioSetting setting, int minDepth)
        {
            this.Setting = setting;
            this.MinDepth = minDepth;
            this.ExclusiveMaxDepth = minDepth + setting.DepthInterval * setting.PlannedMaxCameraCount;
        }
    }

    public class StudioSetting
    {
        public int Id { get; }
        public int DepthInterval { get; }
        public int PlannedMaxCameraCount { get; }

        public StudioSetting(
            int id,
            int depthInterval,
            int plannedMaxCameraCount)
        {
            this.Id = id;
            this.DepthInterval = depthInterval;
            this.PlannedMaxCameraCount = plannedMaxCameraCount;
        }
    }
}
