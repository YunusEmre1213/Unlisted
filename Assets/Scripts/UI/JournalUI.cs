using System.Text;
using UnityEngine;
using TMPro;
using Backrooms.Player;
using Backrooms.Items;

namespace Backrooms.UI
{
    public class JournalUI : MonoBehaviour
    {
        public static JournalUI Instance { get; private set; }

        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TextMeshProUGUI contentText;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private PlayerLook playerLook;
        [SerializeField] private PlayerInteractor playerInteractor;

        public bool IsOpen { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (panelRoot != null)
            {
                panelRoot.SetActive(false);
            }
        }

        private void Update()
        {
            if (NoteReaderUI.Instance != null && NoteReaderUI.Instance.IsOpen) return;

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Toggle();
            }
        }

        public void Toggle()
        {
            if (IsOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        private void Open()
        {
            if (NotesJournal.Instance == null) return;

            RefreshContent();

            if (panelRoot != null) panelRoot.SetActive(true);
            IsOpen = true;

            if (playerMovement != null) playerMovement.enabled = false;
            if (playerLook != null) playerLook.enabled = false;
            if (playerInteractor != null) playerInteractor.enabled = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Close()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            IsOpen = false;

            if (playerMovement != null) playerMovement.enabled = true;
            if (playerLook != null) playerLook.enabled = true;
            if (playerInteractor != null) playerInteractor.enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void RefreshContent()
        {
            if (contentText == null) return;

            var notes = NotesJournal.Instance.CollectedNotes;

            if (notes.Count == 0)
            {
                contentText.text = "Henuz hicbir not toplamadim.";
                return;
            }

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < notes.Count; i++)
            {
                builder.AppendLine("<b>" + notes[i].displayName + "</b>");
                builder.AppendLine(notes[i].description);

                if (i < notes.Count - 1)
                {
                    builder.AppendLine();
                    builder.AppendLine("----------");
                    builder.AppendLine();
                }
            }

            contentText.text = builder.ToString();
        }
    }
}