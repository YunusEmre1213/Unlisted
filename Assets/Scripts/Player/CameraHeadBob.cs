using UnityEngine;
using Backrooms.Core;

namespace Backrooms.Player
{
    public class CameraHeadBob : MonoBehaviour
    {
        [Header("Referanslar")]
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private CharacterController controller;

        [Header("Yurume")]
        [SerializeField] private float walkFrequency = 6f;
        [SerializeField] private float walkVerticalAmplitude = 0.05f;
        [SerializeField] private float walkHorizontalAmplitude = 0.025f;

        [Header("Kosma")]
        [SerializeField] private float runFrequency = 10f;
        [SerializeField] private float runVerticalAmplitude = 0.08f;
        [SerializeField] private float runHorizontalAmplitude = 0.04f;

        [Header("Egilme")]
        [SerializeField] private float crouchFrequency = 4f;
        [SerializeField] private float crouchVerticalAmplitude = 0.02f;
        [SerializeField] private float crouchHorizontalAmplitude = 0.01f;

        [Header("Yorgunluk Titremesi")]
        [SerializeField] private float exhaustedJitterAmount = 0.015f;

        [Header("Inis Sarsintisi")]
        [SerializeField] private float landDipAmount = 0.12f;
        [SerializeField] private float landDipRecoverySpeed = 6f;

        [Header("Genel")]
        [SerializeField] private float blendSpeed = 8f;
        [SerializeField] private PlayerHiding playerHiding;

        public Vector3 ShakeOffset { get; set; }

        private Vector3 baseHorizontalPosition;
        private float bobTimer;
        private float currentVerticalAmplitude;
        private float currentHorizontalAmplitude;
        private float currentFrequency;
        private float landDipOffset;

        private void Awake()
        {
            baseHorizontalPosition = new Vector3(transform.localPosition.x, 0f, transform.localPosition.z);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerLandedEvent>(OnPlayerLanded);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerLandedEvent>(OnPlayerLanded);
        }

        private void OnPlayerLanded(PlayerLandedEvent evt)
        {
            landDipOffset = landDipAmount;
        }

        private void Update()
        {
            bool isHiding = playerHiding != null && playerHiding.IsHiding;
            Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0f, controller.velocity.z);
            bool isMoving = !isHiding && controller.isGrounded && horizontalVelocity.magnitude > 0.1f;

            float targetFrequency = walkFrequency;
            float targetVerticalAmplitude = 0f;
            float targetHorizontalAmplitude = 0f;

            if (isMoving)
            {
                if (playerMovement.IsCrouching)
                {
                    targetFrequency = crouchFrequency;
                    targetVerticalAmplitude = crouchVerticalAmplitude;
                    targetHorizontalAmplitude = crouchHorizontalAmplitude;
                }
                else if (playerMovement.IsSprinting)
                {
                    targetFrequency = runFrequency;
                    targetVerticalAmplitude = runVerticalAmplitude;
                    targetHorizontalAmplitude = runHorizontalAmplitude;
                }
                else
                {
                    targetFrequency = walkFrequency;
                    targetVerticalAmplitude = walkVerticalAmplitude;
                    targetHorizontalAmplitude = walkHorizontalAmplitude;
                }
            }

            currentFrequency = Mathf.Lerp(currentFrequency, targetFrequency, Time.deltaTime * blendSpeed);
            currentVerticalAmplitude = Mathf.Lerp(currentVerticalAmplitude, targetVerticalAmplitude, Time.deltaTime * blendSpeed);
            currentHorizontalAmplitude = Mathf.Lerp(currentHorizontalAmplitude, targetHorizontalAmplitude, Time.deltaTime * blendSpeed);

            if (isMoving)
            {
                bobTimer += Time.deltaTime * currentFrequency;
            }

            float verticalBob = Mathf.Sin(bobTimer) * currentVerticalAmplitude;
            float horizontalBob = Mathf.Cos(bobTimer * 0.5f) * currentHorizontalAmplitude;

            if (playerMovement.IsExhausted && isMoving)
            {
                verticalBob += (Mathf.PerlinNoise(Time.time * 5f, 0f) - 0.5f) * exhaustedJitterAmount;
                horizontalBob += (Mathf.PerlinNoise(0f, Time.time * 5f) - 0.5f) * exhaustedJitterAmount;
            }

            landDipOffset = Mathf.Lerp(landDipOffset, 0f, Time.deltaTime * landDipRecoverySpeed);

            float finalHeight = playerMovement.CurrentCameraHeight + verticalBob - landDipOffset;
            float finalX = baseHorizontalPosition.x + horizontalBob;
            float finalZ = baseHorizontalPosition.z;

            transform.localPosition = new Vector3(finalX, finalHeight, finalZ) + ShakeOffset;
        }
    }
}