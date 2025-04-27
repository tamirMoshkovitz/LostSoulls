using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game_Flow.UI
{
    public class ItemsUpdater : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private RectTransform container;
        [SerializeField] private GameObject itemIconPrefab; // Prefab with an Image component

        [Header("Sprite References")]
        [SerializeField] private Sprite[] itemSprites; // Set order to match item IDs

        private readonly List<GameObject> activeIcons = new();

        
        
        public void AddItem(int itemId)
        {
            if (itemId < 0 || itemId >= itemSprites.Length)
            {
                Debug.LogWarning($"Invalid item ID: {itemId}");
                return;
            }

            GameObject icon = Instantiate(itemIconPrefab, container);
            icon.GetComponent<Image>().sprite = itemSprites[itemId];
            activeIcons.Add(icon);
        }

        public void RemoveItem(int index)
        {
            if (index < 0 || index >= activeIcons.Count) return;

            Destroy(activeIcons[index]);
            activeIcons.RemoveAt(index);
        }

    }
}