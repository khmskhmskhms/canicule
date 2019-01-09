using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    private Cinemachine.CinemachineVirtualCamera[] cameraList;
    public int currentCamera;
    
    void Awake()
    {
        cameraList = GetComponentsInChildren<Cinemachine.CinemachineVirtualCamera>();

        if (cameraList.Length > 0)
        {            
            currentCamera = 0;
        }
    }

    public void ChangeCamera(Cinemachine.CinemachineVirtualCamera newCamera)
    {
        for (int i =0; i< cameraList.Length; i++)
        {
            if (cameraList[i] == newCamera)
            {
                print("nouvelle caméra : "+i);
                cameraList[i].MoveToTopOfPrioritySubqueue();
                currentCamera = i;
            }
        }        
    }
}
