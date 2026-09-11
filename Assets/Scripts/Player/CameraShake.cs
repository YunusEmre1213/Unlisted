using System.Collections;
using UnityEngine;
using Backrooms.Core;
using Backrooms.Rooms;

namespace Backrooms.Player
{
    public class CameraShake : MonoBehaviour
    {
        [SerializeField] private CameraHeadBob headBob;
        [SerializeField] private float shakeDuration = 0.6f;
        [SerializeField] private float shakeMagnitude = 0.15f;

        private Coroutine shakeCoroutine;

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
            if (shakeCoroutine != null)
            {
                StopCoroutine(shakeCoroutine);
            }

            shakeCoroutine = StartCoroutine(ShakeRoutine());
        }

        private IEnumerator ShakeRoutine()
        {
            float elapsed = 0f;

            while (elapsed < shakeDuration)
            {
                elapsed += Time.deltaTime;

                float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
                float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

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