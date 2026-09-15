using System.Collections;
using UnityEngine;
using Backrooms.Player;

namespace Backrooms.UI
{
    public class IntroSequence : MonoBehaviour
    {
        [Header("Referanslar")]
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private Transform playerBody;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private PlayerLook playerLook;
        [SerializeField] private PlayerInteractor playerInteractor;
        [SerializeField] private FlashlightController flashlightController;
        [SerializeField] private NaturalBlink naturalBlink;

        [Header("Zamanlama")]
        [SerializeField] private float holdBlackDuration = 1.5f;
        [SerializeField] private float fadeInDuration = 3f;
        [SerializeField] private float lookAroundDuration = 2.5f;
        [SerializeField] private float lookAroundAngle = 20f;

        [Header("Ic Ses")]
        [SerializeField] private string[] introMonologueLines;

        private void Start()
        {
            SetControlsEnabled(false);

            if (flashlightController != null)
            {
                flashlightController.ForceOff();
            }

            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = 1f;
            }

            StartCoroutine(PlayIntro());
        }

        private IEnumerator PlayIntro()
        {
            yield return new WaitForSeconds(holdBlackDuration);

            float elapsed = 0f;

            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;

                if (fadeCanvasGroup != null)
                {
                    fadeCanvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / fadeInDuration);
                }

                yield return null;
            }

            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = 0f;
            }

            if (introMonologueLines != null && introMonologueLines.Length > 0 && Backrooms.UI.SubtitleManager.Instance != null)
            {
                Backrooms.UI.SubtitleManager.Instance.ShowSequence(introMonologueLines);
            }

            if (playerBody != null)
            {
                yield return StartCoroutine(LookAroundRoutine());
            }

            SetControlsEnabled(true);
        }

        private IEnumerator LookAroundRoutine()
        {
            Quaternion baseRotation = playerBody.rotation;
            Quaternion rightRotation = baseRotation * Quaternion.Euler(0f, lookAroundAngle, 0f);
            Quaternion leftRotation = baseRotation * Quaternion.Euler(0f, -lookAroundAngle, 0f);

            yield return RotateOverTime(baseRotation, rightRotation, lookAroundDuration * 0.35f);
            yield return RotateOverTime(rightRotation, leftRotation, lookAroundDuration * 0.5f);
            yield return RotateOverTime(leftRotation, baseRotation, lookAroundDuration * 0.35f);
        }

        private IEnumerator RotateOverTime(Quaternion from, Quaternion to, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float easedT = t * t * (3f - 2f * t);
                playerBody.rotation = Quaternion.Slerp(from, to, easedT);
                yield return null;
            }

            playerBody.rotation = to;
        }

        private void SetControlsEnabled(bool enabled)
        {
            if (playerMovement != null) playerMovement.enabled = enabled;
            if (playerLook != null) playerLook.enabled = enabled;
            if (playerInteractor != null) playerInteractor.enabled = enabled;
            if (flashlightController != null) flashlightController.enabled = enabled;
            if (naturalBlink != null) naturalBlink.enabled = enabled;
        }
    }
}