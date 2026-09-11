using System.Collections;
using UnityEngine;
using Backrooms.Core;
using Backrooms.Player;

namespace Backrooms.Rooms
{
    [RequireComponent(typeof(Collider))]
    public class HidingSpot : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform hidePosition;
        [SerializeField] private LockerDoor door;
        [SerializeField] private float enterDelay = 1.1f;
        [SerializeField] private float autoCloseDelay = 0.8f;

        private PlayerHiding playerHiding;
        private bool isBusy;

        public void Interact()
        {
            if (isBusy) return;

            if (playerHiding == null)
            {
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null)
                {
                    playerHiding = playerObject.GetComponent<PlayerHiding>();
                }
            }

            if (playerHiding == null || playerHiding.IsHiding) return;

            StartCoroutine(EnterSequence());
        }

        public void RequestExit()
        {
            if (isBusy) return;
            if (playerHiding == null || !playerHiding.IsHiding) return;

            StartCoroutine(ExitSequence());
        }

        private IEnumerator EnterSequence()
        {
            isBusy = true;

            if (door != null)
            {
                door.Open();
                yield return new WaitForSeconds(enterDelay);
            }

            playerHiding.EnterHiding(hidePosition.position, CalculateFacingRotation(), this);

            if (door != null)
            {
                yield return new WaitForSeconds(autoCloseDelay);
                door.Close();
            }

            isBusy = false;
        }

        private IEnumerator ExitSequence()
        {
            isBusy = true;

            if (door != null)
            {
                door.Open();
                yield return new WaitForSeconds(enterDelay);
            }

            playerHiding.ExitHiding();

            if (door != null)
            {
                yield return new WaitForSeconds(autoCloseDelay);
                door.Close();
            }

            isBusy = false;
        }

        private Quaternion CalculateFacingRotation()
        {
            Vector3 lookDirection = transform.position - hidePosition.position;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude < 0.01f)
            {
                return hidePosition.rotation;
            }

            return Quaternion.LookRotation(lookDirection);
        }
    }
}