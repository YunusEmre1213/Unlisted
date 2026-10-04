using System.Collections;
using UnityEngine;
using Backrooms.Player;
using Backrooms.UI;

namespace Backrooms.Rooms
{
    [System.Serializable]
    public class TransformStage
    {
        public GameObject[] objectsToDisable;
        public GameObject[] objectsToEnable;
        public float blinkDuration = 0.3f;
        public float pauseAfter = 0.5f;
        public AudioClip sound;
        public float soundVolume = 0.8f;
        public float shakeMagnitude;
        public float shakeDuration = 0.5f;
        public float dangerDistortionDuration;
    }

    [RequireComponent(typeof(Collider))]
    public class RoomTransformTrigger : MonoBehaviour
    {
        [Header("Referanslar")]
        [SerializeField] private NaturalBlink naturalBlink;

        [Header("On Belirti")]
        [SerializeField] private float anticipationDuration = 3f;
        [SerializeField] private Light[] anticipationLights;
        [SerializeField] private AudioClip anticipationSound;
        [SerializeField] private float anticipationSoundVolume = 0.7f;
        [SerializeField] private float uneaseTailDuration = 2f;

        [Header("Asamalar")]
        [SerializeField] private TransformStage[] stages;

        private bool hasTriggered;
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasTriggered) return;
            if (!other.CompareTag("Player")) return;

            hasTriggered = true;
            StartCoroutine(PlaySequence());
        }

        private IEnumerator PlaySequence()
        {
            if (naturalBlink != null)
            {
                naturalBlink.enabled = false;
            }

            StartUneasePulse();

            if (anticipationSound != null)
            {
                audioSource.PlayOneShot(anticipationSound, anticipationSoundVolume);
            }

            yield return StartCoroutine(AnticipationRoutine());

            foreach (TransformStage stage in stages)
            {
                yield return StartCoroutine(PlayStage(stage));
            }

            if (naturalBlink != null)
            {
                naturalBlink.enabled = true;
            }
        }

        private void StartUneasePulse()
        {
            if (ThreatVisualDistortion.Instance == null) return;

            float total = anticipationDuration + uneaseTailDuration;

            foreach (TransformStage stage in stages)
            {
                total += stage.blinkDuration + stage.pauseAfter;
            }

            ThreatVisualDistortion.Instance.PulseUnease(total);
        }

        private IEnumerator AnticipationRoutine()
        {
            float[] baseIntensities = new float[anticipationLights.Length];

            for (int i = 0; i < anticipationLights.Length; i++)
            {
                if (anticipationLights[i] != null)
                {
                    baseIntensities[i] = anticipationLights[i].intensity;
                }
            }

            float elapsed = 0f;

            while (elapsed < anticipationDuration)
            {
                elapsed += Time.deltaTime;

                float progress = Mathf.Clamp01(elapsed / anticipationDuration);
                float speed = Mathf.Lerp(3f, 22f, progress);
                float noise = Mathf.Clamp01((Mathf.PerlinNoise(Time.time * speed, 0f) - 0.25f) * 2f);
                float multiplier = Mathf.Lerp(1f, noise, progress);

                for (int i = 0; i < anticipationLights.Length; i++)
                {
                    if (anticipationLights[i] != null)
                    {
                        anticipationLights[i].intensity = baseIntensities[i] * multiplier;
                    }
                }

                yield return null;
            }

            for (int i = 0; i < anticipationLights.Length; i++)
            {
                if (anticipationLights[i] != null)
                {
                    anticipationLights[i].intensity = baseIntensities[i];
                }
            }
        }

        private IEnumerator PlayStage(TransformStage stage)
        {
            if (ScreenBlink.Instance != null)
            {
                ScreenBlink.Instance.Blink(stage.blinkDuration, () => ApplyStage(stage));
            }
            else
            {
                ApplyStage(stage);
            }

            yield return new WaitForSeconds(stage.blinkDuration + stage.pauseAfter);
        }

        private void ApplyStage(TransformStage stage)
        {
            foreach (GameObject obj in stage.objectsToDisable)
            {
                if (obj != null) obj.SetActive(false);
            }

            foreach (GameObject obj in stage.objectsToEnable)
            {
                if (obj != null) obj.SetActive(true);
            }

            if (stage.sound != null)
            {
                audioSource.PlayOneShot(stage.sound, stage.soundVolume);
            }

            if (stage.shakeMagnitude > 0f && CameraShake.Instance != null)
            {
                CameraShake.Instance.Shake(stage.shakeDuration, stage.shakeMagnitude);
            }

            if (stage.dangerDistortionDuration > 0f && ThreatVisualDistortion.Instance != null)
            {
                ThreatVisualDistortion.Instance.PulseDanger(stage.dangerDistortionDuration);
            }
        }
    }
}