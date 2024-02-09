using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

using FinksQuest.Core;

public class PauseMenu : MonoBehaviour
{
    private VisualElement _root;
    private VisualElement _container;

    private PlayerInput _playerInput;

    private string _lastMapActionName;

    [HideInInspector] public InputAction _pauseAction;
    [HideInInspector] public InputAction _unpauseAction;

    private void Awake()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _container = _root.Q<VisualElement>("container");

        _playerInput = GameManager.Instance.Player._playerInput;

        _pauseAction = _playerInput.actions["Pause"];
        _unpauseAction = _playerInput.actions["Unpause"];

        _lastMapActionName = _playerInput.currentActionMap.name;
    }

    private void OnEnable()
    {
        _pauseAction.performed += TogglePause;
        _pauseAction.Enable();

        _unpauseAction.performed += TogglePause;
        _unpauseAction.Enable();
    }

    private void OnDisable()
    {
        _pauseAction.Disable();
        _unpauseAction.Disable();
    }

    public void TogglePause(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

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
        Time.timeScale = 0f;
        AudioListener.pause = true;

        _container.RemoveFromClassList("hide");

        // On stocke l'action map actuelle
        _lastMapActionName = _playerInput.currentActionMap.name;

        // On change le schéma d'input pour le menu
        GameManager.Instance.Player._playerInput.SwitchCurrentActionMap("UI");

        // On met le focus sur le bouton Settings
        _root.Q<Button>("ButtonSettings").parent.Focus();

        GameManager.Instance._currentUIDocument = GetComponent<UIDocument>();

        GameManager.Instance.paused = true;
    }

    public void OnResume()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        _container.AddToClassList("hide");

        Debug.Log("Switch action map (player)");

        // On change le schéma d'input pour le joueur
        GameManager.Instance.Player._playerInput.SwitchCurrentActionMap(_lastMapActionName);

        // GameManager.Instance._currentUIDocument = null;

        GameManager.Instance.paused = false;
    }
}
