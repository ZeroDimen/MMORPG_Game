using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dim : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject target;
    [SerializeField] private bool isActiveSelf;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        target.gameObject.SetActive(false);
        if(isActiveSelf)
            gameObject.SetActive(false);
    }
}
