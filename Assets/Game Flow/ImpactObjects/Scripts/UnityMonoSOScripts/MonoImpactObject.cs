using System;
using System.Collections.Generic;
using Game_Flow.ImpactObjects.Scripts.Audio;
using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.Types;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts
{
    public class MonoImpactObject : MonoBehaviour
    {
        [Header("Material components")]
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private Color impactColor;
        [SerializeField] private Color lockedColor;
        [SerializeField] private Light light;
        
        [Header("Impact Object")]
        private IImpactObject _impactObject;
        [SerializeField] private List<ImpactObjectTypes> decoratorOrder;
        [SerializeField] private ImpactObjectStats stats;
        [SerializeField] private MultiImpactObjectLinker linker;
        [FormerlySerializedAs("gridVisualizer")] [SerializeField] private Grid grid;
        [SerializeField] private List<MonoImpactObject> nonCollidingObjects = new List<MonoImpactObject>();
        
        [Header("Audio")]
        [SerializeField] private AudioSource objectAudioSource;
        [SerializeField] private AudioClip objectAudio;
        
        private bool _updated;
        private bool _activated;
        private MoovingObjectAudio _objectAudio;

        public Renderer[] Renderers => renderers;
        public Color ImpactColor => impactColor;
        public Color LockedColor => lockedColor;
        public Light Light => light;
        
        public bool IsMoveable { get; set; }
        public bool IsOpenable { get; set; }
        
        public bool IsOpen { get; private set; }

        public bool IsSoul {get; private set;}
        public bool IsBlocked { get; set; } = false;
        public List<MonoImpactObject> NonCollidingObjects => nonCollidingObjects;
        void Start()
        {
            IsSoul = false;
            _impactObject = new BasicImpactObject(this,stats);
            
            foreach (var type in decoratorOrder)
            {
                if (type == ImpactObjectTypes.Soul) IsSoul = true;
                bool shouldSnapToGrid = grid != null && type is ImpactObjectTypes.OneBlockGrid
                    or ImpactObjectTypes.TwoBlockHorizontalGrid
                    or ImpactObjectTypes.TwoBlockVerticalGrid
                    or ImpactObjectTypes.ThreeBlockHorizontalGrid
                    or ImpactObjectTypes.ThreeBlockVerticalGrid
                    or ImpactObjectTypes.FourBlocksSquareGrid;
                
                
                _impactObject = ImpactObjectFactory.CreateImpactObject(type, _impactObject, this,stats,grid);
                if(shouldSnapToGrid) _impactObject.StopImpact();
                if (type == ImpactObjectTypes.MovingShader)
                {
                    IsMoveable = true;
                    if (light != null)
                    {
                        light.enabled = false;
                    }
                }
            }
            
            if (objectAudioSource != null && objectAudio != null)
            {
                _objectAudio = new MoovingObjectAudio(objectAudioSource, objectAudio);
            }
        }


        public void Activate()
        {
            if(_activated) return;
            _activated = true;
            _impactObject.StartImpact();
            if (linker != null)
            {
                linker.ActivateSiblings();
            }
        }
        
        public void UpdateObject(Vector3 direction)
        {
            if (_updated || direction.Equals(Vector3.zero)) return;
            _updated = true;
            Vector3 snapped = GetClosestCardinalDirection(direction);
            IsBlocked = false;
            _impactObject.UpdateImpact(snapped);
            if (linker != null)
            {
                linker.ConnectObjects();
                linker.UpdateSiblings(snapped);
            }
        }
        
        public void DeActivate()
        {
            if(!_activated) return;
            _activated = false;
            _impactObject.StopImpact();
            if (linker != null)
            {
                linker.DeActivateSiblings();
            }
        }

        public void Update()
        {
            _updated = false;
        }

        void OnDrawGizmos()
        {
            _impactObject?.DrawGizmos();
        }
        
        private Vector3 GetClosestCardinalDirection(Vector3 direction)
        {
            direction.y = 0; // Ignore vertical component
            direction.Normalize();

            Vector3[] cardinalDirections =
            {
                Vector3.forward,  // (0, 0, 1)
                Vector3.back,     // (0, 0, -1)
                Vector3.right,    // (1, 0, 0)
                Vector3.left      // (-1, 0, 0)
            };

            Vector3 best = Vector3.zero;
            float maxDot = -Mathf.Infinity;

            foreach (var dir in cardinalDirections)
            {
                float dot = Vector3.Dot(direction, dir);
                if (dot > maxDot)
                {
                    maxDot = dot;
                    best = dir;
                }
            }

            return best;
        }
        
        public Vector3 GetBottomCenter()
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer == null) return transform.position;

            Bounds bounds = renderer.bounds;
            Vector3 bottomCenter = bounds.center;
            bottomCenter.y = bounds.min.y;
            return bottomCenter;
        }
        
        public void HighlightObject()
        {
            if (! IsMoveable) return;
            foreach (var renderer in renderers)
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
                    Color hdrColor = GetHDRColor(impactColor, 5f);
                    material.SetColor("_FlatRimColor",hdrColor);
                }
            }
            if (light != null)
            {
                light.enabled = true;
            }
        }
        
        public void UnhighlightObject()
        {
            if (! IsMoveable) return;
            foreach (var renderer in renderers)
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
            if (light != null)
            {
                light.enabled = false;
            }
        }
        
        private Color GetHDRColor(Color baseColor, float intensity)
        {
            return new Color(baseColor.r * intensity, baseColor.g * intensity, baseColor.b * intensity, baseColor.a);
        }

        public void OpenImpactObject()
        {
            if (!IsOpenable) return;
            OpenCloseImpactObject openCloseImpactObject = (OpenCloseImpactObject)_impactObject;
            openCloseImpactObject.OpenImpactObject();
            IsOpen = true;
        }
        
        public void CloseImpactObject()
        {
            if (!IsOpenable) return;
            OpenCloseImpactObject openCloseImpactObject = (OpenCloseImpactObject)_impactObject;
            openCloseImpactObject.CloseImpactObject();
            IsOpen = false;
        }

    }
}