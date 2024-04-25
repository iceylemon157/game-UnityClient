using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {
    private enum LookAtType {
        LookAtCamera,
        LookAtCameraInvert,
        LookAtCameraForward,
        LookAtCameraForwardInvert
    }
    [SerializeField] private LookAtType lookAtType;
    private Camera _camera;

    private void Start() {
        _camera = Camera.main;
    }

    private void LateUpdate() {
        switch (lookAtType) {
            case LookAtType.LookAtCamera:
                transform.LookAt(_camera.transform);
                break;
            case LookAtType.LookAtCameraInvert:
                transform.LookAt(_camera.transform);
                transform.Rotate(0, 180, 0);
                break;
            case LookAtType.LookAtCameraForward:
                transform.forward = _camera.transform.forward;
                break;
            case LookAtType.LookAtCameraForwardInvert:
                transform.forward = -_camera.transform.forward;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}