using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public abstract class CameraCut : MonoBehaviour
{
    public int id;
    public CinemachineCamera cinemachine { get; private set; }
    public Action OnCameraAction;
    public LayerMask EveryThing;
    public LayerMask NPC;
    protected Camera mainCam;
    
    protected virtual void Start()
    {
        cinemachine = GetComponent<CinemachineCamera>();
    }
}