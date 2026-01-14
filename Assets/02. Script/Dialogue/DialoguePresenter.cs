using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueEntry
{
    public int id;
    public string speaker;
    public string context;
    public string eventType;
    public string eventParam;
}

public class DialoguePresenter : MonoBehaviour
{
    [SerializeField] private DialogueView view;
    [SerializeField] private TextAsset csvFile;

    private List<DialogueEntry> _dialogueList = new List<DialogueEntry>();
    private int _currentIndex = 0;

    void Start()
    {
        LoadCSV();
        view.OnNextClick += ShowNextLine;
        view.Show(false); // 처음엔 끄기

        // 테스트용: 게임 시작 2초 뒤 첫 번째 대화 시작
        Invoke("StartTestDialogue", 2f);
    }

    private void LoadCSV()
    {
        string[] lines = csvFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] row = lines[i].Split(',');

            _dialogueList.Add(new DialogueEntry {
                id = int.Parse(row[0]),
                speaker = row[1],
                context = row[2],
                eventType = row[3],
                eventParam = row[4].Trim()
            });
        }
    }

    public void StartTestDialogue()
    {
        _currentIndex = 0;
        view.Show(true);
        DisplayCurrentLine();
    }

    private void ShowNextLine()
    {
        _currentIndex++;
        if (_currentIndex < _dialogueList.Count)
        {
            DisplayCurrentLine();
        }
        else
        {
            view.Show(false);
            Debug.Log("대화 종료");
        }
    }

    private void DisplayCurrentLine()
    {
        var data = _dialogueList[_currentIndex];
        view.UpdateUI(data.speaker, data.context);
        
        // 여기서 data.eventType을 체크하여 퀘스트 등을 처리할 수 있습니다.
    }
}
