using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class CameraFacing : MonoBehaviour
    {
        Camera myCamera;
        void Awake ()
        {
            myCamera = Camera.main;
        }

        void Update ()
        {
            transform.forward = myCamera.transform.forward;
        }
    }
}