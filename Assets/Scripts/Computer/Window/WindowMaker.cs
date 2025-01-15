using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WindowMaker : MonoBehaviour
{
    public TextMeshProUGUI contentText;
    public TextMeshProUGUI titleText;
    public DragUI dragUI;
    public TMP_InputField TMP_inputField;
    public void CreateWindow(TextIcon textIcon){
        contentText.text = textIcon.FileData;
        titleText.text = $"{textIcon.name}.{textIcon.textType}";
    }
    
    public void setDragUI(Camera camera, Canvas canvas, RectTransform canvasRectTransform)
    {
        dragUI._Camera = camera;
        dragUI._Canvas = canvas;
        dragUI._CanvasRectTransform = canvasRectTransform;
    }
}
