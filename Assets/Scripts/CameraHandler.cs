using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private float cameraSizeOffset;
    [SerializeField] private float cameraVerticalOffset;

    private void Start()
    {
        CameraEvents.Current.OnPositionCamera += PositionCamera;
        CameraEvents.Current.OnChangeOrtographicSize += SetOrtographicSize;
    }

    private void PositionCamera(float width, float height)
    {
        // float newPositionX = width/2;
        // float newPositionY = height/2;
        Camera.main.transform.position = new Vector3(width, height + cameraVerticalOffset, -10f);
    }

    private void SetOrtographicSize(float horizontal, float vertical)
    {
        Camera.main.orthographicSize = horizontal > vertical ? horizontal + cameraSizeOffset : vertical;
    }
}
