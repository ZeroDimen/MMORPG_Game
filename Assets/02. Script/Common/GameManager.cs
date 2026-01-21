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
    [SerializeField] private GameObject chattingInputField;

    public Canvas Canvas => GetCanvas();
    
    public EGameState GameState { get; private set; }
    
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

    public void SetGameState(EGameState state)
    {
        if (state == EGameState.Interaction)
        {
            Cursor.visible  = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (state == EGameState.Play)
        {
            Cursor.visible  = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        GameState = state;
        
        if (PhotonNetwork.LocalPlayer?.TagObject is PlayerController pc)
        {
            pc.SetPlayerInputEnabled(state == EGameState.Play);
        }
    }
    
    public void SetChattingInputField()
    {
        chattingInputField.SetActive(!chattingInputField.activeSelf);
        if (chattingInputField.activeSelf)
        {
            SetGameState(EGameState.Interaction);
        }
        else
        {
            SetGameState(EGameState.Play);
        }
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