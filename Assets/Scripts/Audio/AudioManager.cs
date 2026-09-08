using System.Collections;
using UnityEngine;
using Backrooms.Core;
using Backrooms.AI;

namespace Backrooms.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Taban Ortam Sesi")]
        [SerializeField] private AudioClip baseAmbientClip;
        [SerializeField] private float baseAmbientVolume = 0.4f;

        [Header("Tehdit Katmani Sesleri")]
        [SerializeField] private AudioClip passiveLayerClip;
        [SerializeField] private AudioClip activeLayerClip;
        [SerializeField] private float passiveLayerVolume = 0.3f;
        [SerializeField] private float activeLayerVolume = 0.6f;
        [SerializeField] private float crossfadeDuration = 1.5f;

        [Header("Sahte Ipucu (One-Shot)")]
        [SerializeField] private AudioClip[] fakeClueClips;
        [SerializeField] private float fakeClueVolume = 0.7f;

        private AudioSource baseAmbientSource;
        private AudioSource oneShotSource;
        private AudioSource activeStateSource;
        private AudioSource inactiveStateSource;

        private Coroutine crossfadeCoroutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            baseAmbientSource = gameObject.AddComponent<AudioSource>();
            baseAmbientSource.loop = true;
            baseAmbientSource.playOnAwake = false;
            baseAmbientSource.volume = baseAmbientVolume;

            AudioSource sourceA = gameObject.AddComponent<AudioSource>();
            sourceA.loop = true;
            sourceA.playOnAwake = false;
            sourceA.volume = 0f;

            AudioSource sourceB = gameObject.AddComponent<AudioSource>();
            sourceB.loop = true;
            sourceB.playOnAwake = false;
            sourceB.volume = 0f;

            oneShotSource = gameObject.AddComponent<AudioSource>();
            oneShotSource.playOnAwake = false;

            activeStateSource = sourceA;
            inactiveStateSource = sourceB;
        }

        private void Start()
        {
            if (baseAmbientClip != null)
            {
                baseAmbientSource.clip = baseAmbientClip;
                baseAmbientSource.Play();
            }

            if (passiveLayerClip != null)
            {
                activeStateSource.clip = passiveLayerClip;
                activeStateSource.volume = passiveLayerVolume;
                activeStateSource.Play();
            }
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
                    CrossfadeTo(passiveLayerClip, passiveLayerVolume);
                    break;
                case ThreatStateType.ActiveChase:
                    CrossfadeTo(activeLayerClip, activeLayerVolume);
                    break;
                case ThreatStateType.FakeClue:
                    PlayFakeClue();
                    break;
            }
        }

        private void CrossfadeTo(AudioClip newClip, float targetVolume)
        {
            if (newClip == null) return;
            if (activeStateSource.clip == newClip) return;

            if (crossfadeCoroutine != null)
            {
                StopCoroutine(crossfadeCoroutine);
            }

            crossfadeCoroutine = StartCoroutine(CrossfadeRoutine(newClip, targetVolume));
        }

        private IEnumerator CrossfadeRoutine(AudioClip newClip, float targetVolume)
        {
            inactiveStateSource.clip = newClip;
            inactiveStateSource.volume = 0f;
            inactiveStateSource.Play();

            float startVolumeActive = activeStateSource.volume;
            float elapsed = 0f;

            while (elapsed < crossfadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / crossfadeDuration;

                inactiveStateSource.volume = Mathf.Lerp(0f, targetVolume, t);
                activeStateSource.volume = Mathf.Lerp(startVolumeActive, 0f, t);

                yield return null;
            }

            inactiveStateSource.volume = targetVolume;
            activeStateSource.volume = 0f;
            activeStateSource.Stop();

            AudioSource temp = activeStateSource;
            activeStateSource = inactiveStateSource;
            inactiveStateSource = temp;
        }

        private void PlayFakeClue()
        {
            if (fakeClueClips == null || fakeClueClips.Length == 0) return;

            AudioClip clip = fakeClueClips[Random.Range(0, fakeClueClips.Length)];
            oneShotSource.PlayOneShot(clip, fakeClueVolume);
        }
    }
}