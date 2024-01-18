using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Linq;
using System;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;


public class MainMenu : MonoBehaviour
{
    int _minVolume = -35;
    int _maxVolume = 0;

    private VisualElement _root;

    private Button _newGame;
    private Button _settings;
    private Button _back;
    private Button _image;
    private Button _sound;
    private Button _shortcuts;
    private Button _exit;
    private VisualElement _fondu;

    // Settings
    // Image
    private Toggle _fullscreenToggle;
    private bool _fullscreen = false;
    private DropdownField _resolutionDropdown;
    private string _resolution;

    // Sound
    private SliderInt _masterVolumeSlider;
    private int _masterVolume;

    private SliderInt _musicVolumeSlider;
    private int _musicVolume;

    private SliderInt _soundVolumeSlider;
    private int _soundVolume;

    // Resolutions Choices
    [SerializeField] private List<string> _resolutions = new List<string> { "1920x1080", "1280x720", "800x600" };

    [SerializeField] PlayerController _playerController;
    Animator _playerAnimator;
    Rigidbody _playerRigibody;

    [SerializeField] AudioMixer _audioMixer;

    public static string[] panels = { "MainPanel", "SettingsPanel" };

    string _currentPanel = "MainPanel";

    string _currentTab = "";

    private AudioSource _uiAudio;

    private VisualElement _focusedElement;

    Focusable _currentFocusedElement;

    private void Awake()
    {
        if (GameManager.Instance._currentRoom  == null)
        {
            // On switch les actions maps
            _playerController._playerInput.SwitchCurrentActionMap("UI");
        }
        
        Debug.Log("On switch sur le map UI");
        _root = GetComponent<UIDocument>().rootVisualElement;
        _currentFocusedElement = _root.focusController.focusedElement;
    }

    private void Update()
    {
        if (_currentFocusedElement != _root.focusController.focusedElement)
        {
            _currentFocusedElement = _root.focusController.focusedElement;
            Debug.Log("Focused element changed");

            if (_currentFocusedElement is VisualElement visualElement)
            {
                // On remove la classe active de tous les elements ayant la classe .tab
                foreach (VisualElement tab in _root.Query<VisualElement>().Where(t => t.ClassListContains("menu-li")).ToList())
                {
                    tab.RemoveFromClassList("active");
                }

                // On ajoute la classe active à l'element parent de l'element focus
                visualElement.parent.AddToClassList("active");
                return;
            }

        }
    }

    private void OnEnable()
    {
        _uiAudio = GetComponent<AudioSource>();
        _root = GetComponent<UIDocument>().rootVisualElement;

        _newGame = _root.Q<Button>("ButtonNewGame");
        // Focus on new game button
        // _newGame.parent.AddToClassList("active");
        _newGame.Focus();

        _settings = _root.Q<Button>("ButtonSettings");
        _back = _root.Q<Button>("ButtonBack");
        _image = _root.Q<Button>("ButtonImage");
        _sound = _root.Q<Button>("ButtonSound");
        _shortcuts = _root.Q<Button>("ButtonShortcuts");
        _exit = _root.Q<Button>("ButtonExit");
        _fondu = _root.Q<VisualElement>("fondu");

        _masterVolumeSlider = _root.Q<SliderInt>("MasterVolume");
        _musicVolumeSlider = _root.Q<SliderInt>("MusicVolume");
        _soundVolumeSlider = _root.Q<SliderInt>("SoundVolume");

        if (_fondu != null)
        {
            _fondu.RemoveFromClassList("show");
        }

        _playerController._player._drive = Player.Drive.BODY;

        _playerAnimator = _playerController._playerAnimator;
        _playerRigibody = _playerController._playerRigidbody;

        BindForm();

        PrepareEvents();
    }

    private void PrepareEvents()
    {
        _newGame.clicked += () => {
            _uiAudio.PlayOneShot(_uiAudio.clip);

            StartCoroutine(LoadSceneWithTransition("DUNGEON"));
        };

        _settings.clicked += () =>
        {
            LoadPanel("SettingsPanel");
        };
        _back.clicked += () => LoadPanel("MainPanel");
        _image.clicked += () => LoadTab("ImageTab");
        _sound.clicked += () => LoadTab("SoundTab");
        _shortcuts.clicked += () => LoadTab("ShortcutsTab");

        _exit.clicked += ExitGame;

        _fullscreenToggle.RegisterValueChangedCallback((evt) => {
            _fullscreen = evt.newValue;
            Screen.fullScreen = _fullscreen;
        });

        _resolutionDropdown.RegisterValueChangedCallback((evt) =>
        {
            _resolution = evt.newValue;

            string[] resolution = _resolution.Split('x');
            Screen.SetResolution(int.Parse(resolution[0]), int.Parse(resolution[1]), _fullscreen);
        });

        _masterVolumeSlider.RegisterValueChangedCallback((evt) => {
            _masterVolume = (int)evt.newValue;
            // Change Group "Master" volume
            _audioMixer.SetFloat("MasterVolume", ConvertSliderValueToReal(_masterVolume));
        });
        _musicVolumeSlider.RegisterValueChangedCallback((evt) => {
            _musicVolume = (int)evt.newValue;
            // Change music volume
            _audioMixer.SetFloat("Master/MusicVolume", ConvertSliderValueToReal(_musicVolume));
        });
        _soundVolumeSlider.RegisterValueChangedCallback((evt) =>
        {
            _soundVolume = (int)evt.newValue;
            // Change sound volume
            _audioMixer.SetFloat("Master/SoundVolume", ConvertSliderValueToReal(_soundVolume));
        });
    }

    private void BindForm()
    {
        _fullscreen = Screen.fullScreen;
        // Settings
        // Image
        _fullscreenToggle = _root.Q<Toggle>("Fullscreen");
        _fullscreenToggle.value = _fullscreen;

        _resolution = Screen.currentResolution.width + "x" + Screen.currentResolution.height;
        _resolutionDropdown = _root.Q<DropdownField>("Resolution");
        _resolutionDropdown.value = _resolution;
        _fullscreenToggle.value = _fullscreen;

        // Sound
        _masterVolume = GetVolume("Master");
        _musicVolume = GetVolume("Master/Music"); 
        _soundVolume = GetVolume("Master/Sound");

        Debug.Log("MasterVolume: " + _masterVolume);
        Debug.Log("MusicVolume: " + _musicVolume);
        Debug.Log("SoundVolume: " + _soundVolume);

        _masterVolumeSlider.value = ConvertRealValueToSlider(_masterVolume);
        _musicVolumeSlider.value = ConvertRealValueToSlider(_musicVolume);
        _soundVolumeSlider.value = ConvertRealValueToSlider(_soundVolume);

        // Resolutions Choices
        _resolutionDropdown.choices = _resolutions;
        _resolutionDropdown.value = _resolution;
    }

    private int ConvertSliderValueToReal(float sliderValue)
    {
        // Utiliser la formule de conversion
        float realValue = (sliderValue / 100f) * (_maxVolume - _minVolume) + _minVolume;

        return (int) realValue;
    }

    private int ConvertRealValueToSlider(float realValue)
    {
        // Utiliser la formule de conversion
        float sliderValue = (realValue - _minVolume) / (_maxVolume - _minVolume) * 100f;

        return (int) sliderValue;
    }

    private int GetVolume(string groupName)
    {
        // Assurez-vous que l'AudioMixer et le groupe sont valides
        if (_audioMixer != null && !string.IsNullOrEmpty(groupName))
        {
            // Créez une variable pour stocker la valeur du volume
            float volume;

            // Utilisez GetFloat pour obtenir la valeur actuelle du volume du groupe
            _audioMixer.GetFloat(groupName + "Volume", out volume);

            return (int) volume;
        }
        else
        {
            Debug.LogError("AudioMixer ou le nom du groupe est invalide.");
            return 0;
        }
    }

    private IEnumerator LoadSceneWithTransition(string SceneName)
    {
        Debug.Log("LoadSceneWithTransition");
        _fondu.AddToClassList("show");

        // Wait 1s
        yield return new WaitForSeconds(1f);

        // Charger la scène
        SceneManager.LoadScene(SceneName);

        yield return null;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadPanel(string panelName)
    {
        Debug.Log("Load Panel launched");
        if (_currentPanel != panelName)
        {
            _uiAudio.PlayOneShot(_uiAudio.clip);

            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            VisualElement current = root.Q<VisualElement>(_currentPanel);
            VisualElement next = root.Q<VisualElement>(panelName);

            current.RemoveFromClassList("active");
            next.AddToClassList("active");

            _currentPanel = panelName;

            if (panelName != "SettingsPanel")
            {
                // On enlève la classe active de tous les elements ayant la classe .tab
                foreach (VisualElement tab in root.Query<VisualElement>().Where(t => t.ClassListContains("tab")).ToList())
                {
                    tab.RemoveFromClassList("active");
                }
            }

            _currentTab = "";
        }
    }

    public void LoadTab(string tabName)
    {
        if (tabName == _currentTab)
        {
            return;
        }

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        var newTab = root.Q<VisualElement>(tabName);

        if (newTab == null) {
            return;
        }

        _uiAudio.PlayOneShot(_uiAudio.clip);

        // On enlève la classe active de tous les elements ayant la classe .tab
        foreach (VisualElement tab in root.Query<VisualElement>().Where(t => t.ClassListContains("tab")).ToList())
        {
            tab.RemoveFromClassList("active");
        }

        // On ajoute la classe active à l'element ayant l'id tabName
        newTab.AddToClassList("active");

        _currentTab = tabName;
    }
}
