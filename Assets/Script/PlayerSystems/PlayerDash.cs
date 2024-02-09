using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

using FinksQuest.Core;

namespace FinksQuest.PlayerSystems
{
    public class PlayerDash : PlayerSystem
    {
        [SerializeField] float _dashForce = 1000f; // Ajustez la force du dash selon vos besoins
        [SerializeField] float _dashDuration = 0.5f; // Durée du dash en secondes
        [SerializeField] AnimationCurve _dashCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
        [SerializeField] private float _timeBetweenDashes = 1f; // Temps entre chaque dash
        private float _lastDashTime = 0f;

        [SerializeField] private GameObject _stepEffect;
        [SerializeField] private AudioClip _dashSound;

        [SerializeField] private TrailRenderer trail;

        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.performed && _player.CanDash())
            {
                _player._dashCoroutine = StartCoroutine(PerformDash());
            }
        }

        IEnumerator PerformDash()
        {
            // On ne fait pas de dash si le joueur a fait un dash il y a moins de _timeBetweenDashes secondes
            if (Time.time - _lastDashTime < _timeBetweenDashes)
            {
                yield break;
            }

            _lastDashTime = Time.time;

            // TrailRenderer trail = _playerObject.GetComponentInChildren<TrailRenderer>();
            // Réinitialisez les positions du Trail Renderer
            trail.Clear();
            trail.emitting = true;
            trail.enabled = true;
            trail.transform.position = _player.GetRigidbody().transform.position;

            // Stockez la vélocité actuelle du Rigidbody
            Vector3 initialVelocity = _player.GetRigidbody().velocity;

            Vector3 initialRotation = _player.GetRigidbody().transform.rotation.eulerAngles;

            // On prend en compte la move direction du joueur pour le dash
            Vector2 moveDirection = _player._playerInput.actions["Move"].ReadValue<Vector2>();

            Vector3 destination;
            if (moveDirection == Vector2.zero)
            {
                destination = _player.GetRigidbody().transform.position + _player.GetRigidbody().transform.forward * _dashForce;
            }
            else
            {
                destination = _player.GetRigidbody().transform.position + new Vector3(moveDirection.x, 0f, moveDirection.y) * _dashForce;
            }

            // On instantie un stepPoof
            var stepEffect = Instantiate(_stepEffect, _player.GetRigidbody().transform.position, Quaternion.identity);
            Destroy(stepEffect, 2);

            // On lance le son de dash
            if (_dashSound != null)
            {
                GameManager.Instance._audioManager._audioSource.PlayOneShot(_dashSound);
            }

            float elapsedTime = 0f;

            while (elapsedTime < _dashDuration)
            {
                // On trace un raycast devant le joueur pour détecter les collisions
                RaycastHit hit;
                if (Physics.Raycast(_player.GetRigidbody().transform.position, _player.GetRigidbody().transform.forward, out hit, 0.5f, LayerMask.GetMask("Wall")))
                {
                    if (_player.GetDrive() == Player.Drive.BODY)
                    {
                        // StartCoroutine(_hittable.FlashRed());

                        // Appliquer la vélocité initiale
                        _player.GetRigidbody().velocity = initialVelocity;

                        // On bloque la rotation du joueur pendant le dash
                        _player.GetRigidbody().transform.rotation = Quaternion.Euler(initialRotation);
                    }

                    break; // Arrêter le dash en cas de collision avec un mur
                }

                _player.GetRigidbody().transform.position = Vector3.Lerp(_player.GetRigidbody().transform.position, destination, _dashCurve.Evaluate(elapsedTime / _dashDuration));


                elapsedTime += Time.deltaTime;
                yield return null;
            }

            trail.emitting = false;
            trail.enabled = false;
        }
    }
}