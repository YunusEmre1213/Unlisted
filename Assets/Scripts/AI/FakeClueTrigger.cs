using UnityEngine;
using Backrooms.Core;

namespace Backrooms.AI
{
    [RequireComponent(typeof(Collider))]
    public class FakeClueTrigger : MonoBehaviour
    {
        [SerializeField] private float cooldown = 8f;
        [SerializeField] private AudioClip specificClip;
        [SerializeField] private float specificClipVolume = 0.7f;

        private float lastTriggerTime = -100f;
        private AudioSource audioSource;

        private void Awake()
        {
            if (specificClip != null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (Time.time - lastTriggerTime < cooldown) return;

            EventBus.Publish(new ThreatStateChangedEvent(ThreatStateType.FakeClue));

            if (specificClip != null && audioSource != null)
            {
                audioSource.PlayOneShot(specificClip, specificClipVolume);
            }

            lastTriggerTime = Time.time;
        }
    }
}