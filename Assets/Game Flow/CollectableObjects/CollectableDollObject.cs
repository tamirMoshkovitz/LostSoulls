using Game_Flow.ImpactObjects.Scripts.Types;
using Game_Flow.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace Game_Flow.CollectableObjects
{
    public class CollectableDollObject:MonoBehaviour
    {
        [SerializeField] private OpenCloseImpactObject showcaseDoor1;
        [SerializeField] private OpenCloseImpactObject showcaseDoor2;
        [SerializeField] private ItemsUpdater itemsUpdater;
        public void OnCollect()
        {
            if (showcaseDoor1.IsLocked || showcaseDoor2.IsLocked)
            {
                return;
            }
            itemsUpdater.AddItem(1);
            Destroy(gameObject);
        }
    }
}