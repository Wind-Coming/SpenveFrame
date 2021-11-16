using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Matrix2x2
{
    public float m00;
    public float m01;
    public float m10;
    public float m11;
    
    public Matrix2x2(float _m00, float _m01, float _m10, float _m11)
    {
        m00 = _m00;
        m01 = _m01;
        m10 = _m10;
        m11 = _m11;
    }

    public Vector2 Mul(Vector2 v2)
    {
        return new Vector2(m00 * v2.x + m10 * v2.y, m01 * v2.x + m11 * v2.y);
    }
    
    public void SetRotation(float angle)
    {
        angle *= Mathf.Deg2Rad;
        m00 = Mathf.Cos(angle);
        m01 = -Mathf.Sin(angle);
        m10 = Mathf.Sin(angle);
        m11 = Mathf.Cos(angle);
    }

    public void Identity()
    {
        m00 = m11 = 1;
        m01 = m10 = 0;
    }
    

    public Matrix2x2 Inverse() {
        Matrix2x2 temp = this;
        float ad = (m00 * m11);
        float bc = (m01 * m10);

        if ((ad - bc) == 0.0) {
            Debug.LogError("矩阵错误");
        }


        float fract = 1 / (ad - bc);
        float tempA = temp.m00;
        temp.m00 = temp.m11 * fract;
        temp.m11 = tempA * fract;
        temp.m01 = -temp.m01 * fract;
        temp.m10 = -temp.m10 * fract;
        return temp;
    }
    
    public static Vector2 operator *(Matrix2x2 value1, Vector2 value2)
    {
        return value1.Mul(value2);
    }
}