using UnityEngine;
using UnityEngine.InputSystem;

namespace FinksQuest.Core
{
    public class Player : MonoBehaviour
    {
        public PlayerID ID;

        // Constante "drive" : BODY or SOUL
        public enum Drive
        {
            BODY,
            SOUL
        }

        public Drive _drive = Drive.BODY;

        // Player RBs and Animators
        [SerializeField] private GameObject _playerBody;
        [HideInInspector] public Rigidbody _playerBodyRB;
        [HideInInspector] public Animator _playerBodyAnimator;
        [SerializeField] private GameObject _playerSoul;
        [HideInInspector] public Rigidbody _playerSoulRB;
        [HideInInspector] public Light _playerSoulLight;
        [HideInInspector] public Animator _playerSoulAnimator;

        [HideInInspector] public PlayerInput _playerInput;

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioManager _audioManager;

        // States & Cooldowns
        public bool _canMove = true;
        public bool _canGrab = true;

        public bool _isGrabbing = false;
        public bool _isDashing = false;

        public Coroutine _dashCoroutine;

        private void Awake()
        {
            _playerBodyRB = _playerBody.GetComponent<Rigidbody>();
            _playerBodyAnimator = _playerBody.GetComponent<Animator>();

            _playerSoulRB = _playerSoul.GetComponent<Rigidbody>();
            _playerSoulLight = _playerSoul.GetComponent<Light>();
            _playerSoulAnimator = _playerSoul.GetComponent<Animator>();

            _playerInput = GetComponent<PlayerInput>();
        }

        public Rigidbody GetRigidbody()
        {
            if (_drive == Drive.BODY)
                return _playerBodyRB;
            else
                return _playerSoulRB;
        }

        public Animator GetAnimator()
        {
            if (_drive == Drive.BODY)
                return _playerBodyAnimator;
            else
                return _playerSoulAnimator;
        }

        public Drive GetDrive()
        {
            return _drive;
        }

        public bool CanDash()
        {
            return !_isDashing && !_isGrabbing && _canMove && GetDrive() != Player.Drive.SOUL; // !_striker._isStriking
        }
    }
}