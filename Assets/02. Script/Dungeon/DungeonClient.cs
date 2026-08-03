using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Cinemachine;
using Unity.VisualScripting;
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
        if (PhotonNetwork.IsMasterClient) return;
        if(!dungeonPanel.activeSelf) dungeonPanel.SetActive(true);
        dungeonPanel.GetComponent<DungeonPanelView>().UpdateUI(acceptMember, totalMember);
    }

    [PunRPC]
    public void OnMessagePanel(string message)
    {
        if (PhotonNetwork.IsMasterClient) return;
        var messagePanel = Instantiate(messagePrefab, transform);
        messagePanel.GetComponent<MessageView>().ViewText(message);
    }

    [PunRPC]
    public void OnCancel(string message)
    {
        if (PhotonNetwork.IsMasterClient) return;
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
    }

    private IEnumerator ExitDungeonCoroutine(PlayerController player)
    {
        yield return StartCoroutine(FadeIn(3f));

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        player.transform.position = fieldSpawnPos.position;
        if (cc != null) cc.enabled = true;
        dungeonLight.ExitDungeon();
        RequestDestroyBoss();
        UIManager.Instance.OffBossHpBar();

        exitDungeonButton.SetActive(false);
        PartySystem.instance.RequestSecede(PhotonNetwork.NickName);
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(FadeOut(3f));
    }

    public void RequestDestroyBoss()
    {
        pv.RPC(nameof(DestroyBoss), RpcTarget.MasterClient, PhotonNetwork.NickName);
    }

#if UNITY_EDITOR
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
            var player = PhotonNetwork.LocalPlayer.TagObject as PlayerController;
            if (player != null)
            {
                CharacterController cc = player.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;
                player.transform.position = cheat.position;
                if (cc != null) cc.enabled = true;
            }
        }
    }
#endif

    [PunRPC]
    public void OnBossClear()
    {
        if (PhotonNetwork.IsMasterClient) return; 

        StartCoroutine(BossClearRoutine());
    }

    private IEnumerator BossClearRoutine()
    {
        UIManager.Instance.OffBossHpBar();
        float origFixed = Time.fixedDeltaTime;
        Time.timeScale = 0.4f;
        Time.fixedDeltaTime = origFixed * Time.timeScale;
        yield return new WaitForSecondsRealtime(6.5f);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = origFixed;

        QuestManager.Instance.HandleProgressUpdate(QuestType.Kill, 100, 1);

        if (AudioManager._instance != null)
            AudioManager._instance.SfxPlay("DungeonClear");

        if (clearBanner != null) clearBanner.SetActive(true);
        yield return new WaitForSeconds(3f);
        if (clearBanner != null) clearBanner.SetActive(false);

        var player = PhotonNetwork.LocalPlayer.TagObject as PlayerController;
        if (player != null)
            yield return StartCoroutine(ExitDungeonCoroutine(player));
    }

    [PunRPC]
    public void OnBossSpawnEffect()
    {
        if (PhotonNetwork.IsMasterClient) return; 

        StartCoroutine(BossSpawnEffectRoutine());
    }

    private IEnumerator BossSpawnEffectRoutine()
    {
        GameManager.Instance.PushState(Constants.EGameState.Cutscene);
        
        fadePanel.color = new Color(0, 0, 0, 1);
        yield return new WaitForSeconds(1f);
        
        CutTo(bossCam);
        bossCam.LookAt = CurrentBoss.transform;
        fadePanel.color = new Color(0, 0, 0, 0);
        yield return _letterbox.Show();
        StartCoroutine(CameraShake(0.8f, 0.6f));
        
        yield return new WaitForSeconds(4f);

        CutTo(bossCam2);
        StartCoroutine(MoveCamera(bossCam2.transform,
            bossCam2.transform.position + bossCam2.transform.forward * 4f, 3f));
        yield return new WaitForSeconds(3f);

        CutTo(bossCam3);
        yield return new WaitForSeconds(3f);

        if (CurrentBoss != null)
            RequestBossIdleStateToMaster();

        
        yield return StartCoroutine(_letterbox.Hide());
        CutTo(null);
        UIManager.Instance.OnBossHpBar();

        GameManager.Instance.PopState(Constants.EGameState.Cutscene);
    }

    public void RequestBossIdleStateToMaster()
    {
        photonView.RPC(nameof(RequestBossIdleState), RpcTarget.MasterClient);
    }

    private IEnumerator CameraShake(float duration, float magnitude)
    {
        if (impulseSource == null) yield break;

        float elapsed = 0f;
        const float interval = 0.04f; 
        while (elapsed < duration)
        {
            float falloff = 1f - (elapsed / duration);          
            Vector3 dir = Random.insideUnitSphere;
            dir.z *= 0.3f;                                       
            impulseSource.GenerateImpulseWithVelocity(dir * magnitude * falloff);
            elapsed += interval;
            yield return new WaitForSecondsRealtime(interval);  
        }
    }
    
    private void CutTo(CinemachineCamera cam)
    {
        brain.DefaultBlend = new CinemachineBlendDefinition(
            CinemachineBlendDefinition.Styles.Cut, 0f);

        bossCam.Priority.Value = 0;
        bossCam2.Priority.Value = 0;
        bossCam3.Priority.Value  = 0;

        if (cam != null) cam.Priority.Value = 100;
        else
            brain.DefaultBlend = new CinemachineBlendDefinition(
                CinemachineBlendDefinition.Styles.EaseIn, 1f);
    }
    
    private IEnumerator MoveCamera(Transform camT, Vector3 to, float duration)
    {
        Vector3 from = camT.position;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / duration);
            camT.position = Vector3.Lerp(from, to, k);
            yield return null;
        }
        camT.position = to;
    }
    
    
    private IEnumerator RotateCamera(Transform camT, Quaternion to, float duration)
    {
        Quaternion from = camT.rotation;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / duration);
            camT.rotation = Quaternion.Slerp(from, to, k);
            yield return null;
        }
        camT.rotation = to;
    }
}
