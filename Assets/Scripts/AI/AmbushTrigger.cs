using UnityEngine;

namespace Backrooms.AI
{
    [RequireComponent(typeof(Collider))]
    public class AmbushTrigger : MonoBehaviour
    {
        [SerializeField] private ThreatController threatController;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private bool triggerOnce = true;

        private bool hasTriggered;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (triggerOnce && hasTriggered) return;
            if (threatController == null || spawnPoint == null) return;

            hasTriggered = true;
            threatController.Ambush(spawnPoint.position, spawnPoint.rotation);
        }
    }
}