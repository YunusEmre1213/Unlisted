using System.Collections;
using UnityEngine;

namespace Backrooms.Rooms
{
    [RequireComponent(typeof(Collider))]
    public class LightBurnoutTrigger : MonoBehaviour
    {
        [SerializeField] private FluorescentLight targetLight;
        [SerializeField] private float delayBeforeBurnout = 2.5f;

        private bool hasTriggered;

        private void OnTriggerEnter(Collider other)
        {
            if (hasTriggered) return;
            if (!other.CompareTag("Player")) return;
            if (targetLight == null) return;

            hasTriggered = true;
            StartCoroutine(BurnoutAfterDelay());
        }

        private IEnumerator BurnoutAfterDelay()
        {
            yield return new WaitForSeconds(delayBeforeBurnout);
            targetLight.Extinguish();
        }
    }
}