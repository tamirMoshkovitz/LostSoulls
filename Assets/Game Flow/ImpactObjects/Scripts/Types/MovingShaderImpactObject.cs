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
        
        public MovingShaderImpactObject(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats) : base(inner, mono, stats)
        {
            _renderers = mono.Renderers;
            _impactColor = mono.ImpactColor;
            _lockedColor = mono.LockedColor;
            _light = mono.Light;
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
                    Color hdrColor = GetHDRColor(_lockedColor, 5f);
                    material.SetColor("_FlatRimColor", hdrColor);
                }
            }
            if (_light != null)
            {
                _light.enabled = true;
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
            if (_light != null)
            {
                _light.enabled = false;
            }
        }
        
        private Color GetHDRColor(Color baseColor, float intensity)
        {
            return new Color(baseColor.r * intensity, baseColor.g * intensity, baseColor.b * intensity, baseColor.a);
        }
    }
}