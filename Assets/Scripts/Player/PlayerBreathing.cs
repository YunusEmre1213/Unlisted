using System.Collections;
using UnityEngine;

namespace Backrooms.Player
{
    public enum BreathingState
    {
        Calm,
        Light,
        Heavy
    }

    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerBreathing : MonoBehaviour
    {
        [Header("Esikler (Stamina Yuzdesi)")]
        [SerializeField] private float lightBreathingThreshold = 0.6f;
        [SerializeField] private float heavyBreathingThreshold = 0.25f;

        [Header("Klipler")]
        [SerializeField] private AudioClip calmClip;
        [SerializeField] private AudioClip lightClip;
        [SerializeField] private AudioClip heavyClip;

        [Header("Sesler")]
        [SerializeField] private float calmVolume = 0.15f;
        [SerializeField] private float lightVolume = 0.3f;
        [SerializeField] private float heavyVolume = 0.5f;
        [SerializeField] private float crossfadeDuration = 0.8f;

        public BreathingState CurrentState { get; private set; } = BreathingState.Calm;
        public bool IsHeavyBreathing => CurrentState == BreathingState.Heavy;

        private PlayerMovement playerMovement;
        private AudioSource sourceA;
        private AudioSource sourceB;
        private AudioSource activeSource;
        private AudioSource inactiveSource;
        private Coroutine crossfadeCoroutine;

        private void Awake()
        {
            playerMovement = GetComponent<PlayerMovement>();

            sourceA = gameObject.AddComponent<AudioSource>();
            sourceA.loop = true;
            sourceA.playOnAwake = false;

            sourceB = gameObject.AddComponent<AudioSource>();
            sourceB.loop = true;
            sourceB.playOnAwake = false;

            activeSource = sourceA;
            inactiveSource = sourceB;
        }

        private void Start()
        {
            if (calmClip != null)
            {
                activeSource.clip = calmClip;
                activeSource.volume = calmVolume;
                activeSource.Play();
            }
        }

        private void Update()
        {
            BreathingState newState = DetermineState();

            if (newState != CurrentState)
            {
                CurrentState = newState;
                TransitionTo(newState);
            }
        }

        private BreathingState DetermineState()
        {
            float staminaPercent = playerMovement.StaminaPercent;

            if (staminaPercent <= heavyBreathingThreshold || playerMovement.IsExhausted)
            {
                return BreathingState.Heavy;
            }

            if (staminaPercent <= lightBreathingThreshold)
            {
                return BreathingState.Light;
            }

            return BreathingState.Calm;
        }

        private void TransitionTo(BreathingState state)
        {
            AudioClip newClip = calmClip;
            float newVolume = calmVolume;

            switch (state)
            {
                case BreathingState.Light:
                    newClip = lightClip;
                    newVolume = lightVolume;
                    break;
                case BreathingState.Heavy:
                    newClip = heavyClip;
                    newVolume = heavyVolume;
                    break;
            }

            if (newClip == null) return;

            if (crossfadeCoroutine != null)
            {
                StopCoroutine(crossfadeCoroutine);
            }

            crossfadeCoroutine = StartCoroutine(CrossfadeRoutine(newClip, newVolume));
        }

        private IEnumerator CrossfadeRoutine(AudioClip newClip, float targetVolume)
        {
            inactiveSource.clip = newClip;
            inactiveSource.volume = 0f;
            inactiveSource.Play();

            float startVolume = activeSource.volume;
            float elapsed = 0f;

            while (elapsed < crossfadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / crossfadeDuration;

                inactiveSource.volume = Mathf.Lerp(0f, targetVolume, t);
                activeSource.volume = Mathf.Lerp(startVolume, 0f, t);

                yield return null;
            }

            inactiveSource.volume = targetVolume;
            activeSource.volume = 0f;
            activeSource.Stop();

            AudioSource temp = activeSource;
            activeSource = inactiveSource;
            inactiveSource = temp;
        }
    }
}