using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;
using Random = UnityEngine.Random;

[System.Serializable]
public class DialogueEntry
{
    public string groupKey;   // 대화 뭉치 식별자 (예: CHIEF_INTRO)
    public string speaker;    // 화자 이름
    public string context;    // 대화 내용
    public string eventType;  // 이벤트 타입 (GiveItem, GiveQuest 등)
    public string eventParam; // 이벤트 매개변수 (아이템 ID 등)
}

public class DialoguePresenter : MonoBehaviour
{
    [SerializeField] private DialogueView view;
    [SerializeField] private TextAsset csvFile;

    // 모든 대화를 그룹별로 저장하는 딕셔너리 (Key: GroupKey)
    private Dictionary<string, List<DialogueEntry>> _dialogueGroups = new Dictionary<string, List<DialogueEntry>>();
    
    // 현재 진행 중인 대화 리스트 및 인덱스
    private List<DialogueEntry> _currentGroup;
    private int _currentIndex;
    
    private void Awake()
    {
        LoadCSV();
    }

    private void OnEnable()
    {
        GameEvents.OnDialogueRequested += StartDialogue;
    }

    private void OnDisable()
    {
        GameEvents.OnDialogueRequested -= StartDialogue;
    }

    void Start()
    {
        view.OnNextClick += ShowNextLine;
        view.Show(false); // 처음엔 끄기
    }

    private void LoadCSV()
    {
        _dialogueGroups.Clear();

        // 줄바꿈 문자(\n, \r)를 기준으로 행 분리
        string[] lines = csvFile.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        // 첫 번째 줄(헤더)은 건너뛰고 1번 인덱스부터 시작
        for (int i = 1; i < lines.Length; i++)
        {
            string[] row = lines[i].Split(',');

            if (row.Length < 5) continue;

            DialogueEntry entry = new DialogueEntry
            {
                groupKey = row[0].Trim().Replace("\r", ""),
                speaker = row[1].Trim(),
                context = row[2].Trim(),
                eventType = row[3].Trim(),
                eventParam = row[4].Trim()
            };

            // 딕셔너리에 그룹이 없으면 생성 후 추가
            if (!_dialogueGroups.ContainsKey(entry.groupKey))
            {
                _dialogueGroups[entry.groupKey] = new List<DialogueEntry>();
            }
            _dialogueGroups[entry.groupKey].Add(entry);
        }
        Debug.Log($"CSV 로드 완료: {_dialogueGroups.Count}개의 대화 그룹 저장됨.");
    }

    // 외부(NPC 등)에서 대화를 시작할 때 호출
    public void StartDialogue(string groupKey)
    {
        if (!_dialogueGroups.ContainsKey(groupKey))
        {
            Debug.LogError($"대화 그룹 키를 찾을 수 없습니다: {groupKey}");
            return;
        }

        _currentGroup = _dialogueGroups[groupKey];
        _currentIndex = 0;
        
        view.Show(true);
        DisplayCurrentLine();
    }

    private void ShowNextLine()
    {
        _currentIndex++;

        if (_currentIndex < _currentGroup.Count)
        {
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    private void DisplayCurrentLine()
    {
        var data = _currentGroup[_currentIndex];
        view.UpdateUI(data.speaker, data.context);
        
        // 대화 한 줄이 나올 때마다 이벤트 실행 체크
        ExecuteEvent(data.eventType, data.eventParam);
    }

    private void ExecuteEvent(string type, string param)
    {
        if (string.IsNullOrEmpty(type) || type == "None") return;

        switch (type)
        {
            case "GiveItem":
                // TODO: InventoryModel.Instance.AddItem(int.Parse(param));
                
                break;
            case "GiveQuest":
                bool isMain = (param == "Main");
                view.ShowQuestButton(isMain);
                break;
            case "NpcCamera":
                if (!param.IsNullOrEmpty())
                    GameEvents.OnCameraChanged?.Invoke(int.Parse(param));
                break;
            case "CutScene":
                if (!param.IsNullOrEmpty())
                    GameEvents.OnCameraChanged?.Invoke(int.Parse(param));
                break;
            case "CreateMonster":
                StartCoroutine(SpawnMonster());
                break;
        }
    }

    IEnumerator SpawnMonster()
    {
        for (int i = 0; i < 3; i++)
        {
            GameManager.Instance.Set_Spawner("Mutant");
            float rendomTime = Random.Range(0.2f, 1f);
            yield return new WaitForSeconds(rendomTime);
        }
    }

    private void EndDialogue()
    {
        GameEvents.OnDialogueEnded?.Invoke();
        view.Show(false);
        _currentGroup = null;
        Debug.Log("대화 종료");
    }
}
