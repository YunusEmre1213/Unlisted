using System.Collections.Generic;
using UnityEngine;
using Backrooms.Core;

namespace Backrooms.Items
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        private readonly Dictionary<string, int> itemCounts = new Dictionary<string, int>();

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

            string id = evt.Item.itemId;

            if (itemCounts.ContainsKey(id))
            {
                itemCounts[id]++;
            }
            else
            {
                itemCounts[id] = 1;
            }

            Debug.Log("[Inventory] Toplanan obje: " + evt.Item.displayName + " (adet: " + itemCounts[id] + ")");
        }

        public bool HasItem(string itemId)
        {
            return itemCounts.ContainsKey(itemId) && itemCounts[itemId] > 0;
        }

        public bool HasAllItems(IEnumerable<string> itemIds)
        {
            foreach (var id in itemIds)
            {
                if (!HasItem(id)) return false;
            }

            return true;
        }

        public int GetItemCount(string itemId)
        {
            return itemCounts.ContainsKey(itemId) ? itemCounts[itemId] : 0;
        }

        public bool ConsumeItem(string itemId)
        {
            if (!HasItem(itemId)) return false;

            itemCounts[itemId]--;
            return true;
        }
    }
}