using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ShowInputInfo : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;

    [SerializeField] private MainMenu _mainMenu;

    [SerializeField] private TextMeshProUGUI _map;
    [SerializeField] private TextMeshProUGUI _device;
    [SerializeField] private TextMeshProUGUI _debugInput;
    [SerializeField] private TextMeshProUGUI _UIfocus;


    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();

        foreach (PlayerInput playerInput in playerInputs)
        {
            Debug.Log(playerInput.gameObject.name + " has a PlayerInput component.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        _map.text = "Map : " + _playerInput.currentActionMap.name ?? "NaN";
        _device.text = "Control scheme : " + (_playerInput.currentControlScheme ?? "NaN");
        if (_debugInput != null)
        {
            Vector2 moveDirection = _playerInput.actions["Move"].ReadValue<Vector2>();
            _debugInput.text = moveDirection.ToString();
        }

        if (_UIfocus != null)
        {
            VisualElement focusedElement = GameManager.Instance._currentUIDocument.rootVisualElement.focusController.focusedElement as VisualElement;

            if (focusedElement != null)
            {
                if (focusedElement.Q<Button>() != null)
                {
                    _UIfocus.text = focusedElement.Q<Button>().name;
                }
                else if (focusedElement.Q<SliderInt>() != null)
                {
                    _UIfocus.text = "SliderInt : " + focusedElement.Q<SliderInt>().name;
                }
                else if (focusedElement.Q<Slider>() != null)
                {
                    _UIfocus.text = "Slider : " + focusedElement.Q<Slider>().name;
                } else if (focusedElement.Q<Toggle>() != null)
                {
                    _UIfocus.text = "Toggle : " + focusedElement.Q<Toggle>().name;
                }
                else if (focusedElement.Q<DropdownField>() != null)
                {
                    _UIfocus.text = "DropdownField : " + focusedElement.Q<DropdownField>().name;
                }
                else
                {
                    _UIfocus.text = "NaN";
                }
            } else
            {
                _UIfocus.text = "NaN";
            }

            // On passe sur la navigation standard si on a plus de focus sinon, on reste sur la navigation UI
            GameManager.Instance._eventSystem.sendNavigationEvents = (_UIfocus.text == "NaN");
        }
    }
}
