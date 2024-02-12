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

            /*_meshRenderer = GetComponent<MeshRenderer>();
            _boxCollider = GetComponent<BoxCollider>();*/
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
            Debug.Log("SwitchVisibility() called ! : " + GameManager.Instance.Player.GetDrive());

            if (GameManager.Instance.Player.GetDrive() == Player.Drive.SOUL)
            {
                if (isVisibleForSoul)
                {
                    // Activate _meshRenderer && _boxCollider
                    if (_meshRenderer != null)
                        _meshRenderer.enabled = true;

                    if (_boxCollider != null)
                        _boxCollider.enabled = true;

                    // Find all the children of the object
                    foreach (Transform child in transform)
                    {
                        child.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                if (isVisibleForSoul)
                {
                    if (_meshRenderer != null)
                        _meshRenderer.enabled = false;

                    if (_boxCollider != null)
                    _boxCollider.enabled = false;

                    foreach (Transform child in transform)
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}