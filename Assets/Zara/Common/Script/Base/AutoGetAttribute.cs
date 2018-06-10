namespace Zara.Common.ExBase
{
    public class AutoGetAttribute : AutoAttribute
    {
        public override sealed bool ShouldAddAutomatilcally => false;
    }
}