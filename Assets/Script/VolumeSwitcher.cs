using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class VolumeSwitcher : MonoBehaviour
{
    private Volume _volume;
    [SerializeField] private VolumeProfile _soulVolume;
    [SerializeField] private VolumeProfile _bodyVolume;

    private void Awake()
    {
        _volume = GetComponent<Volume>();
        _volume.profile = _bodyVolume;

        // Add an event handler for the drive switch event.
        PlayerSwitchDrive.OnSwitchDriveEvent += SwitchVolume;
    }

    public void SwitchVolume()
    {
        Debug.Log("SwitchVolume (volumeSwitcher)... ");
        _volume.profile = (GameManager.Instance.Player.GetDrive() == Player.Drive.SOUL) ? _soulVolume : _bodyVolume;
    }
}
