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
                
                // Only process renderers that are children with "Highlightable" tag
                if (!renderer.gameObject.CompareTag("Highlightable")) continue;
                
                var material = renderer.material;
                if (material == null) continue;
                
                // Enable outline first
                material.EnableKeyword("DR_OUTLINE_ON");
                
                if (material.HasProperty("_OutlineEnabled"))
                {
                    material.SetInt("_OutlineEnabled", 1);
                    
                    // Only set color after confirming outline is enabled
                    if (material.HasProperty("_OutlineColor"))
                    {
                        Color hdrColor = GetHDRColor(_lockedColor, 3f);
                        material.SetColor("_OutlineColor", hdrColor);
                    }
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
                
                // Only process renderers that are children with "Highlightable" tag
                if (!renderer.gameObject.CompareTag("Highlightable")) continue;
                
                var material = renderer.material;
                if (material == null) continue;
                
                // Disable outline
                material.DisableKeyword("DR_OUTLINE_ON");
                
                if (material.HasProperty("_OutlineEnabled"))
                {
                    material.SetInt("_OutlineEnabled", 0);
                }
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