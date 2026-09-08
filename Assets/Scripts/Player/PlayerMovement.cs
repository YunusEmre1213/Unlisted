using UnityEngine;

namespace Backrooms.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Hareket Hizlari")]
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float sprintSpeed = 7f;
        [SerializeField] private float crouchSpeed = 2f;

        [Header("Ziplama")]
        [SerializeField] private float jumpHeight = 1.2f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private AudioClip jumpStartClip;
        [SerializeField] private AudioClip jumpLandClip;
        [SerializeField] private float jumpVolume = 0.6f;
        [SerializeField] private float minAirTimeForLandSound = 0.15f;

        [Header("Egilme")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float standingControllerHeight = 1.8f;
        [SerializeField] private float crouchingControllerHeight = 1f;
        [SerializeField] private Vector3 standingControllerCenter = new Vector3(0f, 1f, 0f);
        [SerializeField] private Vector3 crouchingControllerCenter = new Vector3(0f, 0.5f, 0f);
        [SerializeField] private float standingCameraHeight = 1.6f;
        [SerializeField] private float crouchingCameraHeight = 1f;
        [SerializeField] private float crouchTransitionSpeed = 8f;

        public bool IsSprinting { get; private set; }
        public bool IsCrouching { get; private set; }

        private CharacterController controller;
        private AudioSource audioSource;
        private Vector3 verticalVelocity;

        private bool wasGroundedLastFrame;
        private float airTime;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            controller.height = standingControllerHeight;
            controller.center = standingControllerCenter;

            wasGroundedLastFrame = true;
        }

        private void Update()
        {
            HandleCrouchInput();
            HandleCrouchTransition();
            HandleMovementAndJump();
        }

        private void HandleCrouchInput()
        {
            IsCrouching = Input.GetKey(KeyCode.LeftControl);
            IsSprinting = Input.GetKey(KeyCode.LeftShift) && !IsCrouching;
        }

        private void HandleCrouchTransition()
        {
            float targetControllerHeight = IsCrouching ? crouchingControllerHeight : standingControllerHeight;
            Vector3 targetControllerCenter = IsCrouching ? crouchingControllerCenter : standingControllerCenter;

            controller.height = Mathf.Lerp(controller.height, targetControllerHeight, Time.deltaTime * crouchTransitionSpeed);
            controller.center = Vector3.Lerp(controller.center, targetControllerCenter, Time.deltaTime * crouchTransitionSpeed);

            if (cameraTransform != null)
            {
                float targetCameraHeight = IsCrouching ? crouchingCameraHeight : standingCameraHeight;
                Vector3 camPos = cameraTransform.localPosition;
                camPos.y = Mathf.Lerp(camPos.y, targetCameraHeight, Time.deltaTime * crouchTransitionSpeed);
                cameraTransform.localPosition = camPos;
            }
        }

        private void HandleMovementAndJump()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            float currentSpeed = IsCrouching ? crouchSpeed : (IsSprinting ? sprintSpeed : walkSpeed);
            Vector3 horizontalMove = (transform.right * horizontal + transform.forward * vertical) * currentSpeed;

            bool isGroundedNow = controller.isGrounded;

            if (isGroundedNow)
            {
                if (!wasGroundedLastFrame && airTime >= minAirTimeForLandSound)
                {
                    PlayLandSound();
                }

                airTime = 0f;

                if (verticalVelocity.y < 0f)
                {
                    verticalVelocity.y = -2f;
                }

                if (Input.GetKeyDown(KeyCode.Space) && !IsCrouching)
                {
                    verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                    PlayJumpStartSound();
                }
            }
            else
            {
                airTime += Time.deltaTime;
            }

            verticalVelocity.y += gravity * Time.deltaTime;

            Vector3 totalMove = horizontalMove + verticalVelocity;
            controller.Move(totalMove * Time.deltaTime);

            wasGroundedLastFrame = isGroundedNow;
        }

        private void PlayJumpStartSound()
        {
            if (jumpStartClip == null) return;
            audioSource.PlayOneShot(jumpStartClip, jumpVolume);
        }

        private void PlayLandSound()
        {
            if (jumpLandClip == null) return;
            audioSource.PlayOneShot(jumpLandClip, jumpVolume);
        }
    }
}