using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backrooms.UI
{
    public class ScreenBlink : MonoBehaviour
    {
        public static ScreenBlink Instance { get; private set; }

        [SerializeField] private RectTransform topLid;
        [SerializeField] private RectTransform bottomLid;
        [SerializeField] private float closedHeightRatio = 0.55f;

        private struct BlinkRequest
        {
            public float Duration;
            public Action OnPeakDarkness;
        }

        private readonly Queue<BlinkRequest> requestQueue = new Queue<BlinkRequest>();
        private Coroutine processingCoroutine;

        public bool IsBlinking => processingCoroutine != null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void Blink(float duration, Action onPeakDarkness = null)
        {
            requestQueue.Enqueue(new BlinkRequest { Duration = duration, OnPeakDarkness = onPeakDarkness });

            if (processingCoroutine == null)
            {
                processingCoroutine = StartCoroutine(ProcessQueue());
            }
        }

        private IEnumerator ProcessQueue()
        {
            while (requestQueue.Count > 0)
            {
                BlinkRequest request = requestQueue.Dequeue();
                yield return StartCoroutine(BlinkRoutine(request.Duration, request.OnPeakDarkness));
            }

            processingCoroutine = null;
        }

        private IEnumerator BlinkRoutine(float duration, Action onPeakDarkness)
        {
            float closedHeight = Screen.height * closedHeightRatio;
            float halfDuration = duration * 0.5f;
            float elapsed = 0f;

            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                SetLidHeight(Mathf.Lerp(0f, closedHeight, elapsed / halfDuration));
                yield return null;
            }

            SetLidHeight(closedHeight);
            onPeakDarkness?.Invoke();

            elapsed = 0f;

            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                SetLidHeight(Mathf.Lerp(closedHeight, 0f, elapsed / halfDuration));
                yield return null;
            }

            SetLidHeight(0f);
        }

        private void SetLidHeight(float height)
        {
            if (topLid != null)
            {
                topLid.sizeDelta = new Vector2(topLid.sizeDelta.x, height);
            }

            if (bottomLid != null)
            {
                bottomLid.sizeDelta = new Vector2(bottomLid.sizeDelta.x, height);
            }
        }
    }
}