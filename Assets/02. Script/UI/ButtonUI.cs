using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonUI : MonoBehaviour
{
    private Button _button;
    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClickSound);
    }

    private void OnClickSound()
    {
        AudioManager._instance.SfxPlay("Button");
    }
}
