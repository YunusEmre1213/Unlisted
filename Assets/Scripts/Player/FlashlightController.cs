using UnityEngine;
using Backrooms.Items;

namespace Backrooms.Player
{
    public class FlashlightController : MonoBehaviour
    {
        [SerializeField] private Light flashlightLight;
        [SerializeField] private AudioSource toggleAudioSource;

        [Header("Pil")]
        [SerializeField] private float maxBattery = 100f;
        [SerializeField] private float drainRate = 3f;
        [SerializeField] private float lowBatteryThreshold = 20f;
        [SerializeField] private float fullIntensity = 3f;
        [SerializeField] private float flickerMinIntensity = 0.5f;
        [SerializeField] private float flickerSpeed = 8f;

        [Header("Pil Degistirme")]
        [SerializeField] private string batteryItemId = "battery";
        [SerializeField] private float refillAmount = 100f;

        private bool isOn = false;
        private float currentBattery;

        public float BatteryPercent => currentBattery / maxBattery;

        private void Start()
        {
            currentBattery = maxBattery;

            if (flashlightLight != null)
            {
                flashlightLight.enabled = isOn;
                flashlightLight.intensity = fullIntensity;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Toggle();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                TryReload();
            }

            if (isOn)
            {
                currentBattery -= drainRate * Time.deltaTime;

                if (currentBattery <= 0f)
                {
                    currentBattery = 0f;
                    isOn = false;

                    if (flashlightLight != null)
                    {
                        flashlightLight.enabled = false;
                    }

                    return;
                }

                UpdateLightIntensity();
            }
        }

        private void UpdateLightIntensity()
        {
            if (flashlightLight == null) return;

            if (currentBattery <= lowBatteryThreshold)
            {
                float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
                flashlightLight.intensity = Mathf.Lerp(flickerMinIntensity, fullIntensity, noise);
            }
            else
            {
                flashlightLight.intensity = fullIntensity;
            }
        }

        public void ForceOff()
        {
            isOn = false;

            if (flashlightLight != null)
            {
                flashlightLight.enabled = false;
            }
        }

        private void Toggle()
        {
            if (currentBattery <= 0f)
            {
                if (toggleAudioSource != null)
                {
                    toggleAudioSource.Play();
                }

                return;
            }

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

        private void TryReload()
        {
            if (InventoryManager.Instance == null) return;
            if (!InventoryManager.Instance.ConsumeItem(batteryItemId)) return;

            currentBattery = Mathf.Min(currentBattery + refillAmount, maxBattery);

            if (toggleAudioSource != null)
            {
                toggleAudioSource.Play();
            }
        }
    }
}