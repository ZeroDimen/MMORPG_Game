using System.Collections;
using Photon.Pun;
using UnityEngine;

public class InteractableDoor : MonoBehaviour
{
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float rotateSpeed = 3f;

    private bool _isOpen = false;   // 실제 상태 — 마스터가 소유
    private Quaternion _closedRot;
    private Quaternion _openRot;

    private PhotonView pv;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        _closedRot = transform.localRotation;
        _openRot = _closedRot * Quaternion.Euler(0f, openAngle, 0f);
    }

    // 플레이어(로컬)가 E를 눌렀을 때 호출 → 마스터에게 요청만 보냄
    public void Interact()
    {
        pv.RPC(nameof(RPC_RequestToggle), RpcTarget.MasterClient);
    }

    // 마스터에서만 실행 — 상태를 결정하고 전원에게 broadcast
    [PunRPC]
    private void RPC_RequestToggle()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        _isOpen = !_isOpen;
        pv.RPC(nameof(RPC_SetDoor), RpcTarget.All, _isOpen);
    }

    // 전원(마스터 포함)이 실제 회전 실행
    [PunRPC]
    private void RPC_SetDoor(bool open)
    {
        StopAllCoroutines();
        StartCoroutine(RotateDoor(open ? _openRot : _closedRot));
    }

    private IEnumerator RotateDoor(Quaternion target)
    {
        while (Quaternion.Angle(transform.localRotation, target) > 0.5f)
        {
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation, target, Time.deltaTime * rotateSpeed);
            yield return null;
        }
        transform.localRotation = target;
    }
}
