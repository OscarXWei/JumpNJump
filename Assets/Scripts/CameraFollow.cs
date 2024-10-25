using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float height = 5.0f;
    public float distance = 8.5f;
    public float rotationSpeed = 5f;
    public float smoothSpeed = 0.1f; // 降低平滑速度以减少抖动
    public float defaultYaw = 0f;
    public float defaultPitch = 20f;
    public float resetSpeed = 10f;

    private float currentYaw;
    private float currentPitch;
    private bool isAdjustingView = false;
    private float targetYaw;
    private float targetPitch;
    private Vector3 currentVelocity = Vector3.zero;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Camera target is not set!");
            return;
        }
        ResetToDefaultView();
        // 初始化当前角度为目标角度
        currentYaw = targetYaw;
        currentPitch = targetPitch;
    }

    void LateUpdate()
    {
        if (target == null) return;

        HandleInput();
        UpdateCameraRotation();
        UpdateCameraPosition();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isAdjustingView = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isAdjustingView = false;
            ResetToDefaultView();
        }

        if (isAdjustingView)
        {
            targetYaw += Input.GetAxis("Mouse X") * rotationSpeed;
            targetPitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
            targetPitch = Mathf.Clamp(targetPitch, -30f, 60f);
        }
        else
        {
            targetYaw = Mathf.MoveTowards(targetYaw, defaultYaw, resetSpeed * Time.deltaTime);
            targetPitch = Mathf.MoveTowards(targetPitch, defaultPitch, resetSpeed * Time.deltaTime);
        }
    }

    void UpdateCameraRotation()
    {
        // 平滑过渡角度，使用更小的平滑系数
        currentYaw = Mathf.Lerp(currentYaw, targetYaw, smoothSpeed);
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, smoothSpeed);
    }

    void UpdateCameraPosition()
    {
        // 计算目标位置
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        Vector3 negDistance = rotation * Vector3.forward * -distance;
        Vector3 targetPosition = target.position + Vector3.up * height + negDistance;

        // 使用 SmoothDamp 进行位置平滑，可以提供更平滑的移动
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref currentVelocity,
            smoothSpeed * 2f  // 位置平滑时间稍长一些
        );

        // 计算目标点，稍微提高注视点以获得更好的视角
        Vector3 lookAtPoint = target.position + Vector3.up * height * 0.7f;

        // 平滑旋转
        Quaternion targetRotation = Quaternion.LookRotation(lookAtPoint - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * 1.5f);
    }

    void ResetToDefaultView()
    {
        targetYaw = defaultYaw;
        targetPitch = defaultPitch;
    }
}
