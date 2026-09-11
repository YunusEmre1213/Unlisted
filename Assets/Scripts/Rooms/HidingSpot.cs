using UnityEngine;
using Backrooms.Core;
using Backrooms.Player;

namespace Backrooms.Rooms
{
    [RequireComponent(typeof(Collider))]
    public class HidingSpot : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform hidePosition;
        [SerializeField] private AudioSource toggleAudioSource;

        private PlayerHiding playerHiding;

        public void Interact()
        {
            if (playerHiding == null)
            {
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null)
                {
                    playerHiding = playerObject.GetComponent<PlayerHiding>();
                }
            }

            if (playerHiding == null || playerHiding.IsHiding) return;

            playerHiding.EnterHiding(hidePosition.position);

            if (toggleAudioSource != null)
            {
                toggleAudioSource.Play();
            }
        }
    }
}