using System.Collections;
using UnityEngine;
using Photon.Pun;

public class GameManager :  MonoBehaviourPun
{
    private static GameManager instance;
    private bool _isCursorLock;

    
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
        
        PhotonNetwork.Instantiate("Maria", Vector3.up, Quaternion.identity);
        Debug.Log("캐릭터 생성");
    }

    public void SetCursorLock()
    {
        Cursor.visible = _isCursorLock;
        Cursor.lockState = _isCursorLock ? CursorLockMode.None : CursorLockMode.Locked;
        _isCursorLock = !_isCursorLock;
    }
    
}