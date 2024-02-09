using UnityEngine;

using FinksQuest.Core;
using FinksQuest.PlayerSystems;

namespace FinksQuest.Effects
{
    public class Visibility : MonoBehaviour
    {
        [SerializeField] bool isVisibleForSoul;
        MeshRenderer _meshRenderer;
        BoxCollider _boxCollider;

        // Listen to Action OnSwitchDriveEvent

        private void Awake()
        {
            // GameManager.Instance._playerController.OnSwitchDriveEvent += OnSwitchDrive;

            _meshRenderer = GetComponent<MeshRenderer>();
            _boxCollider = GetComponent<BoxCollider>();
        }

        private void OnEnable()
        {
            PlayerSwitchDrive.OnSwitchDriveEvent += SwitchVisibility;
        }

        private void OnDisable()
        {
            PlayerSwitchDrive.OnSwitchDriveEvent -= SwitchVisibility;
        }

        // Start is called before the first frame update
        void Start()
        {
            SwitchVisibility();
        }

        private void SwitchVisibility()
        {
            if (GameManager.Instance.Player.GetDrive() == Player.Drive.SOUL)
            {
                if (isVisibleForSoul)
                {
                    // Activate _meshRenderer && _boxCollider
                    _meshRenderer.enabled = true;
                    _boxCollider.enabled = true;
                }
                /*else
                {
                    // Désactivate _meshRenderer && _boxCollider
                    _meshRenderer.enabled = false;
                    _boxCollider.enabled = false;
                }*/
            }
            else
            {
                if (isVisibleForSoul)
                {
                    _meshRenderer.enabled = false;
                    _boxCollider.enabled = false;
                }
                /*else
                {
                    _meshRenderer.enabled = true;
                    _boxCollider.enabled = true;
                }*/
            }
        }
    }
}