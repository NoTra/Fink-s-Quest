using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HiddenDoor : Openable
{
    float _transitionSpeed = 3f;

    // Start is called before the first frame update
    void Start()
    {
        _previousOpenState = _isOpen;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if (_isOpen == _previousOpenState)
        {
            return;
        }

        if (_isOpen)
        {
            StartCoroutine(OpenDoor());
            _previousOpenState = _isOpen;
        }
    }

    public IEnumerator OpenDoor()
    {
        GameManager.Instance._playerController._playerRigidbody.velocity = Vector3.zero;
        GameManager.Instance._playerController._canGrab = false;
        GameManager.Instance._playerController._canMove = false;

        CameraMovement cameraMovement = Camera.main.GetComponent<CameraMovement>();
        cameraMovement._freeMove = true;

        Vector3 currentPosition = Camera.main.transform.position;
        
        // We move the camera to the door
        yield return StartCoroutine(cameraMovement.MoveCameraToLocation(transform.position));

        // We shake the camera
        StartCoroutine(cameraMovement.ShakeCamera(_transitionSpeed, 0.05f));

        // We open the door and play sound
        // Play sound
        AudioSource _audioSource = GetComponent<AudioSource>();
        _audioSource.PlayOneShot(GameManager.Instance._audioManager._hiddenOpenDoorSound);

        float elapsedTime = 0f;
        Vector3 start = transform.position;
        Vector3 end = transform.position + new Vector3(0, -1.5f, 0);

        Vector3 playerPosition = GameManager.Instance._playerController._playerRigidbody.position;

        while (elapsedTime < _transitionSpeed)
        {
            transform.position = Vector3.Lerp(start, end, (elapsedTime / _transitionSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Play sound
        _audioSource.PlayOneShot(AudioManager.Instance._secretFoundSound);
        // Wait for sound to finish
        yield return new WaitForSeconds(AudioManager.Instance._secretFoundSound.length - 1f);

        // We go back to previous position
        yield return StartCoroutine(cameraMovement.MoveCameraToLocation(currentPosition));

        GameManager.Instance._playerController._canGrab = true;
        GameManager.Instance._playerController._canMove = true;

        cameraMovement._freeMove = false;
    }
}
