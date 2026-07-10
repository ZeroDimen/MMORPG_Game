using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public partial class DungeonSystem 
{
    public void OnDungeonPanel()
    {
        if (PartySystem.instance.MyParty == null)
        {
            string message = "파티가 없습니다. 파티 가입 후 시도해 주십시오.";
            OnMessagePanel(message);
            return;
        }
        pv.RPC(nameof(RequestPanel), RpcTarget.MasterClient, PhotonNetwork.NickName);
    }

    public void Accept()
    {
        pv.RPC(nameof(RequestAccept), RpcTarget.MasterClient, PhotonNetwork.NickName);
    }

    public void Cancel()
    {
        pv.RPC(nameof(RequestCancel), RpcTarget.MasterClient, PhotonNetwork.NickName);
    }

    [PunRPC]
    public void UpdatePanelUI(int acceptMember, int totalMember)
    {
        if(!dungeonPanel.activeSelf) dungeonPanel.SetActive(true);
        dungeonPanel.GetComponent<DungeonPanelView>().UpdateUI(acceptMember, totalMember);
    }

    [PunRPC]
    public void OnMessagePanel(string message)
    {
        var messagePanel = Instantiate(messagePrefab, transform);
        messagePanel.GetComponent<MessageView>().ViewText(message);
    }

    [PunRPC]
    public void OnCancel(string message)
    {
        dungeonPanel.SetActive(false);
        OnMessagePanel(message);
    }

    [PunRPC]
    public void OffPanel()
    {
        Debug.Log("패널 끄기");
        dungeonPanel.SetActive(false);
    }

    [PunRPC]
    public void OnElevator()
    {
        elevator.SetActive(true);
    }

    [PunRPC]
    public void TeleportPlayer()
    {
        // var player = PhotonNetwork.LocalPlayer.TagObject as PlayerController;
        // if(player != null)
        //     player.transform.position = new Vector3(-500, 5, 0);
        // dungeonLight.EnterDungeon();

        if (PhotonNetwork.IsMasterClient) return;
        
        var player = PhotonNetwork.LocalPlayer.TagObject as PlayerController;
        if (player != null)
            StartCoroutine(Teleport(player));
    }

    private IEnumerator Teleport(PlayerController player)
    {
        yield return StartCoroutine(FadeIn(3f));
        
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        player.transform.position = new Vector3(-500, 5, 0);
        if (cc != null) cc.enabled = true;
        dungeonLight.EnterDungeon();
        
        exitDungeonButton.SetActive(true);

        yield return new WaitForSeconds(2f);
        AudioManager._instance.BgmPlay("Dungeon");
        yield return StartCoroutine(FadeOut(3f));
    }
    
    // 페이드 아웃 (밝아짐)
    public IEnumerator FadeOut(float duration)
    {
        float elapsed = 0f;
        fadePanel.color = new Color(0, 0, 0, 1); // 검정

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }

    // 페이드 인 (어두워짐)
    public IEnumerator FadeIn(float duration)
    {
        float elapsed = 0f;
        fadePanel.color = new Color(0, 0, 0, 0); // 투명

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }

    [PunRPC]
    public void PlayTimeline()
    {
        if (PhotonNetwork.IsMasterClient) return;
        
        DungeonCutsceneController.instance.PlayTimeline();
        Debug.Log("PlayTimeline");
    }
    
    public void ExitDungeon()
    {
        var player = PhotonNetwork.LocalPlayer.TagObject as PlayerController;
        if (player != null)
            StartCoroutine(ExitDungeonCoroutine(player));
        
        PartySystem.instance.RequestSecede(PhotonNetwork.NickName);
    }

    private IEnumerator ExitDungeonCoroutine(PlayerController player)
    {
        yield return StartCoroutine(FadeIn(3f));

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        player.transform.position = fieldSpawnPos.position;
        if (cc != null) cc.enabled = true;
        dungeonLight.ExitDungeon();

        exitDungeonButton.SetActive(false);

        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(FadeOut(3f));
    }

#if UNITY_EDITOR
    // [임시 디버그] 던전 안에서 F9 → 보스 클리어 연출/복귀 강제 실행 (단독 에디터 테스트용)
    // 단일 에디터는 곧 마스터라 OnBossClear가 가드로 막히므로, BossClearRoutine을 직접 호출해 우회.
    // 테스트 후 이 #if UNITY_EDITOR 블록만 삭제하면 됨.
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            Debug.Log("[DEBUG] Boss Clear 강제 실행 (F9)");
            StartCoroutine(BossClearRoutine());
        }
    }
#endif

    // 보스 처치 → 파티 전원에게 브로드캐스트되는 던전 클리어 연출 + 자동 복귀
    [PunRPC]
    public void OnBossClear()
    {
        if (PhotonNetwork.IsMasterClient) return; // 마스터는 호스트 전용(입장 Teleport와 동일 가드)

        StartCoroutine(BossClearRoutine());
    }

    private IEnumerator BossClearRoutine()
    {
        QuestManager.Instance.HandleProgressUpdate(QuestType.Kill, 100, 1);
        
        // 1) 승리 사운드 (클립 미등록 시 SfxPlay가 로그만 남기고 무시)
        if (AudioManager._instance != null)
            AudioManager._instance.SfxPlay("DungeonClear");

        // 2) 클리어 배너 노출
        if (clearBanner != null) clearBanner.SetActive(true);
        yield return new WaitForSeconds(3f);
        if (clearBanner != null) clearBanner.SetActive(false);

        // 3) 자동 복귀 — 기존 ExitDungeonCoroutine 재사용(페이드 + fieldSpawnPos 이동 + 라이팅 복원 + exitButton off)
        var player = PhotonNetwork.LocalPlayer.TagObject as PlayerController;
        if (player != null)
            yield return StartCoroutine(ExitDungeonCoroutine(player));

        // 4) 파티 탈퇴 (기존 ExitDungeon과 동일)
        PartySystem.instance.RequestSecede(PhotonNetwork.NickName);
    }
}
