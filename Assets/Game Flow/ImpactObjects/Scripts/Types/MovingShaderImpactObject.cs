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
        private readonly float _intensity;
        private readonly float _width;
        
        public MovingShaderImpactObject(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats) : base(inner, mono, stats)
        {
            _renderers = mono.Renderers;
            _impactColor = mono.ImpactColor;
            _lockedColor = mono.LockedColor;
            _intensity = mono.Intensity;
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
                if (material.HasProperty("_OutlineEnabled"))
                {
                    material.SetInt("_OutlineEnabled", 1);
                }
                material.EnableKeyword("DR_OUTLINE_ON");
                if (material.HasProperty("_OutlineColor"))
                {
                    material.SetColor("_OutlineColor", _lockedColor);
                }
                if (material.HasProperty("_OutlineWidth"))
                {
                    material.SetFloat("_OutlineWidth", _width);
                }
                if (material.HasProperty("_OutlineIntensity"))
                {
                    material.SetFloat("_OutlineIntensity", _intensity);
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
                if (material.HasProperty("_OutlineEnabled"))
                {
                    material.SetInt("_OutlineEnabled", 0);
                }
                material.DisableKeyword("DR_OUTLINE_ON");
            }
            if (_light != null)
            {
                _light.enabled = false;
            }
        }
    }
}