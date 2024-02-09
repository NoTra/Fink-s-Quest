using UnityEngine;
using UnityEngine.Rendering;

using FinksQuest.Core;
using FinksQuest.PlayerSystems;

namespace FinksQuest.Effects
{
    public class VolumeSwitcher : MonoBehaviour
    {
        private Volume _volume;
        [SerializeField] private VolumeProfile _soulVolume;
        [SerializeField] private VolumeProfile _bodyVolume;

        private void Awake()
        {
            _volume = GetComponent<Volume>();
            _volume.profile = _bodyVolume;
        }

        private void OnEnable()
        {
            // Add an event handler for the drive switch event.
            PlayerSwitchDrive.OnSwitchDriveEvent += SwitchVolume;
        }

        private void OnDisable()
        {
            // Remove the event handler for the drive switch event.
            PlayerSwitchDrive.OnSwitchDriveEvent -= SwitchVolume;
        }

        public void SwitchVolume()
        {
            _volume.profile = (GameManager.Instance.Player.GetDrive() == Player.Drive.SOUL) ? _soulVolume : _bodyVolume;
        }
    }
}