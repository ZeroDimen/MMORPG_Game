using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
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
        AudioManager._instance.BgmPlay("Forest");
        Spawner("Maria");
    }

    public void Spawner(string prefabName)
    {
        Vector3 spownPos;
        switch (prefabName)
        {
            case "Maria":
                spownPos = GetRandomPosition(spawnPoints[0].point, spawnPoints[0].radius);
                PhotonNetwork.Instantiate("Maria", spownPos , Quaternion.identity);
                break;
            case "Mutant":
                spownPos = GetRandomPosition(spawnPoints[1].point, spawnPoints[1].radius);
                PhotonNetwork.Instantiate("Mutant", spownPos, Quaternion.identity);
                break;
            default:
                Debug.Log("파일 없음");
                break;
        }
    }

    Vector3 GetRandomPosition(Transform point, float radius)
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        return point.position + new Vector3(randomCircle.x, 0, randomCircle.y);
    }

    public void SetGameState(EGameState state)
    {
        if (state == EGameState.Interaction)
        {
            Cursor.visible  = true;
            Cursor.lockState = CursorLockMode.None;
            AudioManager._instance.BgmVolume(0.3f);
        }
        else if (state == EGameState.Play)
        {
            Cursor.visible  = false;
            Cursor.lockState = CursorLockMode.Locked;
            AudioManager._instance.BgmVolume(1f);
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