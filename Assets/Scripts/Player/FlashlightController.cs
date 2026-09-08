using UnityEngine;

namespace Backrooms.Player
{
    public class FlashlightController : MonoBehaviour
    {
        [SerializeField] private Light flashlightLight;
        [SerializeField] private AudioSource toggleAudioSource;

        private bool isOn = false;

        private void Start()
        {
            if (flashlightLight != null)
            {
                flashlightLight.enabled = isOn;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Toggle();
            }
        }

        private void Toggle()
        {
            isOn = !isOn;

            if (flashlightLight != null)
            {
                flashlightLight.enabled = isOn;
            }

            if (toggleAudioSource != null)
            {
                toggleAudioSource.Play();
            }
        }
    }
}