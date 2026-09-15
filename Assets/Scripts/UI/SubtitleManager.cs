using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Backrooms.UI
{
    public struct SubtitleLine
    {
        public string Text;
        public float Duration;
        public AudioClip VoiceClip;

        public SubtitleLine(string text, float duration, AudioClip voiceClip = null)
        {
            Text = text;
            Duration = duration;
            VoiceClip = voiceClip;
        }
    }

    public class SubtitleManager : MonoBehaviour
    {
        public static SubtitleManager Instance { get; private set; }

        [SerializeField] private CanvasGroup subtitleCanvasGroup;
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private float fadeDuration = 0.3f;
        [SerializeField] private float charactersPerSecond = 12f;
        [SerializeField] private float minLineDuration = 2.5f;

        public bool IsPlaying { get; private set; }

        private AudioSource voiceAudioSource;
        private readonly Queue<SubtitleLine> lineQueue = new Queue<SubtitleLine>();
        private Coroutine playbackCoroutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            voiceAudioSource = gameObject.AddComponent<AudioSource>();
            voiceAudioSource.playOnAwake = false;

            if (subtitleCanvasGroup != null)
            {
                subtitleCanvasGroup.alpha = 0f;
            }
        }

        public void ShowLine(string text, AudioClip voiceClip = null)
        {
            float duration = voiceClip != null ? voiceClip.length + 0.5f : EstimateDuration(text);
            Enqueue(new SubtitleLine(text, duration, voiceClip));
        }

        public void ShowLine(string text, float duration)
        {
            Enqueue(new SubtitleLine(text, duration));
        }

        public void ShowSequence(string[] lines, float durationPerLine = -1f)
        {
            foreach (string line in lines)
            {
                float duration = durationPerLine > 0f ? durationPerLine : EstimateDuration(line);
                Enqueue(new SubtitleLine(line, duration));
            }
        }

        private void Enqueue(SubtitleLine line)
        {
            lineQueue.Enqueue(line);

            if (playbackCoroutine == null)
            {
                playbackCoroutine = StartCoroutine(PlayQueue());
            }
        }

        private float EstimateDuration(string text)
        {
            float estimated = text.Length / charactersPerSecond;
            return Mathf.Max(estimated, minLineDuration);
        }

        private IEnumerator PlayQueue()
        {
            IsPlaying = true;

            while (lineQueue.Count > 0)
            {
                SubtitleLine line = lineQueue.Dequeue();
                yield return StartCoroutine(PlaySingleLine(line));
            }

            IsPlaying = false;
            playbackCoroutine = null;
        }

        private IEnumerator PlaySingleLine(SubtitleLine line)
        {
            if (subtitleText != null)
            {
                subtitleText.text = line.Text;
            }

            if (line.VoiceClip != null)
            {
                voiceAudioSource.clip = line.VoiceClip;
                voiceAudioSource.Play();
            }

            yield return FadeTo(1f);

            yield return new WaitForSeconds(line.Duration);

            yield return FadeTo(0f);
        }

        private IEnumerator FadeTo(float targetAlpha)
        {
            if (subtitleCanvasGroup == null) yield break;

            float startAlpha = subtitleCanvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                subtitleCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
                yield return null;
            }

            subtitleCanvasGroup.alpha = targetAlpha;
        }
    }
}