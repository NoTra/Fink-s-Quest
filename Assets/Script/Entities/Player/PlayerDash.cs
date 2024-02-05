using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerDash : PlayerSystem
{
    [SerializeField] float _dashForce = 1000f; // Ajustez la force du dash selon vos besoins
    [SerializeField] float _dashDuration = 0.5f; // Durée du dash en secondes
    [SerializeField] AnimationCurve _dashCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    [SerializeField] private float _timeBetweenDashes = 1f; // Temps entre chaque dash
    private float _lastDashTime = 0f;

    [SerializeField] private TrailRenderer trail;

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && Player.CanDash())
        {
            Debug.Log("Dash");
            Player._dashCoroutine = StartCoroutine(PerformDash());
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
        trail.transform.position = Player.GetRigidbody().transform.position;

        // Stockez la vélocité actuelle du Rigidbody
        Vector3 initialVelocity = Player.GetRigidbody().velocity;

        Vector3 initialRotation = Player.GetRigidbody().transform.rotation.eulerAngles;

        // On prend en compte la move direction du joueur pour le dash
        Vector2 moveDirection = Player._playerInput.actions["Move"].ReadValue<Vector2>();

        Vector3 destination;
        if (moveDirection == Vector2.zero)
        {
            destination = Player.GetRigidbody().transform.position + Player.GetRigidbody().transform.forward * _dashForce;
        }
        else
        {
            destination = Player.GetRigidbody().transform.position + new Vector3(moveDirection.x, 0f, moveDirection.y) * _dashForce;
        }

        float elapsedTime = 0f;

        while (elapsedTime < _dashDuration)
        {
            // On trace un raycast devant le joueur pour détecter les collisions
            RaycastHit hit;
            if (Physics.Raycast(Player.GetRigidbody().transform.position, Player.GetRigidbody().transform.forward, out hit, 0.5f, LayerMask.GetMask("Wall")))
            {
                if (Player.GetDrive() == Player.Drive.BODY)
                {
                    Debug.Log("BOOM ! On a dashé dans un mur !");
                    // StartCoroutine(_hittable.FlashRed());

                    // Appliquer la vélocité initiale
                    Player.GetRigidbody().velocity = initialVelocity;

                    // On bloque la rotation du joueur pendant le dash
                    Player.GetRigidbody().transform.rotation = Quaternion.Euler(initialRotation);
                }

                break; // Arrêter le dash en cas de collision avec un mur
            }

            Player.GetRigidbody().transform.position = Vector3.Lerp(Player.GetRigidbody().transform.position, destination, _dashCurve.Evaluate(elapsedTime / _dashDuration));


            elapsedTime += Time.deltaTime;
            yield return null;
        }

        trail.emitting = false;
        trail.enabled = false;
    }
}
