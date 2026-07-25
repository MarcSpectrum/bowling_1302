using UnityEngine;
using UnityEngine.InputSystem;

public class Bowling : MonoBehaviour
{
    [SerializeField] private float forcePower = 10f;
    [SerializeField] private float moveSpeed = 0.5f;

    private Rigidbody rb;
    private bool hasShot;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && !hasShot) ShootBall(); moveInput = 0f;
        if (Keyboard.current.rightArrowKey.isPressed) moveInput = 1f;
        else if (Keyboard.current.leftArrowKey.isPressed) moveInput = -1f;
    }

    void FixedUpdate()
    {
        if (!hasShot && moveInput != 0f)
        {
            rb.MovePosition(rb.position + Vector3.right * moveInput * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void ShootBall()
    {
        rb.AddForce(Vector3.forward * forcePower, ForceMode.Impulse);
        hasShot = true;
    }
}