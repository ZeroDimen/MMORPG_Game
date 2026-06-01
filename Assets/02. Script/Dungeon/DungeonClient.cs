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
        
        exitDungeonButton.SetActive(false);

        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(FadeOut(3f));
    }
}
