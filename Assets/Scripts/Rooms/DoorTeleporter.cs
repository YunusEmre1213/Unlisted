using UnityEngine;

namespace Backrooms.Rooms
{
    [RequireComponent(typeof(Collider))]
    public class DoorTeleporter : MonoBehaviour
    {
        [SerializeField] private Transform destination;
        [SerializeField] private float teleportCooldown = 0.5f;

        private static float lastTeleportTime = -10f;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (Time.time - lastTeleportTime < teleportCooldown) return;
            if (destination == null) return;

            CharacterController controller = other.GetComponent<CharacterController>();

            if (controller != null)
            {
                controller.enabled = false;
                other.transform.position = destination.position;
                other.transform.rotation = destination.rotation;
                controller.enabled = true;
            }
            else
            {
                other.transform.position = destination.position;
                other.transform.rotation = destination.rotation;
            }

            lastTeleportTime = Time.time;
        }
    }
}