using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class MovingShaderImapctObject: ImpactObjectDecorator
    {
        private readonly Renderer _renderer;

        public MovingShaderImapctObject(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats) : base(inner, mono, stats)
        {
            _renderer = mono.GetComponent<Renderer>();
        }

        public override void StartImpact()
        {
            base.StartImpact();
            
        }

        public override void StopImpact()
        {
            base.StopImpact();
            
        }
    }
}