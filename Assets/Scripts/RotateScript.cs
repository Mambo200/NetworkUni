using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RotateScript
{

    public static void RotationX(GameObject _go, float _rotSpeed)
    {
        _go.transform.Rotate(new Vector3(_rotSpeed * Time.deltaTime, 0, 0));
    }

    public static void RotationY(GameObject _go, float _rotSpeed)
    {
        _go.transform.Rotate(new Vector3(0, _rotSpeed * Time.deltaTime, 0));
    }

    public static void RotationZ(GameObject _go, float _rotSpeed)
    {
        _go.transform.Rotate(new Vector3(0, 0, _rotSpeed * Time.deltaTime));
    }
}
