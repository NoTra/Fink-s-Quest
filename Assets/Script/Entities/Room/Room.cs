using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Room : MonoBehaviour
{
    public Vector3 _cameraPosition;
    public float _maxZoom;
    public float _cameraMinX;
    public float _cameraMaxX;
    public float _cameraMinZ;
    public float _cameraMaxZ;

    private DialogBox _dialogBox;

    private void Awake()
    {
        _dialogBox = GetComponent<DialogBox>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // On est la room courante ? 
        if (GameManager.Instance._currentRoom == this.gameObject)
        {
            Debug.Log("Room " + this.name + " is the current room");
            _dialogBox.enabled = true;
        } else
        {
            Debug.Log("Room " + this.name + " is not the current room");
        }

    }

    private void OnEnable()
    {
        Debug.Log("Room enabled");

        GameManager.Instance._playerInput.SwitchCurrentActionMap("Player");

        if (GameManager.Instance._currentRoom == this.gameObject && _dialogBox != null && _dialogBox._message != "")
        {
            _dialogBox.enabled = true;
        }
    }

    private void OnDisable()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
