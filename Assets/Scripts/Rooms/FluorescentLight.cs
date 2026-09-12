using System.Collections;
using UnityEngine;

namespace Backrooms.Rooms
{
    public class FluorescentLight : MonoBehaviour
    {
        [SerializeField] private Light lightSource;
        [SerializeField] private float minInstanceStrength = 0.3f;
        [SerializeField] private float maxInstanceStrength = 1f;

        [Header("Titreme")]
        [SerializeField] private float flickerCapableChance = 0.4f;
        [SerializeField] private float flickerChance = 0.25f;
        [SerializeField] private float flickerMinIntensity = 0.2f;
        [SerializeField] private float minFlickerInterval = 2f;
        [SerializeField] private float maxFlickerInterval = 6f;
        [SerializeField] private float flickerDuration = 0.1f;

        [Header("Kalici Sonme")]
        [SerializeField] private AudioClip dyingSoundClip;

        private float baseIntensity;
        private float instanceIntensity;
        private float nextCheckTime;
        private float flickerEndTime;
        private bool isFlickering;
        private bool isExtinguished;
        private bool canFlicker;
        private AudioSource audioSource;

        private void Start()
        {
            if (lightSource != null)
            {
                baseIntensity = lightSource.intensity;
            }

            instanceIntensity = baseIntensity * Random.Range(minInstanceStrength, maxInstanceStrength);
            canFlicker = Random.value < flickerCapableChance;

            if (lightSource != null)
            {
                lightSource.intensity = instanceIntensity;
            }

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            if (canFlicker)
            {
                ScheduleNextCheck();
            }
        }

        private void Update()
        {
            if (isExtinguished) return;
            if (!canFlicker) return;

            if (isFlickering)
            {
                if (Time.time >= flickerEndTime)
                {
                    isFlickering = false;

                    if (lightSource != null)
                    {
                        lightSource.intensity = instanceIntensity;
                    }

                    ScheduleNextCheck();
                }

                return;
            }

            if (Time.time >= nextCheckTime)
            {
                if (Random.value < flickerChance)
                {
                    StartFlicker();
                }
                else
                {
                    ScheduleNextCheck();
                }
            }
        }

        private void StartFlicker()
        {
            isFlickering = true;
            flickerEndTime = Time.time + flickerDuration;

            if (lightSource != null)
            {
                lightSource.intensity = flickerMinIntensity;
            }
        }

        private void ScheduleNextCheck()
        {
            nextCheckTime = Time.time + Random.Range(minFlickerInterval, maxFlickerInterval);
        }

        public void Extinguish()
        {
            if (isExtinguished) return;

            StartCoroutine(ExtinguishRoutine());
        }

        private IEnumerator ExtinguishRoutine()
        {
            isExtinguished = true;

            if (dyingSoundClip != null && audioSource != null)
            {
                audioSource.PlayOneShot(dyingSoundClip);
            }

            for (int i = 0; i < 3; i++)
            {
                if (lightSource != null) lightSource.intensity = flickerMinIntensity;
                yield return new WaitForSeconds(0.08f);

                if (lightSource != null) lightSource.intensity = instanceIntensity;
                yield return new WaitForSeconds(0.12f);
            }

            if (lightSource != null)
            {
                lightSource.intensity = 0f;
            }
        }
    }
}