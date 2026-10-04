using UnityEngine;
using Backrooms.UI;

namespace Backrooms.Player
{
    public class NaturalBlink : MonoBehaviour
    {
        [SerializeField] private float minInterval = 4f;
        [SerializeField] private float maxInterval = 8f;
        [SerializeField] private float blinkDuration = 0.16f;

        private float nextBlinkTime;

        private void OnEnable()
        {
            ScheduleNextBlink();
        }

        private void Update()
        {
            if (Time.time < nextBlinkTime) return;

            ScreenBlink blink = ScreenBlink.Instance;

            if (blink != null && !blink.IsBlinking)
            {
                blink.Blink(blinkDuration);
            }

            ScheduleNextBlink();
        }

        private void ScheduleNextBlink()
        {
            nextBlinkTime = Time.time + Random.Range(minInterval, maxInterval);
        }
    }
}