using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using static Constants;

public class GameManager :  MonoBehaviourPun
{
    private static GameManager instance;
    private bool _isCursorLock;
    [SerializeField] private SpawnZone[] spawnPoints;

    public Canvas Canvas => GetCanvas();
    
    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = new GameManager();
            return instance;
        }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator Start()
    {
        Application.targetFrameRate = 120;
        yield return null;
        Spawner("Maria");
        Spawner("Mutant");
    }

    private void Spawner(string prefabName)
    {
        if (prefabName == "Maria")
        {
            PhotonNetwork.Instantiate("Maria", spawnPoints[0].point.position , Quaternion.identity);
        }
        else if (prefabName == "Mutant")
        {
            PhotonNetwork.Instantiate("Mutant", spawnPoints[1].point.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("파일 없음");
        }
    }
    
    public void SetCursorLock()
    {
        Cursor.visible = _isCursorLock;
        Cursor.lockState = _isCursorLock ? CursorLockMode.None : CursorLockMode.Locked;
        _isCursorLock = !_isCursorLock;
    }
    
    private Canvas GetCanvas()
    {
        var canvasObject = GameObject.FindGameObjectWithTag("Canvas");
        Canvas result = null;

        if (!canvasObject)
        {
            canvasObject = new GameObject("Canvas");
            canvasObject.AddComponent<Canvas>();
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            
            result = canvasObject.GetComponent<Canvas>();
            result.renderMode = RenderMode.ScreenSpaceOverlay;
            result.tag = "Canvas";
        }
        else
        {
            result = canvasObject.GetComponent<Canvas>();
        }

        return result;
    }
    
}