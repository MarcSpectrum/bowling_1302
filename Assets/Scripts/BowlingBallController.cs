using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public sealed class BowlingBallController : MonoBehaviour
{
    public event Action ChargeStarted;
    public event Action<float> Launched;
    [SerializeField] float moveSpeed = 3.5f, laneHalfWidth = 1.25f, aimSpeed = 45f, maxAim = 18f;
    [SerializeField] float minPower = 8f, maxPower = 22f, chargeSeconds = 1.5f;
    Rigidbody body; Vector3 initialPosition; Quaternion initialRotation; float aim, charge; bool controlsEnabled;
    public float Charge01 => charge; public float AimAngle => aim; public Rigidbody Body => body;

    void Awake() { body = GetComponent<Rigidbody>(); initialPosition = transform.position; initialRotation = transform.rotation; LockBody(); }
    void Update()
    {
        if (!controlsEnabled || Keyboard.current == null) return;
        var kb = Keyboard.current;
        float move = (kb.dKey.isPressed || kb.rightArrowKey.isPressed ? 1 : 0) - (kb.aKey.isPressed || kb.leftArrowKey.isPressed ? 1 : 0);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x + move * moveSpeed * Time.deltaTime, -laneHalfWidth, laneHalfWidth), transform.position.y, transform.position.z);
        aim = Mathf.Clamp(aim + ((kb.eKey.isPressed ? 1 : 0) - (kb.qKey.isPressed ? 1 : 0)) * aimSpeed * Time.deltaTime, -maxAim, maxAim);
        if (kb.spaceKey.wasPressedThisFrame) { charge = 0; controlsEnabled = false; ChargeStarted?.Invoke(); }
    }
    public void TickCharging()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current.spaceKey.isPressed) charge = Mathf.Clamp01(charge + Time.unscaledDeltaTime / chargeSeconds);
        if (Keyboard.current.spaceKey.wasReleasedThisFrame) Launch();
    }
    void Launch()
    {
        float power = Mathf.Lerp(minPower, maxPower, charge);
        body.isKinematic = false; body.linearVelocity = Quaternion.Euler(0, aim, 0) * Vector3.forward * power;
        Launched?.Invoke(charge);
    }
    public void SetControlsEnabled(bool value) { controlsEnabled = value; }
    public void ResetBall() { LockBody(); transform.SetPositionAndRotation(initialPosition, initialRotation); aim = 0; charge = 0; }
    void LockBody() { body = body != null ? body : GetComponent<Rigidbody>(); body.linearVelocity = Vector3.zero; body.angularVelocity = Vector3.zero; body.isKinematic = true; }
}
