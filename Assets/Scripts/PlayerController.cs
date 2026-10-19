using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpHeight = 1.8f;
    [SerializeField] float coyoteTime = 0.1f;
    [SerializeField] float jumpBuffer = 0.12f;
    [SerializeField] float fallMultiplier = 2.2f;
    [SerializeField] Transform feet;
    [SerializeField] float checkRadius = 0.3f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] Transform body;
    [SerializeField] float squashY = 0.7f;
    [SerializeField] float squashDuration = 0.25f;
    [SerializeField] float minAirTime = 0.1f;

    const float NoTime = -100f;

    Rigidbody rb;
    Transform cam;
    Vector3 move;
    bool isGrounded, jumpQueued;
    float lastGroundedTime = NoTime, lastPressedTime = NoTime;
    float airTime;
    float away;
    Coroutine squashRoutine;

    Vector3 FeetPos => feet != null ? feet.position : transform.position + Vector3.down * 0.95f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        var col = GetComponent<CapsuleCollider>();
        col.material = new PhysicsMaterial("PlayerNoFriction")
        {
            dynamicFriction = 0f,
            staticFriction = 0f,
            frictionCombine = PhysicsMaterialCombine.Minimum
        };

        if (Camera.main != null) cam = Camera.main.transform;
        if (body == null && transform.childCount > 0) body = transform.GetChild(0);
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 fwd = Vector3.forward, right = Vector3.right;
        if (cam != null)
        {
            fwd = cam.forward; fwd.y = 0f; fwd.Normalize();
            right = cam.right; right.y = 0f; right.Normalize();
        }
        move = Vector3.ClampMagnitude(fwd * v + right * h, 1f);
        away = Vector3.Dot(move, fwd);
        isGrounded = Physics.CheckSphere(FeetPos, checkRadius, groundMask,
                                         QueryTriggerInteraction.Ignore);
        if (isGrounded && !jumpQueued && rb.linearVelocity.y <= 0f)
            lastGroundedTime = Time.time;
        if (Input.GetKeyDown(KeyCode.Space))
            lastPressedTime = Time.time;

        bool canJump = Time.time - lastGroundedTime <= coyoteTime;
        bool wantJump = Time.time - lastPressedTime <= jumpBuffer;
        if (canJump && wantJump && !jumpQueued)
        {
            jumpQueued = true;
            lastGroundedTime = NoTime;
            lastPressedTime = NoTime;
        }
        if (isGrounded)
        {
            if (airTime > minAirTime) PlaySquash();
            airTime = 0f;
        }
        else airTime += Time.deltaTime;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(move.x * speed, rb.linearVelocity.y, move.z * speed);
        if (jumpQueued)
        {
            jumpQueued = false;
            Vector3 vel = rb.linearVelocity;
            vel.y = 0f;
            rb.linearVelocity = vel;

            float g = -Physics.gravity.y;
            float v0 = Mathf.Sqrt(2f * g * jumpHeight);
            rb.AddForce(Vector3.up * rb.mass * v0, ForceMode.Impulse);
        }
        else if (rb.linearVelocity.y < 0f)
        {
            rb.AddForce(Physics.gravity * (fallMultiplier - 1f), ForceMode.Acceleration);
        }
    }

    void PlaySquash()
    {
        if (body == null) return;
        if (squashRoutine != null) StopCoroutine(squashRoutine);
        squashRoutine = StartCoroutine(Squash());
    }

    IEnumerator Squash()
    {
        for (float elapsed = 0f; elapsed < squashDuration; elapsed += Time.deltaTime)
        {
            float p = Ease.OutBack(elapsed / squashDuration);
            float y = Mathf.LerpUnclamped(squashY, 1f, p);
            float xz = 1f / Mathf.Sqrt(y);
            body.localScale = new Vector3(xz, y, xz);
            yield return null;
        }
        body.localScale = Vector3.one;
        squashRoutine = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(FeetPos, checkRadius);
        Gizmos.color = away >= 0f ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, move * 2f);
    }
}
