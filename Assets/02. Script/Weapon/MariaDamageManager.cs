using Photon.Pun;
using UnityEngine;

// 플레이어가 적에게 데미지를 주는 스크립트
public class MariaDamageManager : MonoBehaviour
{
    public int Damage = 1;
    private PhotonView playerPV;
    private PlayerStatus playerStatus;
    private string ObjName = null;

    private void Start()
    {
        playerPV = gameObject.transform.parent.GetComponent<PhotonView>();
        playerStatus = gameObject.transform.parent.GetComponent<MariaPlayerController>().Status;
        ObjName = gameObject.name;
        
        DamageSetting(ObjName);
    }

    private void OnEnable()
    {
        DamageSetting(ObjName);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!playerPV.IsMine) return;
        var enemyController = other.GetComponent<EnemyController>();
        if (enemyController)
        {
            PhotonView enemyView = enemyController.photonView;
            GameManager.Instance.HitEnemy(enemyView,playerPV, Damage);
        }
    }

    private void DamageSetting(string text)
    {
        if (playerStatus == null)
        {
            return;
        }
        
        string name = text.Substring(text.IndexOf(']') + 1).Trim(); // 오브젝트 이름으로 검색하기 위함
        
        switch (name)
        {
            case "Attack":
                Damage = playerStatus.ATK;
                break;
                
            case "Skill1":
                Damage = playerStatus.Skill1;
                break;
        }
    }
}
