using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Camera _camera;

    private float _cameraY;

    private Bounds _roomBounds;

    public float _cameraMinX;
    public float _cameraMaxX;
    public float _cameraMinZ;
    public float _cameraMaxZ;

    [SerializeField] private float _cameraZOffsetForward;
    [SerializeField] private float _cameraZOffsetIdle;
    [SerializeField] private float _cameraZOffsetBackward;

    [SerializeField] private Room _currentRoom;

    public bool _freeMove = false;

    [SerializeField] private AnimationCurve _transitionCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
        _currentRoom = GameManager.Instance._currentRoom.GetComponent<Room>();
        _roomBounds = GetMaxBounds(GameManager.Instance._currentRoom);

        Init();
    }

    public void Init()
    {
        Debug.Log("Init Camera Movement...");
        _currentRoom = GameManager.Instance._currentRoom.GetComponent<Room>();

        _roomBounds = GetMaxBounds(GameManager.Instance._currentRoom);

        SetDeviationsCoordinates();
    }

    private void SetDeviationsCoordinates()
    {
        Debug.Log("Calculating coordinates deviations...");
        Debug.Log("Max zoom camera : " + _cameraY);

        _cameraY = _currentRoom._maxZoom;

        // On setup la camera sur le joueur
        transform.position = new Vector3(
            GameManager.Instance.Player.GetRigidbody().transform.position.x,
            _cameraY,
            GameManager.Instance.Player.GetRigidbody().transform.position.z
        );

        var _topLeftCoordinate = new Vector2();
        var _topRightCoordinate = new Vector2();
        // var _bottomLeftCoordinate = new Vector2();
        // var _bottomRightCoordinate = new Vector2();

        Debug.Log("On tire les raycasts");
        // On trace un Raycast depuis le centre de la camera vers le haut gauche de l'écran en World Space et on récupère le point d'impact sur le Layer Ground
        RaycastHit hit;
        Ray ray = _camera.ScreenPointToRay(new Vector3(0, Screen.height, 0));
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            // Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.yellow, 20f);
            _topLeftCoordinate = new Vector2(hit.point.x, hit.point.z);
        }

        // On trace un Raycast depuis le centre de la camera vers le bas gauche de l'écran en World Space et on récupère le point d'impact sur le Layer Ground
        /*ray = _camera.ScreenPointToRay(new Vector3(0, 0, 0));
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            // Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.blue, 20f);
            _bottomLeftCoordinate = new Vector2(hit.point.x, hit.point.z);
        }*/

        // On trace un Raycast depuis le centre de la camera vers le haut droit de l'écran en World Space et on récupère le point d'impact sur le Layer Ground
        ray = _camera.ScreenPointToRay(new Vector3(Screen.width, Screen.height, 0));
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            // Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, 20f);
            _topRightCoordinate = new Vector2(hit.point.x, hit.point.z);
        }

        /*// On trace un Raycast depuis le centre de la camera vers le bas droit de l'écran en World Space et on récupère le point d'impact sur le Layer Ground
        ray = _camera.ScreenPointToRay(new Vector3(Screen.width, 0, 0));
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            // Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green, 20f);
            _bottomRightCoordinate = new Vector2(hit.point.x, hit.point.z);
        }*/

        _cameraMinX = _roomBounds.min.x + (_camera.transform.position.x - _topLeftCoordinate.x);
        _cameraMaxX = _roomBounds.max.x - (_topRightCoordinate.x - _camera.transform.position.x);
        // _cameraMinZ = _roomBounds.min.z + (_camera.transform.position.z - _bottomRightCoordinate.y);
        _cameraMinZ = _roomBounds.min.z + (_camera.transform.position.z - _topRightCoordinate.y);
        _cameraMaxZ = _roomBounds.max.z - (_topLeftCoordinate.y - _camera.transform.position.z);
    }

    Bounds GetMaxBounds(GameObject g)
    {
        var renderers = g.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(g.transform.position, Vector3.zero);
        var b = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            b.Encapsulate(r.bounds);
        }
        return b;
    }

    private void Update()
    {
    }

    public Vector3 GetConstrainedCameraPosition(Vector3 positionWanted)
    {
        float clampX = Mathf.Clamp(
            positionWanted.x,
            _cameraMinX - 1,
            _cameraMaxX + 1
        );

        float clampZ = Mathf.Clamp(
            positionWanted.z,
            _cameraMinZ - 0.5f,
            _cameraMaxZ + 0.5f 
        );

        return new Vector3(
            clampX,
            _cameraY,
            clampZ
        );
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (_freeMove)
        {
            //Debug.Log("Camera is free...");
            return;
        }

        /*_camera.transform.position = new Vector3(
            GameManager.Instance.Player.GetRigidbody().transform.position.x,
            5,
            GameManager.Instance.Player.GetRigidbody().transform.position.z - _cameraZOffset
        );*/

        // On clamp les x de la camera entre xLeftMax & xRightMax
        // On clamp les y de la camera entre yBottomMax & yTopMax
        // Debug.Log("On centre sur le player... " + GameManager.Instance.Player.GetRigidbody().transform.position);
        _camera.transform.position = GetConstrainedCameraPosition(GameManager.Instance.Player.GetRigidbody().transform.position);
    }

    public IEnumerator MoveCameraToLocation(Vector3 location)
    {
        float elapsedTime = 0f;
        float duration = 1f;

        Vector3 start = transform.position;
        Vector3 end = GetConstrainedCameraPosition(location);

        Debug.Log("Moving camera to location : " + end);

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(start, end, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator ShakeCamera(float duration, float magnitude)
    {
        Vector3 originalPos = transform.position;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = originalPos;
    }
}
