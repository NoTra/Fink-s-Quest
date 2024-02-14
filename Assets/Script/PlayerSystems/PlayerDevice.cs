using UnityEngine;
using UnityEngine.InputSystem;

using FinksQuest.Core;

namespace FinksQuest.PlayerSystems
{
    public class PlayerDevice : MonoBehaviour
    {
        public string deviceName;
        [SerializeField] private PlayerInput _playerInput;

        void Start()
        {
            deviceName = (_playerInput != null) ? _playerInput.currentControlScheme : "";
        }

        void Update()
        {
            if (_playerInput != null && _playerInput.currentControlScheme != deviceName)
            {
                deviceName = _playerInput.currentControlScheme;

                // On sauvegarde le device actuel
                PlayerPrefs.SetString("PlayerDevice", deviceName);
            }
        }
    }
}