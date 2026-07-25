using UnityEngine;
using UnityEngine.InputSystem;

public class Bowling : MonoBehaviour
{
    [SerializeField] private float forcePower = 10f;
    [SerializeField] private float moveSpeed = 3f;

    private Rigidbody rb;
    private bool hasShot;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Shoot the ball
        if (Keyboard.current.spaceKey.wasPressedThisFrame && !hasShot)
            ShootBall();

        // Only allow steering before the ball is launched
        moveInput = 0f;
        if (!hasShot)
        {
            if (Keyboard.current.rightArrowKey.isPressed)
                moveInput = 1f;
            else if (Keyboard.current.leftArrowKey.isPressed)
                moveInput = -1f;
        }
    }

    void FixedUpdate()
    {
        if (moveInput != 0f)
        {
            Vector3 movement = Vector3.right * moveInput * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);
        }
    }

    private void ShootBall()
    {
        rb.AddForce(Vector3.forward * forcePower, ForceMode.Impulse);
        hasShot = true;
    }
}