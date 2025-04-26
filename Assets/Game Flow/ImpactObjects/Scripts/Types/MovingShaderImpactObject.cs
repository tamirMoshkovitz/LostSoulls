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
        
        public MovingShaderImpactObject(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats) : base(inner, mono, stats)
        {
            _renderers = mono.Renderers;
            _impactColor = mono.ImpactColor;
            _lockedColor = mono.LockedColor;
        }

        public override void StartImpact()
        {
            base.StartImpact();
            foreach (var renderer in _renderers)
            {
                if (renderer == null) continue;
                var material = renderer.material;
                if (material == null) continue;
                if (material.HasProperty("_RimEnabled"))
                {
                    material.SetInt("_RimEnabled", 1);
                }

                material.EnableKeyword("DR_RIM_ON");
                if (material.HasProperty("_FlatRimColor"))
                {
                    material.SetColor("_FlatRimColor", _lockedColor);
                }
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
                if (material.HasProperty("_RimEnabled"))
                {
                    material.SetInt("_RimEnabled", 0);
                }
                material.DisableKeyword("DR_RIM_ON");
            }
        }
    }
}