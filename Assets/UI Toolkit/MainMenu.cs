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
using System.Runtime.CompilerServices;

public class MainMenu : MonoBehaviour
{
    int _minVolume = -35;
    int _maxVolume = 0;

    public VisualElement _root;

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

    private SliderInt _interfaceVolumeSlider;
    private int _interfaceVolume;

    // Resolutions Choices
    [SerializeField] private List<string> _resolutions = new List<string> { "1920x1080", "1280x720", "800x600" };

    [SerializeField] PlayerController _playerController;
    Animator _playerAnimator;
    Rigidbody _playerRigibody;

    [SerializeField] AudioMixer _audioMixer;

    public static string[] panels = { "MainPanel", "SettingsPanel" };

    public string _currentPanel = "MainPanel";

    public string _currentTab = "";

    private AudioSource _uiAudio;

    public Focusable _currentFocusedElement;

    [SerializeField] private PauseMenu _pauseMenu;

    private void Awake()
    {
        if (GameManager.Instance._currentRoom  == null)
        {
            // On switch les actions maps
            _playerController._playerInput.SwitchCurrentActionMap("UI");
        }
        
        _root = GetComponent<UIDocument>().rootVisualElement;

        if (_pauseMenu == null)
        {
            _pauseMenu = GetComponent<PauseMenu>();
        }

        _uiAudio = GetComponent<AudioSource>();
        _root = GetComponent<UIDocument>().rootVisualElement;

        _newGame = _root.Q<Button>("ButtonNewGame");
        _settings = _root.Q<Button>("ButtonSettings");
        _back = _root.Q<Button>("ButtonBack");
        _image = _root.Q<Button>("ButtonImage");
        _sound = _root.Q<Button>("ButtonSound");
        _shortcuts = _root.Q<Button>("ButtonShortcuts");
        _exit = _root.Q<Button>("ButtonExit");
        _fondu = _root.Q<VisualElement>("fondu");

        // Get _fondu markup attribute data-value

        _masterVolumeSlider = _root.Q<SliderInt>("MasterVolume");
        _musicVolumeSlider = _root.Q<SliderInt>("MusicVolume");
        _soundVolumeSlider = _root.Q<SliderInt>("SoundVolume");
        _interfaceVolumeSlider = _root.Q<SliderInt>("InterfaceVolume");

        if (_fondu != null)
        {
            _fondu.RemoveFromClassList("show");
        }

        _playerController._player._drive = Player.Drive.BODY;

        _playerAnimator = _playerController._playerAnimator;
        _playerRigibody = _playerController._playerRigidbody;

        BindForm();

        _currentFocusedElement = _root.focusController.focusedElement;
    }

    private void Update()
    {
        if (_root.focusController.focusedElement != null && _currentFocusedElement != _root.focusController.focusedElement)
        {
            _currentFocusedElement = _root.focusController.focusedElement;
            Debug.Log("Focused element changed : " + _currentFocusedElement.ToString());

            if (_currentFocusedElement is VisualElement visualElement)
            {
                // On remove la classe active de tous les elements ayant la classe .tab
                foreach (VisualElement tab in _root.Query<VisualElement>().Where(t => t.ClassListContains("menu-li")).ToList())
                {
                    tab.RemoveFromClassList("active");
                }

                // On ajoute la classe active à l'element parent de l'element focus
                visualElement.AddToClassList("active");
                return;
            }
        }

        if (_currentFocusedElement != null)
        {
            _currentFocusedElement.Focus();
        }
    }

    private void OnEnable()
    {
        _newGame.clicked += () => {
            Debug.Log("New Game clicked");
            _uiAudio.PlayOneShot(_uiAudio.clip);

            // On remet le temps à 1 (à 0 si le jeu est en pause)
            if (GameManager.Instance.paused)
            {
                _pauseMenu.OnResume();
            }

            StartCoroutine(LoadSceneWithTransition("DUNGEON"));
        };

        _settings.clicked += () =>
        {
            Debug.Log("Settings clicked");
            LoadPanel("SettingsPanel");
        };

        _back.clicked += () =>
        {
            Debug.Log("Back clicked");
            LoadPanel("MainPanel");
        };

        _image.clicked += () =>
        {
            Debug.Log("Image clicked");

            if (_image.viewDataKey == null)
            {
                return;
            }
            
            LoadTab(_image.viewDataKey);
        };

        _sound.clicked += () =>
        {
            Debug.Log("Sound clicked");

            if (_sound.viewDataKey == null)
            {
                return;
            }
            LoadTab(_sound.viewDataKey);
            // LoadTab("SoundTab");
        };

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
        _interfaceVolumeSlider.RegisterValueChangedCallback((evt) =>
        {
            _interfaceVolume = (int)evt.newValue;
            // Change sound volume
            _audioMixer.SetFloat("Master/InterfaceVolume", ConvertSliderValueToReal(_interfaceVolume));
        });
    }

    private void OnDisable()
    {
        _newGame.clicked -= () =>
        {
            Debug.Log("New Game clicked");
            _uiAudio.PlayOneShot(_uiAudio.clip);

            // On remet le temps à 1 (à 0 si le jeu est en pause)
            if (GameManager.Instance.paused)
            {
                _pauseMenu.OnResume();
            }

            StartCoroutine(LoadSceneWithTransition("DUNGEON"));
        };

        _settings.clicked -= () =>
        {
            Debug.Log("Settings clicked");
            LoadPanel("SettingsPanel");
        };
        _back.clicked -= () =>
        {
            Debug.Log("Back clicked");
            LoadPanel("MainPanel");
        };
        _image.clicked -= () =>
        {
            Debug.Log("Image clicked");
            LoadTab("ImageTab");
        };
        _sound.clicked -= () => LoadTab("SoundTab");
        _shortcuts.clicked -= () => LoadTab("ShortcutsTab");

        _exit.clicked -= ExitGame;

        _fullscreenToggle.UnregisterValueChangedCallback((evt) =>
        {
            _fullscreen = evt.newValue;
            Screen.fullScreen = _fullscreen;
        });

        _resolutionDropdown.UnregisterValueChangedCallback((evt) =>
        {
            _resolution = evt.newValue;

            string[] resolution = _resolution.Split('x');
            Screen.SetResolution(int.Parse(resolution[0]), int.Parse(resolution[1]), _fullscreen);
        });

        _masterVolumeSlider.UnregisterValueChangedCallback((evt) =>
        {
            _masterVolume = (int)evt.newValue;
            // Change Group "Master" volume
            _audioMixer.SetFloat("MasterVolume", ConvertSliderValueToReal(_masterVolume));
        });
        _musicVolumeSlider.UnregisterValueChangedCallback((evt) =>
        {
            _musicVolume = (int)evt.newValue;
            // Change music volume
            _audioMixer.SetFloat("Master/MusicVolume", ConvertSliderValueToReal(_musicVolume));
        });
        _soundVolumeSlider.UnregisterValueChangedCallback((evt) =>
        {
            _soundVolume = (int)evt.newValue;
            // Change sound volume
            _audioMixer.SetFloat("Master/SoundVolume", ConvertSliderValueToReal(_soundVolume));
        });
        _interfaceVolumeSlider.UnregisterValueChangedCallback((evt) =>
        {
            _interfaceVolume = (int)evt.newValue;
            // Change sound volume
            _audioMixer.SetFloat("Master/InterfaceVolume", ConvertSliderValueToReal(_interfaceVolume));
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
        _soundVolume = GetVolume("Master/Interface");

        _masterVolumeSlider.value = ConvertRealValueToSlider(_masterVolume);
        _musicVolumeSlider.value = ConvertRealValueToSlider(_musicVolume);
        _soundVolumeSlider.value = ConvertRealValueToSlider(_soundVolume);
        _interfaceVolumeSlider.value = ConvertRealValueToSlider(_interfaceVolume);

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
        if (_currentPanel != panelName)
        {
            Debug.Log("Load panel " + panelName);

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

            // On met le focus sur le premier parent d'un bouton
            Button firstOfPanel = next.Q<Button>();

            _currentFocusedElement = firstOfPanel.parent;

            firstOfPanel.parent.Focus();
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

        Debug.Log("Opened tab " + tabName);

        // On met le focus sur le premier element focusable child de l'element ayant l'id tabName
        Focusable firstOfTab = root.Query<VisualElement>(tabName).Children<VisualElement>().First();

        firstOfTab.Focus();

        Debug.Log("Focus sur " + firstOfTab.ToString());
    }

    public void UnLoadTab()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        VisualElement current = root.Q<VisualElement>(_currentTab);

        if (_currentTab != null && _currentTab != "")
        {
            Debug.Log("current tab : " + _currentTab);

            current.RemoveFromClassList("active");

            // On cherche le bouton qui a comme viewDataKey le panelName
            Button button = root.Query<Button>().Where(b => b.viewDataKey == _currentTab).First();

            // On met le focus sur le bouton
            button.parent.Focus();

            Debug.Log("Focus sur le parent de " + button.ToString());

            _currentTab = null;

            // On lance le son
            _uiAudio.PlayOneShot(_uiAudio.clip);

            return;
        }

        Debug.Log("Pas de tab chargé");
        return;
    }
}
