using System;
using Unity.Cinemachine;
using UnityEngine;

public class CutScene : MonoBehaviour
{
    public CinemachineCamera monsterCam;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            monsterCam.Priority = (monsterCam.Priority == 0) ? 11 : 0;
        }
    }
}
