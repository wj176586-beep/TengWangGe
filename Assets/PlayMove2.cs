using UnityEngine;

public class PlayMove2 : MonoBehaviour
{
    public float playerSpeed = 3f;

    public CharacterController cC;

    public float gravity = -9.8f;

    public Animator animator;

    public float tunCalmTime = 0.2f;

    private float turnCalmVelocity;

    public Transform cameraRig; // 不是主相机，是 CameraRig


    Vector3 velocity;
    public Transform surfaceCheck;
    bool onSurface;
    public float surfaceDistance = 0.4f; //脚底检测半径
    public LayerMask surfaceMask;


    private void Update()
    {
        onSurface = Physics.CheckSphere(surfaceCheck.position, surfaceDistance, surfaceMask);
        // 将角色保持一个轻微向下的力
        if (onSurface && velocity.y < 0)
            // if (velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 每一帧给角色加上重力的加速度  实现自然下落效果
        velocity.y += gravity * Time.deltaTime;
        // 根据速度，移动角色


        PlayerMove();
    }

    private void PlayerMove()
    {
        float horizontal_axis = Input.GetAxisRaw("Horizontal");
        float vertical_axis = Input.GetAxisRaw("Vertical");
        //长度归一化
        Vector3 direction = new Vector3(horizontal_axis, 0, vertical_axis).normalized;
        // 判断移动的距离
        if (direction.magnitude >= 0.1f)
        {
            // 获取旋转角度   Mathf.Atan2 返回的是弧度   Mathf.Rad2Deg弧度转成 角度  Mathf.Rad2Deg固定的57  1弧度=57角度
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg
                                + cameraRig.eulerAngles.y;

            float angle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref turnCalmVelocity,
                tunCalmTime
            );

            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            cC.Move(moveDirection * (playerSpeed * Time.deltaTime));

            // 设置动画
            animator.SetBool("walk", true);
        }
        else
        {
            animator.SetBool("walk", false);
        }
    }
}