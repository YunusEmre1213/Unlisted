using UnityEngine;
using Backrooms.Player;

namespace Backrooms.Rooms
{
    [RequireComponent(typeof(Collider))]
    public class CinematicTrigger : MonoBehaviour
    {
        [SerializeField] private string[] subtitleLines;
        [SerializeField] private float zoomedFieldOfView = 50f;
        [SerializeField] private bool triggerOnce = true;

        private bool hasTriggered;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (triggerOnce && hasTriggered) return;
            if (CinematicMoment.Instance == null) return;

            hasTriggered = true;
            CinematicMoment.Instance.Play(subtitleLines, zoomedFieldOfView);
        }
    }
}