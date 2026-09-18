using System.Collections.Generic;
using UnityEngine;
using Backrooms.Core;

namespace Backrooms.Items
{
    public class NotesJournal : MonoBehaviour
    {
        public static NotesJournal Instance { get; private set; }

        private readonly List<ItemData> collectedNotes = new List<ItemData>();

        public IReadOnlyList<ItemData> CollectedNotes => collectedNotes;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            EventBus.Subscribe<NoteReadEvent>(OnNoteRead);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<NoteReadEvent>(OnNoteRead);
        }

        private void OnNoteRead(NoteReadEvent evt)
        {
            if (evt.Item == null) return;
            if (collectedNotes.Contains(evt.Item)) return;

            collectedNotes.Add(evt.Item);
        }
    }
}