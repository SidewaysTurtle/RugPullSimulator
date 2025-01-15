using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Maximize : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private WindowState windowState;
    [SerializeField] private GameObject window;
    [SerializeField] private DragUI drag;
    [SerializeField] private TextMeshProUGUI filename;

    public void OnPointerClick(PointerEventData eventData)
    {
        windowState.ToggleSize();
    }
}
