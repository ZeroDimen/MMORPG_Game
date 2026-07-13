using Photon.Pun;
using UnityEngine;

public class DungeonBossTrigger : MonoBehaviour
{
    private PhotonView _pv;
    
    private void Start()
    {
        _pv = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void BossTrigger(PhotonMessageInfo info)
    {
        DungeonSystem.instance.RequestSpawnBoss(info.Sender.NickName);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !PhotonNetwork.IsMasterClient)
        {
            _pv.RPC(nameof(BossTrigger), RpcTarget.MasterClient);
        }
    }
}
