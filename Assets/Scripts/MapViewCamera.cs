using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float fastMoveSpeed = 20f;
    public float freeLookSensitivity = 3f;
    public float zoomSensitivity = 10f;
    public float fastZoomSensitivity = 50f;

    private bool looking = false;
    private Vector3 lastMousePosition;
    private float currentSpeed;
    private bool isTopDownView = true;
    private Vector3 topDownPosition;
    private Quaternion topDownRotation;

    void Start()
    {
        // 初始化俯视位置和旋转
        topDownPosition = new Vector3(0, 50, 0); // 假设地图中心在(0,0,0)，高度50
        topDownRotation = Quaternion.Euler(90, 0, 0);

        // 设置初始位置和旋转为俯视视角
        transform.position = topDownPosition;
        transform.rotation = topDownRotation;
    }

    void Update()
    {
        // 切换视角模式
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleTopDownView();
        }

        if (isTopDownView)
        {
            HandleTopDownView();
        }
        else
        {
            HandleFreeView();
        }
    }

    void HandleTopDownView()
    {
        // 在俯视模式下移动
        Vector3 moveDirection = GetMoveDirection();
        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? fastMoveSpeed : moveSpeed;
        transform.Translate(moveDirection * currentSpeed * Time.deltaTime, Space.World);
    }

    void HandleFreeView()
    {
        // 自由视角下的移动
        Vector3 moveDirection = GetMoveDirection();
        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? fastMoveSpeed : moveSpeed;
        transform.Translate(moveDirection * currentSpeed * Time.deltaTime);

        // 鼠标右键旋转视角
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;
            looking = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            looking = false;
        }

        if (looking)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            transform.Rotate(Vector3.up, mouseDelta.x * freeLookSensitivity);
            transform.Rotate(Vector3.left, mouseDelta.y * freeLookSensitivity);
            lastMousePosition = Input.mousePosition;
        }

        // 鼠标滚轮缩放
        float zoomAmount = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime;
        if (Mathf.Abs(zoomAmount) > 0)
        {
            float zoomFactor = Input.GetKey(KeyCode.LeftShift) ? fastZoomSensitivity : zoomSensitivity;
            transform.position += transform.forward * zoomAmount * zoomFactor;
        }
    }

    Vector3 GetMoveDirection()
    {
        Vector3 moveDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) moveDirection += isTopDownView ? Vector3.forward : transform.forward;
        if (Input.GetKey(KeyCode.S)) moveDirection += isTopDownView ? Vector3.back : -transform.forward;
        if (Input.GetKey(KeyCode.A)) moveDirection += isTopDownView ? Vector3.left : -transform.right;
        if (Input.GetKey(KeyCode.D)) moveDirection += isTopDownView ? Vector3.right : transform.right;
        if (Input.GetKey(KeyCode.E)) moveDirection += Vector3.up;
        if (Input.GetKey(KeyCode.Q)) moveDirection += Vector3.down;
        return moveDirection.normalized;
    }

    void ToggleTopDownView()
    {
        isTopDownView = !isTopDownView;
        if (isTopDownView)
        {
            // 切换到俯视图
            transform.position = topDownPosition;
            transform.rotation = topDownRotation;
        }
        else
        {
            // 切换到自由视角，保持当前位置，稍微调整角度
            transform.rotation = Quaternion.Euler(30, transform.eulerAngles.y, 0);
        }
    }
}