using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public Player _player;
    private GameObject _playerObject;
    [HideInInspector]
    public Rigidbody _playerRigidbody;
    [HideInInspector] public Animator _playerAnimator;

    public PlayerInput _playerInput;

    [SerializeField] private GameObject _playerBody;
    public Rigidbody _playerBodyRB;
    public Animator _playerBodyAnimator;

    [SerializeField] private GameObject _playerSoul;
    private Rigidbody _playerSoulRB;
    private Light _playerSoulLight;
    public Animator _playerSoulAnimator;
    [SerializeField] private float _playerSoulLightIntensity;
    [SerializeField] AnimationCurve _switchDriveCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    /*[HideInInspector] public UnityEvent OnDashEvent;
    [HideInInspector] public UnityEvent OnActivateEvent;
    [HideInInspector] public UnityEvent OnStrikeEvent;
    [HideInInspector] public UnityEvent OnSwitchDriveEvent;*/

    public float _speed = 2f;
    [SerializeField] float _dashForce = 1000f; // Ajustez la force du dash selon vos besoins
    [SerializeField] float _dashDuration = 0.5f; // Durée du dash en secondes
    [SerializeField] AnimationCurve _dashCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    [SerializeField] private float _timeBetweenDashes = 1f; // Temps entre chaque dash
    private float _lastDashTime = 0f;
    private bool _isDashing = false;
    private Coroutine _dashCoroutine;

    private Striker _striker;
    private Hittable _hittable;

    [HideInInspector] public bool _canMove = true;
    private bool _isJoiningSoulToBody = false;
    public bool _isGrabbing = false;
    public bool _canGrab = true;

    private void Awake()
    {
        Debug.Log("PlayerController Awake");
        _player = GetComponent<Player>();
        _playerBodyRB = _playerBody.GetComponent<Rigidbody>();
        _playerBodyAnimator = _playerBody.GetComponent<Animator>();

        _playerSoulRB = _playerSoul.GetComponent<Rigidbody>();
        _playerSoulLight = _playerSoul.GetComponentInChildren<Light>();
        _playerSoulAnimator = _playerSoul.GetComponent<Animator>();

        _player._drive = Player.Drive.BODY;
        _playerRigidbody = _playerBodyRB;
        _playerObject = _playerBody;
        _playerAnimator = _playerBodyAnimator;

        _striker = _playerObject.GetComponent<Striker>();

        InitPlayer();
        
        _hittable = _playerObject.GetComponent<Hittable>();
        _striker = _playerObject.GetComponent<Striker>();
        _canGrab = true;
    }

    private void InitPlayer()
    {
        SkinnedMeshRenderer soulSkinnedMeshRenderer = _playerSoul.GetComponentInChildren<SkinnedMeshRenderer>();

        if (_player._drive == Player.Drive.BODY)
        {
            // Stop dash coroutine if it's running
            if (_isDashing && _dashCoroutine != null)
            {
                StopCoroutine(_dashCoroutine);
                _isDashing = false;

                // Reset velocity
                _playerRigidbody.velocity = Vector3.zero;
            }

            if (soulSkinnedMeshRenderer.enabled)
            {
                // The soul goes back to the body
                StartCoroutine(JoinSoulToBody());
            }
        }
        else
        {
            if (!soulSkinnedMeshRenderer.enabled)
            {
                soulSkinnedMeshRenderer.enabled = true;
                StartCoroutine(DimmUpLight());

                // On lance le son de switch
                if (GameManager.Instance._audioManager._switchDriveSound != null)
                {
                    GameManager.Instance._audioManager._audioSource.PlayOneShot(GameManager.Instance._audioManager._switchDriveSound);
                }
            }

            // Stopping body movement
            _playerBodyRB.velocity = Vector3.zero;

            _playerRigidbody = _playerSoulRB;
            _playerObject = _playerSoul;
            _playerAnimator = _playerSoulAnimator;

            _striker = _playerObject.GetComponent<Striker>();
        }

        // OnSwitchDriveEvent.Invoke();
    }

    #region Soul
    IEnumerator JoinSoulToBody()
    {
        // Reduce velocity of player body and soul to zero
        _playerBodyRB.velocity = Vector3.zero;
        _playerSoulRB.velocity = Vector3.zero;
        _canMove = false;
        _isJoiningSoulToBody = true;
        Vector3 from = _playerSoul.transform.position;
        Vector3 to = _playerBody.transform.position;

        float elapsedTime = 0f;

        // Make duration depends on distance between soul and body
        float duration = Vector3.Distance(from, to) / 8f;

        // On lance le son de switch
        if (GameManager.Instance._audioManager._switchDriveSound != null)
        {
            GameManager.Instance._audioManager._audioSource.PlayOneShot(GameManager.Instance._audioManager._switchDriveSound);
        }

        // removing all velocity to soul
        _playerSoulRB.velocity = Vector3.zero;

        var playerBodyRotation = _playerBody.transform.forward;

        var startRotation = _playerSoul.transform.rotation;

        while (elapsedTime < duration)
        {
            // On rotate le _playerSoul comme le _playerBody
            _playerSoul.transform.rotation = Quaternion.Lerp(startRotation, Quaternion.LookRotation(playerBodyRotation), _switchDriveCurve.Evaluate(elapsedTime / duration));

            _playerSoul.transform.position = Vector3.Lerp(from, to, _switchDriveCurve.Evaluate(elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _playerSoul.transform.position = to;

        yield return StartCoroutine(DimmDownLight());

        _playerRigidbody = _playerBodyRB;
        _playerObject = _playerBody;
        _playerAnimator = _playerBodyAnimator;

        _striker = _playerObject.GetComponent<Striker>();
    }

    IEnumerator DimmDownLight()
    {
        float elapsedTime = 0f;
        float duration = 0.5f;

        float fromIntensity = _playerSoulLightIntensity;
        float toIntensity = 0f;

        while (elapsedTime < duration)
        {
            _playerSoulLight.intensity = Mathf.Lerp(fromIntensity, toIntensity, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _playerSoul.GetComponentInChildren<Light>().enabled = false;
        _playerSoul.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        _isJoiningSoulToBody = false;
        _canMove = true;
    }

    IEnumerator DimmUpLight()
    {
        float elapsedTime = 0f;
        float duration = 0.5f;

        float fromIntensity = 0f;
        float toIntensity = _playerSoulLightIntensity;

        _playerSoulLight.intensity = 0f;
        _playerSoul.GetComponentInChildren<Light>().enabled = true;

        while (elapsedTime < duration)
        {
            _playerSoulLight.intensity = Mathf.Lerp(fromIntensity, toIntensity, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public void OnSwitchDrive(InputAction.CallbackContext context)
    {
        if (_canMove == false || !context.performed)
        {
            return;
        }

        Debug.Log("Switch drive from PlayerController::OnSwitchDrive (current state : " + _player._drive + ")");

        if (_player._drive == Player.Drive.BODY)
        {
            _playerAnimator.SetBool("isRunning", false);
            _playerRigidbody.velocity = Vector3.zero;

            _player._drive = Player.Drive.SOUL;
        }
        else
        {
            _player._drive = Player.Drive.BODY;
        }

        InitPlayer();
    }
    #endregion

    #region Dash
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && CanDash())
        {
            Debug.Log("Dash");
            _dashCoroutine = StartCoroutine(PerformDash());
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

        TrailRenderer trail = _playerObject.GetComponentInChildren<TrailRenderer>();
        // Réinitialisez les positions du Trail Renderer
        trail.Clear();
        trail.emitting = true;
        trail.enabled = true;
        trail.transform.position = _playerRigidbody.transform.position;

        // Stockez la vélocité actuelle du Rigidbody
        Vector3 initialVelocity = _playerRigidbody.velocity;

        Vector3 initialRotation = _playerRigidbody.transform.rotation.eulerAngles;

        // On prend en compte la move direction du joueur pour le dash
        Vector2 moveDirection = _playerInput.actions["Move"].ReadValue<Vector2>();

        Vector3 destination;
        if(moveDirection == Vector2.zero)
        {
            destination = _playerRigidbody.transform.position + _playerRigidbody.transform.forward * _dashForce;
        } else
        {
            destination = _playerRigidbody.transform.position + new Vector3(moveDirection.x, 0f, moveDirection.y) * _dashForce;
        }

        float elapsedTime = 0f;
        _isDashing = true;

        while (elapsedTime < _dashDuration)
        {
            // On trace un raycast devant le joueur pour détecter les collisions
            RaycastHit hit;
            if (Physics.Raycast(_playerRigidbody.transform.position, _playerRigidbody.transform.forward, out hit, 0.5f, LayerMask.GetMask("Wall")))
            {
                if (_player._drive == Player.Drive.BODY)
                {
                    Debug.Log("BOOM ! On a dashé dans un mur !");
                    StartCoroutine(_hittable.FlashRed());

                    // Appliquer la vélocité initiale
                    _playerRigidbody.velocity = initialVelocity;

                    // On bloque la rotation du joueur pendant le dash
                    _playerRigidbody.transform.rotation = Quaternion.Euler(initialRotation);
                }

                break; // Arrêter le dash en cas de collision avec un mur
            }

            _playerRigidbody.transform.position = Vector3.Lerp(_playerRigidbody.transform.position, destination, _dashCurve.Evaluate(elapsedTime / _dashDuration));


            elapsedTime += Time.deltaTime;
            yield return null;
        }

        trail.emitting = false;
        trail.enabled = false;

        _isDashing = false;
    }

    #endregion

    #region Strike

    public void OnStrike(InputAction.CallbackContext context)
    {
        if (_striker._isStriking)
        {
            return;
        }

        _striker.Strike();
    }

    #endregion

    #region Activate
    public void OnActivate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // On met la position du raycast au milieu du joueur
            Vector3 raycastStartPosition = _playerRigidbody.transform.position + new Vector3(0f, 0.5f, 0f);

            // Si on détecte un objet avec lequel on peut interagir (layer "Interactable") à moins de 0.5 devant le joueur
            RaycastHit hit;
            Debug.DrawRay(raycastStartPosition, _playerRigidbody.transform.forward * 0.5f, Color.red, 2f);
            if (Physics.Raycast(raycastStartPosition, _playerRigidbody.transform.forward, out hit, 0.5f, LayerMask.GetMask("Interactable")))
            {
                Debug.Log("On a touché un objet interactable : " + hit.collider.gameObject.name);
                // On récupère le composant Interactable de l'objet
                PushButton interactable = hit.collider.GetComponent<PushButton>();
                if (interactable != null)
                {
                    // On appelle la méthode Toggle() de l'objet
                    interactable.Toggle();
                }
            }
        }
    }
    #endregion

    private void Update()
    {
        // On trace un raycast devant le joueur pour détecter les collisions
        RaycastHit hit;
        if (Physics.Raycast(_playerRigidbody.transform.position, _playerRigidbody.transform.forward, out hit, 0.2f, LayerMask.GetMask("Wall")))
        {
            _playerRigidbody.velocity = Vector3.zero;
        }

        // On récupère l'input du joueur
        Vector2 moveDirection = _playerInput.actions["Move"].ReadValue<Vector2>();

        // On passe à l'animator que la variable velocity = _playerRigidbody.velocity.magnitude
        _playerAnimator.SetBool("isRunning", (moveDirection != Vector2.zero));
    }

    private void FixedUpdate()
    {
        if (!_canMove || !String.Equals(_playerInput.currentActionMap.name, "Player", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        Vector2 moveDirection = _playerInput.actions["Move"].ReadValue<Vector2>();

        Vector3 velocity = _playerRigidbody.velocity;

        // Si le current device est Gamepad
        // if (IsGamepad())
        // {
            velocity.x = moveDirection.x * _speed;
            velocity.z = moveDirection.y * _speed;
            _playerRigidbody.velocity = velocity;

            // Rotation du joueur en fonction de la direction du movement
            if (moveDirection != Vector2.zero && CanRotate())
            {
                float angle = Mathf.Atan2(moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
                _playerRigidbody.transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }
        /*} else
        {
            // On trace un raycast du centre de la camera vers le ground
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Ground")))
            {
                // On rotate le joueur sur l'axe Y vers le point d'impact
                Vector3 targetPosition = new Vector3(hit.point.x, _playerRigidbody.transform.position.y, hit.point.z);
                _playerRigidbody.transform.LookAt(targetPosition);
            }

            // Calcule la direction du déplacement en fonction des coordonnées de la souris
            // Vector3 moveDirectionMouse = new Vector3(moveDirection.x, 0f, moveDirection.y);

            // Transforme la direction du mouvement en fonction de la rotation actuelle du joueur
            // moveDirectionMouse = _playerRigidbody.transform.TransformDirection(moveDirectionMouse);

            // Déplace le joueur vers la direction de la souris
            // _playerRigidbody.velocity = moveDirectionMouse.normalized * _speed;

            velocity.x = moveDirection.x * _speed;
            velocity.z = moveDirection.y * _speed;
            _playerRigidbody.velocity = velocity;
        }*/
    }

    private void LateUpdate()
    {
        if (_player._drive == Player.Drive.BODY && !_isJoiningSoulToBody)
        {
            // On déplace le _playerSoul à la position du _playerBody
            _playerSoul.transform.position = _playerBody.transform.position;

            // On rotate le _playerSoul comme le _playerBody
            _playerSoul.transform.rotation = _playerBody.transform.rotation;
        }
    }

    public bool CanRotate()
    {
        return !_isDashing && !_striker._isStriking && !_isGrabbing;
    }

    public bool CanDash()
    {
        return !_isDashing && !_striker._isStriking && !_isGrabbing && _canMove;
    }

    public bool IsGamepad()
    {
        return _playerInput.currentControlScheme.Equals("Gamepad");
    }
}
