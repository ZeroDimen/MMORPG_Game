using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

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
        if (Input.GetKeyDown(KeyCode.F8))
        {
            DungeonCutsceneController.instance.OnFogFadeOut();
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
        // 0) 우승 순간 슬로우모션 (실시간 대기 → 슬로우 중에도 정확히 1.2초)
        float origFixed = Time.fixedDeltaTime;
        Time.timeScale = 0.4f;
        Time.fixedDeltaTime = origFixed * Time.timeScale;
        yield return new WaitForSecondsRealtime(8f);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = origFixed;

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

    // 보스 등장 → 파티 전원에게 브로드캐스트되는 등장 연출 (입력잠금 + 슬로우모션 + 카메라 흔들림)
    [PunRPC]
    public void OnBossSpawnEffect()
    {
        if (PhotonNetwork.IsMasterClient) return; // 마스터는 호스트 전용

        StartCoroutine(BossSpawnEffectRoutine());
    }

    private IEnumerator BossSpawnEffectRoutine()
    {
        var input = GameManager.LocalPlayer != null
            ? GameManager.LocalPlayer.GetComponent<PlayerInput>()
            : null;
        if (input != null) input.enabled = false; // 입력 잠금

        float origFixed = Time.fixedDeltaTime;
        Time.timeScale = 0.4f;                      // 슬로우모션
        Time.fixedDeltaTime = origFixed * Time.timeScale;

        StartCoroutine(CameraShake(1.2f, 0.6f)); // 지진형 카메라 흔들림(슬로우모션 창과 동시 진행)

        yield return new WaitForSecondsRealtime(4f);

        Time.timeScale = 1f;                        // 복원
        Time.fixedDeltaTime = origFixed;
        if (input != null) input.enabled = true;
    }

    // 지진형 카메라 흔들림: 짧은 임펄스를 랜덤 방향으로 반복 → 지속 떨림 후 감쇠
    private IEnumerator CameraShake(float duration, float magnitude)
    {
        if (impulseSource == null) yield break;

        float elapsed = 0f;
        const float interval = 0.04f; // 발생 간격(작을수록 촘촘한 떨림)
        while (elapsed < duration)
        {
            float falloff = 1f - (elapsed / duration);          // 점점 약해짐
            Vector3 dir = Random.insideUnitSphere;
            dir.z *= 0.3f;                                       // 전후 흔들림은 약하게(좌우/상하 위주)
            impulseSource.GenerateImpulseWithVelocity(dir * magnitude * falloff);
            elapsed += interval;
            yield return new WaitForSecondsRealtime(interval);  // 슬로우모션 중에도 실시간 진행
        }
    }
}
