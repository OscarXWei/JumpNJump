using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // 玩家的 Transform
    public float height = 2f; // 相机高度
    public float distance = 5f; // 相机与玩家的水平距离
    public float rotationSpeed = 5f; // 鼠标旋转速度
    public float smoothSpeed = 0.125f; // 相机跟随的平滑度

    private float yaw = 0f; // 水平旋转角度
    private float pitch = 0f; // 垂直旋转角度
    private Vector3 refVelocity = Vector3.zero;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Camera target is not set!");
            return;
        }

        // 初始化视角（从玩家后面开始）
        yaw = target.eulerAngles.y;
        pitch = 20f; // 视角的初始仰角
    }

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        // 检查鼠标左键是否按下
        if (Input.GetMouseButton(0)) // 0 表示鼠标左键
        {
            // 鼠标输入来控制视角
            yaw += Input.GetAxis("Mouse X") * rotationSpeed;
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;

            // 限制 pitch 角度（防止相机过度俯仰）
            pitch = Mathf.Clamp(pitch, -30f, 60f);
        }

        // 计算相机的位置
        Vector3 targetPosition = target.position - (Quaternion.Euler(pitch, yaw, 0f) * Vector3.forward * distance) + Vector3.up * height;

        // 平滑移动相机
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref refVelocity, smoothSpeed);

        // 使相机始终看向玩家
        transform.LookAt(target.position + Vector3.up * height * 0.5f); // 调整相机看向玩家的中部
    }
}
