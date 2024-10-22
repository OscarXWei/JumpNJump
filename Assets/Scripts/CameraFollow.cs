using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // 玩家的 Transform
    public float height = 5.0f; // 相机高度
    public float distance = 8.5f; // 相机与玩家的水平距离
    public float rotationSpeed = 5f; // 鼠标旋转速度
    public float smoothSpeed = 0.125f; // 相机跟随的平滑度
    public float defaultYaw = 0f; // 默认水平角度
    public float defaultPitch = 20f; // 默认垂直角度
    public float resetSpeed = 10f; // 视角复位速度

    private float yaw;
    private float pitch;
    private Vector3 refVelocity = Vector3.zero;
    private bool isAdjustingView = false;
    private float targetYaw;
    private float targetPitch;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Camera target is not set!");
            return;
        }

        // 初始化视角
        ResetToDefaultView();
    }

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        // 检查鼠标左键的状态
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
            // 鼠标输入来控制视角
            targetYaw += Input.GetAxis("Mouse X") * rotationSpeed;
            targetPitch -= Input.GetAxis("Mouse Y") * rotationSpeed;

            // 限制 pitch 角度
            targetPitch = Mathf.Clamp(targetPitch, -30f, 60f);
        }
        else
        {
            // 平滑地恢复到默认视角
            targetYaw = Mathf.MoveTowards(targetYaw, defaultYaw, resetSpeed * Time.deltaTime);
            targetPitch = Mathf.MoveTowards(targetPitch, defaultPitch, resetSpeed * Time.deltaTime);
        }

        // 平滑地更新实际的 yaw 和 pitch
        yaw = Mathf.Lerp(yaw, targetYaw, smoothSpeed);
        pitch = Mathf.Lerp(pitch, targetPitch, smoothSpeed);

        // 计算相机的位置
        Vector3 targetPosition = target.position - (Quaternion.Euler(pitch, yaw, 0f) * Vector3.forward * distance) + Vector3.up * height;

        // 平滑移动相机
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref refVelocity, smoothSpeed);

        // 使相机始终看向玩家
        transform.LookAt(target.position + Vector3.up * height * 0.5f);
    }

    void ResetToDefaultView()
    {
        targetYaw = defaultYaw;
        targetPitch = defaultPitch;
    }
}
