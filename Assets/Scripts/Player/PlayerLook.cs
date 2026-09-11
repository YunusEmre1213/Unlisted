using UnityEngine;

namespace Backrooms.Player
{
    public class PlayerLook : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float minPitch = -80f;
        [SerializeField] private float maxPitch = 80f;

        private float pitch;
        private bool yawLocked;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            if (!yawLocked)
            {
                transform.Rotate(Vector3.up * mouseX);
            }

            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        public void SetYawLocked(bool locked)
        {
            yawLocked = locked;
        }
    }
}