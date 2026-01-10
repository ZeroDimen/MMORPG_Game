using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject playerPrefab;
    private bool _isCursorLock;
    
    private void Start()
    {
        Application.targetFrameRate = 120;
    }

    public void SetCursorLock()
    {
        Cursor.visible = _isCursorLock;
        Cursor.lockState = _isCursorLock ? CursorLockMode.None : CursorLockMode.Locked;
        _isCursorLock = !_isCursorLock;
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnSceneUnloaded(Scene scene)
    {
        throw new System.NotImplementedException();
    }
}