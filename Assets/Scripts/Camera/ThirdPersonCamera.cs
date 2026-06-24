using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float height;

    private Quaternion targetRotation;

    private float yRotation;
    private float xRotation;
    private float xRotationClamped;

    [SerializeField] private float xRotationMin;
    [SerializeField] private float xRotationMax;

    [SerializeField] private float xSensitivity;
    [SerializeField] private float ySensitivity;

    [SerializeField] private bool invertX;
    private int xInvertedValue;

    private Vector3 desiredPos;

    [Header("Camera Collision")]
    [SerializeField] private float collisionRadius = 0.2f;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private float minDistance = 0.5f;

    [Tooltip("Vertical offset of the pivot from the target")]
    [SerializeField] private float pivotHeight = 1.5f;



    private void Start()
    {

        xInvertedValue = invertX ? -1 : 1;

        float sens = PlayerPrefs.HasKey("Sensitivity")
        ? PlayerPrefs.GetFloat("Sensitivity")
        : 3f;

        xSensitivity = sens;
        ySensitivity = sens;
    }

    private void Update()
    {
        yRotation += Input.GetAxis("Mouse X") * ySensitivity * Time.deltaTime;
        xRotation += Input.GetAxis("Mouse Y") * xSensitivity * xInvertedValue * Time.deltaTime;
    }

    void LateUpdate()
    {
        xRotationClamped = Mathf.Clamp(xRotation, xRotationMin, xRotationMax);
        targetRotation = Quaternion.Euler(xRotationClamped, yRotation, 0.0f);

        Vector3 pivotPos = target.position + Vector3.up * pivotHeight;

        desiredPos = pivotPos - targetRotation * offset + Vector3.up * height;

        Vector3 dir = desiredPos - pivotPos;
        float desiredDistance = dir.magnitude;

        if (desiredDistance > 0.01f && Physics.SphereCast(
            pivotPos,
            collisionRadius,
            dir.normalized,
            out RaycastHit hit,
            desiredDistance,
            collisionMask))
        {
            float safeDistance = Mathf.Max(hit.distance - collisionRadius, minDistance);
            desiredPos = pivotPos + dir.normalized * safeDistance;
        }

        transform.SetPositionAndRotation(desiredPos, targetRotation);
    }

    public Quaternion YRotation => Quaternion.Euler(0.0f, yRotation, 0.0f);

    public void SetSensitivity(float value)
    {
        xSensitivity = value;
        ySensitivity = value;
    }


}
