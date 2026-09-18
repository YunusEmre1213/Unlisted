using System;
using UnityEngine;
using TMPro;
using Backrooms.Player;

namespace Backrooms.UI
{
    public class NoteReaderUI : MonoBehaviour
    {
        public static NoteReaderUI Instance { get; private set; }

        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI bodyText;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private PlayerLook playerLook;

        public bool IsOpen { get; private set; }
        public event Action OnClosed;

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

        public void Show(string title, string body)
        {
            if (panelRoot != null) panelRoot.SetActive(true);
            if (titleText != null) titleText.text = title;
            if (bodyText != null) bodyText.text = body;

            IsOpen = true;

            if (playerMovement != null) playerMovement.enabled = false;
            if (playerLook != null) playerLook.enabled = false;
        }

        public void Hide()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            IsOpen = false;

            if (playerMovement != null) playerMovement.enabled = true;
            if (playerLook != null) playerLook.enabled = true;

            OnClosed?.Invoke();
        }
    }
}