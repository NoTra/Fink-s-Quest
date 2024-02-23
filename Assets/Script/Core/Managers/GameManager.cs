using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FinksQuest.Core
{
    public class GameManager : MonoBehaviour
    {
        public EventSystem _eventSystem;
        public GameObject _currentRoom;
        public bool paused = false;

        public static GameManager Instance;
        public Player Player;

        public AudioManager _audioManager;

        public RectTransform _dialogPanel;
        public TextMeshProUGUI _textDialog;

        public UIManager UIManager;

        // Singleton
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            if (PlayerPrefs.HasKey("PlayerDevice"))
            {
                Player._playerInput.SwitchCurrentControlScheme(PlayerPrefs.GetString("PlayerDevice"));
            }

            if (_currentRoom == null)
            {
                // Disable all action maps except UI
                Player._playerInput.SwitchCurrentActionMap("UI");
            }
            else
            {
                // Disable all action maps except Player
                Player._playerInput.SwitchCurrentActionMap("Player");
            }

            PlayerPrefs.SetInt("DialogBox", 0);
        }

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

        public void RestartLevel()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}