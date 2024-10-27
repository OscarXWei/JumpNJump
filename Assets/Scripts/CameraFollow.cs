using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float height = 5.0f;
    public float distance = 8.5f;
    public float rotationSpeed = 3f;
    public float smoothSpeed = 0.1f;
    public float resetSpeed = 10f;

    // 新添加的变量
    public float introAnimationDuration = 3f;
    public float startHeight = 1000f; // 起始高度
    public float startDistance = 1000f; // 起始距离
    private bool isPlayingIntro = true;
    private float introStartTime;

    private float currentYaw;
    private float currentPitch;
    private bool isAdjustingView = false;
    private float targetYaw;
    private float targetPitch;
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 initialTargetPosition;
    private Vector3 startPosition;
    private bool shouldPlayIntro = true;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Camera target is not set!");
            return;
        }

        InitializeCamera();
        currentYaw = targetYaw;
        currentPitch = targetPitch;
    }

    void InitializeCamera()
    {
        if (shouldPlayIntro)
        {
            // 计算开始位置(在目标上方远处)
            startPosition = target.position + Vector3.up * startHeight + Vector3.back * startDistance;
            transform.position = startPosition;

            // 计算最终目标位置
            Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
            Vector3 negDistance = rotation * Vector3.forward * -distance;
            initialTargetPosition = target.position + Vector3.up * height + negDistance;

            // 让相机朝向目标
            transform.LookAt(target.position);

            // 开始动画
            introStartTime = Time.time;
            isPlayingIntro = true;
            shouldPlayIntro = false; // 防止重复播放
        }
    }

    // 外部调用的方法，用于触发新的运镜效果
    public void PlayIntroAnimation()
    {
        shouldPlayIntro = true;
        InitializeCamera();
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (isPlayingIntro)
        {
            UpdateIntroAnimation();
        }
        else
        {
            HandleInput();
            UpdateCameraRotation();
            UpdateCameraPosition();
        }
    }

    void UpdateIntroAnimation()
    {
        float elapsedTime = Time.time - introStartTime;
        float progress = elapsedTime / introAnimationDuration;

        if (progress >= 1f)
        {
            // 动画结束
            isPlayingIntro = false;
            transform.position = initialTargetPosition;
            return;
        }

        // 使用 SmoothStep 使动画更流畅
        float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

        // 在起始位置和目标位置之间插值
        transform.position = Vector3.Lerp(startPosition, initialTargetPosition, smoothProgress);

        // 保持相机朝向目标
        Vector3 lookAtPoint = target.position + Vector3.up * height * 0.7f;
        transform.LookAt(lookAtPoint);
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isAdjustingView = true;
        }
        if (isAdjustingView)
        {
            targetYaw += Input.GetAxis("Mouse X") * rotationSpeed;
            targetPitch = Mathf.Clamp(targetPitch, -30f, 60f);
        }
    }

    void UpdateCameraRotation()
    {
        currentYaw = Mathf.Lerp(currentYaw, targetYaw, smoothSpeed);
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, smoothSpeed);
    }

    void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        Vector3 negDistance = rotation * Vector3.forward * -distance;
        Vector3 targetPosition = target.position + Vector3.up * height + negDistance;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref currentVelocity,
            smoothSpeed * 2f
        );

        Vector3 lookAtPoint = target.position + Vector3.up * height * 0.7f;
        Quaternion targetRotation = Quaternion.LookRotation(lookAtPoint - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * 1.5f);
    }
}
