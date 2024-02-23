using UnityEngine;

namespace FinksQuest.PlayerSystems
{
    public class PlayerMovement : PlayerSystem
    {
        [SerializeField] private float _speed = 2f;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Update()
        {
            // On trace un raycast devant le joueur pour détecter les collisions
            RaycastHit hit;
            if (Physics.Raycast(_player.GetRigidbody().transform.position, _player.GetRigidbody().transform.forward, out hit, 0.2f, LayerMask.GetMask("Wall")))
            {
                _player.GetRigidbody().velocity = Vector3.zero;
            }

            if (_player._canMove == false)
            {
                return;
            }

            // On récupère l'input du joueur
            Vector2 moveDirection = _player._playerInput.actions["Move"].ReadValue<Vector2>();

            // On passe à l'animator que la variable velocity = Player.GetRigidbody().velocity.magnitude
            _player.GetAnimator().SetBool("isRunning", (moveDirection != Vector2.zero));
        }

        private void FixedUpdate()
        {
            if (_player._canMove == false)
            {
                return;
            }

            // On récupère l'input du joueur
            Vector2 moveDirection = _player._playerInput.actions["Move"].ReadValue<Vector2>();

            Vector3 velocity = _player.GetRigidbody().velocity;

            velocity.x = moveDirection.x * _speed;
            velocity.z = moveDirection.y * _speed;
            _player.GetRigidbody().velocity = velocity;

            // Rotation du joueur en fonction de la direction du movement
            if (moveDirection != Vector2.zero && CanRotate())
            {
                float angle = Mathf.Atan2(moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
                _player.GetRigidbody().transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }

            // On déplace le joueur
            _player.GetRigidbody().velocity = new Vector3(moveDirection.x, 0, moveDirection.y) * _speed;
        }

        public bool CanRotate()
        {
            return !_player._isDashing && !_player._isGrabbing;
        }
    }
}