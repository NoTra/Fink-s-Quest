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
        // _playerObject = _playerBody;
        _playerAnimator = _playerBodyAnimator;

        // _striker = _playerObject.GetComponent<Striker>();
        
        /*_hittable = _playerObject.GetComponent<Hittable>();
        _striker = _playerObject.GetComponent<Striker>();*/
        _canGrab = true;
    }

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

    

    public bool IsGamepad()
    {
        return _playerInput.currentControlScheme.Equals("Gamepad");
    }
}
