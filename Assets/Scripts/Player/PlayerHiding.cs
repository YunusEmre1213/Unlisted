using UnityEngine;
using Backrooms.Rooms;

namespace Backrooms.Player
{
    public class PlayerHiding : MonoBehaviour
    {
        public bool IsHiding { get; private set; }
        public HidingSpot CurrentSpot { get; private set; }

        private PlayerMovement playerMovement;
        private PlayerLook playerLook;
        private CharacterController controller;
        private Vector3 previousPosition;
        private Quaternion previousRotation;

        private void Awake()
        {
            playerMovement = GetComponent<PlayerMovement>();
            playerLook = GetComponent<PlayerLook>();
            controller = GetComponent<CharacterController>();
        }

        public void EnterHiding(Vector3 hidePosition, Quaternion hideRotation, HidingSpot spot)
        {
            if (IsHiding) return;

            IsHiding = true;
            CurrentSpot = spot;
            previousPosition = transform.position;
            previousRotation = transform.rotation;

            controller.enabled = false;
            transform.position = hidePosition;
            transform.rotation = hideRotation;
            controller.enabled = true;

            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }

            if (playerLook != null)
            {
                playerLook.SetYawLocked(true);
            }
        }

        public void ExitHiding()
        {
            if (!IsHiding) return;

            IsHiding = false;
            CurrentSpot = null;

            controller.enabled = false;
            transform.position = previousPosition;
            transform.rotation = previousRotation;
            controller.enabled = true;

            if (playerMovement != null)
            {
                playerMovement.enabled = true;
            }

            if (playerLook != null)
            {
                playerLook.SetYawLocked(false);
            }
        }
    }
}