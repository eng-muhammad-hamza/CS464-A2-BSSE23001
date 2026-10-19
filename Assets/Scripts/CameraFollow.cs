using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new Vector3(0f, 5f, 7f);
    [SerializeField] float sharpness = 5f;
    [SerializeField] float lookAhead = 0.35f;
    [SerializeField] float mouseSensitivity = 3f;
    float yaw;

    Rigidbody targetRb;
    Vector3 lookAheadOffset;

    void Start() => FindTarget();

    void FindTarget()
    {
        if (target == null)
        {
            var p = FindAnyObjectByType<PlayerController>();
            if (p != null) target = p.transform;
        }
        if (target != null)
        {
            targetRb = target.GetComponent<Rigidbody>();
            transform.position = target.position + offset;
            transform.LookAt(target.position);
        }
    }

    void LateUpdate()
    {
        if (Input.GetMouseButton(1)) yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        if (target == null) { FindTarget(); return; }

        float blend = 1f - Mathf.Exp(-sharpness * Time.deltaTime);

        Vector3 flatVel = Vector3.zero;
        if (targetRb != null)
        {
            flatVel = targetRb.linearVelocity;
            flatVel.y = 0f;
        }
        lookAheadOffset = Vector3.Lerp(lookAheadOffset, flatVel * lookAhead, blend);

        Vector3 aim = target.position + lookAheadOffset;
        Vector3 wanted = aim + Quaternion.Euler(0f, yaw, 0f) * offset;

        transform.position = Vector3.Lerp(transform.position, wanted, blend);
        transform.LookAt(aim);
    }
}
