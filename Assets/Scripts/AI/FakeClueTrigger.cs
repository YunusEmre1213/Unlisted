using UnityEngine;
using Backrooms.Core;

namespace Backrooms.AI
{
    [RequireComponent(typeof(Collider))]
    public class FakeClueTrigger : MonoBehaviour
    {
        [SerializeField] private float cooldown = 8f;

        private float lastTriggerTime = -100f;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (Time.time - lastTriggerTime < cooldown) return;

            Debug.Log("[FakeClue] Sahte ipucu tetiklendi: " + gameObject.name);
            EventBus.Publish(new ThreatStateChangedEvent(ThreatStateType.FakeClue));

            lastTriggerTime = Time.time;
        }
    }
}