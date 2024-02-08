using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerSwitchDrive : PlayerSystem
{
    [SerializeField] private float _playerSoulLightIntensity;
    [SerializeField] AnimationCurve _switchDriveCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    // Events
    public static event Action OnSwitchDriveEvent;

    public void OnSwitch(InputAction.CallbackContext context)
    {
        if (!context.performed || Player._canMove == false)
        {
            return;
        }

        SwitchDrive();
    }

    public void SwitchDrive()
    {
        if (Player._drive == Player.Drive.BODY)
        {
            Player.GetAnimator().SetBool("isRunning", false);
            Player.GetRigidbody().velocity = Vector3.zero;

            Player._playerSoulRB.transform.position = Player._playerBodyRB.transform.position;

            if (Player._playerSoulRB.gameObject.activeSelf == false)
            {
                Player._playerSoulRB.gameObject.SetActive(true);
                StartCoroutine(DimmUpLight());

                // On lance le son de switch
                if (GameManager.Instance._audioManager._switchDriveSound != null)
                {
                    GameManager.Instance._audioManager._audioSource.PlayOneShot(GameManager.Instance._audioManager._switchDriveSound);
                }
            }

            Debug.Log("Changement de body à soul");
            Player._drive = Player.Drive.SOUL;

            // Stopping body movement
            Player.GetRigidbody().velocity = Vector3.zero;

            // On trigger l'event OnSwitchDrive
            OnSwitchDriveEvent?.Invoke();

        }
        else
        {
            // Stop dash coroutine if it's running
            if (Player._isDashing && Player._dashCoroutine != null)
            {
                StopCoroutine(Player._dashCoroutine);
                Player._isDashing = false;

                // Reset velocity
                Player.GetRigidbody().velocity = Vector3.zero;
            }

            if (Player._playerSoulRB.gameObject.activeSelf == true)
            {
                // The soul goes back to the body
                StartCoroutine(JoinSoulToBody());
            }
        }
    }

    IEnumerator JoinSoulToBody()
    {
        // Reduce velocity of player body and soul to zero
        Player.GetRigidbody().velocity = Vector3.zero;
        Player._canMove = false;
        Vector3 from = Player._playerSoulRB.transform.position;
        Vector3 to = Player._playerBodyRB.transform.position;

        float elapsedTime = 0f;

        // Make duration depends on distance between soul and body
        float duration = Vector3.Distance(from, to) / 8f;

        // On lance le son de switch
        if (GameManager.Instance._audioManager._switchDriveSound != null)
        {
            GameManager.Instance._audioManager._audioSource.PlayOneShot(GameManager.Instance._audioManager._switchDriveSound);
        }

        // removing all velocity to soul
        Player._playerSoulRB.velocity = Vector3.zero;

        var playerBodyRotation = Player._playerBodyRB.transform.forward;

        var startRotation = Player._playerSoulRB.transform.rotation;

        while (elapsedTime < duration)
        {
            // On rotate le _playerSoul comme le _playerBody
            Player._playerSoulRB.transform.rotation = Quaternion.Lerp(startRotation, Quaternion.LookRotation(playerBodyRotation), _switchDriveCurve.Evaluate(elapsedTime / duration));
            Player._playerSoulRB.transform.position = Vector3.Lerp(from, to, _switchDriveCurve.Evaluate(elapsedTime / duration));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Player._playerSoulRB.transform.position = to;

        yield return StartCoroutine(DimmDownLight());

        // On swap finalement
        Debug.Log("Changement de soul à body");
        Player._drive = Player.Drive.BODY;
        Player._canMove = true;

        // On trigger l'event OnSwitchDrive
        OnSwitchDriveEvent?.Invoke();
    }

    IEnumerator DimmDownLight()
    {
        float elapsedTime = 0f;
        float duration = 0.5f;

        float fromIntensity = _playerSoulLightIntensity;
        float toIntensity = 0f;

        while (elapsedTime < duration)
        {
            Player._playerSoulLight.intensity = Mathf.Lerp(fromIntensity, toIntensity, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Player._playerSoulRB.gameObject.SetActive(false);

        Player._canMove = true;
    }

    IEnumerator DimmUpLight()
    {
        float elapsedTime = 0f;
        float duration = 0.5f;

        float fromIntensity = 0f;
        float toIntensity = _playerSoulLightIntensity;

        Player._playerSoulLight.intensity = 0f;
        Player._playerSoulLight.enabled = true;

        while (elapsedTime < duration)
        {
            Player._playerSoulLight.intensity = Mathf.Lerp(fromIntensity, toIntensity, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
