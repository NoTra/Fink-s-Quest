using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

using FinksQuest.Core;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    private string _lastMapActionName;

    [HideInInspector] public InputAction _pauseAction;
    [HideInInspector] public InputAction _unpauseAction;

    private VolumeProfile _previousVolumeProfile;
    [SerializeField] private VolumeProfile _menuVolumeProfile;

    [SerializeField] private PlayerInput _playerInput;

    [SerializeField] private GameObject _menu;

    [SerializeField] private GameObject _focusButton;

    private void Awake()
    {
        // _playerInput = GameManager.Instance.Player._playerInput;

        _pauseAction = _playerInput.actions["Pause"];
        _unpauseAction = _playerInput.actions["Unpause"];

        _lastMapActionName = _playerInput.currentActionMap.name;
    }

    public void TogglePause(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        TogglePause();
    }

    public void TogglePause()
    {
        if (GameManager.Instance.paused)
        {
            OnResume();
        }
        else
        {
            OnPause();
        }
    }

    public void OnPause()
    {
        _previousVolumeProfile = Camera.main.GetComponent<Volume>().profile;
        Debug.Log("Pause... Changement de volume... before : " + _previousVolumeProfile.name + " -> after : " + _menuVolumeProfile.name);
        Camera.main.GetComponent<Volume>().profile = _menuVolumeProfile;

        Time.timeScale = 0f;
        AudioListener.pause = true;

        // On stocke l'action map actuelle
        _lastMapActionName = _playerInput.currentActionMap.name;

        // On change le schéma d'input pour le menu
        _playerInput.SwitchCurrentActionMap("UI");

        // On met le focus sur le bouton Settings
        // _root.Q<Button>("ButtonSettings").parent.Focus();

        // GameManager.Instance._currentUIDocument = GetComponent<UIDocument>();

        GameManager.Instance.paused = true;

        _menu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(_focusButton);
    }

    public void OnResume()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        // _container.AddToClassList("hide");
        enabled = false;

        Debug.Log("Switch action map (player)");

        // On change le schéma d'input pour le joueur
        _playerInput.SwitchCurrentActionMap("Player");

        // GameManager.Instance._currentUIDocument = null;

        GameManager.Instance.paused = false;

        Camera.main.GetComponent<Volume>().profile = _previousVolumeProfile;

        _menu.SetActive(false);
    }
}
