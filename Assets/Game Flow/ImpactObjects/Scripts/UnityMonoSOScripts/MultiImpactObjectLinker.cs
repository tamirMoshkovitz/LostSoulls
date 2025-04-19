using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts
{
    public class MultiImpactObjectLinker : MonoBehaviour
    {
        [SerializeField] private List<MonoImpactObject> linkedObjects = new();
        private Dictionary<MonoImpactObject, Vector3> lastPositions = new();

        void Start()
        {
            foreach (var obj in linkedObjects)
            {
                lastPositions[obj] = obj.transform.position;
            }
        }

        void LateUpdate()
        {
            MonoImpactObject reference = null;
            Vector3 delta = Vector3.zero;

            // Step 1: Find the first object that moved this frame
            foreach (var obj in linkedObjects)
            {
                Vector3 current = obj.transform.position;
                Vector3 last = lastPositions[obj];
                Vector3 move = current - last;

                if (move != Vector3.zero)
                {
                    reference = obj;
                    delta = move;
                    break;
                }
            }

            // Step 2: If no one moved, do nothing
            if (reference == null) return;
            
            // Step 3: Check for any blockers
            bool anyBlocked = false;
            foreach (var obj in linkedObjects)
            {
                if (obj.IsBlocked)
                {
                    anyBlocked = true;
                    break;
                }
            }
            foreach (var obj in linkedObjects)
            {
                obj.transform.position = lastPositions[obj];
            }
            if(!anyBlocked)
            {
                // Apply delta to all others
                foreach (var obj in linkedObjects)
                {
                    obj.transform.position += delta;
                }
            }

            UpdateLastPositions();
        }

        private void UpdateLastPositions()
        {
            foreach (var obj in linkedObjects)
            {
                lastPositions[obj] = obj.transform.position;
            }
        }

        public void ActivateSiblings(Vector3 snapped)
        {
            foreach (var obj in linkedObjects)
            {
                   obj.Activate(snapped);
            }
        }
    }
}
