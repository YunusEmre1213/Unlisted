using UnityEngine;

namespace Backrooms.Items
{
    public class ItemLock : MonoBehaviour
    {
        [SerializeField] private string[] requiredItemIds;

        public bool IsUnlocked()
        {
            if (requiredItemIds == null || requiredItemIds.Length == 0) return true;
            if (InventoryManager.Instance == null) return false;

            return InventoryManager.Instance.HasAllItems(requiredItemIds);
        }
    }
}