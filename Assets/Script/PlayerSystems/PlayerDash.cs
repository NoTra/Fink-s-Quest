using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

using FinksQuest.Core;

namespace FinksQuest.PlayerSystems
{
    public class PlayerDash : PlayerSystem
    {
        [SerializeField] float _dashForce = 1.2f; // Ajustez la force du dash selon vos besoins
        [SerializeField] float _dashDuration = 0.5f; // Durée du dash en secondes
        [SerializeField] AnimationCurve _dashCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
        [SerializeField] private float _timeBetweenDashes = 1f; // Temps entre chaque dash
        private float _lastDashTime = 0f;

        [SerializeField] private GameObject _stepEffect;
        [SerializeField] private AudioClip _dashSound;

        [SerializeField] private TrailRenderer trail;

        private Vector3 _desiredPosition;

        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.performed && _player.CanDash())
            {
                // On ne fait pas de dash si le joueur a fait un dash il y a moins de _timeBetweenDashes secondes
                if (Time.time - _lastDashTime >= _timeBetweenDashes)
                {
                    _player._dashCoroutine = StartCoroutine(PerformDash());
                }
            }
        }

        IEnumerator PerformDash()
        {
            Debug.Log("PerformDash");
            _player._isDashing = true;
            _player._canMove = false;

            _lastDashTime = Time.time;

            // TrailRenderer trail = _playerObject.GetComponentInChildren<TrailRenderer>();
            // Réinitialisez les positions du Trail Renderer
            trail.Clear();
            trail.emitting = true;
            trail.enabled = true;
            trail.transform.position = _player.GetRigidbody().transform.position;

            // Stockez la vélocité actuelle du Rigidbody
            Vector3 initialVelocity = _player.GetRigidbody().velocity;

            Vector3 initialRotation = _player.GetRigidbody().transform.localRotation.eulerAngles;

            // On prend en compte la move direction du joueur pour le dash
            Vector2 moveDirection = _player._playerInput.actions["Move"].ReadValue<Vector2>();

            // On instantie un stepPoof
            var stepEffect = Instantiate(_stepEffect, _player.GetRigidbody().transform.position, Quaternion.identity);
            Destroy(stepEffect, 2);

            // On lance le son de dash
            if (_dashSound != null)
            {
                GameManager.Instance._audioManager._audioSource.PlayOneShot(_dashSound, 0.1f);
            }

            var startPosition = _player.GetRigidbody().transform.localPosition;

            Vector3 destination;
            if (moveDirection == Vector2.zero)
            {
                destination = _player.GetRigidbody().transform.localPosition + _player.GetRigidbody().transform.forward * _dashForce;
            }
            else
            {
                destination = _player.GetRigidbody().transform.localPosition + new Vector3(moveDirection.x, 0f, moveDirection.y) * _dashForce;
            }

            // On trace un raycast devant le joueur pour détecter les collisions
            if (Physics.Raycast(_player.GetRigidbody().transform.position, _player.GetRigidbody().transform.forward, out RaycastHit hit, _dashForce * 1.1f, LayerMask.GetMask("Wall")))
            {
                if (hit.distance > (_dashForce * 0.5f))
                {
                    Debug.Log("Hit object : " + hit.collider.gameObject.name);
                    if (_player.GetDrive() == Player.Drive.BODY)
                    {
                        // Appliquer la vélocité initiale
                        _player.GetRigidbody().velocity = initialVelocity;

                        // On bloque la rotation du joueur pendant le dash
                        _player.GetRigidbody().transform.localRotation = Quaternion.Euler(initialRotation);
                    }

                    var direction = new Vector3(hit.point.x, _player.GetRigidbody().transform.position.y, hit.point.z) - _player.GetRigidbody().transform.position;
                    var bounds = _player.GetRigidbody().GetComponent<CapsuleCollider>().radius;

                    destination = _player.GetRigidbody().transform.localPosition + direction.normalized * (_dashForce * 0.8f - bounds);
                }
                else
                {
                    destination = _player.GetRigidbody().transform.localPosition;
                }
            }

            float elapsedTime = 0f;
            while (elapsedTime < _dashDuration)
            {
                elapsedTime += Time.deltaTime;

                _desiredPosition = Vector3.Lerp(startPosition, destination, _dashCurve.Evaluate(elapsedTime / _dashDuration));

                yield return null;
            }

            _desiredPosition = destination;

            trail.emitting = false;
            trail.enabled = false;

            _player._canMove = true;
            _player._isDashing = false;
        }

        private void LateUpdate()
        {
            if (_player._isDashing) {
                _player.GetRigidbody().transform.localPosition = _desiredPosition;
            }
        }
    }
}