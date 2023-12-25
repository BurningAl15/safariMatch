using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEvents : MonoSingleton<CameraEvents>
{
    public event Action<float, float> OnPositionCamera;
    public event Action<float, float> OnChangeOrtographicSize;

    public void PositionCamera(float width, float height)
    {
        if (OnPositionCamera != null)
        {
            OnPositionCamera(width, height);
        }
    }

    public void ChangeOrtographicSize(float horizontal, float vertical)
    {
        if (OnChangeOrtographicSize != null)
        {
            OnChangeOrtographicSize(horizontal, vertical);
        }
    }
}
