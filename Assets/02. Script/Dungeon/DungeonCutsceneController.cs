using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class DungeonCutsceneController : MonoBehaviour
{
    public static DungeonCutsceneController instance;

    private PhotonView _pv;
    private float fogFadeDuration = 10f;
    public PlayableDirector timeline;
    private bool hasPlayed = false;
    private bool hasStart = false;
    private bool hasEnd = false;

    private List<GameObject> _monsters = new List<GameObject>();
    public Transform[] spawnPos;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _pv = GetComponent<PhotonView>();
    }

    public void OnFogFadeOut()
    {
        StartCoroutine(FogFadeOut());
    }

    public void OnCutsceneStart()
    {
        if (GameManager.LocalPlayer == null)
        {
            Debug.LogError("LocalPlayer가 null입니다!");
            return;
        }

        _pv.RPC(nameof(StartCutScene), RpcTarget.MasterClient);

        PlayerInput playerInput = GameManager.LocalPlayer.GetComponent<PlayerInput>();
        if (playerInput != null)
            playerInput.enabled = false;
        RenderSettings.fogDensity = 0.1f;
    }

    public void OnCutsceneEnd()
    {
        _pv.RPC(nameof(EndCutScene), RpcTarget.MasterClient);
        PlayerInput playerInput = GameManager.LocalPlayer.GetComponent<PlayerInput>();
        if (playerInput != null)
            playerInput.enabled = true;
    }

    private IEnumerator FogFadeOut()
    {
        float startDensity = RenderSettings.fogDensity;
        float elapsed = 0f;

        while (elapsed < fogFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fogFadeDuration;
            RenderSettings.fogDensity = Mathf.Lerp(startDensity, 0.02f, t);
            yield return null;
        }
    }

    public void PlayTimeline()
    {
        timeline.Play();
    }

    [PunRPC]
    public void StartCutScene()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (hasStart) return;   

        Debug.Log("StartCutScene");
        hasStart = true;
        for (int i = 0; i < DungeonSystem.MonsterNum; i++)
        {
            var obj = PhotonNetwork.Instantiate("Mutant", spawnPos[i].position, Quaternion.identity);
            _monsters.Add(obj);
            obj.GetComponent<Animator>().enabled = false;
            obj.GetComponent<NavMeshAgent>().enabled = false;
        }
    }

    [PunRPC]
    public void EndCutScene()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (hasEnd) return;

        Debug.Log("EndCutScene");
        hasEnd = true;
        foreach (var monster in _monsters)
        {
            monster.GetComponent<Animator>().enabled = true;
            monster.GetComponent<NavMeshAgent>().enabled = true;
        }
    }

    [PunRPC]
    public void RequestPlayTimeline(PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (!hasPlayed)
        {
            Debug.Log(info.Sender.NickName);
            Debug.Log("RequestPlayTimeline");
            hasPlayed = true; // 한 번만 재생
            DungeonSystem.instance.RequestTimeline(info.Sender.NickName);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !PhotonNetwork.IsMasterClient)
        {
            _pv.RPC(nameof(RequestPlayTimeline), RpcTarget.MasterClient);
        }
    }
}