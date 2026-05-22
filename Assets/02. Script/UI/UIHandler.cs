using UnityEngine;
using UnityEngine.InputSystem;

public class UIInputHandler : MonoBehaviour
{
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        _playerInput.actions["Inventory"].performed += OnInventory;
        _playerInput.actions["Quest"].performed += OnQeust;
        _playerInput.actions["Party"].performed += OnParty;
        _playerInput.actions["Menu"].performed += OnMenu;
    }

    private void OnDisable()
    {
        _playerInput.actions["Inventory"].performed -= OnInventory;
        _playerInput.actions["Quest"].performed -= OnQeust;
        _playerInput.actions["Party"].performed -= OnParty;
        _playerInput.actions["Menu"].performed -= OnMenu;
    }

    private void OnInventory(InputAction.CallbackContext context)
    {
        UIManager.Instance.OnInventoryPanel();
    }

    private void OnQeust(InputAction.CallbackContext context)
    {
        UIManager.Instance.OnQuestPanel();
    }

    private void OnParty(InputAction.CallbackContext context)
    {
        UIManager.Instance.OnPartySearchPanel();
    }

    private void OnMenu(InputAction.CallbackContext context)
    {
        UIManager.Instance.OnMenuPanel();
    }
}
