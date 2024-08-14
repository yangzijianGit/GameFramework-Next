using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 贝塞尔曲线组件.
/// </summary>
public class BezierCurve : MonoBehaviour
{
    public Vector3[] Points;
    private int arrLen = 3;
    private Transform selfTransform;

    /// <summary>
    /// 定义起点和终点.
    /// </summary>
    public void Init(Vector3 startPos, Vector3 endPos, int arrayLength = 3)
    {
        Points = new Vector3[arrayLength];
        Points[0] = startPos;
        Points[arrayLength - 1] = endPos;
        arrLen = arrayLength;
        selfTransform = this.transform;
    }
    /// <summary>
    /// 添加曲线点.
    /// </summary>
    public void AddPoint(params Vector3[] vec3)
    {
        if (Points == null || vec3 == null)
        {
            Debug.LogError("bezierCurve addPoint Points[] || vec3 == null");
            return;
        }
        //扩容.
        if (Points.Length - 2 < vec3.Length)
        {
            Array.Resize(ref Points, Points.Length + 1);
            Points[Points.Length - 1] = Points[Points.Length - 2];
        }
        for (int i = 1; i <= vec3.Length; i++)
        {
            Points[i] = vec3[i - 1];
        }
        //扩容长度有变化.
        arrLen = Points.Length;
    }

    /// <summary>
    /// 获取曲线上的点坐标.
    /// </summary>
    public Vector3 GetPointPosition(float t)
    {
        if (Points == null)
        {
            Debug.LogError( "bezierCurve GetPointPosition Points = null");
            return Vector3.zero;
        }
        if (arrLen == 4)
            return GetPoints(t);
        return GetPoint(t);
    }

    public Vector3 GetVelocity(float t)
    {
        return transform.TransformPoint(Bezier.GetFirstDerivative(Points[0], Points[1], Points[2], t)) -
            transform.position;
    }

    public Vector3 GetPoint(float t)
    {
        return transform.TransformPoint(Bezier.GetPoint(Points[0], Points[1], Points[2], t));
    }
    public Vector3 GetPointWorld(float t)
    {
        return Bezier.GetPoint(Points[0], Points[1], Points[2], t);
    }
    public Vector3 GetPoints(float t)
    {
        return transform.TransformPoint(Bezier.GetPoint(Points[0], Points[1], Points[2], Points[3], t));
    }

    public Vector3 GetVelocitys(float t)
    {
        return transform.TransformPoint(
            Bezier.GetFirstDerivative(Points[0], Points[1], Points[2], Points[3], t)) - transform.position;
    }

    public Vector3 GetDirection(float t)
    {
        return GetVelocitys(t).normalized;
    }

    public Vector3 ShowPoint(int index)
    {
        Vector3 point = selfTransform.TransformPoint(Points[index]);
        Points[index] = selfTransform.InverseTransformPoint(point);
        return point;
    }

    public void Reset()
    {
        Array.Clear(Points, 0, Points.Length);
    }

}