using System.Collections;
using UnityEngine;

namespace FinksQuest.Core
{
    public class UIManager : MonoBehaviour
    {
        public GameObject PauseMenu;
        public GameObject GameOverMenu;
        public GameObject GameWinMenu;

        [SerializeField] private Animator _transitionAnimator;

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
