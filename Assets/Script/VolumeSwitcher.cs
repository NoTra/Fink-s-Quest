using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class VolumeSwitcher : MonoBehaviour
{
    [SerializeField] private Volume _soulVolume;
    [SerializeField] private Volume _bodyVolume;

    private void Awake()
    {
        // GameManager.Instance._playerController.OnSwitchDriveEvent += OnSwitchDrive;

        _bodyVolume.enabled = true;
        _soulVolume.enabled = false;
    }

    private void OnSwitchDrive()
    {
        if (GameManager.Instance._playerController._player._drive == Player.Drive.SOUL)
        {
            _soulVolume.enabled = true;
            _bodyVolume.enabled = false;
        }
        else
        {
            _soulVolume.enabled = false;
            _bodyVolume.enabled = true;
        }
    }
}
