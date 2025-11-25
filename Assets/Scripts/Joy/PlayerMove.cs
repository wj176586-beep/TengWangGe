using UnityEngine;
using Cinemachine;

public class PlayerMove : MonoBehaviour
{
    [Header("Player Settings")]
    public GameObject player;
    public Rigidbody playerRB;
    public float moveSpeed = 5f;

    [Header("Input")]
    public Joystick joystick; // 左摇杆
    public float lookSpeed = 0.2f; // 右滑灵敏度

    [Header("Animation")]
    public Animator animator;

    [Header("Camera")]
    public CinemachineVirtualCamera virtualCam;
    public Transform cameraPivot; // 用于旋转摄像机的空物体

    private Vector3 moveDir;

    private float yaw = 0f;
    private float pitch = 10f;

    private Quaternion defaultPivotRotation; // 默认角度

    private void Start()
    {
        if (cameraPivot != null)
            defaultPivotRotation = cameraPivot.localRotation;
    }

    private void Update()
    {
        HandleMovementInput();
        HandleCameraRotation();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    // ================== 移动输入 ==================
    private void HandleMovementInput()
    {
        float horizontal = 0f;
        float vertical = 0f;

        // 优先使用摇杆
        if (joystick != null && (Mathf.Abs(joystick.Horizontal) > 0.1f || Mathf.Abs(joystick.Vertical) > 0.1f))
        {
            horizontal = joystick.Horizontal;
            vertical = joystick.Vertical;
        }
        else
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }

        moveDir = new Vector3(horizontal, 0f, vertical).normalized;
        animator.SetBool("walk", moveDir.magnitude > 0.1f);
    }

    // ================== 移动玩家 ==================
    private void HandleMovement()
    {
        if (moveDir.magnitude > 0.01f)
        {
            // 使用世界轴方向移动
            Vector3 move = Vector3.zero;

            if (joystick != null && moveDir.magnitude > 0.1f)
            {
                // 摇杆移动
                move = new Vector3(moveDir.x, 0f, moveDir.z);
            }
            else
            {
                // 键盘移动
                if (Input.GetKey(KeyCode.W)) move = Vector3.forward;
                else if (Input.GetKey(KeyCode.S)) move = Vector3.back;
                else if (Input.GetKey(KeyCode.A)) move = Vector3.left;
                else if (Input.GetKey(KeyCode.D)) move = Vector3.right;
            }

            move.Normalize();
            playerRB.MovePosition(playerRB.position + move * moveSpeed * Time.fixedDeltaTime);

            if (move.magnitude > 0.01f)
                player.transform.forward = move; // 角色朝向移动方向
        }
    }

    // ================== 右滑旋转摄像机 ==================
    private void HandleCameraRotation()
    {
        bool isTouching = false;

        // 手机触摸
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.position.x > Screen.width / 2)
            {
                isTouching = true;

                if (touch.phase == TouchPhase.Moved)
                {
                    yaw += touch.deltaPosition.x * lookSpeed * Time.deltaTime;
                    pitch -= touch.deltaPosition.y * lookSpeed * Time.deltaTime;
                    pitch = Mathf.Clamp(pitch, -20f, 60f);

                    if (cameraPivot != null)
                        cameraPivot.localRotation = Quaternion.Euler(pitch, yaw, 0f);
                }
            }
        }

#if UNITY_EDITOR
        // PC右键旋转
        if (Input.GetMouseButton(1))
        {
            isTouching = true;

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            yaw += mouseX * lookSpeed * 10f;
            pitch -= mouseY * lookSpeed * 10f;
            pitch = Mathf.Clamp(pitch, -20f, 60f);

            if (cameraPivot != null)
                cameraPivot.localRotation = Quaternion.Euler(pitch, yaw, 0f);
        }
#endif

        // 松手后回到默认角度
        if (!isTouching && cameraPivot != null)
        {
            cameraPivot.localRotation = Quaternion.Slerp(cameraPivot.localRotation, defaultPivotRotation, Time.deltaTime * 5f);
            // 同时重置 yaw 和 pitch
            yaw = cameraPivot.localEulerAngles.y;
            pitch = cameraPivot.localEulerAngles.x;
        }
    }
}
