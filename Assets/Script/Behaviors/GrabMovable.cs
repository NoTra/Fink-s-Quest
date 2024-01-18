using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrabMovable : MonoBehaviour
{
    PlayerInput _playerInput;

    private InputAction _grabAction;

    private GameObject _grabbedGOParent;
    private GameObject _grabbedGO;
    [SerializeField] private GameObject _grabbedSlot;

    

    private PlayerController _playerController;

    private void Awake()
    {
        _playerInput = GameManager.Instance._playerInput;
        _playerController = gameObject.transform.parent.GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        _grabAction = _playerInput.actions["Grab"];
        // _grabAction.started += OnGrab;
        _grabAction.canceled += OnUngrab;
        _grabAction.Enable();
    }

    private void OnDisable()
    {
        // _grabAction.performed -= OnGrab;
        _grabAction.canceled -= OnUngrab;
        _grabAction.Disable();
    }

    void Start()
    {

    }

    void Update()
    {
        if (_grabbedGO != null && _playerController._canMove)
        {
            _grabbedGO.transform.localPosition = Vector3.zero;
        }
    }

    public void OnGrab(InputAction.CallbackContext context)
    {
        if (!_playerController._canGrab || !context.performed)
        {
            return;
        }

        Debug.Log("Ongrab");

        Vector3 offsetBetweenGrabSlotAndPlayer = _grabbedSlot.transform.position - transform.position;

        Vector3 origin = new Vector3 (transform.position.x, 0.1f, transform.position.z);

        // On draw un raycast pour voir si on touche un Grabbable
        RaycastHit hit;
        if (Physics.Raycast(origin, transform.forward, out hit, 0.5f, LayerMask.GetMask("Grabbable")))
        {
            _playerController._isGrabbing = true;
            _playerController._canMove = false;

            _grabbedGO = hit.transform.gameObject;

            _grabbedGOParent = _grabbedGO.transform.parent.gameObject;
            Vector3 targetPosition = _grabbedGO.transform.position + hit.normal / 2f;

            targetPosition = new Vector3(targetPosition.x, 0, targetPosition.z);

            StartCoroutine(MoveTo(targetPosition, 0.2f, -hit.normal));
        }
    }

    public void OnUngrab(InputAction.CallbackContext context)
    {
        if (_grabbedGO == null || !context.performed)
        {
            return;
        }

        Debug.Log("OnUngrab");

        // On récupère les infos du collider de l'objet
        BoxCollider grabbedBoxCollider = _grabbedGO.GetComponent<BoxCollider>();
        // On réactive le collider du grabbedGO
        grabbedBoxCollider.enabled = true;

        Rigidbody grabbedRigibdoy = _grabbedGO.GetComponent<Rigidbody>();
        grabbedRigibdoy.isKinematic = false;
        grabbedRigibdoy.interpolation = RigidbodyInterpolation.Interpolate;
        grabbedRigibdoy.collisionDetectionMode = CollisionDetectionMode.Continuous;

        _playerController._isGrabbing = false;

        // On remet le grabbedGO dans le monde
        _grabbedGO.transform.parent = _grabbedGOParent.transform;

        // On remet le grabbedGO à sa position initiale
        _grabbedGO.transform.position = _grabbedSlot.transform.position;
        _grabbedGO = null;
    }

    public IEnumerator MoveTo(Vector3 targetPosition, float duration, Vector3 objectPositionToLookAt)
    {
        float elapsedTime = 0f;

        Vector3 from = transform.position;
        Vector3 to = targetPosition;

        while (elapsedTime < duration)
        {
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, objectPositionToLookAt, 1f, 0.0f); ;
            // On regarde l'objet
            transform.rotation = Quaternion.LookRotation(newDirection);

            // On va vers la position donnée
            transform.position = Vector3.Lerp(from, to, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (_grabbedGO == null)
        {
            yield break;
        }

        // Set the initial local position within the new parent
        _grabbedGO.transform.localPosition = Vector3.zero;

        // Change parent of grabbed object to player
        _grabbedGO.transform.parent = _grabbedSlot.transform;

        _playerController._canMove = true;
    }
}
