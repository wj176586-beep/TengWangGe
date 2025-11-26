// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class PlayMove : MonoBehaviour
// {
//     public float playerSpeed = 1.9f;
//
//     public CharacterController cC;
//
//     public float gravity = -9.8f;
//
//     public Animator animator;
//
//     public float tunCalmTime = 0.5f;
//
//     public Transform playerCamera;
//
//     private float turnCalmVelocity;
//
//     Vector3 velocity;
//     public Transform surfaceCheck;
//     bool onSurface;
//     public float surfaceDistance = 0.4f; //脚底检测半径
//     public LayerMask surfaceMask;
//
//
//     private void Update()
//     {
//         onSurface = Physics.CheckSphere(surfaceCheck.position, surfaceDistance, surfaceMask);
//         // 将角色保持一个轻微向下的力
//         if (onSurface && velocity.y < 0)
//             // if (velocity.y < 0)
//         {
//             velocity.y = -2f;
//         }
//
//         // 每一帧给角色加上重力的加速度  实现自然下落效果
//         velocity.y += gravity * Time.deltaTime;
//         // 根据速度，移动角色
//         // cC.Move(velocity * Time.deltaTime);
//
//
//         PlayerMove();
//     }
//
//     private void PlayerMove()
//     {
//         float horizontal_axis = Input.GetAxisRaw("Horizontal");
//         float vertical_axis = Input.GetAxisRaw("Vertical");
//         //长度归一化
//         Vector3 direction = new Vector3(horizontal_axis, 0, vertical_axis).normalized;
//         // 判断移动的距离
//         if (direction.magnitude >= 0.1f)
//         {
//             // 获取旋转角度   Mathf.Atan2 返回的是弧度   Mathf.Rad2Deg弧度转成 角度  Mathf.Rad2Deg固定的57  1弧度=57角度
//             float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
//             // 平滑速度
//             float angle =
//                 Mathf.SmoothDampAngle(
//                     transform.eulerAngles.y, //当前角度
//                     targetAngle, //目标角度
//                     ref turnCalmVelocity, //当前旋转速度
//                     tunCalmTime //平滑时间
//                 );
//             Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
//             // 旋转
//             transform.rotation = Quaternion.Euler(0, angle, 0);
//             //Move是每帧移动多少距离   moveDirection玩家移动方向
//             cC.Move(moveDirection * (playerSpeed * Time.deltaTime));
//
//             // 设置动画
//             animator.SetBool("walk", true);
//         }
//         else
//         {
//             animator.SetBool("walk", false);
//         }
//     }
// }

using UnityEngine;

public class PlayMove : MonoBehaviour
{
    public float playerSpeed = 1.9f;
    public CharacterController cC;

    public float gravity = -9.8f;
    Vector3 velocity;

    public Animator animator;
    public float tunCalmTime = 0.5f;
    private float turnCalmVelocity;

    public Transform playerCamera;
    public float cameraSensitivity = 120f; // 鼠标旋转速度
    private float cameraYaw = 0f;
    private float cameraPitch = 20f; // 初始俯角
    public float minPitch = -30f;
    public float maxPitch = 60f;

    public Transform surfaceCheck;
    public float surfaceDistance = 0.4f;
    public LayerMask surfaceMask;
    bool onSurface;

    void Update()
    {
        // --- 检测地面 ---
        onSurface = Physics.CheckSphere(surfaceCheck.position, surfaceDistance, surfaceMask);
        if (onSurface && velocity.y < 0)
            velocity.y = -2f;

        // --- 重力 ---
        velocity.y += gravity * Time.deltaTime;

        // --- 相机控制 ---
        HandleCamera();

        // --- 玩家移动 ---
        PlayerMove();
    }

    private void HandleCamera()
    {
        // 摄像机位置跟随角色
        playerCamera.position = transform.position;

        // 右键按住旋转视角
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;

            cameraYaw += mouseX;
            cameraPitch -= mouseY;
            cameraPitch = Mathf.Clamp(cameraPitch, minPitch, maxPitch);
        }

        // 更新摄像机旋转
        playerCamera.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);
    }

    private void PlayerMove()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // 角色旋转（根据摄像机方向 + 移动方向）
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg
                                + playerCamera.eulerAngles.y;

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

        // 应用重力
        cC.Move(velocity * Time.deltaTime);
    }
}
