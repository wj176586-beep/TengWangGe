using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public float radius = 3f;
    public Transform target;

    private float angleX = 0;
    public float angleY = 30;

    public float rotateSpeed = 2f;

    private int rotateFingerId = -1; // 当前控制旋转的手指 ID

    void LateUpdate()
    {
        HandlePC();
        HandleMobile();

        UpdateCameraPosition();
    }

    // ---------------- PC 鼠标右键控制 ----------------
    void HandlePC()
    {
        if (Input.GetMouseButton(1))
        {
            float hx = -Input.GetAxis("Mouse X") * rotateSpeed;
            float vy = Input.GetAxis("Mouse Y") * rotateSpeed;

            angleX = (angleX + hx) % 360;
            angleY = Mathf.Clamp(angleY - vy, -10, 60);
        }
    }

    // ---------------- 手机触屏控制 ----------------
    void HandleMobile()
    {
        if (Input.touchCount == 0)
        {
            rotateFingerId = -1;
            return;
        }

        foreach (Touch t in Input.touches)
        {
            Vector2 pos = t.position;

            // 只允许 "右半屏" 控制旋转
            if (rotateFingerId == -1 && t.phase == TouchPhase.Began)
            {
                if (pos.x > Screen.width * 0.5f)
                {
                    rotateFingerId = t.fingerId; // 锁定该手指
                }
            }

            // 如果是被锁定的手指，执行旋转
            if (t.fingerId == rotateFingerId)
            {
                if (t.phase == TouchPhase.Moved)
                {
                    // deltaPosition 本帧移动的像素差值
                    float hx = -t.deltaPosition.x * 0.1f * rotateSpeed;
                    float vy = t.deltaPosition.y * 0.1f * rotateSpeed;

                    angleX = (angleX + hx) % 360;
                    angleY = Mathf.Clamp(angleY - vy, -10, 60);
                }

                // 手指抬起→释放控制权
                if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
                {
                    rotateFingerId = -1;
                }
            }
        }
    }

    // ---------------- 更新相机位置 ----------------
    void UpdateCameraPosition()
    {
        Vector3 offset;
        offset.x = Mathf.Cos(angleX * Mathf.Deg2Rad) * radius;
        offset.z = Mathf.Sin(angleX * Mathf.Deg2Rad) * radius;
        offset.y = Mathf.Sin(angleY * Mathf.Deg2Rad) * radius;

        transform.position = target.position + offset;
        transform.LookAt(target);
    }
}