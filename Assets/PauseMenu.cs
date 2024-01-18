using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    private VisualElement _root;
    private VisualElement _container;

    private PlayerInput _playerInput;

    [HideInInspector] public InputAction _pauseAction;
    [HideInInspector] public InputAction _unpauseAction;

    private void Awake()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _container = _root.Q<VisualElement>("container");

        _playerInput = GameManager.Instance._playerInput;

        _pauseAction = _playerInput.actions["Pause"];
        _unpauseAction = _playerInput.actions["Unpause"];
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

    private void TogglePause(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        // On freeze le temps
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
        else
        {
            Time.timeScale = 0f;
        }

        Debug.Log("Pause/Unpause");

        if (_container.ClassListContains("hide"))
        {
            _container.RemoveFromClassList("hide");

            Debug.Log("Switch action map (UI)");
            // On change le schéma d'input pour le menu
            GameManager.Instance._playerController._playerInput.SwitchCurrentActionMap("UI");

            // On met le focus sur le bouton Settings
            _root.Q<Button>("ButtonSettings").parent.Focus();
        }
        else
        {
            _container.AddToClassList("hide");

            Debug.Log("Switch action map (player)");

            // On change le schéma d'input pour le joueur
            GameManager.Instance._playerController._playerInput.SwitchCurrentActionMap("Player");
        }

    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
