using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Backrooms.Core;
using Backrooms.AI;

namespace Backrooms.Player
{
    public class ThreatVisualDistortion : MonoBehaviour
    {
        public static ThreatVisualDistortion Instance { get; private set; }

        [SerializeField] private Volume volume;
        [SerializeField] private float transitionSpeed = 2f;
        [SerializeField] private float fakeClueRevertDelay = 4f;

        [Header("Sakin")]
        [SerializeField] private float calmGrain = 0.10f;
        [SerializeField] private float calmChromatic = 0.03f;
        [SerializeField] private float calmVignette = 0.10f;
        [SerializeField] private float calmDistortion = 0f;

        [Header("Tedirginlik")]
        [SerializeField] private float uneaseGrain = 0.18f;
        [SerializeField] private float uneaseChromatic = 0.10f;
        [SerializeField] private float uneaseVignette = 0.20f;
        [SerializeField] private float uneaseDistortion = -0.08f;

        [Header("Tehlike")]
        [SerializeField] private float dangerGrain = 0.35f;
        [SerializeField] private float dangerChromatic = 0.25f;
        [SerializeField] private float dangerVignette = 0.40f;
        [SerializeField] private float dangerDistortion = -0.19f;

        private FilmGrain filmGrain;
        private ChromaticAberration chromaticAberration;
        private Vignette vignette;
        private LensDistortion lensDistortion;

        private float targetGrain;
        private float targetChromatic;
        private float targetVignette;
        private float targetDistortion;

        private int stateVersion;

        private void Awake()
        {
            Instance = this;

            volume.profile.TryGet(out filmGrain);
            volume.profile.TryGet(out chromaticAberration);
            volume.profile.TryGet(out vignette);
            volume.profile.TryGet(out lensDistortion);

            SetTargetsCalm();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<ThreatStateChangedEvent>(OnThreatStateChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ThreatStateChangedEvent>(OnThreatStateChanged);
        }

        private void OnThreatStateChanged(ThreatStateChangedEvent evt)
        {
            switch (evt.NewState)
            {
                case ThreatStateType.Passive:
                    stateVersion++;
                    SetTargetsCalm();
                    break;
                case ThreatStateType.Searching:
                    stateVersion++;
                    SetTargetsUnease();
                    break;
                case ThreatStateType.ActiveChase:
                    stateVersion++;
                    SetTargetsDanger();
                    break;
                case ThreatStateType.FakeClue:
                    PulseUnease(fakeClueRevertDelay);
                    break;
            }
        }

        public void PulseUnease(float duration)
        {
            stateVersion++;
            SetTargetsUnease();
            StartCoroutine(RevertToCalmAfterDelay(stateVersion, duration));
        }

        public void PulseDanger(float duration)
        {
            stateVersion++;
            SetTargetsDanger();
            StartCoroutine(RevertToCalmAfterDelay(stateVersion, duration));
        }

        private IEnumerator RevertToCalmAfterDelay(int expectedVersion, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (stateVersion == expectedVersion)
            {
                SetTargetsCalm();
            }
        }

        private void SetTargetsCalm()
        {
            targetGrain = calmGrain;
            targetChromatic = calmChromatic;
            targetVignette = calmVignette;
            targetDistortion = calmDistortion;
        }

        private void SetTargetsUnease()
        {
            targetGrain = uneaseGrain;
            targetChromatic = uneaseChromatic;
            targetVignette = uneaseVignette;
            targetDistortion = uneaseDistortion;
        }

        private void SetTargetsDanger()
        {
            targetGrain = dangerGrain;
            targetChromatic = dangerChromatic;
            targetVignette = dangerVignette;
            targetDistortion = dangerDistortion;
        }

        private void Update()
        {
            float t = Time.deltaTime * transitionSpeed;

            if (filmGrain != null)
            {
                filmGrain.intensity.value = Mathf.Lerp(filmGrain.intensity.value, targetGrain, t);
            }

            if (chromaticAberration != null)
            {
                chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberration.intensity.value, targetChromatic, t);
            }

            if (vignette != null)
            {
                vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, targetVignette, t);
            }

            if (lensDistortion != null)
            {
                lensDistortion.intensity.value = Mathf.Lerp(lensDistortion.intensity.value, targetDistortion, t);
            }
        }
    }
}