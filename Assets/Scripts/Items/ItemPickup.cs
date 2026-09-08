using UnityEngine;
using Backrooms.Core;

namespace Backrooms.Items
{
    [RequireComponent(typeof(Collider))]
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] private ItemData itemData;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (itemData == null) return;

            EventBus.Publish(new ItemCollectedEvent(itemData));
            gameObject.SetActive(false);
        }
    }
}