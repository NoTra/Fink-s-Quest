using FinksQuest.Entities.Camera;
using FinksQuest.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FinksQuest
{
    public class TriggerHub : MonoBehaviour
    {
        private bool _hasEntered = false;
        [SerializeField] private List<GameObject> _highlightObjects = new List<GameObject>();

        private CameraMovement _cameraMovement;
        private Player _player;

        private void Awake()
        {
            Camera camera = Camera.main;
            _cameraMovement = camera.GetComponent<CameraMovement>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && !_hasEntered)
            {
                Debug.Log("Trigger enter : " + other.gameObject.name);
                // Lancement du déplacement de la caméra
                _hasEntered = true;

                _cameraMovement._freeMove = true;
                // _player = other.gameObject.GetComponent<Player>();

                /*if (_player != null)
                {
                    _player = other.gameObject.GetComponentInParent<Player>();
                }*/

                _player = GameManager.Instance.Player;

                if (_player != null)
                {
                    _player._playerInput.enabled = false;
                } else
                {
                    Debug.Log("playerMovement not found");
                }

                // other.GetComponent<PlayerMovement>().enabled = false;

                StartCoroutine(ShowRoom(other.transform));
            }
        }

        private IEnumerator ShowRoom(Transform origin)
        {
            foreach (GameObject highlightObject in _highlightObjects)
            {
                StartCoroutine(_cameraMovement.MoveCameraToLocation(highlightObject.transform.position));

                yield return new WaitForSeconds(2);
            }

            // Return to player
            StartCoroutine(_cameraMovement.MoveCameraToLocation(origin.position));

            yield return new WaitForSeconds(2);

            _cameraMovement._freeMove = false;
            // _player._canMove = true;
            _player._playerInput.enabled = true;
        }
    }
}
