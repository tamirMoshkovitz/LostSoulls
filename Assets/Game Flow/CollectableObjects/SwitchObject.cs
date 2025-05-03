using UnityEngine;

namespace Game_Flow.CollectableObjects
{
    public class SwitchObject:MonoBehaviour
    {
        [SerializeField] private GameObject objectToSwitch;
        public void ControlLights()
        {
            objectToSwitch.SetActive(!objectToSwitch.activeSelf);
        }
    }
}