using Zara.Common.ExBase;

namespace Zara.Expansion.ExBase
{
    public class AutoGetParticleAttribute : AutoGetAttribute
    {
#if UNITY_EDIOTR
        public override void SetDefault(object obj)
        {
            ParticleSystemRenderer particleSystemRenderer = Cast<ParticleSystemRenderer>(obj);

            if (particleSystemRenderer != null)
                DefaultSettingUtility.SetParticleSystemRenderer(particleSystemRenderer);
        }
#endif
    }
}