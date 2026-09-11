using UnityEngine;

namespace Backrooms.Player
{
    public class PlayerHiding : MonoBehaviour
    {
        public bool IsHiding { get; private set; }

        private PlayerMovement playerMovement;
        private CharacterController controller;
        private Vector3 previousPosition;

        private void Awake()
        {
            playerMovement = GetComponent<PlayerMovement>();
            controller = GetComponent<CharacterController>();
        }

        public void EnterHiding(Vector3 hidePosition)
        {
            if (IsHiding) return;

            IsHiding = true;
            previousPosition = transform.position;

            controller.enabled = false;
            transform.position = hidePosition;
            controller.enabled = true;

            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }
        }

        public void ExitHiding()
        {
            if (!IsHiding) return;

            IsHiding = false;

            controller.enabled = false;
            transform.position = previousPosition;
            controller.enabled = true;

            if (playerMovement != null)
            {
                playerMovement.enabled = true;
            }
        }
    }
}