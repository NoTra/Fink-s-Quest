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

    private Rigidbody _player;

    // Start is called before the first frame update
    void Start()
    {
        _strikeBehavior = GetComponent<Striker>();
        _hittable = GetComponent<Hittable>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        _player = GameManager.Instance._playerController._playerRigidbody;
    }

    // Update is called once per frame
    void Update()
    {
        // Distance between player and skeleton
        float distance = Vector3.Distance(transform.position, _player.transform.position);

        // Skeleton va vers le joueur s'il est à une certain distance de lui
        if (distance < 3f)
        {
            _navMeshAgent.SetDestination(_player.transform.position);
            Debug.Log("Set destination to player + isWalking = true");
            _animator.SetBool("isWalking", true);
        } else
        {
            _navMeshAgent.SetDestination(transform.position);
            _animator.SetBool("isWalking", false);
        }

        Debug.DrawRay(transform.position, transform.forward * 0.2f, Color.red);
        // Si le joueur est à distance de frappe et que la cible est la target
        if (Vector3.Distance(transform.position, _player.transform.position) < _strikeBehavior._strikeRange)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, _strikeBehavior._strikeRange, ~_strikeBehavior._layerToExclude))
            {
                if (hit.collider.CompareTag(_strikeBehavior._targetTag))
                {
                    _strikeBehavior.Strike();
                }
            }
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
