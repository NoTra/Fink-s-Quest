using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public EventSystem _eventSystem;
    public GameObject _currentRoom;
    public bool paused = false;

    public static GameManager Instance;
    public Player Player;

    public AudioManager _audioManager;

    [SerializeField] private UIDocument _startMenu;
    [SerializeField] private UIDocument _pauseMenu;
    [SerializeField] private UIDocument _gameOverMenu;

    [HideInInspector] public UIDocument _currentUIDocument;

    public RectTransform _dialogPanel;
    public TextMeshProUGUI _textDialog;

    // Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (PlayerPrefs.HasKey("PlayerDevice"))
        {
            Player._playerInput.SwitchCurrentControlScheme(
                PlayerPrefs.GetString("PlayerDevice")
                );
        }

        if (_currentRoom == null)
        {
            _currentUIDocument = _startMenu;

            // Disable all action maps except UI
            Player._playerInput.SwitchCurrentActionMap("UI");
        } else
        {
            Debug.Log("Current room is not null, we activate player map");

            // Disable all action maps except Player
            Player._playerInput.SwitchCurrentActionMap("Player");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_currentRoom == null)
        {
            return;
        }

        Camera.main.transform.position = new Vector3(
            Player.GetRigidbody().transform.position.x, 
            _currentRoom.GetComponent<Room>()._maxZoom,
            Player.GetRigidbody().transform.position.z
        );
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
