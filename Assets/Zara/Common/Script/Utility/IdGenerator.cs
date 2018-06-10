namespace Zara.Common.Utility
{
    public class IdGenerator
    {
        int latestId = 0;

        public int Get()
        {
            latestId++;
            latestId %= int.MaxValue;
            return latestId;
        }
    }
}