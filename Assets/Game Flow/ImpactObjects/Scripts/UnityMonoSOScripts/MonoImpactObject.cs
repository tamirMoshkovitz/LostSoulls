using System;
using System.Collections.Generic;
using System.Linq;
using Game_Flow.ImpactObjects.Scripts.Audio;
using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.Types;
using Unity.Mathematics;
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
        [FormerlySerializedAs("intensity")] [SerializeField] private float scale = 100f;
        [SerializeField] private float width = 10f;
        
        [Header("Impact Object")]
        private IImpactObject _impactObject;
        [SerializeField] private List<ImpactObjectTypes> decoratorOrder;
        [SerializeField] private ImpactObjectStats stats;
        [FormerlySerializedAs("gridVisualizer")] [SerializeField] private Grid grid;
        [SerializeField] private List<MonoImpactObject> nonCollidingObjects = new List<MonoImpactObject>();

        [Header("Grid Occupation")]
        [SerializeField] private List<Vector2Int> initialCells = new(); // (row, col)
        [SerializeField] private float timePerMove = 0.3f;

        [Header("Audio")]
        [SerializeField] private AudioSource objectAudioSource;
        [SerializeField] private AudioClip objectAudio;
        
        private bool _updated;
        private bool _activated;
        private MoovingObjectAudio _objectAudio;
        public MoovingObjectAudio ObjectAudio => _objectAudio;

        public Renderer[] Renderers => renderers;
        public Color ImpactColor => impactColor;
        public Color LockedColor => lockedColor;
        public float Scale => scale;
        public float Width => width;
        
        public bool IsMoveable { get; set; }

        public bool IsSoul {get; private set;}
        public bool IsBlocked { get; set; } = false;
        
        
        public List<(int x, int y)> UsedCells {get; set;}
        public float TimePerMove
        {
            get => timePerMove;
            set => timePerMove = value;
        }

        private void Start()
        {
            _objectAudio = new MoovingObjectAudio(objectAudioSource, objectAudio);
            IsSoul = false;
            _impactObject = new BasicImpactObject(this, stats);
            UsedCells = initialCells.Select(cell => (cell.x, cell.y)).ToList();
            foreach (var type in decoratorOrder)
            {
                if (type == ImpactObjectTypes.Soul)
                    IsSoul = true;

                _impactObject = ImpactObjectFactory.CreateImpactObject(type, _impactObject, this, stats, grid);

                if (grid != null && IsGridBasedDecorator(type))
                {
                    SetupGridOccupation();
                }

                if (type == ImpactObjectTypes.MovingShader)
                {
                    IsMoveable = true;
                }
            }

            if (objectAudioSource != null && objectAudio != null)
            {
                _objectAudio = new MoovingObjectAudio(objectAudioSource, objectAudio);
            }
        }

        private bool IsGridBasedDecorator(ImpactObjectTypes type)
        {
            return type is ImpactObjectTypes.OneBlockGrid
                or ImpactObjectTypes.TwoBlockHorizontalGrid
                or ImpactObjectTypes.TwoBlockVerticalGrid
                or ImpactObjectTypes.ThreeBlockHorizontalGrid
                or ImpactObjectTypes.ThreeBlockVerticalGrid
                or ImpactObjectTypes.FourBlocksSquareGrid
                or ImpactObjectTypes.LShapeImpactObject;
        }

        private void SetupGridOccupation()
        {
            if (initialCells == null || initialCells.Count == 0)
            {
                Debug.LogWarning($"{name} has no initial grid cells assigned.");
                return;
            }

            Vector3 newPosition = grid.MarkOccupied(this, UsedCells);
            transform.position = newPosition;
        }

        public void Activate()
        {
            if (_activated) return;
            _activated = true;
            _impactObject.StartImpact();
        }

        public void UpdateObject(Vector3 direction)
        {
            if (_updated || direction.Equals(Vector3.zero)) return;
            _updated = true;
            Vector3 snapped = GetClosestCardinalDirection(direction);
            IsBlocked = false;
            _impactObject.UpdateImpact(snapped);
        }

        public void DeActivate()
        {
            if (!_activated) return;
            _activated = false;
            _impactObject.StopImpact();
        }

        private void Update()
        {
            _updated = false;
        }

        private void OnDrawGizmos()
        {
            _impactObject?.DrawGizmos();
        }

        private Vector3 GetClosestCardinalDirection(Vector3 direction)
        {
            direction.y = 0;
            direction.Normalize();

            Vector3[] cardinalDirections =
            {
                Vector3.forward,
                Vector3.back,
                Vector3.right,
                Vector3.left
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
    
        public void HighlightObject()
        {
            if (! IsMoveable) return;
            foreach (var renderer in renderers)
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
                    material.SetColor("_OutlineColor", impactColor);
                }
                if (material.HasProperty("_OutlineWidth"))
                {
                    material.SetFloat("_OutlineWidth", width);
                }
                if (material.HasProperty("_OutlineScale"))
                {
                    material.SetFloat("_OutlineScale", scale);
                }
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
                if (material.HasProperty("_OutlineEnabled"))
                {
                    material.SetInt("_OutlineEnabled", 0);
                }
                material.DisableKeyword("DR_OUTLINE_ON");
            }
        }
        
        private Color GetHDRColor(Color baseColor, float intensity)
        {
            return new Color(baseColor.r * intensity, baseColor.g * intensity, baseColor.b * intensity, baseColor.a);
        }
        

    }
}