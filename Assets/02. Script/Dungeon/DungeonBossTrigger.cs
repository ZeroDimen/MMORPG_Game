using Photon.Pun;
using UnityEngine;

public class DungeonBossTrigger : MonoBehaviour
{
    private PhotonView _pv;
    
    private void Start()
    {
        _pv = GetComponent<PhotonView>();
    }

    // 보스 소환, 컷씬 등 요청 함수
    public void BossTrigger()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !PhotonNetwork.IsMasterClient)
        {
            _pv.RPC(nameof(BossTrigger), RpcTarget.MasterClient);
        }
    }
}
