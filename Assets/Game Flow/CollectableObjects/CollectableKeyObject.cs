using Game_Flow.ImpactObjects.Scripts.Types;
using Game_Flow.UI;
using UnityEngine;

namespace Game_Flow.CollectableObjects
{
    public class CollectableKeyObject : MonoBehaviour
    {
        [SerializeField] private OpenCloseImpactObject showcaseDoor1;
        [SerializeField] private OpenCloseImpactObject showcaseDoor2;
        [SerializeField] private ItemsUpdater itemsUpdater;
        public void OnCollect()
        {
            showcaseDoor1.IsLocked = false;
            showcaseDoor2.IsLocked = false;
            itemsUpdater.AddItem(2);
            Destroy(gameObject);
        }
    }
}