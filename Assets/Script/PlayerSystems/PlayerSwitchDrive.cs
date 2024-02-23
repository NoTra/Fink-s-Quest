using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

using FinksQuest.Core;

namespace FinksQuest.PlayerSystems
{
    public class PlayerSwitchDrive : PlayerSystem
    {
        [SerializeField] private float _playerSoulLightIntensity;
        [SerializeField] AnimationCurve _switchDriveCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

        // Events
        public static event Action OnSwitchDriveEvent;

        public void OnSwitch(InputAction.CallbackContext context)
        {
            if (!context.performed || _player._canMove == false)
            {
                return;
            }

            Debug.Log("OnSwitch");

            SwitchDrive();
        }

        public void SwitchDrive()
        {
            if (_player._drive == Player.Drive.BODY)
            {
                _player.GetAnimator().SetBool("isRunning", false);
                _player.GetRigidbody().velocity = Vector3.zero;

                _player._playerSoulRB.transform.position = _player._playerBodyRB.transform.position;

                if (_player._playerSoulRB.gameObject.activeSelf == false)
                {
                    _player._playerSoulRB.gameObject.SetActive(true);
                    StartCoroutine(DimmUpLight());

                    // On lance le son de switch
                    if (GameManager.Instance._audioManager._switchDriveSound != null)
                    {
                        GameManager.Instance._audioManager._audioSource.PlayOneShot(GameManager.Instance._audioManager._switchDriveSound);
                    }
                }

                _player._drive = Player.Drive.SOUL;

                // Stopping body movement
                _player.GetRigidbody().velocity = Vector3.zero;

                // On trigger l'event OnSwitchDrive
                OnSwitchDriveEvent?.Invoke();

            }
            else
            {
                // Stop dash coroutine if it's running
                if (_player._isDashing && _player._dashCoroutine != null)
                {
                    StopCoroutine(_player._dashCoroutine);
                    _player._isDashing = false;

                    // Reset velocity
                    _player.GetRigidbody().velocity = Vector3.zero;
                }

                if (_player._playerSoulRB.gameObject.activeSelf == true)
                {
                    Debug.Log("JoinSoulToBody");
                    // The soul goes back to the body
                    StartCoroutine(JoinSoulToBody());
                }
            }
        }

        IEnumerator JoinSoulToBody()
        {
            // Reduce velocity of player body and soul to zero
            _player.GetRigidbody().velocity = Vector3.zero;
            _player._canMove = false;
            Vector3 from = _player._playerSoulRB.transform.position;
            Vector3 to = _player._playerBodyRB.transform.position;

            float elapsedTime = 0f;

            // Make duration depends on distance between soul and body
            float duration = Vector3.Distance(from, to) / 8f;

            // On lance le son de switch
            if (GameManager.Instance._audioManager._switchDriveSound != null)
            {
                GameManager.Instance._audioManager._audioSource.PlayOneShot(GameManager.Instance._audioManager._switchDriveSound);
            }

            // removing all velocity to soul
            _player._playerSoulRB.velocity = Vector3.zero;

            var playerBodyRotation = _player._playerBodyRB.transform.forward;

            var startRotation = _player._playerSoulRB.transform.rotation;

            while (elapsedTime < duration)
            {
                // On rotate le _playerSoul comme le _playerBody
                _player._playerSoulRB.transform.rotation = Quaternion.Lerp(startRotation, Quaternion.LookRotation(playerBodyRotation), _switchDriveCurve.Evaluate(elapsedTime / duration));
                _player._playerSoulRB.transform.position = Vector3.Lerp(from, to, _switchDriveCurve.Evaluate(elapsedTime / duration));

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _player._playerSoulRB.transform.position = to;

            yield return StartCoroutine(DimmDownLight());

            // On swap finalement
            _player._drive = Player.Drive.BODY;
            _player._canMove = true;

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
                _player._playerSoulLight.intensity = Mathf.Lerp(fromIntensity, toIntensity, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _player._playerSoulRB.gameObject.SetActive(false);

            _player._canMove = true;
        }

        IEnumerator DimmUpLight()
        {
            float elapsedTime = 0f;
            float duration = 0.5f;

            float fromIntensity = 0f;
            float toIntensity = _playerSoulLightIntensity;

            _player._playerSoulLight.intensity = 0f;
            _player._playerSoulLight.enabled = true;

            while (elapsedTime < duration)
            {
                _player._playerSoulLight.intensity = Mathf.Lerp(fromIntensity, toIntensity, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}