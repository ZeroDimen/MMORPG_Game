using UnityEngine;

public class CutSceneCamera : CameraCut
{
    protected override void Start()
    {
        base.Start();
        OnCameraAction = CameraCutScene;
    }
    
    public void CameraCutScene()
    {
        mainCam = Camera.main;
        cinemachine.Priority = cinemachine.Priority == 0 ? 11 : 0;
        if (mainCam != null) mainCam.cullingMask = mainCam.cullingMask == EveryThing ? NPC : EveryThing;
    }
}
