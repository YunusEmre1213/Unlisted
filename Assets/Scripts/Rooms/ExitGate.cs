using UnityEngine;
using Backrooms.Items;
using Backrooms.Core;

namespace Backrooms.Rooms
{
    [RequireComponent(typeof(Collider))]
    public class ExitGate : MonoBehaviour
    {
        [SerializeField] private string[] requiredItemIds;
        [SerializeField] private float triggerCooldown = 2f;
        [SerializeField] private AudioSource deniedAudioSource;

        private float lastTriggerTime = -10f;
        private bool isUnlocked;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (isUnlocked) return;
            if (Time.time - lastTriggerTime < triggerCooldown) return;

            lastTriggerTime = Time.time;

            if (CheckRequiredItems())
            {
                isUnlocked = true;
                Debug.Log("[ExitGate] Tum parcalar tamam, cikis acildi.");
                EventBus.Publish(new ExitUnlockedEvent());
            }
            else
            {
                Debug.Log("[ExitGate] Eksik parca var, cikis reddedildi.");

                if (deniedAudioSource != null)
                {
                    deniedAudioSource.Play();
                }

                EventBus.Publish(new ExitDeniedEvent());
            }
        }

        private bool CheckRequiredItems()
        {
            if (InventoryManager.Instance == null) return false;
            return InventoryManager.Instance.HasAllItems(requiredItemIds);
        }
    }
}