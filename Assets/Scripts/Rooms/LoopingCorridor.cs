using UnityEngine;
using Backrooms.UI;

namespace Backrooms.Rooms
{
    [RequireComponent(typeof(Collider))]
    public class LoopingCorridor : MonoBehaviour
    {
        [SerializeField] private Transform loopBackPoint;
        [SerializeField] private float loopCooldown = 1f;
        [SerializeField] private float blinkDuration = 0.25f;
        [SerializeField] private AudioClip[] loopEventClips;
        [SerializeField] private float loopEventVolume = 0.6f;

        private float lastLoopTime = -10f;
        private AudioSource audioSource;
        private Collider pendingPlayerCollider;

        private void Awake()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (Time.time - lastLoopTime < loopCooldown) return;

            lastLoopTime = Time.time;
            pendingPlayerCollider = other;

            if (ScreenBlink.Instance != null)
            {
                ScreenBlink.Instance.Blink(blinkDuration, TeleportPlayer);
            }
            else
            {
                TeleportPlayer();
            }

            PlayRandomLoopEvent();
        }

        private void TeleportPlayer()
        {
            if (pendingPlayerCollider == null) return;

            CharacterController controller = pendingPlayerCollider.GetComponent<CharacterController>();

            if (controller != null)
            {
                controller.enabled = false;
                pendingPlayerCollider.transform.position = loopBackPoint.position;
                controller.enabled = true;
            }
            else
            {
                pendingPlayerCollider.transform.position = loopBackPoint.position;
            }
        }

        private void PlayRandomLoopEvent()
        {
            if (loopEventClips == null || loopEventClips.Length == 0) return;

            AudioClip clip = loopEventClips[Random.Range(0, loopEventClips.Length)];
            audioSource.PlayOneShot(clip, loopEventVolume);
        }
    }
}