using Photon.Pun;
using UnityEngine;

// 플레이어가 적에게 데미지를 주는 스크립트
public class MariaDamageManager : MonoBehaviour
{
    public int Damage = 1;
    private PhotonView playerPV;
    [SerializeField] private MariaPlayerController mariaPlayerController;
    private PlayerStatus playerStatus;
    private SkillMold skillMold;
    private string ObjName = null;

    private void Start()
    {
        playerPV = gameObject.transform.parent.GetComponent<PhotonView>();
        playerStatus = mariaPlayerController.Status;
        
        ObjName = gameObject.name.Replace("[Collider] ", "");
        skillMold = mariaPlayerController.skillManager.GetSkillData(ObjName);
        
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
    

    private void DamageSetting(string skillName)
    {
        if (playerStatus == null)
        {
            return;
        }

        switch (skillName)
        {
            case "Attack":
                Damage = playerStatus.ATK;
                break;

            case "Fire Strike":
                Damage = skillMold.skillDamage;
                Debug.Log($"{skillName} : {Damage}");
                break;
            case "Water Spin":
                Damage = skillMold.skillDamage;
                Debug.Log($"{skillName} : {Damage}");
                break;
        }
    }

    private void SkillInstate()
    {
        
    }
}
