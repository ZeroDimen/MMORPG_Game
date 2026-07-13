using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;

public class UIInputHandler : MonoBehaviour, IPunOwnershipCallbacks
{
    private PlayerInput _playerInput;
    private PhotonView _photonView;
    private bool _subscribed = false;

    private bool IsBlocked => GameManager.Instance.GameState == EGameState.TextInput;
    
    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _photonView = GetComponent<PhotonView>();
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void Start()
    {
        TrySubscribe();
    }

    // IPunOwnershipCallbacks 인터페이스 구현
    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer) { }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (targetView == _photonView)
            TrySubscribe();
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest) { }

    private void TrySubscribe()
    {
        if (_subscribed) return;
        if (!_photonView.IsMine) return;

        _playerInput.actions["Inventory"].performed += OnInventory;
        _playerInput.actions["Quest"].performed += OnQeust;
        _playerInput.actions["Party"].performed += OnParty;
        _playerInput.actions["Menu"].performed += OnMenu;
        _subscribed = true;
    }

    private void OnDestroy()
    {
        if (!_subscribed) return;

        _playerInput.actions["Inventory"].performed -= OnInventory;
        _playerInput.actions["Quest"].performed -= OnQeust;
        _playerInput.actions["Party"].performed -= OnParty;
        _playerInput.actions["Menu"].performed -= OnMenu;
    }

    private void OnInventory(InputAction.CallbackContext context)
    {
        if (IsBlocked) return;
        UIManager.Instance.OnInventoryPanel();
    }
    private void OnQeust(InputAction.CallbackContext context)
    {
        if (IsBlocked) return;
        UIManager.Instance.OnQuestPanel();
    }

    private void OnParty(InputAction.CallbackContext context)
    {
        if (IsBlocked) return;
        UIManager.Instance.OnPartySearchPanel();
    }

    private void OnMenu(InputAction.CallbackContext context)
    {
        if (IsBlocked) return;
        UIManager.Instance.OnMenuPanel();
    }
}
