using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // 玩家的 Transform
    public float height = 10f; // 相机高度
    public float distance = 5f; // 相机与玩家的水平距离
    public float angle = 45f; // 相机俯视角度
    public float smoothSpeed = 0.125f; // 相机跟随的平滑度
    public float trackingSpeed = 5f; // 跟踪点跟随玩家的速度

    private Transform trackingPoint;
    private Vector3 refVelocity = Vector3.zero;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("Camera target is not set!");
            return;
        }

        // 创建一个空物体作为跟踪点
        GameObject trackingObject = new GameObject("CameraTrackingPoint");
        trackingPoint = trackingObject.transform;
        trackingPoint.position = target.position;

        // 初始化相机位置
        Vector3 targetPosition = CalculateTargetPosition(trackingPoint.position);
        transform.position = targetPosition;
        transform.rotation = Quaternion.Euler(angle, 0, 0);
    }

    private void LateUpdate()
    {
        if (target == null || trackingPoint == null)
        {
            return;
        }

        // 平滑地移动跟踪点towards玩家位置
        trackingPoint.position = Vector3.Lerp(trackingPoint.position, target.position, trackingSpeed * Time.deltaTime);

        // 计算目标位置
        Vector3 targetPosition = CalculateTargetPosition(trackingPoint.position);

        // 平滑移动相机
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref refVelocity, smoothSpeed);

        // 保持固定的俯视角度
        transform.rotation = Quaternion.Euler(angle, 0, 0);
    }

    private Vector3 CalculateTargetPosition(Vector3 targetPos)
    {
        // 计算相机在水平面上的偏移
        Vector3 horizontalOffset = -Vector3.forward * distance;

        // 旋转偏移量，使其与玩家的朝向一致（如果需要的话）
        // horizontalOffset = Quaternion.Euler(0, target.eulerAngles.y, 0) * horizontalOffset;

        // 计算最终的目标位置
        return targetPos + horizontalOffset + Vector3.up * height;
    }

    // 公共方法，用于在需要时重置相机位置（例如玩家重生时）
    public void ResetCamera()
    {
        if (target != null && trackingPoint != null)
        {
            trackingPoint.position = target.position;
            Vector3 targetPosition = CalculateTargetPosition(trackingPoint.position);
            transform.position = targetPosition;
            transform.rotation = Quaternion.Euler(angle, 0, 0);
        }
    }
}