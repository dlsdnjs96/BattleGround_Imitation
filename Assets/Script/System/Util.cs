using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    // Start is called before the first frame update
    public static Vector3 GetRandomPointInCircle(Vector3 _pos, float _len)
    {
        Vector3 point;

        point.x = _pos.x + Random.Range(-_len, _len);
        point.z = _pos.z + Random.Range(-_len, _len);
        point.y = 0.0f;


        if (Mathf.Sqrt(point.x - _pos.x) + Mathf.Sqrt(point.z - _pos.z) > Mathf.Sqrt(_len))
        {
            point -= _pos;
            point.Normalize();
            point *= _len;
            point += _pos;
        }

        return point;
    }
}
