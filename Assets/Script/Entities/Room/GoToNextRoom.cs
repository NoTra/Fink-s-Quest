using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GoToNextRoom : MonoBehaviour
{
    public GameObject _room;
    public GameObject _nextRoom;
    public GameObject _mirrorTrigger;
    public GameObject _outPoint;
    private GameObject _player;
    private PlayerController _playerController;
    private CameraMovement _cameraMovement;

    [SerializeField] AnimationCurve _transitionCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);


    // Start is called before the first frame update
    void Start()
    {
        _cameraMovement = Camera.main.GetComponent<CameraMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // On trigger enter
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Récupération du parent du joueur
            _player = other.gameObject.transform.parent.gameObject;
            _playerController = _player.GetComponent<PlayerController>();

            // On Lerp la caméra vers la prochaine salle, on monte de la hauteur de la salle actuelle
            StartCoroutine(PerformGoToNextRoom());
        }
    }

    private IEnumerator PerformGoToNextRoom()
    {
        _cameraMovement._freeMove = true;

        // On désactive le mouvement du joueur
        _playerController._canMove = false;
        _playerController._canGrab = false;
        _playerController._playerRigidbody.velocity = Vector3.zero;
        _playerController._playerAnimator.SetBool("isRunning", false);

        float elapsedTime = 0;
        float duration = 1f;

        // On active la prochaine salle
        // _nextRoom.SetActive(true);
        DialogBox dialogBox = _nextRoom.GetComponent<DialogBox>();

        if (dialogBox != null)
        {
            Debug.Log("DialogBox of " + _nextRoom.name + " enabled !");
            dialogBox.enabled = true;
        }

        Vector3 playerStartPosition = _playerController._playerRigidbody.transform.position;
        Vector3 playerEndPosition = new (
            _outPoint.transform.position.x,
            _playerController._playerRigidbody.transform.position.y, 
            _outPoint.transform.position.z
        );

        // On définit la position de départ et d'arrivée de la caméra pour la transition
        Vector3 cameraStartPosition = Camera.main.transform.position;
        Vector3 cameraEndPosition = _cameraMovement.GetConstrainedCameraPosition(playerEndPosition, _nextRoom.GetComponent<Room>());

        Vector3 direction = playerEndPosition - playerStartPosition;

        Vector3 playerStartRotation = _playerController._playerRigidbody.transform.forward;

        // On désactive les trigger durant la transition
        _mirrorTrigger.SetActive(false);

        // On désactive le collider du joueur durant la transition
        _playerController._playerRigidbody.GetComponent<Collider>().enabled = false;
        _playerController._playerAnimator.SetBool("isRunning", true);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // On Lerp la caméra vers la prochaine salle
            Camera.main.transform.position = Vector3.Lerp(cameraStartPosition, cameraEndPosition, elapsedTime / duration);

            // On fait marcher le joueur vers le trigger mirroir de la prochaine salle
            _playerController._playerRigidbody.transform.position = Vector3.Lerp(playerStartPosition, playerEndPosition, _transitionCurve.Evaluate(elapsedTime / duration));

            // On rotate le joueur vers la direction
            _playerController._playerRigidbody.transform.forward = Vector3.Lerp(playerStartRotation, direction, _transitionCurve.Evaluate(elapsedTime / duration));

            yield return null;
        }

        // On réactive les triggers quand la transition est terminée
        _mirrorTrigger.SetActive(true);
        _playerController._playerRigidbody.GetComponent<Collider>().enabled = true;
        _playerController._canMove = true;
        _playerController._canGrab = true;
        _playerController._playerAnimator.SetBool("isRunning", false);

        // On désactive la salle actuelle
        // _room.SetActive(false);

        // On change de room
        GameManager.Instance._currentRoom = _nextRoom;

        // On réinitialise la caméra
        _cameraMovement.Init();
        _cameraMovement._freeMove = false;

        yield return null;
    }
}
