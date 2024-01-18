using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Camera _camera;

    private float _cameraY;

    private Bounds _roomBounds;

    private Vector2 _topLeftCoordinate;
    private Vector2 _topRightCoordinate;
    private Vector2 _bottomLeftCoordinate;
    private Vector2 _bottomRightCoordinate;

    private float _XRightDiff;
    private float _ZBottomDiff;
    private float _XLeftDiff;
    private float _ZTopDiff;

    public float _cameraMinX;
    public float _cameraMaxX;
    public float _cameraMinZ;
    public float _cameraMaxZ;

    private float _cameraZOffset = 0f;

    [SerializeField] private float _cameraZOffsetForward;
    [SerializeField] private float _cameraZOffsetIdle;
    [SerializeField] private float _cameraZOffsetBackward;

    private float _lastZVelocity = 0;
    private float _lastVelocityChange = 0;
    private float _direction = 0;
    [SerializeField] private PlayerController _playerController;
    private bool _cameraMoving = false;

    public bool _freeMove = false;

    [SerializeField] private AnimationCurve _transitionCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
        _roomBounds = GetMaxBounds(GameManager.Instance._currentRoom);

        Init();
    }

    private void CalculateCoordinatesDeviations()
    {
        Debug.Log("Max zoom camera : " + _cameraY);

        // On setup la camera sur le joueur
        transform.position = new Vector3(
            _playerController._playerRigidbody.transform.position.x,
            _cameraY,
            _playerController._playerRigidbody.transform.position.z
        );

        Debug.Log("On tire les raycasts");
        // On trace un Raycast depuis le centre de la camera vers le haut gauche de l'écran en World Space et on récupère le point d'impact sur le Layer Ground
        RaycastHit hit;
        Ray ray = _camera.ScreenPointToRay(new Vector3(0, Screen.height, 0));
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.yellow, 20f);
            _topLeftCoordinate = new Vector2(hit.point.x, hit.point.z);
        }

        // On trace un Raycast depuis le centre de la camera vers le bas gauche de l'écran en World Space et on récupère le point d'impact sur le Layer Ground
        ray = _camera.ScreenPointToRay(new Vector3(0, 0, 0));
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.blue, 20f);
            _bottomLeftCoordinate = new Vector2(hit.point.x, hit.point.z);
        }

        // On trace un Raycast depuis le centre de la camera vers le haut droit de l'écran en World Space et on récupère le point d'impact sur le Layer Ground
        ray = _camera.ScreenPointToRay(new Vector3(Screen.width, Screen.height, 0));
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, 20f);
            _topRightCoordinate = new Vector2(hit.point.x, hit.point.z);
        }

        // On trace un Raycast depuis le centre de la camera vers le bas droit de l'écran en World Space et on récupère le point d'impact sur le Layer Ground
        ray = _camera.ScreenPointToRay(new Vector3(Screen.width, 0, 0));
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green, 20f);
            _bottomRightCoordinate = new Vector2(hit.point.x, hit.point.z);
        }

        Debug.Log("On définit les décalages de coordonnées par rapport au joueur : ");
        Debug.Log("TopLEFT : " + _topLeftCoordinate);
        Debug.Log("TopRIGHT : " + _topRightCoordinate);
        Debug.Log("BottomLEFT : " + _bottomLeftCoordinate);
        Debug.Log("BottomRIGHT : " + _bottomRightCoordinate);

        Debug.Log("Position de la camera : " + _camera.transform.position);

        _XRightDiff = _topRightCoordinate.x - _camera.transform.position.x;
        _ZBottomDiff = _camera.transform.position.z - _bottomRightCoordinate.y;
        _XLeftDiff = _camera.transform.position.x - _topLeftCoordinate.x;
        _ZTopDiff = _topLeftCoordinate.y - _camera.transform.position.z;

        float xRightMax = (_roomBounds.max.x - _XRightDiff);
        float xLeftMax = (_roomBounds.min.x + _XLeftDiff);
        _cameraMinX = xLeftMax;
        _cameraMaxX = xRightMax;

        float zTopMax = (_roomBounds.max.z - _ZTopDiff);
        float zBottomMax = (_roomBounds.min.z + _ZBottomDiff);
        _cameraMinZ = zBottomMax;
        _cameraMaxZ = zTopMax;
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

    public void Init()
    {
        Room currentRoom = GameManager.Instance._currentRoom.GetComponent<Room>();
        _cameraY = currentRoom._maxZoom;

        _cameraMinX = currentRoom._cameraMinX;
        _cameraMaxX = currentRoom._cameraMaxX;
        _cameraMinZ = currentRoom._cameraMinZ;
        _cameraMaxZ = currentRoom._cameraMaxZ;

        // CalculateCoordinatesDeviations();
    }

    private void Update()
    {
    }

    public Vector3 GetConstrainedCameraPosition(Vector3 positionWanted, Room room = null)
    {
        float cameraMinX = room != null ? room._cameraMinX : _cameraMinX;
        float cameraMaxX = room != null ? room._cameraMaxX : _cameraMaxX;
        float cameraMinZ = room != null ? room._cameraMinZ : _cameraMinZ;
        float cameraMaxZ = room != null ? room._cameraMaxZ : _cameraMaxZ;
        float cameraY = room != null ? room._maxZoom : _cameraY;

        float clampX = Mathf.Clamp(
            positionWanted.x,
            cameraMinX,
            cameraMaxX
        );

        float clampZ = Mathf.Clamp(
            positionWanted.z,
            cameraMinZ,
            cameraMaxZ
        );

        return new Vector3(
            clampX,
            cameraY,
            clampZ
        );
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (_freeMove)
        {
            return;
        }
        /*_direction = Mathf.Sign(_playerController._playerRigidbody.velocity.z);

        if (_direction != _lastZVelocity)
        {
            _lastZVelocity = _direction;
            _lastVelocityChange = Time.time;
        }

        if (Time.time - _lastVelocityChange > 1f && _lastVelocityChange != 0f)
        {
            StartCoroutine(ChangePlayerOffset());
            _lastVelocityChange = 0f;
            _lastZVelocity = _direction;
        }

        if (_cameraMoving)
        {
            return;
        }*/

        _camera.transform.position = GetConstrainedCameraPosition(_playerController._playerRigidbody.transform.position);
    }
    
    /*IEnumerator ChangePlayerOffset()
    {
        _cameraMoving = true;
        Debug.Log("ChangePlayerOffset called !");

        float currentOffset = _cameraZOffset;
        float targetOffset = 0;
        // Si le joueur va vers le haut, on déplace la position théorique du joueur vers l'arrière afin qu'il voit ce qu'il y a devant lui
        if (_direction > 0)
        {
            targetOffset += _cameraZOffsetForward;
        } else if (_direction < 0)
        {
            targetOffset += _cameraZOffsetBackward;
        } else
        {
            targetOffset += _cameraZOffsetIdle;
        }

        float elapsedTime = 0;
        float duration = 2f;

        Debug.Log("On lerp");
        while (elapsedTime < duration)
        {
            _cameraZOffset = Mathf.Lerp(currentOffset, targetOffset, (elapsedTime / duration));
            
            float z = _playerController._playerRigidbody.transform.position.z + _cameraZOffset;
            float maxZ = Mathf.Clamp(z, -26.88f, -1.51367f);
            _camera.transform.position = new Vector3(_camera.transform.position.x, _camera.transform.position.y, maxZ);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _lastVelocityChange = 0f;
        _cameraZOffset = targetOffset;
        _cameraMoving = false;
    }*/

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
