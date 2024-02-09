using UnityEngine;
using UnityEngine.AI;

using FinksQuest.Core;

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

        void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _player = GameManager.Instance.Player.GetRigidbody();
            _roomGO = GetComponentInParent<Room>().gameObject;
            _timeSinceLastStrike = Time.time;
            _startingPosition = transform.position;
            _room = GetComponentInParent<Room>();
            _room.AddSkeleton(this);
        }

        void Update()
        {
            // Skeleton va vers le joueur s'il est dans la même pièce que lui
            if (_roomGO == GameManager.Instance._currentRoom)// (distance < 3f)
            {
                _navMeshAgent.SetDestination(_player.transform.position);
                _animator.SetBool("isWalking", true);

                Debug.DrawRay(transform.position, transform.forward * 0.2f, Color.red);

                float elapsedTime = Time.time - _timeSinceLastStrike;

                // Chaque seconde on frappe
                if (elapsedTime > _timeBetweenStrikes)
                {
                    _animator.SetTrigger("Strike");
                }
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