using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FinksQuest.Core
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Animator _transitionAnimator;

        [Header("Gameover Menu")]
        public GameObject GameOverMenu;
        public GameObject _focusMenuItemGameOver;

        [Header("Game Win Menu")]
        public GameObject GameWinMenu;
        public GameObject _focusMenuItemGameWin;

        [Header("Pause Menu")]
        public GameObject PauseMenu;
        public GameObject _focusMenuItemPause;

        // Singleton
        public static UIManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void DisplayGameOverScreen()
        {
            GameOverMenu.SetActive(true);

            // Start UI controls
            GameManager.Instance.Player._playerInput.SwitchCurrentActionMap("UI");

            // Focus on the menu item
            EventSystem.current.SetSelectedGameObject(_focusMenuItemGameOver);
        }

        public void DisplayGameWinScreen()
        {
            GameWinMenu.SetActive(true);

            // Start UI controls
            GameManager.Instance.Player._playerInput.SwitchCurrentActionMap("UI");

            // Focus on the menu item
            EventSystem.current.SetSelectedGameObject(_focusMenuItemGameWin);
        }

        public void DisplayPauseScreen()
        {
            PauseMenu.SetActive(true);

            // Start UI controls
            GameManager.Instance.Player._playerInput.SwitchCurrentActionMap("UI");

            // Focus on the menu item
            EventSystem.current.SetSelectedGameObject(_focusMenuItemPause);
        }

        public IEnumerator StartRoomTransition()
        {
            _transitionAnimator.SetTrigger("StartTransition");
            yield return null;
        }

        public IEnumerator EndRoomTransition()
        {
            _transitionAnimator.SetTrigger("EndTransition");
            yield return null;
        }
    }
}
