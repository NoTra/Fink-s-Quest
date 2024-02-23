using UnityEngine;
using UnityEngine.InputSystem;

using FinksQuest.Core;
using UnityEngine.EventSystems;

namespace FinksQuest.PlayerSystems
{
    public class PlayerDevice : MonoBehaviour
    {
        public string deviceName;
        [SerializeField] private PlayerInput _playerInput;

        void Start()
        {
            deviceName = (_playerInput != null) ? _playerInput.currentControlScheme : "";
        }

        void Update()
        {
            if (_playerInput != null && _playerInput.currentControlScheme != deviceName)
            {
                deviceName = _playerInput.currentControlScheme;

                // On sauvegarde le device actuel
                PlayerPrefs.SetString("PlayerDevice", deviceName);

                if (UIManager.Instance.GameOverMenu.activeSelf || UIManager.Instance.GameWinMenu.activeSelf || UIManager.Instance.PauseMenu.activeSelf)
                {
                    if (UIManager.Instance.GameOverMenu.activeSelf)
                    {
                        EventSystem.current.SetSelectedGameObject(UIManager.Instance._focusMenuItemGameOver);
                    }
                    else if (UIManager.Instance.GameWinMenu.activeSelf)
                    {
                        EventSystem.current.SetSelectedGameObject(UIManager.Instance._focusMenuItemGameWin);
                    }
                    else if (UIManager.Instance.PauseMenu.activeSelf)
                    {
                        EventSystem.current.SetSelectedGameObject(UIManager.Instance._focusMenuItemPause);
                    }
                }
            }
        }
    }
}