using System.Collections.Generic;
using UnityEngine;
using Backrooms.Core;

namespace Backrooms.Items
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        private readonly HashSet<string> collectedItemIds = new HashSet<string>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            EventBus.Subscribe<ItemCollectedEvent>(OnItemCollected);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ItemCollectedEvent>(OnItemCollected);
        }

        private void OnItemCollected(ItemCollectedEvent evt)
        {
            if (evt.Item == null) return;

            collectedItemIds.Add(evt.Item.itemId);
            Debug.Log("[Inventory] Toplanan obje: " + evt.Item.displayName);
        }

        public bool HasItem(string itemId)
        {
            return collectedItemIds.Contains(itemId);
        }

        public bool HasAllItems(IEnumerable<string> itemIds)
        {
            foreach (var id in itemIds)
            {
                if (!collectedItemIds.Contains(id)) return false;
            }

            return true;
        }
    }
}