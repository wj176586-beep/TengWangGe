using UnityEngine;

public class OperateModel : MonoBehaviour
{
    [Header("目标模型")]
    public Transform target;
    public Rigidbody rb;             // Rigidbody，isKinematic=false
    public Collider modelCollider;   // 模型 Collider

    [Header("操作参数")]
    public float moveStep = 0.1f;    // 上升/下降步长
    public float rotateStep = 45f;   // 每次旋转角度
    public float scaleStep = 0.1f;   // 放大缩小步长

    [Header("缩放范围")]
    public float maxScaleMultiplier = 3f;
    public float minScaleMultiplier = 0.33f;

    [Header("碰撞检测")]
    public LayerMask obstacleLayer;  // 地形 + 障碍物

    private Vector3 originalPos;
    private Vector3 originalScale;
    private Quaternion originalRot;

    private bool isPreview = false;

    void Start()
    {
        if (target == null || rb == null || modelCollider == null)
        {
            Debug.LogError("请拖入目标模型、Rigidbody 和 Collider！");
            enabled = false;
            return;
        }

        // 保存原始状态
        originalPos = target.position;
        originalScale = target.localScale;
        originalRot = target.rotation;

        // Rigidbody 设置
        rb.useGravity = false;
        rb.isKinematic = false;              
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (isPreview)
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, 50f * Time.deltaTime, 0));
        }
    }

    // ----------------- 上升 / 下降 -----------------
    public void OnMoveUp() => Move(Vector3.up);
    public void OnMoveDown() => Move(Vector3.down);

    private void Move(Vector3 dir)
    {
        Vector3 newPos = rb.position + dir * moveStep;

        if (!Physics.CheckBox(newPos, modelCollider.bounds.extents, rb.rotation, obstacleLayer))
        {
            rb.MovePosition(newPos);
        }
    }

    // ----------------- 旋转 -----------------
    public void OnRotate()
    {
        Quaternion newRot = rb.rotation * Quaternion.Euler(0, rotateStep, 0);

        if (!Physics.CheckBox(rb.position, modelCollider.bounds.extents, newRot, obstacleLayer))
        {
            rb.MoveRotation(newRot);
        }
    }

    // ----------------- 放大 / 缩小 -----------------
    public void OnScaleUp() => ScaleModel(scaleStep);
    public void OnScaleDown() => ScaleModel(-scaleStep);

    private void ScaleModel(float step)
    {
        Vector3 newScale = target.localScale + Vector3.one * step;

        // 限制缩放范围
        float max = originalScale.x * maxScaleMultiplier;
        float min = originalScale.x * minScaleMultiplier;
        newScale = new Vector3(
            Mathf.Clamp(newScale.x, min, max),
            Mathf.Clamp(newScale.y, min, max),
            Mathf.Clamp(newScale.z, min, max)
        );

        // 计算缩放比例（按分量）
        Vector3 scaleRatio = new Vector3(
            newScale.x / target.localScale.x,
            newScale.y / target.localScale.y,
            newScale.z / target.localScale.z
        );

        Vector3 halfExtents = Vector3.Scale(modelCollider.bounds.extents, scaleRatio);

        // 检测缩放后是否会与障碍物碰撞
        if (!Physics.CheckBox(rb.position, halfExtents, rb.rotation, obstacleLayer))
        {
            target.localScale = newScale;
        }
    }

    // ----------------- 重置 -----------------
    public void OnReset()
    {
        target.position = originalPos;
        target.localScale = originalScale;
        target.rotation = originalRot;
        rb.MovePosition(originalPos);
        rb.MoveRotation(originalRot);
        isPreview = false;
    }

    // ----------------- 预览 -----------------
    public void OnPreview()
    {
        isPreview = !isPreview;
    }
}
