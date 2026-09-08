using UnityEngine;

namespace Backrooms.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerFootsteps : MonoBehaviour
    {
        [Header("Yurume")]
        [SerializeField] private AudioClip[] walkClips;
        [SerializeField] private float walkStepInterval = 0.5f;
        [SerializeField] private float walkVolume = 0.5f;

        [Header("Kosma")]
        [SerializeField] private AudioClip[] runClips;
        [SerializeField] private float runStepInterval = 0.32f;
        [SerializeField] private float runVolume = 0.7f;

        [Header("Egilme")]
        [SerializeField] private float crouchStepInterval = 0.7f;
        [SerializeField] private float crouchVolume = 0.2f;

        [SerializeField] private float minMoveSpeed = 0.1f;

        private CharacterController controller;
        private PlayerMovement playerMovement;
        private AudioSource audioSource;
        private float stepTimer;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            playerMovement = GetComponent<PlayerMovement>();
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        private void Update()
        {
            Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0f, controller.velocity.z);

            if (controller.isGrounded && horizontalVelocity.magnitude > minMoveSpeed)
            {
                stepTimer -= Time.deltaTime;

                if (stepTimer <= 0f)
                {
                    PlayFootstep();
                    stepTimer = GetCurrentStepInterval();
                }
            }
            else
            {
                stepTimer = 0f;
            }
        }

        private float GetCurrentStepInterval()
        {
            if (playerMovement.IsCrouching) return crouchStepInterval;
            if (playerMovement.IsSprinting) return runStepInterval;
            return walkStepInterval;
        }

        private void PlayFootstep()
        {
            bool useRunClips = playerMovement.IsSprinting && !playerMovement.IsCrouching;
            AudioClip[] clipPool = useRunClips ? runClips : walkClips;
            float clipVolume = playerMovement.IsCrouching ? crouchVolume : (playerMovement.IsSprinting ? runVolume : walkVolume);

            if (clipPool == null || clipPool.Length == 0) return;

            AudioClip clip = clipPool[Random.Range(0, clipPool.Length)];
            audioSource.PlayOneShot(clip, clipVolume);
        }
    }
}