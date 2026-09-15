using UnityEngine;
using Backrooms.Core;
using Backrooms.UI;

namespace Backrooms.Items
{
    [RequireComponent(typeof(Collider))]
    public class ReadableNote : MonoBehaviour, IInteractable
    {
        [SerializeField] private ItemData itemData;
        [SerializeField] private bool collectOnRead = true;

        private bool hasBeenRead;

        public void Interact()
        {
            if (itemData == null) return;

            if (NoteReaderUI.Instance != null)
            {
                NoteReaderUI.Instance.Show(itemData.displayName, itemData.description);
            }

            if (collectOnRead && !hasBeenRead)
            {
                hasBeenRead = true;
                EventBus.Publish(new ItemCollectedEvent(itemData));
                gameObject.SetActive(false);
            }
        }
    }
}