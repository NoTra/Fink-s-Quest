using System.Collections;
using UnityEngine;

using FinksQuest.Core;
using FinksQuest.UI;
using UnityEngine.UI;

namespace FinksQuest.Entities.Transition
{
    public class GoToNextRoom : MonoBehaviour
    {
        // Triggers
        [SerializeField] private Room _roomA;
        [SerializeField] private Trigger _triggerA;
        [SerializeField] private Transform _outPointA;

        [SerializeField] private Room _roomB;
        [SerializeField] private Trigger _triggerB;
        [SerializeField] private Transform _outPointB;

        // Variables utilisées pour réaliser la transition
        private GameObject _nextRoom;
        private GameObject _mirrorTrigger;
        private Transform _outPoint;

        // Player
        private Player _player;
        private Rigidbody _playerRigidbody;
        private Animator _playerAnimator;

        // Camera
        private Camera.CameraMovement _cameraMovement;
        private UnityEngine.Camera _mainCamera;

        [SerializeField] AnimationCurve _transitionCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

        [SerializeField] private Animator _transitionAnimator;

        private void Awake()
        {
            _mainCamera = UnityEngine.Camera.main;
            _cameraMovement = _mainCamera.GetComponent<Camera.CameraMovement>();

            _player = GameManager.Instance.Player;
            _playerRigidbody = _player.GetRigidbody();
            _playerAnimator = _player.GetAnimator();
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
            // On désactive le mouvement de la caméra
            _cameraMovement._freeMove = true;

            // On désactive le mouvement du joueur
            _player._canMove = false;
            _player._canGrab = false;
            _playerRigidbody.velocity = Vector3.zero;
            _playerAnimator.SetBool("isRunning", false);

            float elapsedTime = 0;
            float duration = 2f;

            Vector3 playerStartPosition = _playerRigidbody.transform.position;
            Vector3 playerEndPosition = new(
                _outPoint.transform.position.x,
                _playerRigidbody.transform.position.y,
                _outPoint.transform.position.z
            );

            var direction = playerEndPosition - playerStartPosition;

            var playerStartRotation = _playerRigidbody.transform.forward;

            // On désactive les trigger durant la transition
            _mirrorTrigger.SetActive(false);

            // On désactive le collider du joueur durant la transition
            _playerRigidbody.GetComponent<Collider>().enabled = false;
            _playerAnimator.SetBool("isRunning", true);

            // Début transition
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

            // On réactive les triggers quand la transition est terminée
            _mirrorTrigger.SetActive(true);
            _playerRigidbody.GetComponent<Collider>().enabled = true;
            _player._canMove = true;
            _player._canGrab = true;
            _playerAnimator.SetBool("isRunning", false);

            GameManager.Instance._currentRoom = _nextRoom;
            _nextRoom.GetComponent<Room>().OnLeaveRoom();

            // Activation du dialog si présent
            DialogBox dialogBox = _nextRoom.GetComponent<DialogBox>();

            if (dialogBox != null)
            {
                dialogBox.enabled = true;
            }

            // On réinitialise la caméra sur la nouvelle salle
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
}