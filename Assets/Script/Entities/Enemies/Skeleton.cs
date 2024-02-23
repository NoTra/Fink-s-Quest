using UnityEngine;
using UnityEngine.AI;

using FinksQuest.Core;
using FinksQuest.Behavior;

namespace FinksQuest.Entities.Enemies
{
    public class Skeleton : MonoBehaviour
    {
        private NavMeshAgent _navMeshAgent;
        [SerializeField] private Animator _animator;
        private GameObject _roomGO;

        private Rigidbody _player;
        [SerializeField] private float _timeBetweenStrikes = 1f;
        private float _timeSinceLastStrike = 0f;

        public Vector3 _startingPosition;
        private Room _room;

        private Striker _striker;

        void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _player = GameManager.Instance.Player.GetRigidbody();
            _roomGO = GetComponentInParent<Room>().gameObject;
            _timeSinceLastStrike = Time.time;
            _startingPosition = transform.position;
            _room = GetComponentInParent<Room>();
            _room.AddSkeleton(this);

            _striker = GetComponentInChildren<Striker>();
        }

        void Update()
        {
            // Skeleton va vers le joueur s'il est dans la même pièce que lui et qu'il est en vie
            if (
                _roomGO == GameManager.Instance._currentRoom && 
                GameManager.Instance.Player.GetRigidbody().GetComponent<Hittable>()._currentHP > 0
            )
            {
                // Déplacement vers le joueur
                if (Vector3.Distance(transform.position, _player.transform.position) > 1.2f)
                {
                    _navMeshAgent.SetDestination(_player.transform.position);
                    _animator.SetBool("isWalking", true);
                }
                else
                {
                    if (!_striker._isStriking)
                    {
                        _animator.SetTrigger("Strike");
                    }
                }

                transform.LookAt(new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z));
            }
            else
            {
                _navMeshAgent.SetDestination(transform.position);
                _animator.SetBool("isWalking", false);
            }

            // On bloque les la position y à 0 pour éviter que l'ennemi ne vole
            transform.position = new Vector3(transform.position.x, 0.289f, transform.position.z);
        }
    }
}