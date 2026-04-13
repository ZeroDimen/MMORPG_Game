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
    public void TeleportPlayer()
    {
        var player = PhotonNetwork.LocalPlayer.TagObject as PlayerController;
        if(player != null)
            player.transform.position = new Vector3(-500, 5, 0);
    }
}
