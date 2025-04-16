using System;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;

namespace Game_Flow.DotVisual.Scripts
{
    public class DotVisualController : MonoBehaviour
    {
        [SerializeField] private Transform rayOrigin;
        [SerializeField] private float rayLength = 50f;
        
        [SerializeField] private LayerMask targetLayers;
        
        [SerializeField] private GameObject dotPrefab;
        
        [SerializeField] private Color defaultColor = Color.red;
        [SerializeField] private Color hitColor = Color.green;
        
        private Renderer dotRenderer;
        private GameObject dotInstance;
        public MonoImpactObject CurrentTarget { get; private set; }

        public void Start()
        {
            if (dotPrefab != null)
            {
                dotInstance = Instantiate(dotPrefab);
                dotRenderer = dotInstance.GetComponent<Renderer>();
                dotRenderer.material.color = defaultColor;
            }
        }
        
        public void Update()
        {
            float sphereRadius = 1f;
            //Debug.DrawRay(rayOrigin.position, rayOrigin.forward * rayLength, Color.red);
            RaycastHit hit;
            if (Physics.SphereCast(rayOrigin.position, sphereRadius, rayOrigin.forward, out hit, rayLength, targetLayers))
            {
                dotInstance.transform.position = hit.point;
                var impactObject = hit.collider.GetComponent<MonoImpactObject>();
                if (impactObject != null)
                {
                    CurrentTarget = impactObject;
                    dotRenderer.material.color = hitColor;
                }
                else
                {
                    CurrentTarget = null;
                    dotRenderer.material.color = defaultColor;
                }
            }
            else
            {
                dotInstance.transform.position = rayOrigin.position + rayOrigin.forward * rayLength;
                CurrentTarget = null;
                dotRenderer.material.color = defaultColor;
            }
        }
    }
}