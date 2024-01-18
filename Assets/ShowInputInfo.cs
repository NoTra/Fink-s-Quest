using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ShowInputInfo : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;

    [SerializeField] private TextMeshProUGUI _map;
    [SerializeField] private TextMeshProUGUI _device;
    [SerializeField] private TextMeshProUGUI _debugInput;


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
    }
}
