using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public float radius = 3f;
    public Transform target;

    private float angleX = 0;
    public float angleY = 30; // 初始俯角

    public float rotateSpeed = 2f; // 鼠标灵敏度

    void LateUpdate()
    {
        // 只有按住鼠标右键时才允许旋转
        if (Input.GetMouseButton(1))
        {
            float hx = -Input.GetAxis("Mouse X") * rotateSpeed;
            float vy = Input.GetAxis("Mouse Y") * rotateSpeed;

            angleX = (angleX + hx) % 360;
            angleY = Mathf.Clamp(angleY - vy, -10, 60);
        }

        // 计算相机位置（围绕目标）
        Vector3 offset;
        offset.x = Mathf.Cos(angleX * Mathf.Deg2Rad) * radius;
        offset.z = Mathf.Sin(angleX * Mathf.Deg2Rad) * radius;
        offset.y = Mathf.Sin(angleY * Mathf.Deg2Rad) * radius;

        transform.position = target.position + offset;

        transform.LookAt(target);
    }
}