using UnityEngine;
using UnityEngine.EventSystems;

public class Minimize : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private WindowState windowState;

    public void OnPointerClick(PointerEventData eventData)
    {
        windowState.Minimize();
    }
}
