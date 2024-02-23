using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using FinksQuest.Core;

namespace FinksQuest
{
    public class GrabLastItem : MonoBehaviour
    {
        [SerializeField] private Animator _fadeToWhiteAnimator;
        [SerializeField] private GameObject _winScreen;
        [SerializeField] private GameObject _focusMenuItem;

        // On trigger enter, fade to white
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(LaunchWinScreen());
            }
        }

        private IEnumerator LaunchWinScreen()
        {
            GameManager.Instance.Player._playerInput.SwitchCurrentActionMap("UI");

            _fadeToWhiteAnimator.SetTrigger("FadeToWhite");

            yield return new WaitForSeconds(2);

            _winScreen.SetActive(true);

            _fadeToWhiteAnimator.SetTrigger("FadeToWhiteEnd");

            yield return new WaitForSeconds(2);

            // Focus on the menu item
            EventSystem.current.SetSelectedGameObject(_focusMenuItem);
        }
    }
}
