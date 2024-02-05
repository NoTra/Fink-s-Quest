using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class Visibility : MonoBehaviour
{
    [SerializeField] bool isVisibleForSoul;
    MeshRenderer _meshRenderer;
    BoxCollider _boxCollider;

    // Listen to Action OnSwitchDriveEvent

    private void Awake()
    {
        Debug.Log("Visibility Awake");
        // GameManager.Instance._playerController.OnSwitchDriveEvent += OnSwitchDrive;

        _meshRenderer = GetComponent<MeshRenderer>();
        _boxCollider = GetComponent<BoxCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Visibility Start");
        SwitchVisibility();
    }

    private void OnSwitchDrive()
    {
        Debug.Log("OnSwitchDrive called on Visibility!");
        SwitchVisibility();
    }

    private void SwitchVisibility()
    {
        if (GameManager.Instance.Player.GetDrive() == Player.Drive.SOUL)
        {
            Debug.Log("player is soul !");
            if (isVisibleForSoul)
            {
                Debug.Log("Activate crate because player is soul");
                // Activate _meshRenderer && _boxCollider
                _meshRenderer.enabled = true;
                _boxCollider.enabled = true;
            }
            else
            {
                Debug.Log("Désactivate crate because player is soul");
                // Désactivate _meshRenderer && _boxCollider
                _meshRenderer.enabled = false;
                _boxCollider.enabled = false;
            }
        }
        else
        {
            if (isVisibleForSoul)
            {
                Debug.Log("Activate crate because player is not soul");
                _meshRenderer.enabled = false;
                _boxCollider.enabled = false;
            }
            else
            {
                Debug.Log("Désactivate crate because player is not soul");
                _meshRenderer.enabled = true;
                _boxCollider.enabled = true;
            }
        }
    }
}
