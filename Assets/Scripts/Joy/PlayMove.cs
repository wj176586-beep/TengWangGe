using UnityEngine;

public class PlayMove : MonoBehaviour
{
    public float playerSpeed = 3f;
    public CharacterController cC;
    public float gravity = -9.8f;
    public Animator animator;

    public float tunCalmTime = 0.2f;
    private float turnCalmVelocity;

    public Transform cameraRig;

    Vector3 velocity;
    public Transform surfaceCheck;
    bool onSurface;
    public float surfaceDistance = 0.4f;
    public LayerMask surfaceMask;

    public Joystick joystick; // 手机摇杆

    void Update()
    {
        // --- 重力 ---
        onSurface = Physics.CheckSphere(surfaceCheck.position, surfaceDistance, surfaceMask);
        if (onSurface && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;

        // --- 通用移动 ---
        HandleMove();

        // 应用重力
        cC.Move(velocity * Time.deltaTime);
    }

    void HandleMove()
    {
        // 1️⃣ 获取摇杆输入（Unity Remote 中也能用）
        float hJoy = joystick != null ? joystick.Horizontal : 0f;
        float vJoy = joystick != null ? joystick.Vertical : 0f;

        // 2️⃣ 获取键盘输入（Editor 中有效）
        float hKey = Input.GetAxisRaw("Horizontal");
        float vKey = Input.GetAxisRaw("Vertical");

        // 3️⃣ 优先使用摇杆（如果摇杆确实有输入）
        float horizontal = Mathf.Abs(hJoy) > 0.1f ? hJoy : hKey;
        float vertical = Mathf.Abs(vJoy) > 0.1f ? vJoy : vKey;

        // --- 移动 ---
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg
                                + cameraRig.eulerAngles.y;

            float angle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref turnCalmVelocity,
                tunCalmTime
            );

            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            cC.Move(moveDir * (playerSpeed * Time.deltaTime));

            animator.SetBool("walk", true);
        }
        else
        {
            animator.SetBool("walk", false);
        }
    }
}