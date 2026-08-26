using System.Collections.Generic;
using UnityEngine;

public sealed class PinDeckController : MonoBehaviour
{
    struct Pose { public Vector3 p; public Quaternion r; public Pose(Transform t) { p=t.position; r=t.rotation; } }
    readonly Dictionary<Rigidbody, Pose> poses = new();
    void Awake() { foreach (var pin in GetComponentsInChildren<Rigidbody>()) poses[pin] = new Pose(pin.transform); }
    public int CountFallenPins() { int n=0; foreach(var pair in poses) if (pair.Key.gameObject.activeSelf && IsFallen(pair.Key, pair.Value)) n++; return n; }
    public void PrepareSecondRoll() { foreach(var pair in poses) if (pair.Key.gameObject.activeSelf && IsFallen(pair.Key,pair.Value)) pair.Key.gameObject.SetActive(false); else Freeze(pair.Key); }
    public void Rerack() { foreach(var pair in poses) { var rb=pair.Key; rb.gameObject.SetActive(true); rb.transform.SetPositionAndRotation(pair.Value.p,pair.Value.r); Freeze(rb); } }
    static bool IsFallen(Rigidbody rb, Pose start) => Vector3.Dot(rb.transform.up, Vector3.up) < .72f || rb.position.y < start.p.y-.25f || Mathf.Abs(rb.position.x)>2f || Mathf.Abs(rb.position.z-start.p.z)>3f;
    static void Freeze(Rigidbody rb) { rb.linearVelocity=Vector3.zero; rb.angularVelocity=Vector3.zero; rb.Sleep(); }
}
