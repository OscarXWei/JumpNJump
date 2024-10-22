using UnityEngine;

public class CameraController : MonoBehaviour
{
    // 保持原有参数不变
    public Transform target;
    public float height = 2f;
    public float distance = 5f;
    public float rotationSpeed = 5f;
    public float smoothSpeed = 0.125f;
    public float defaultYaw = 0f;
    public float defaultPitch = 20f;
    public float resetSpeed = 10f;

    // 初始视角参数
    public float overviewHeight = 1000f;  // 初始高度
    public float overviewDistance = 1000f; // 初始距离
    public float overviewYaw = -45f;    // 初始水平旋转角度
    public float overviewPitch = 10f;   // 初始俯视角度
    public float transitionDuration = 2f;
    public float delayBeforeTransition = 2f;

    private float yaw;
    private float pitch;
    private Vector3 refVelocity = Vector3.zero;
    private bool isAdjustingView = false;
    private float targetYaw;
    private float targetPitch;

    // 转换状态
    private bool isTransitioning = false;
    private float transitionStartTime;
    private Vector3 initialPosition;
    private bool hasStartedTransition = false;
    private float initialYaw;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Camera target is not set!");
            return;
        }

        // 设置初始视角
        yaw = overviewYaw;
        pitch = overviewPitch;
        initialYaw = overviewYaw;

        // 计算并设置初始位置
        Vector3 overviewPosition = target.position -
            (Quaternion.Euler(overviewPitch, overviewYaw, 0f) * Vector3.forward * overviewDistance) +
            Vector3.up * overviewHeight;

        transform.position = overviewPosition;
        transform.LookAt(target.position);

        // 保存初始状态
        initialPosition = transform.position;

        // 延迟开始转换
        Invoke("StartTransition", delayBeforeTransition);
    }

    void StartTransition()
    {
        isTransitioning = true;
        hasStartedTransition = true;
        transitionStartTime = Time.time;
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (isTransitioning)
        {
            HandleTransition();
        }
        else if (hasStartedTransition)
        {
            HandleNormalCamera();
        }
    }

    void HandleTransition()
    {
        float elapsedTime = Time.time - transitionStartTime;
        float t = elapsedTime / transitionDuration;

        if (t >= 1f)
        {
            isTransitioning = false;
            pitch = defaultPitch;
            yaw = defaultYaw;
            ResetToDefaultView();
            return;
        }

        // 使用平滑的插值曲线
        t = Mathf.SmoothStep(0, 1, t);

        // 计算过渡中的参数
        float currentHeight = Mathf.Lerp(overviewHeight, height, t);
        float currentPitch = Mathf.Lerp(overviewPitch, defaultPitch, t);
        float currentYaw = Mathf.Lerp(initialYaw, defaultYaw, t);
        float currentDistance = Mathf.Lerp(overviewDistance, distance, t);

        // 计算并设置相机位置
        Vector3 targetPosition = target.position -
            (Quaternion.Euler(currentPitch, currentYaw, 0f) * Vector3.forward * currentDistance) +
            Vector3.up * currentHeight;

        transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
        transform.LookAt(target.position + Vector3.up * height * 0.5f);
    }

    void HandleNormalCamera()
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

        yaw = Mathf.Lerp(yaw, targetYaw, smoothSpeed);
        pitch = Mathf.Lerp(pitch, targetPitch, smoothSpeed);

        Vector3 targetPosition = target.position -
            (Quaternion.Euler(pitch, yaw, 0f) * Vector3.forward * distance) +
            Vector3.up * height;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref refVelocity, smoothSpeed);
        transform.LookAt(target.position + Vector3.up * height * 0.5f);
    }

    void ResetToDefaultView()
    {
        targetYaw = defaultYaw;
        targetPitch = defaultPitch;
    }
}
