using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobaCameraController : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private Transform cameraRigRoot;
    private void Awake()
    {
        if (camera == null)
        {
            camera = Camera.main;
        }

        cameraRigRoot.parent = null;
    }

    public Camera currentCamera => camera;
}
