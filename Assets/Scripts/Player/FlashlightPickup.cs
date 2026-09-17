using UnityEngine;
using Backrooms.Core;
using Backrooms.UI;

namespace Backrooms.Player
{
    [RequireComponent(typeof(Collider))]
    public class FlashlightPickup : MonoBehaviour, IInteractable
    {
        [SerializeField] private FlashlightController flashlightController;
        [SerializeField] private AudioSource pickupAudioSource;
        [SerializeField] private string subtitleLine = "Nihayet... bir fener.";
        [SerializeField] private float subtitleDuration = 2.5f;

        public void Interact()
        {
            if (flashlightController == null) return;
            if (flashlightController.HasFlashlight) return;

            flashlightController.Acquire();

            if (pickupAudioSource != null)
            {
                pickupAudioSource.Play();
            }

            if (SubtitleManager.Instance != null && !string.IsNullOrEmpty(subtitleLine))
            {
                SubtitleManager.Instance.ShowLine(subtitleLine, subtitleDuration);
            }

            gameObject.SetActive(false);
        }
    }
}