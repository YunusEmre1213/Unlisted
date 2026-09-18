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

        [Header("Kapanis Sonrasi Altyazi (opsiyonel)")]
        [SerializeField] private string postCloseSubtitleLine;
        [SerializeField] private float postCloseSubtitleDuration = 2.5f;

        private bool hasBeenRead;

        public void Interact()
        {
            if (itemData == null) return;

            if (NoteReaderUI.Instance != null)
            {
                NoteReaderUI.Instance.Show(itemData.displayName, itemData.description);

                if (!string.IsNullOrEmpty(postCloseSubtitleLine))
                {
                    NoteReaderUI.Instance.OnClosed += HandleClosed;
                }
            }

            EventBus.Publish(new NoteReadEvent(itemData));

            if (collectOnRead && !hasBeenRead)
            {
                hasBeenRead = true;
                EventBus.Publish(new ItemCollectedEvent(itemData));
                gameObject.SetActive(false);
            }
        }

        private void HandleClosed()
        {
            if (NoteReaderUI.Instance != null)
            {
                NoteReaderUI.Instance.OnClosed -= HandleClosed;
            }

            if (SubtitleManager.Instance != null)
            {
                SubtitleManager.Instance.ShowLine(postCloseSubtitleLine, postCloseSubtitleDuration);
            }
        }
    }
}