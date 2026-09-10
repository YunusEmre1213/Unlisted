using UnityEngine;
using Backrooms.Core;

namespace Backrooms.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Hareket Hizlari")]
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float sprintSpeed = 7f;
        [SerializeField] private float crouchSpeed = 2f;

        [Header("Stamina")]
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float staminaDrainRate = 20f;
        [SerializeField] private float staminaRegenRate = 15f;
        [SerializeField] private float crouchRegenMultiplier = 1.5f;
        [SerializeField] private float sprintReenableThreshold = 25f;

        [Header("Ziplama")]
        [SerializeField] private float jumpHeight = 1.2f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private AudioClip jumpStartClip;
        [SerializeField] private AudioClip jumpLandClip;
        [SerializeField] private float jumpVolume = 0.6f;
        [SerializeField] private float minAirTimeForLandSound = 0.15f;

        [Header("Egilme")]
        [SerializeField] private float standingControllerHeight = 1.8f;
        [SerializeField] private float crouchingControllerHeight = 1f;
        [SerializeField] private Vector3 standingControllerCenter = new Vector3(0f, 1f, 0f);
        [SerializeField] private Vector3 crouchingControllerCenter = new Vector3(0f, 0.5f, 0f);
        [SerializeField] private float standingCameraHeight = 1.6f;
        [SerializeField] private float crouchingCameraHeight = 1f;
        [SerializeField] private float crouchTransitionSpeed = 8f;

        public bool IsSprinting { get; private set; }
        public bool IsCrouching { get; private set; }
        public bool IsExhausted { get; private set; }
        public float StaminaPercent => currentStamina / maxStamina;
        public float CurrentCameraHeight => currentCameraHeight;

        private CharacterController controller;
        private AudioSource audioSource;
        private Vector3 verticalVelocity;
        private float currentStamina;
        private float currentCameraHeight;

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
            currentStamina = maxStamina;
            currentCameraHeight = standingCameraHeight;
        }

        private void Update()
        {
            HandleCrouchInput();
            HandleStamina();
            HandleCrouchTransition();
            HandleMovementAndJump();
        }

        private void HandleCrouchInput()
        {
            IsCrouching = Input.GetKey(KeyCode.LeftControl);
        }

        private void HandleStamina()
        {
            Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0f, controller.velocity.z);
            bool isMoving = horizontalVelocity.magnitude > 0.1f;
            bool wantsSprint = Input.GetKey(KeyCode.LeftShift) && !IsCrouching;

            bool canSprint = wantsSprint && isMoving && !IsExhausted && currentStamina > 0f;
            IsSprinting = canSprint;

            if (IsSprinting)
            {
                currentStamina -= staminaDrainRate * Time.deltaTime;

                if (currentStamina <= 0f)
                {
                    currentStamina = 0f;
                    IsExhausted = true;
                }
            }
            else
            {
                float regenRate = IsCrouching ? staminaRegenRate * crouchRegenMultiplier : staminaRegenRate;
                currentStamina = Mathf.Min(currentStamina + regenRate * Time.deltaTime, maxStamina);

                if (IsExhausted && currentStamina >= sprintReenableThreshold)
                {
                    IsExhausted = false;
                }
            }
        }

        private void HandleCrouchTransition()
        {
            float targetControllerHeight = IsCrouching ? crouchingControllerHeight : standingControllerHeight;
            Vector3 targetControllerCenter = IsCrouching ? crouchingControllerCenter : standingControllerCenter;

            controller.height = Mathf.Lerp(controller.height, targetControllerHeight, Time.deltaTime * crouchTransitionSpeed);
            controller.center = Vector3.Lerp(controller.center, targetControllerCenter, Time.deltaTime * crouchTransitionSpeed);

            float targetCameraHeight = IsCrouching ? crouchingCameraHeight : standingCameraHeight;
            currentCameraHeight = Mathf.Lerp(currentCameraHeight, targetCameraHeight, Time.deltaTime * crouchTransitionSpeed);
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
                    EventBus.Publish(new PlayerLandedEvent());
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