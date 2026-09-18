using System.Collections;
using UnityEngine;
using Backrooms.UI;

namespace Backrooms.Player
{
    public class CinematicMoment : MonoBehaviour
    {
        public static CinematicMoment Instance { get; private set; }

        [SerializeField] private Camera playerCamera;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private PlayerLook playerLook;
        [SerializeField] private PlayerInteractor playerInteractor;
        [SerializeField] private PlayerFootsteps playerFootsteps;
        [SerializeField] private CameraHeadBob cameraHeadBob;

        [Header("Zoom Ayarlari")]
        [SerializeField] private float zoomDuration = 1.5f;
        [SerializeField] private float holdDuration = 2f;

        private float defaultFov;
        private bool isPlaying;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (playerCamera != null)
            {
                defaultFov = playerCamera.fieldOfView;
            }
        }

        public void Play(string[] lines, float zoomedFov)
        {
            if (isPlaying) return;
            StartCoroutine(PlayRoutine(lines, zoomedFov));
        }

        private IEnumerator PlayRoutine(string[] lines, float zoomedFov)
        {
            isPlaying = true;

            SetControlsEnabled(false);

            yield return StartCoroutine(ZoomTo(zoomedFov, zoomDuration));

            if (SubtitleManager.Instance != null && lines != null && lines.Length > 0)
            {
                SubtitleManager.Instance.ShowSequence(lines);

                while (SubtitleManager.Instance.IsPlaying)
                {
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(holdDuration);
            }

            yield return StartCoroutine(ZoomTo(defaultFov, zoomDuration));

            SetControlsEnabled(true);
            isPlaying = false;
        }

        private IEnumerator ZoomTo(float targetFov, float duration)
        {
            if (playerCamera == null) yield break;

            float startFov = playerCamera.fieldOfView;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float eased = t * t * (3f - 2f * t);
                playerCamera.fieldOfView = Mathf.Lerp(startFov, targetFov, eased);
                yield return null;
            }

            playerCamera.fieldOfView = targetFov;
        }

        private void SetControlsEnabled(bool enabled)
        {
            if (playerMovement != null) playerMovement.enabled = enabled;
            if (playerLook != null) playerLook.enabled = enabled;
            if (playerInteractor != null) playerInteractor.enabled = enabled;
            if (playerFootsteps != null) playerFootsteps.enabled = enabled;
            if (cameraHeadBob != null) cameraHeadBob.enabled = enabled;
        }
    }
}