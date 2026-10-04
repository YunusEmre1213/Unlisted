using System.Collections;
using UnityEngine;
using Backrooms.Core;
using Backrooms.Rooms;

namespace Backrooms.Player
{
    public class CameraShake : MonoBehaviour
    {
        public static CameraShake Instance { get; private set; }

        [SerializeField] private CameraHeadBob headBob;
        [SerializeField] private float shakeDuration = 0.6f;
        [SerializeField] private float shakeMagnitude = 0.15f;

        private Coroutine shakeCoroutine;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            EventBus.Subscribe<ExitDeniedEvent>(OnExitDenied);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ExitDeniedEvent>(OnExitDenied);
        }

        private void OnExitDenied(ExitDeniedEvent evt)
        {
            Shake(shakeDuration, shakeMagnitude);
        }

        public void Shake(float duration, float magnitude)
        {
            if (shakeCoroutine != null)
            {
                StopCoroutine(shakeCoroutine);
            }

            shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
        }

        private IEnumerator ShakeRoutine(float duration, float magnitude)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float falloff = 1f - Mathf.Clamp01(elapsed / duration);
                float offsetX = Random.Range(-1f, 1f) * magnitude * falloff;
                float offsetY = Random.Range(-1f, 1f) * magnitude * falloff;

                if (headBob != null)
                {
                    headBob.ShakeOffset = new Vector3(offsetX, offsetY, 0f);
                }

                yield return null;
            }

            if (headBob != null)
            {
                headBob.ShakeOffset = Vector3.zero;
            }
        }
    }
}