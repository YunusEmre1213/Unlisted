using System.Collections;
using UnityEngine;

namespace Backrooms.Rooms
{
    public class LockerDoor : MonoBehaviour
    {
        [SerializeField] private Transform doorTransform;
        [SerializeField] private float openAngle = 90f;
        [SerializeField] private float animationDuration = 0.4f;
        [SerializeField] private AudioSource doorAudioSource;

        private Quaternion closedRotation;
        private Quaternion openRotation;
        private Coroutine animationCoroutine;

        private void Awake()
        {
            closedRotation = doorTransform.localRotation;
            openRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
        }

        public void Open()
        {
            PlayDoorSound();
            AnimateTo(openRotation);
        }

        public void Close()
        {
            PlayDoorSound();
            AnimateTo(closedRotation);
        }

        private void AnimateTo(Quaternion targetRotation)
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            animationCoroutine = StartCoroutine(AnimateRoutine(targetRotation));
        }

        private IEnumerator AnimateRoutine(Quaternion targetRotation)
        {
            Quaternion startRotation = doorTransform.localRotation;
            float elapsed = 0f;

            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / animationDuration;
                doorTransform.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
                yield return null;
            }

            doorTransform.localRotation = targetRotation;

            if (doorAudioSource != null && doorAudioSource.isPlaying)
            {
                doorAudioSource.Stop();
            }
        }

        private void PlayDoorSound()
        {
            if (doorAudioSource != null)
            {
                doorAudioSource.Play();
            }
        }
    }
}