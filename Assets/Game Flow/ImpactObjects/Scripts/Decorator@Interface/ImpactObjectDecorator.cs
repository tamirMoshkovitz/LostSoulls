using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;

namespace Game_Flow.ImpactObjects.Scripts.Decorator_Interface
{
    public abstract class ImpactObjectDecorator : IImpactObject
    {
        protected readonly IImpactObject Inner;
        protected MonoImpactObject Mono;
        protected ImpactObjectStats Stats;

        public ImpactObjectDecorator(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats)
        {
            Inner = inner;
            Mono = mono;
            Stats = stats;
        }

        public virtual void Impact(UnityEngine.Vector3 direction)
        {
            Inner?.Impact(direction);
        }

        public virtual void DrawGizmos()
        {
            Inner.DrawGizmos();
        }
    }
}