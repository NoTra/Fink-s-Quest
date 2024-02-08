using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GoToNextRoom : MonoBehaviour
{
    // Triggers
    [SerializeField] private Room _roomA;
    [SerializeField] private Trigger _triggerA;
    [SerializeField] private Transform _outPointA;

    [SerializeField] private Room _roomB;
    [SerializeField] private Trigger _triggerB;
    [SerializeField] private Transform _outPointB;

    // Variable utilis�e pour r�aliser la transition
    private GameObject _nextRoom;
    private GameObject _mirrorTrigger;
    private Transform _outPoint;

    // Player
    private Player _player;
    private Rigidbody _playerRigidbody;
    private Animator _playerAnimator;

    // Camera
    private CameraMovement _cameraMovement;
    private Camera _mainCamera;

    [SerializeField] AnimationCurve _transitionCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    [SerializeField] private Animator _transitionAnimator;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _cameraMovement = _mainCamera.GetComponent<CameraMovement>();

        _player = GameManager.Instance.Player;
        _playerRigidbody = _player.GetRigidbody();
        _playerAnimator = _player.GetAnimator();
    }

    void Start()
    {
        
    }

    private void Update()
    {
        if (_triggerA._isTriggered)
        {
            _nextRoom = _roomB.gameObject;
            _outPoint = _outPointA;
            _triggerA._isTriggered = false;
            _mirrorTrigger = _triggerB.gameObject;
            StartCoroutine(PerformGoToNextRoom());
        }
        else if (_triggerB._isTriggered)
        {
            _nextRoom = _roomA.gameObject;
            _outPoint = _outPointB;
            _triggerB._isTriggered = false;
            _mirrorTrigger = _triggerA.gameObject;
            StartCoroutine(PerformGoToNextRoom());
        }
    }

    private IEnumerator PerformGoToNextRoom()
    {
        Debug.Log("PerformGoToNextRoom...");

        // On d�sactive le mouvement de la cam�ra
        _cameraMovement._freeMove = true;

        // On d�sactive le mouvement du joueur
        _player._canMove = false;
        _player._canGrab = false;
        _playerRigidbody.velocity = Vector3.zero;
        _playerAnimator.SetBool("isRunning", false);

        float elapsedTime = 0;
        float duration = 2f;

        Vector3 playerStartPosition = _playerRigidbody.transform.position;
        Vector3 playerEndPosition = new (
            _outPoint.transform.position.x,
            _playerRigidbody.transform.position.y, 
            _outPoint.transform.position.z
        );

        // On d�finit la position de d�part et d'arriv�e de la cam�ra pour la transition
        var cameraStartPosition = _mainCamera.transform.position;
        var cameraEndPosition = new Vector3(_outPoint.transform.position.x, _nextRoom.GetComponent<Room>()._maxZoom, _outPoint.transform.position.z);//_cameraMovement.GetConstrainedCameraPosition(playerEndPosition);

        var direction = playerEndPosition - playerStartPosition;

        var playerStartRotation = _playerRigidbody.transform.forward;

        // On d�sactive les trigger durant la transition
        _mirrorTrigger.SetActive(false);

        // On d�sactive le collider du joueur durant la transition
        _playerRigidbody.GetComponent<Collider>().enabled = false;
        _playerAnimator.SetBool("isRunning", true);

        // D�but transition
        StartCoroutine(StartRoomTransition());

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // On fait marcher le joueur vers le trigger mirroir de la prochaine salle
            _playerRigidbody.transform.position = Vector3.Lerp(playerStartPosition, playerEndPosition, _transitionCurve.Evaluate(elapsedTime / duration));

            // On rotate le joueur vers la direction
            _playerRigidbody.transform.forward = Vector3.Lerp(playerStartRotation, direction, _transitionCurve.Evaluate(elapsedTime / duration));

            yield return null;
        }

        // On r�active les triggers quand la transition est termin�e
        _mirrorTrigger.SetActive(true);
        _playerRigidbody.GetComponent<Collider>().enabled = true;
        _player._canMove = true;
        _player._canGrab = true;
        _playerAnimator.SetBool("isRunning", false);

        GameManager.Instance._currentRoom = _nextRoom;

        // Activation du dialog si pr�sent
        DialogBox dialogBox = _nextRoom.GetComponent<DialogBox>();

        if (dialogBox != null)
        {
            Debug.Log("DialogBox of " + _nextRoom.name + " enabled !");
            dialogBox.enabled = true;
        }

        Debug.Log("Camera movement init...");
        // On r�initialise la cam�ra sur la nouvelle salle
        _cameraMovement.Init();
        _cameraMovement._freeMove = false;

        yield return null;

        // Fin transition
        StartCoroutine(EndRoomTransition());
    }

    IEnumerator StartRoomTransition()
    {
        _transitionAnimator.SetTrigger("StartTransition");
        yield return null;
    }

    IEnumerator EndRoomTransition()
    {
        _transitionAnimator.SetTrigger("EndTransition");
        yield return null;
    }
}
