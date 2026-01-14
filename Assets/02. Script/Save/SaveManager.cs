using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    // 데이터를 가지고 있는 주체들을 참조합니다.
    private InventoryModel _inventoryModel;
    [SerializeField] private PlayerController playerController;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void Init(InventoryModel model)
    {
        _inventoryModel = model;
    }

    private static string SavePath => Path.Combine(Application.persistentDataPath, "savefile.json");

    // [중앙 통제실 역할] 데이터를 모아서 한 번에 저장 요청
    public void SaveGame()
    {
        // 1. 저장할 바구니 생성
        CharacterSaveData data = new CharacterSaveData();

        // 2. 각 시스템에서 정보 수집
        data.playerName = playerController.playerName;
        data.level = playerController.level;
        data.gold = playerController.gold;
        data.pos = playerController.transform.position;
        
        // 인벤토리 모델에서 리스트 추출 (지난번에 이야기한 GetSaveData() 형태)
        data.inventoryItems = _inventoryModel.GetSaveData();

        // 3. 파일로 굽기 (기존에 만든 static 메서드 호출)
        Save(data);
        Debug.Log("게임 데이터 통합 저장 성공!");
    }

    public static void Save(CharacterSaveData data)
    {
        string json = JsonUtility.ToJson(data, true); // true는 가독성 좋게 포맷팅
        File.WriteAllText(SavePath, json);
        Debug.Log($"저장 완료: {SavePath}");
    }

    public static CharacterSaveData Load()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<CharacterSaveData>(json);
        }
        Debug.LogWarning("저장된 파일을 찾을 수 없습니다.");
        return null;
    }
    
    public void LoadGame()
    {
        // 1. 파일에서 데이터 읽어오기
        CharacterSaveData data = Load();

        if (data == null)
        {
            Debug.LogWarning("불러올 저장 파일이 없습니다.");
            return;
        }

        // 2. 플레이어 정보 복구
        playerController.playerName = data.playerName;
        playerController.level = data.level;
        playerController.gold = data.gold;
    
        // 위치 복구 (캐릭터 컨트롤러 등이 있다면 일시적으로 끄고 이동해야 할 수 있음)
        playerController.transform.position = data.pos;

        // 3. 인벤토리 정보 복구
        if (_inventoryModel != null)
        {
            _inventoryModel.LoadData(data.inventoryItems);
        }

        Debug.Log("게임 데이터 복구 완료!");
    }
}
