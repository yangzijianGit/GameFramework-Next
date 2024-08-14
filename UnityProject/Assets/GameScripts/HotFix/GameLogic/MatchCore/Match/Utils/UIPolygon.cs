using UnityEngine;
using UnityEngine.UI;
using jc;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 多边形点击区域
/// </summary>
[RequireComponent(typeof(PolygonCollider2D))]
public class UIPolygon : Image
{
    private PolygonCollider2D _polygon = null;
    private PolygonCollider2D polygon
    {
        get
        {
            if (_polygon == null)
                _polygon = GetComponent<PolygonCollider2D>();
            return _polygon;
        }
    }

    //设置只响应点击，不进行渲染
    protected UIPolygon()
    {
        useLegacyMeshGeneration = true;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
    }

    /// <summary>
    /// 判断点击区域
    /// </summary>
    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        return polygon.OverlapPoint(eventCamera.ScreenToWorldPoint(screenPoint));
    }

    /// <summary>
    /// 设置点击区域
    /// </summary>
    /// <param name="vec">构成区域的点</param>
    public void SetPoints(Vector2[] vec)
    {
        if (vec == null || vec.Length == 0)
        {
            Debug.LogError( "UIPolygon - SetPoints - vector2[] = null");
            return;
        }
        
        polygon.points = vec;
    }

    /// <summary>
    /// 设置偏移区域
    /// </summary>
    /// <param name="vec"></param>
    public void SetOffset(Vector2 vec)
    {
        polygon.offset = vec;
    }

#if UNITY_EDITOR
    protected override void Reset()
    {
        //重置不规则区域
        base.Reset();
        transform.position = Vector3.zero;
        float w = (rectTransform.sizeDelta.x * 0.5f) + 0.1f;
        float h = (rectTransform.sizeDelta.y * 0.5f) + 0.1f;
        polygon.points = new Vector2[]{
            new Vector2(-w,-h),
            new Vector2(w,-h),
            new Vector2(w,h),
            new Vector2(-w,h)
          };
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(UIPolygon), true)]
public class UIPolygonInspector : Editor
{
    public override void OnInspectorGUI()
    {
        //什么都不写用于隐藏面板的显示
    }
}
#endif