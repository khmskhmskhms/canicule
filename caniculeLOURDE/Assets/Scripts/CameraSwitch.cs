using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    private Cinemachine.CinemachineVirtualCamera[] cameraList;
    private int currentCamera;
    
    // Start is called before the first frame update
    void Awake()
    {
        cameraList = GetComponentsInChildren<Cinemachine.CinemachineVirtualCamera>();

        if (cameraList.Length > 0)
        {
            for (int i = 0; i < cameraList.Length; i++)
            {
                cameraList[i].gameObject.SetActive(false);
            }

            cameraList[0].gameObject.SetActive(true);
            currentCamera = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            changeCamera();
        }
    }

    private void changeCamera()
    {
        cameraList[currentCamera].gameObject.SetActive(false);

        int nextCamera = currentCamera + 1;

        if (nextCamera < cameraList.Length)
        {
            cameraList[nextCamera].gameObject.SetActive(true);
            currentCamera = nextCamera;
        }
        else
        {
            cameraList[0].gameObject.SetActive(true);
            currentCamera = 0;
        }
        
    }
}
