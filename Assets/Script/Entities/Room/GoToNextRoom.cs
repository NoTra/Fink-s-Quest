using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GoToNextRoom : MonoBehaviour
{
    public GameObject _room;
    [HideInInspector] public GameObject _nextRoom;
    public GameObject _mirrorTrigger;
    public GameObject _outPoint;
    private Player _player;
    private Rigidbody _playerRigidbody;
    private Animator _playerAnimator;
    private CameraMovement _cameraMovement;
    private Camera _mainCamera;

    [SerializeField] AnimationCurve _transitionCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    private void Awake()
    {
        _mainCamera = Camera.main;
        _cameraMovement = _mainCamera.GetComponent<CameraMovement>();

        _player = GameManager.Instance.Player;
        _playerRigidbody = _player.GetRigidbody();
        _playerAnimator = _player.GetAnimator();

        _nextRoom = _mirrorTrigger.GetComponent<GoToNextRoom>()._room.gameObject;
    }

    void Start()
    {
        
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            GameManager.Instance._currentRoom = _nextRoom;

            // On Lerp la caméra vers la prochaine salle, on monte de la hauteur de la salle actuelle
            StartCoroutine(PerformGoToNextRoom());
        }
    }

    private IEnumerator PerformGoToNextRoom()
    {
        Debug.Log("PerformGoToNextRoom...");
        // On désactive le mouvement de la caméra
        _cameraMovement._freeMove = true;

        // On désactive le mouvement du joueur
        _player._canMove = false;
        _player._canGrab = false;
        _playerRigidbody.velocity = Vector3.zero;
        _playerAnimator.SetBool("isRunning", false);

        float elapsedTime = 0;
        float duration = 1f;

        Vector3 playerStartPosition = _playerRigidbody.transform.position;
        Vector3 playerEndPosition = new (
            _outPoint.transform.position.x,
            _playerRigidbody.transform.position.y, 
            _outPoint.transform.position.z
        );

        // On définit la position de départ et d'arrivée de la caméra pour la transition
        var cameraStartPosition = _mainCamera.transform.position;
        var cameraEndPosition = new Vector3(_outPoint.transform.position.x, _nextRoom.GetComponent<Room>()._maxZoom, _outPoint.transform.position.z);//_cameraMovement.GetConstrainedCameraPosition(playerEndPosition);

        var direction = playerEndPosition - playerStartPosition;

        var playerStartRotation = _playerRigidbody.transform.forward;

        // On désactive les trigger durant la transition
        _mirrorTrigger.SetActive(false);

        // On désactive le collider du joueur durant la transition
        _playerRigidbody.GetComponent<Collider>().enabled = false;
        _playerAnimator.SetBool("isRunning", true);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // On Lerp la caméra vers la prochaine salle
            _mainCamera.transform.position = Vector3.Lerp(cameraStartPosition, cameraEndPosition, elapsedTime / duration);

            // On fait marcher le joueur vers le trigger mirroir de la prochaine salle
            _playerRigidbody.transform.position = Vector3.Lerp(playerStartPosition, playerEndPosition, _transitionCurve.Evaluate(elapsedTime / duration));

            // On rotate le joueur vers la direction
            _playerRigidbody.transform.forward = Vector3.Lerp(playerStartRotation, direction, _transitionCurve.Evaluate(elapsedTime / duration));

            yield return null;
        }

        // On réactive les triggers quand la transition est terminée
        _mirrorTrigger.SetActive(true);
        _playerRigidbody.GetComponent<Collider>().enabled = true;
        _player._canMove = true;
        _player._canGrab = true;
        _playerAnimator.SetBool("isRunning", false);

        // Activation du dialog si présent
        DialogBox dialogBox = _nextRoom.GetComponent<DialogBox>();

        if (dialogBox != null)
        {
            Debug.Log("DialogBox of " + _nextRoom.name + " enabled !");
            dialogBox.enabled = true;
        }

        Debug.Log("Camera movement init...");
        // On réinitialise la caméra sur la nouvelle salle
        _cameraMovement.Init();
        _cameraMovement._freeMove = false;

        yield return null;
    }
}
