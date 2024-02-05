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
            if (isVisibleForSoul)
            {
                // Activate _meshRenderer && _boxCollider
                _meshRenderer.enabled = true;
                _boxCollider.enabled = true;
            }
            else
            {
                // Désactivate _meshRenderer && _boxCollider
                _meshRenderer.enabled = false;
                _boxCollider.enabled = false;
            }
        }
        else
        {
            if (isVisibleForSoul)
            {
                _meshRenderer.enabled = false;
                _boxCollider.enabled = false;
            }
            else
            {
                _meshRenderer.enabled = true;
                _boxCollider.enabled = true;
            }
        }
    }
}
