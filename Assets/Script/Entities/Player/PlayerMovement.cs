using UnityEngine;

public class PlayerMovement : PlayerSystem
{
    [SerializeField] private float _speed = 2f;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
    }

    private void Update()
    {
        // On trace un raycast devant le joueur pour détecter les collisions
        RaycastHit hit;
        if (Physics.Raycast(Player.GetRigidbody().transform.position, Player.GetRigidbody().transform.forward, out hit, 0.2f, LayerMask.GetMask("Wall")))
        {
            Player.GetRigidbody().velocity = Vector3.zero;
        }

        // On récupère l'input du joueur
        Vector2 moveDirection = Player._playerInput.actions["Move"].ReadValue<Vector2>();

        // On passe à l'animator que la variable velocity = Player.GetRigidbody().velocity.magnitude
        Player.GetAnimator().SetBool("isRunning", (moveDirection != Vector2.zero));
    }

    private void FixedUpdate()
    {
        if (Player._canMove == false)
        {
            return;
        }

        // On récupère l'input du joueur
        Vector2 moveDirection = Player._playerInput.actions["Move"].ReadValue<Vector2>();

        Vector3 velocity = Player.GetRigidbody().velocity;

        velocity.x = moveDirection.x * _speed;
        velocity.z = moveDirection.y * _speed;
        Player.GetRigidbody().velocity = velocity;

        // Rotation du joueur en fonction de la direction du movement
        if (moveDirection != Vector2.zero && CanRotate())
        {
            float angle = Mathf.Atan2(moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
            Player.GetRigidbody().transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        // On déplace le joueur
        Player.GetRigidbody().velocity = new Vector3(moveDirection.x, 0, moveDirection.y) * _speed;
    }

    public bool CanRotate()
    {
        return !Player._isDashing && !Player._isGrabbing;
    }
}
