using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrab : PlayerSystem
{
    private GameObject _grabbedGOParent;
    private GameObject _grabbedGO;
    [SerializeField] private GameObject _grabbedSlot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_grabbedGO != null && Player._canMove)
        {
            _grabbedGO.transform.localPosition = Vector3.zero;
        }
    }

    public void OnGrab(InputAction.CallbackContext context)
    {
        if (!Player._canGrab || !context.performed)
        {
            return;
        }

        // Vector3 offsetBetweenGrabSlotAndPlayer = _grabbedSlot.transform.position - transform.position;

        Vector3 origin = new Vector3(Player.GetRigidbody().transform.position.x, Player.GetRigidbody().transform.position.y, Player.GetRigidbody().transform.position.z);

        Debug.Log(origin);

        Debug.DrawRay(origin, Player.GetRigidbody().transform.forward * 0.5f, Color.blue, 1f);
        // On draw un raycast pour voir si on touche un Grabbable
        RaycastHit hit;
        if (Physics.Raycast(origin, transform.forward, out hit, 0.5f, LayerMask.GetMask("Grabbable")))
        {
            Debug.Log("OnGrab hit " + hit.transform.gameObject.name);
            Player._isGrabbing = true;
            Player._canMove = false;
            Player.GetAnimator().SetBool("isPushing", true);

            // On store l'objet et son parent
            _grabbedGO = hit.transform.gameObject;
            _grabbedGOParent = _grabbedGO.transform.parent.gameObject;

            Vector3 targetPosition = _grabbedGO.transform.position + (hit.normal / 2f);
            targetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);

            StartCoroutine(MoveTo(targetPosition, 0.2f, -hit.normal));
        } else
        {
            Debug.Log("OnGrab hit nothing");
        }
    }

    public void OnUngrab(InputAction.CallbackContext context)
    {
        if (_grabbedGO == null || !context.performed)
        {
            return;
        }

        Player.GetAnimator().SetBool("isPushing", false);

        Debug.Log("OnUngrab");

        // On récupère les infos du collider de l'objet
        BoxCollider grabbedBoxCollider = _grabbedGO.GetComponent<BoxCollider>();
        // On réactive le collider du grabbedGO
        grabbedBoxCollider.enabled = true;

        Rigidbody grabbedRigibdoy = _grabbedGO.GetComponent<Rigidbody>();
        grabbedRigibdoy.isKinematic = false;
        grabbedRigibdoy.interpolation = RigidbodyInterpolation.Interpolate;
        grabbedRigibdoy.collisionDetectionMode = CollisionDetectionMode.Continuous;

        Player._isGrabbing = false;

        // On remet le grabbedGO dans le monde
        _grabbedGO.transform.parent = _grabbedGOParent.transform;

        // On remet le grabbedGO à sa position initiale
        _grabbedGO.transform.position = _grabbedSlot.transform.position;
        _grabbedGO = null;
    }

    public IEnumerator MoveTo(Vector3 targetPosition, float duration, Vector3 objectPositionToLookAt)
    {
        Debug.Log("MoveTo");
        float elapsedTime = 0f;

        Vector3 from = transform.position;
        Vector3 to = targetPosition;

        while (elapsedTime < duration)
        {
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, objectPositionToLookAt, 1f, 0.0f); ;

            // On va vers l'objet et on regarde l'objet
            transform.SetPositionAndRotation(Vector3.Lerp(from, to, elapsedTime / duration), Quaternion.LookRotation(newDirection));

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

        Player._canMove = true;
    }
}
