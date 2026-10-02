using UnityEngine;
using Backrooms.UI;

namespace Backrooms.Rooms
{
    [RequireComponent(typeof(Collider))]
    public class RoomTransformTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject[] objectsToDisable;
        [SerializeField] private GameObject[] objectsToEnable;
        [SerializeField] private float blinkDuration = 0.4f;
        [SerializeField] private AudioClip transitionSound;

        private bool hasTriggered;
        private AudioSource audioSource;

        private void Awake()
        {
            if (transitionSound != null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasTriggered) return;
            if (!other.CompareTag("Player")) return;

            hasTriggered = true;

            if (ScreenBlink.Instance != null)
            {
                ScreenBlink.Instance.Blink(blinkDuration, PerformSwap);
            }
            else
            {
                PerformSwap();
            }
        }

        private void PerformSwap()
        {
            foreach (var obj in objectsToDisable)
            {
                if (obj != null) obj.SetActive(false);
            }

            foreach (var obj in objectsToEnable)
            {
                if (obj != null) obj.SetActive(true);
            }

            if (transitionSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(transitionSound);
            }
        }
    }
}