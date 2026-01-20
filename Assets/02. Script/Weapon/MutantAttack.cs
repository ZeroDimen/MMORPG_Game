using Photon.Pun;
using UnityEngine;

// 적이 플레이어에게 데미지를 주는 스크립트
public class MutantAttack : MonoBehaviourPun
{
    [SerializeField]
    private int Damage = 10;
    private void OnTriggerEnter(Collider other)
    {
        var playerController = other.GetComponent<PlayerController>();
        if (playerController)
        {
            PhotonView playerPhotonView = other.GetComponent<PhotonView>();
            playerPhotonView.RPC("SetHit", RpcTarget.All, Damage, -transform.forward);
        }
    }
}