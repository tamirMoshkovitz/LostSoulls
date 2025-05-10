using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class MovingShaderImpactObject: ImpactObjectDecorator
    {
        private readonly Renderer[] _renderers;
        private readonly Color _impactColor;
        private readonly Color _lockedColor;
        private readonly Light _light;
        private readonly float _scale;
        private readonly float _width;
        
        public MovingShaderImpactObject(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats) : base(inner, mono, stats)
        {
            _renderers = mono.Renderers;
            _impactColor = mono.ImpactColor;
            _lockedColor = mono.LockedColor;
            _scale = mono.Scale;
            _width = mono.Width;
        }

        public override void StartImpact()
        {
            base.StartImpact();
            foreach (var renderer in _renderers)
            {
                if (renderer == null) continue;
                var material = renderer.material;
                if (material == null) continue;
                material.color = _lockedColor;
            }
        }

        public override void StopImpact()
        {
            base.StopImpact();
            foreach (var renderer in _renderers)
            {
                if (renderer == null) continue;
                var material = renderer.material;
                if (material == null) continue;
                material.color = _impactColor;
            }
            
        }
    }
}