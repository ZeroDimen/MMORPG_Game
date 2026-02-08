using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public List<CameraCut> camList;
    private int currentCamID;

    private void Start()
    {
        GameEvents.OnCameraChanged += CameraChange;
        GameEvents.OnCurrentCameraChanged += CurrentCameraChange;
    }

    private void CameraChange(int id)
    {
        currentCamID = id;
        var nextCam = camList.Find(cam => cam.id == id);
        nextCam.OnCameraAction?.Invoke();
    }

    private void CurrentCameraChange()
    {
        GameEvents.OnCameraChanged?.Invoke(currentCamID);
    }
}
