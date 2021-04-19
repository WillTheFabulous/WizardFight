using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class GrassForceCapsule : MonoBehaviour
{
    [SerializeField] float radius=0.5f;
    [SerializeField] float height=1;

    public float Radius
    {
        get { return radius; }

        set
        {
            radius = value;
        }
    }

    public float Height
    {
        get { return height; }

        set
        {
            height = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalMatrix("_WorldToCharacterMatrix", transform.worldToLocalMatrix);
        Shader.SetGlobalMatrix("_CharacterToWorldMatrix", transform.localToWorldMatrix);
        Shader.SetGlobalFloat("_CharacterCapsuleRadius", radius);
        Shader.SetGlobalFloat("_CharacterCapsuleHeight", height);
    }

    private void OnDrawGizmos()
    {
        DrawCapsuleImp(transform.position, Vector3.zero, transform.localScale, radius,2*radius+ height, Color.white);
    }

    private void DrawCapsuleImp(Vector3 pos, Vector3 center, Vector3 scale,  float radius, float height, Color color)
    {
        // 参数保护
        if (height < 0f)
        {
            Debug.LogWarning("Capsule height can not be negative!");
            return;
        }
        if (radius < 0f)
        {
            Debug.LogWarning("Capsule radius can not be negative!");
            return;
        }
        // 根据朝向找到up 和 高度缩放值
        Vector3 up = Vector3.up;
        // 半径缩放值
        float radiusScale = 1f;
        // 高度缩放值
        float heightScale = 1f;
       
        up = Vector3.up;
        heightScale = Mathf.Abs(scale.y);
        radiusScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z));
        
        float realRadius = radiusScale * radius;
        height = height * heightScale;
        float sideHeight = Mathf.Max(height - 2 * realRadius, 0f);

        center = new Vector3(center.x * scale.x, center.y * scale.y, center.z * scale.z);
        // 为了符合Unity的CapsuleCollider的绘制样式，调整位置
        pos = pos - up.normalized * (sideHeight * 0.5f + realRadius) + center;

        Color oldColor = Gizmos.color;
        Gizmos.color = color;

        up = up.normalized * realRadius;
        Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
        Vector3 right = Vector3.Cross(up, forward).normalized * realRadius;

        Vector3 start = pos + up;
        Vector3 end = pos + up.normalized * (sideHeight + realRadius);

        // 半径圆
        DrawCircleImp(start, up, color, realRadius);
        DrawCircleImp(end, up, color, realRadius);

        // 边线
        Gizmos.DrawLine(start - forward, end - forward);
        Gizmos.DrawLine(start + right, end + right);
        Gizmos.DrawLine(start - right, end - right);
        Gizmos.DrawLine(start + forward, end + forward);
        Gizmos.DrawLine(start - forward, end - forward);

        for (int i = 1; i < 26; i++)
        {
            // 下部的头
            Gizmos.DrawLine(start + Vector3.Slerp(right, -up, (i - 1) / 25f), start + Vector3.Slerp(right, -up, i / 25f));
            Gizmos.DrawLine(start + Vector3.Slerp(-right, -up, (i - 1) / 25f), start + Vector3.Slerp(-right, -up, i / 25f));
            Gizmos.DrawLine(start + Vector3.Slerp(forward, -up, (i - 1) / 25f), start + Vector3.Slerp(forward, -up, i / 25f));
            Gizmos.DrawLine(start + Vector3.Slerp(-forward, -up, (i - 1) / 25f), start + Vector3.Slerp(-forward, -up, i / 25f));

            // 上部的头
            Gizmos.DrawLine(end + Vector3.Slerp(forward, up, (i - 1) / 25f), end + Vector3.Slerp(forward, up, i / 25f));
            Gizmos.DrawLine(end + Vector3.Slerp(-forward, up, (i - 1) / 25f), end + Vector3.Slerp(-forward, up, i / 25f));
            Gizmos.DrawLine(end + Vector3.Slerp(right, up, (i - 1) / 25f), end + Vector3.Slerp(right, up, i / 25f));
            Gizmos.DrawLine(end + Vector3.Slerp(-right, up, (i - 1) / 25f), end + Vector3.Slerp(-right, up, i / 25f));
        }

        Gizmos.color = oldColor;
    }

    private static void DrawCircleImp(Vector3 center, Vector3 up, Color color, float radius)
    {
        var oldColor = Gizmos.color;
        Gizmos.color = color;

        up = (up == Vector3.zero ? Vector3.up : up).normalized * radius;
        var forward = Vector3.Slerp(up, -up, 0.5f);
        var right = Vector3.Cross(up, forward).normalized * radius;
        for (var i = 1; i < 26; i++)
        {
            Gizmos.DrawLine(center + Vector3.Slerp(forward, right, (i - 1) / 25f), center + Vector3.Slerp(forward, right, i / 25f));
            Gizmos.DrawLine(center + Vector3.Slerp(forward, -right, (i - 1) / 25f), center + Vector3.Slerp(forward, -right, i / 25f));
            Gizmos.DrawLine(center + Vector3.Slerp(right, -forward, (i - 1) / 25f), center + Vector3.Slerp(right, -forward, i / 25f));
            Gizmos.DrawLine(center + Vector3.Slerp(-right, -forward, (i - 1) / 25f), center + Vector3.Slerp(-right, -forward, i / 25f));
        }

        Gizmos.color = oldColor;
    }
}
