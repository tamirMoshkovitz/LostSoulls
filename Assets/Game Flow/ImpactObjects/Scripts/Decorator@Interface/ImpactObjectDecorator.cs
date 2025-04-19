using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;

namespace Game_Flow.ImpactObjects.Scripts.Decorator_Interface
{
    public abstract class ImpactObjectDecorator : IImpactObject
    {
        private readonly IImpactObject inner;
        protected MonoImpactObject Mono;
        protected ImpactObjectStats Stats;

        public ImpactObjectDecorator(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats)
        {
            this.inner = inner;
            Mono = mono;
            Stats = stats;
        }

        public virtual void Impact(UnityEngine.Vector3 direction)
        {
            inner?.Impact(direction);
        }

        public virtual void DrawGizmos()
        {
            inner.DrawGizmos();
        }
    }
}