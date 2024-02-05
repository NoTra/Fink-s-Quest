using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skeleton : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private Rigidbody _rigidbody;
    private NavMeshAgent _navMeshAgent;
    private Striker _strikeBehavior;
    private Hittable _hittable;
    [SerializeField] private Animator _animator;
    private GameObject _skeletonRoom;

    private Rigidbody _player;
    [SerializeField] private float _timeBetweenStrikes = 1f;
    private float _timeSinceLastStrike = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _strikeBehavior = GetComponent<Striker>();
        _hittable = GetComponent<Hittable>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        _player = GameManager.Instance.Player.GetRigidbody();
        _skeletonRoom = GetComponentInParent<Room>().gameObject;

        _timeSinceLastStrike = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // Distance between player and skeleton
        float distance = Vector3.Distance(transform.position, _player.transform.position);

        // Skeleton va vers le joueur s'il est dans la même pièce que lui
        if (_skeletonRoom == GameManager.Instance._currentRoom)// (distance < 3f)
        {
            _navMeshAgent.SetDestination(_player.transform.position);
            _animator.SetBool("isWalking", true);

            Debug.DrawRay(transform.position, transform.forward * 0.2f, Color.red);

            float elapsedTime = Time.time - _timeSinceLastStrike;

            // Chaque seconde on frappe
            if (elapsedTime > _timeBetweenStrikes)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, _strikeBehavior._strikeRange, ~_strikeBehavior._layerToExclude))
                {
                    if (hit.collider.CompareTag(_strikeBehavior._targetTag))
                    {
                        _strikeBehavior.Strike();

                        _timeSinceLastStrike = Time.time;
                    }
                }
            }
        } else
        {
            _navMeshAgent.SetDestination(transform.position);
            _animator.SetBool("isWalking", false);
        }

        
        
        /*if (!_hittable._isHit)
        {
            
        }*/

        /*bool isMoving = _navMeshAgent.velocity.magnitude > 0.1f;
        if (isMoving)
        {
            
        } else
        {
            _animator.SetBool("isWalking", false);
        }*/

        // On bloque les la position y à 0 pour éviter que l'ennemi ne vole
        transform.position = new Vector3(transform.position.x, 0.289f, transform.position.z);
    }

    
}
