using UnityEngine;
using Backrooms.Core;

namespace Backrooms.Player
{
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private float interactRange = 3f;
        [SerializeField] private PlayerHiding playerHiding;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                TryInteract();
            }
        }

        private void TryInteract()
        {
            if (playerHiding != null && playerHiding.IsHiding)
            {
                playerHiding.ExitHiding();
                return;
            }

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, interactRange, ~0, QueryTriggerInteraction.Ignore))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();

                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }
}