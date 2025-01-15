using UnityEngine;

public class WindowState : MonoBehaviour
{
    [SerializeField] private RectTransform windowRect;
    [SerializeField] private Vector2 normalSize = new Vector2(400, 300);
    [SerializeField] private Vector2 maximizedSize = new Vector2(800, 600);
    
    private bool isMaximized = false;

    public void ToggleSize()
    {
        isMaximized = !isMaximized;
        windowRect.sizeDelta = isMaximized ? maximizedSize : normalSize;
    }

    public void Minimize()
    {
        // Add minimize animation or state logic here
        gameObject.SetActive(false);
    }
}
