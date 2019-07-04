using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class CameraSwitcher : MonoBehaviour
    {
        [SerializeField] GameObject[] cameraList;
        int currentCamera = 0;
        
        void Awake ()
        {
            for (int i = 0; i < cameraList.Length; i++)
            {
                if (i == currentCamera)
                {
                    cameraList[i].SetActive (true);
                }
                else
                {
                    cameraList[i].SetActive (false);
                }
            }
        }

        void Update ()
        {
            if (Input.GetMouseButtonDown(1))
            {
                int nextCamera = (currentCamera + 1) % cameraList.Length;
                cameraList[nextCamera].SetActive (true);
                cameraList[currentCamera].SetActive (false);
                currentCamera = nextCamera;
            }
        } 
    }
}
