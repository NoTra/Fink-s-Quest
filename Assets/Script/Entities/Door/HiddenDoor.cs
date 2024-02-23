using System.Collections;
using UnityEngine;

using FinksQuest.Core;
using System.Runtime.InteropServices;

namespace FinksQuest.Entities.Door
{
    public class HiddenDoor : Behavior.Openable
    {
        float _transitionSpeed = 3f;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private Statue _statue;

        // Start is called before the first frame update
        void Start()
        {
            _previousOpenState = _isOpen;
        }

        // Update is called once per frame
        new void Update()
        {
            base.Update();

            if (_isOpen == _previousOpenState)
            {
                return;
            }

            if (_isOpen)
            {
                StartCoroutine(OpenDoor());
                _previousOpenState = _isOpen;
            }
        }

        public IEnumerator OpenDoor()
        {
            var currentRoom = GameManager.Instance._currentRoom.GetComponent<Room>();
            // On cherche tous les Thrower de la salle et on les désactive
            foreach (Behavior.Thrower thrower in GameManager.Instance._currentRoom.GetComponentsInChildren<Behavior.Thrower>())
            {
                thrower.isActivated = false;
                thrower.DestroyAllProjectiles();
            }

            currentRoom._isResolved = true;

            GameManager.Instance.Player.GetRigidbody().velocity = Vector3.zero;
            GameManager.Instance.Player._canGrab = false;
            GameManager.Instance.Player._canMove = false;
            GameManager.Instance.Player.GetAnimator().SetBool("isRunning", false);

            Camera.CameraMovement cameraMovement = UnityEngine.Camera.main.GetComponent<Camera.CameraMovement>();
            cameraMovement._freeMove = true;

            Vector3 currentPosition = UnityEngine.Camera.main.transform.position;

            // We move the camera to the door
            yield return StartCoroutine(cameraMovement.MoveCameraToLocation(transform.position));

            // We shake the camera
            StartCoroutine(cameraMovement.ShakeCamera(_transitionSpeed, 0.05f));

            // We open the door and play sound
            // Play sound
            AudioSource _audioSource = GetComponent<AudioSource>();
            _audioSource.PlayOneShot(GameManager.Instance._audioManager._hiddenOpenDoorSound);

            float elapsedTime = 0f;
            Vector3 start = transform.position;
            Vector3 end = transform.position + new Vector3(0, -1.5f, 0);

            while (elapsedTime < _transitionSpeed)
            {
                transform.position = Vector3.Lerp(start, end, (elapsedTime / _transitionSpeed));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Wait for sound to finish
            yield return new WaitForSeconds(2f);

            // Play sound
            _audioSource.PlayOneShot(AudioManager.Instance._secretFoundSound);

            Debug.Log("Statue name : " + _statue.name);
            // On déplace la caméra à la statue
            yield return StartCoroutine(cameraMovement.MoveCameraToLocation(_statue.transform.position, true));

            // On allume la lumière de la statue
            var lightIntensity = _statue._light.intensity;
            _statue._light.intensity = 0f;

            _statue._light.gameObject.SetActive(true);

            // Play sound of light on
            _audioSource.PlayOneShot(AudioManager.Instance._lightOnSound);

            float elapsedTimeStatue = 0f;
            float durationStatue = 1f;
            while (elapsedTimeStatue < durationStatue)
            {
                _statue._light.intensity = Mathf.Lerp(0f, lightIntensity, elapsedTimeStatue / durationStatue);
                elapsedTimeStatue += Time.deltaTime;
                yield return null;
            }

            // Wait 1f
            yield return new WaitForSeconds(1f);

            // Wait for sound to finish
            // yield return new WaitForSeconds(AudioManager.Instance._secretFoundSound.length - 1f);

            // We go back to previous position
            yield return StartCoroutine(cameraMovement.MoveCameraToLocation(currentPosition));

            GameManager.Instance.Player._canGrab = true;
            GameManager.Instance.Player._canMove = true;
            cameraMovement._freeMove = false;
        }
    }
}