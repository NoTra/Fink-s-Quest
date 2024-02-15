using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FinksQuest
{
    public class Menu : MonoBehaviour
    {
        private AudioSource _audioSource;
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private Slider _masterVolumeSlider;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _soundVolumeSlider;
        [SerializeField] private Slider _interfaceVolumeSlider;
        [SerializeField] private TMP_Dropdown _resolutionsDropdown;

        private int _minVolume = -40;
        private int _maxVolume = 0;

        private bool _isFullscreen;

        [SerializeField] private PlayerInput _playerInput;
        private string _currentControlScheme;
        

        private void Awake()
        {
            if (_playerInput != null)
            {
                _currentControlScheme = _playerInput.currentControlScheme;
            }

            _audioSource = GetComponent<AudioSource>();

            if (_audioMixer != null)
            {
                // Sound
                float masterVolume = GetVolume("MasterVolume");
                float musicVolume = GetVolume("Master/MusicVolume");
                float soundVolume = GetVolume("Master/SoundVolume");
                float interfaceVolume = GetVolume("Master/InterfaceVolume");

                _masterVolumeSlider.value = ConvertRealValueToSlider(masterVolume);
                _musicVolumeSlider.value = ConvertRealValueToSlider(musicVolume);
                _soundVolumeSlider.value = ConvertRealValueToSlider(soundVolume);
                _interfaceVolumeSlider.value = ConvertRealValueToSlider(interfaceVolume);
            }

            // Set if the application is in fullscreen
            _isFullscreen = Screen.fullScreen;
        }

        private void Update()
        {
            // _currentControlScheme = _playerInput.currentControlScheme;
        }

        public void NewGame()
        {
            SceneManager.LoadScene("DUNGEON");
        }

        public void MainMenu()
        {
            SceneManager.LoadScene("START_MENU");
        }

        public void Quit()
        {
            Application.Quit();
        }

        public void ToggleFullscreen()
        {
            Screen.fullScreen = !_isFullscreen;

            _isFullscreen = !_isFullscreen;
        }

        public void SetResolution(int index)
        {
            // Load all choices from _resolutionsDropdown
            var resolutionsOptions = _resolutionsDropdown.options.ToArray();

            // Get the resolution from the index
            var selectedResolution = resolutionsOptions[index].text;

            // Split the resolution into width and height (e.g. "1920x1080")
            var resolution = selectedResolution.Split('x');

            // Set the resolution
            Screen.SetResolution(int.Parse(resolution[0]), int.Parse(resolution[1]), Screen.fullScreen);
        }

        public void SetMasterVolume(float volume)
        {
            _audioMixer.SetFloat("MasterVolume", ConvertSliderValueToReal(volume));
        }

        public void SetMusicVolume(float volume)
        {
            _audioMixer.SetFloat("Master/MusicVolume", ConvertSliderValueToReal(volume));
        }

        public void SetSoundVolume(float volume)
        {
            _audioMixer.SetFloat("Master/SoundVolume", ConvertSliderValueToReal(volume));
        }

        public void SetInterfaceVolume(float volume)
        {
            _audioMixer.SetFloat("Master/InterfaceVolume", ConvertSliderValueToReal(volume));
        }

        private int ConvertRealValueToSlider(float realValue)
        {
            // Utiliser la formule de conversion
            float sliderValue = (realValue - _minVolume) / (_maxVolume - _minVolume) * 100f;

            return (int)sliderValue;
        }

        private int ConvertSliderValueToReal(float sliderValue)
        {
            // Utiliser la formule de conversion
            float realValue = (sliderValue / 100f) * (_maxVolume - _minVolume) + _minVolume;

            return (int)realValue;
        }

        private int GetVolume(string groupName)
        {
            // Assurez-vous que l'AudioMixer et le groupe sont valides
            if (_audioMixer != null && !string.IsNullOrEmpty(groupName))
            {
                // Créez une variable pour stocker la valeur du volume
                float volume;

                // Utilisez GetFloat pour obtenir la valeur actuelle du volume du groupe
                _audioMixer.GetFloat(groupName, out volume);

                return (int)volume;
            }
            else
            {
                Debug.LogError("AudioMixer ou le nom du groupe est invalide.");
                return 0;
            }
        }

        public void PlaySound()
        {
            _audioSource.Play();
        }
    }
}
