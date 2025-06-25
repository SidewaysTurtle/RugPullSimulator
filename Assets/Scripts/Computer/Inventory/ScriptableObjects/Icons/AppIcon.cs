using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Types of application windows that can be created
/// </summary>
public enum WindowType
{
    TextContent,
    WebBrowser,
    CLI,
    ControlPanel,
    Custom
}

/// <summary>
/// Categories for control panel windows
/// </summary>
public enum ControlPanelCategory
{
    System,
    Network,
    Security,
    UserAccounts,
    Hardware,
    Software,
    Display,
    Audio
}

[CreateAssetMenu(menuName = "Icons/App", fileName = "AppName.asset")]
[System.Serializable]
public class AppIcon : Icon
{
    [Header("Application Settings")]
    [Tooltip("The title that will appear in the window title bar")]
    public string applicationTitle = "Application";
    
    [Tooltip("Type of window to create")]
    public WindowType windowType = WindowType.TextContent;
    
    [Header("Window Configuration")]
    [Tooltip("Initial size of the application window")]
    public Vector2 windowSize = new Vector2(800, 600);
    
    [Tooltip("Minimum size the window can be resized to")]
    public Vector2 minWindowSize = new Vector2(400, 300);
    
    [Tooltip("Whether the window can be resized by the user")]
    public bool isResizable = true;
    
    [Tooltip("Whether the window can be minimized")]
    public bool canMinimize = true;
    
    [Tooltip("Whether the window can be maximized")]
    public bool canMaximize = true;
    
    [Tooltip("Whether the window can be closed")]
    public bool canClose = true;
    
    [Header("Appearance")]
    [Tooltip("Background color of the application window")]
    public Color backgroundColor = Color.white;
    
    [Tooltip("Icon to show in the window title bar")]
    public Sprite titleBarIcon;
    
    [ConditionalHiddenHeader("Text Content Settings", "windowType", 0)]
    [Tooltip("Text content to display in the application")]
    [TextArea(3, 10)]
    [ConditionalHide("windowType", 0, ConditionalDisplayMode.Hide)]
    public string contentText = "";
    
    [ConditionalHide("windowType", 0, ConditionalDisplayMode.Hide)]
    [Tooltip("Content text color")]
    public Color textColor = Color.black;
    
    [ConditionalHide("windowType", 0, ConditionalDisplayMode.Hide)]
    [Tooltip("Content text size")]
    [Range(8, 72)]
    public int textSize = 14;
    
    [ConditionalHiddenHeader("Web Browser Settings", "windowType", 1)]
    [Tooltip("Starting URL for the web browser")]
    [ConditionalHide("windowType", 0, ConditionalDisplayMode.Hide)]
    public string startingURL = "https://www.google.com";
    
    [ConditionalHide("windowType", 1, ConditionalDisplayMode.Hide)]
    [Tooltip("Show navigation bar (back, forward, refresh, address bar)")]
    public bool showNavigationBar = true;
    
    [ConditionalHide("windowType", 1, ConditionalDisplayMode.Hide)]
    [Tooltip("Show bookmarks bar")]
    public bool showBookmarksBar = false;
    
    [ConditionalHide("windowType", 1, ConditionalDisplayMode.Hide)]
    [Tooltip("Allow JavaScript execution")]
    public bool allowJavaScript = true;
    
    [ConditionalHiddenHeader("CLI Settings", "windowType", 2)]
    [Tooltip("Starting directory for the CLI")]
    [ConditionalHide("windowType", 0, ConditionalDisplayMode.Hide)]
    public string startingDirectory = "C:\\";
    
    [ConditionalHide("windowType", 2, ConditionalDisplayMode.Hide)]
    [Tooltip("Command prompt prefix")]
    public string promptPrefix = "C:\\>";
    
    [ConditionalHide("windowType", 2, ConditionalDisplayMode.Hide)]
    [Tooltip("CLI background color")]
    public Color cliBackgroundColor = Color.black;
    
    [ConditionalHide("windowType", 2, ConditionalDisplayMode.Hide)]
    [Tooltip("CLI text color")]
    public Color cliTextColor = Color.green;
    
    [ConditionalHide("windowType", 2, ConditionalDisplayMode.Hide)]
    [Tooltip("CLI font size")]
    [Range(8, 24)]
    public int cliFontSize = 12;
    
    [ConditionalHiddenHeader("Control Panel Settings", "windowType", 3)]
    [Tooltip("Control panel category")]
    [ConditionalHide("windowType", 0, ConditionalDisplayMode.Hide)]
    public ControlPanelCategory panelCategory = ControlPanelCategory.System;
    
    [ConditionalHide("windowType", 3, ConditionalDisplayMode.Hide)]
    [Tooltip("Show advanced options")]
    public bool showAdvancedOptions = false;
    
    [ConditionalHiddenHeader("Custom Content Settings", "windowType", 4)]
    [Tooltip("Custom prefab to instantiate as window content")]
    [ConditionalHide("windowType", 0, ConditionalDisplayMode.Hide)]
    public GameObject customContentPrefab;

    private void OnEnable()
    {
        // Subscribe to double-click events when the ScriptableObject is enabled
        IconInputManager.OnDoubleClickEvent += HandleDoubleClick;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        IconInputManager.OnDoubleClickEvent -= HandleDoubleClick;
    }

    /// <summary>
    /// Handles the double-click event to open the application window
    /// </summary>
    /// <param name="clickedIcon">The desktop icon that was double-clicked</param>
    private void HandleDoubleClick(DesktopIcon clickedIcon)
    {
        // Check if this is the icon that was double-clicked
        if (clickedIcon != null && clickedIcon.GetCurrentIcon() == this)
        {
            OpenApplication();
        }
    }

    /// <summary>
    /// Opens the application window with the configured settings
    /// </summary>
    public void OpenApplication()
    {
        CreateApplicationWindow();
    }

    /// <summary>
    /// Creates a new application window using primitives (similar to drag visual)
    /// </summary>
    private void CreateApplicationWindow()
    {
        // Find the desktop canvas to parent the window to
        Canvas desktopCanvas = FindObjectOfType<Canvas>();
        if (desktopCanvas == null)
        {
            Debug.LogError("No Canvas found to create application window!");
            return;
        }

        // Create the main window container
        GameObject windowContainer = new GameObject($"Window_{applicationTitle}");
        windowContainer.transform.SetParent(desktopCanvas.transform, false);
        
        RectTransform windowRect = windowContainer.AddComponent<RectTransform>();
        windowRect.sizeDelta = windowSize;
        windowRect.anchoredPosition = Vector2.zero; // Center on screen
        
        // Add window background
        Image windowBackground = windowContainer.AddComponent<Image>();
        windowBackground.color = new Color(0.9f, 0.9f, 0.9f, 1f); // Light gray window frame
        
        // Add shadow/border effect
        Shadow windowShadow = windowContainer.AddComponent<Shadow>();
        windowShadow.effectColor = new Color(0, 0, 0, 0.5f);
        windowShadow.effectDistance = new Vector2(5, -5);
        
        // Create title bar
        CreateTitleBar(windowContainer);
        
        // Create content area
        CreateContentArea(windowContainer);
        
        // Make window draggable
        MakeWindowDraggable(windowContainer);
        
        Debug.Log($"Opened application: {applicationTitle}");
    }

    /// <summary>
    /// Creates the window title bar with controls
    /// </summary>
    private void CreateTitleBar(GameObject windowContainer)
    {
        // Create title bar container
        GameObject titleBar = new GameObject("TitleBar");
        titleBar.transform.SetParent(windowContainer.transform, false);
        
        RectTransform titleBarRect = titleBar.AddComponent<RectTransform>();
        titleBarRect.anchorMin = new Vector2(0, 1);
        titleBarRect.anchorMax = new Vector2(1, 1);
        titleBarRect.pivot = new Vector2(0.5f, 1);
        titleBarRect.sizeDelta = new Vector2(0, 30);
        titleBarRect.anchoredPosition = Vector2.zero;
        
        // Title bar background
        Image titleBarBg = titleBar.AddComponent<Image>();
        titleBarBg.color = new Color(0.2f, 0.2f, 0.2f, 1f); // Dark title bar
        
        // Create title text
        GameObject titleTextObj = new GameObject("TitleText");
        titleTextObj.transform.SetParent(titleBar.transform, false);
        
        RectTransform titleTextRect = titleTextObj.AddComponent<RectTransform>();
        titleTextRect.anchorMin = new Vector2(0, 0);
        titleTextRect.anchorMax = new Vector2(1, 1);
        titleTextRect.offsetMin = new Vector2(10, 0);
        titleTextRect.offsetMax = new Vector2(-100, 0); // Leave space for buttons
        
        TextMeshProUGUI titleText = titleTextObj.AddComponent<TextMeshProUGUI>();
        titleText.text = applicationTitle;
        titleText.color = Color.white;
        titleText.fontSize = 14;
        titleText.alignment = TextAlignmentOptions.MidlineLeft;
        
        // Create window control buttons
        CreateWindowControls(titleBar);
    }

    /// <summary>
    /// Creates window control buttons (minimize, maximize, close)
    /// </summary>
    private void CreateWindowControls(GameObject titleBar)
    {
        float buttonWidth = 30f;
        float buttonSpacing = 5f;
        float rightOffset = 10f;
        
        // Close button
        if (canClose)
        {
            CreateControlButton(titleBar, "×", Color.red, 
                new Vector2(-rightOffset, 0), 
                () => CloseWindow(titleBar.transform.parent.gameObject));
            rightOffset += buttonWidth + buttonSpacing;
        }
        
        // Maximize button
        if (canMaximize)
        {
            CreateControlButton(titleBar, "□", Color.green, 
                new Vector2(-rightOffset, 0), 
                () => MaximizeWindow(titleBar.transform.parent.gameObject));
            rightOffset += buttonWidth + buttonSpacing;
        }
        
        // Minimize button
        if (canMinimize)
        {
            CreateControlButton(titleBar, "−", Color.yellow, 
                new Vector2(-rightOffset, 0), 
                () => MinimizeWindow(titleBar.transform.parent.gameObject));
        }
    }

    /// <summary>
    /// Creates a single window control button
    /// </summary>
    private void CreateControlButton(GameObject parent, string symbol, Color color, Vector2 position, System.Action onClick)
    {
        GameObject button = new GameObject($"Button_{symbol}");
        button.transform.SetParent(parent.transform, false);
        
        RectTransform buttonRect = button.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(1, 0.5f);
        buttonRect.anchorMax = new Vector2(1, 0.5f);
        buttonRect.pivot = new Vector2(1, 0.5f);
        buttonRect.sizeDelta = new Vector2(25, 20);
        buttonRect.anchoredPosition = position;
        
        // Button background
        Image buttonImage = button.AddComponent<Image>();
        buttonImage.color = color;
        
        // Button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(button.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = symbol;
        buttonText.color = Color.white;
        buttonText.fontSize = 12;
        buttonText.alignment = TextAlignmentOptions.Center;
        
        // Add button functionality
        Button buttonComponent = button.AddComponent<Button>();
        buttonComponent.targetGraphic = buttonImage;
        buttonComponent.onClick.AddListener(() => onClick?.Invoke());
    }

    /// <summary>
    /// Creates the main content area of the window
    /// </summary>
    private void CreateContentArea(GameObject windowContainer)
    {
        GameObject contentArea = new GameObject("ContentArea");
        contentArea.transform.SetParent(windowContainer.transform, false);
        
        RectTransform contentRect = contentArea.AddComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = new Vector2(0, -30); // Account for title bar
        
        // Content background - use specific colors for different window types
        Image contentBg = contentArea.AddComponent<Image>();
        
        // Create content based on window type
        switch (windowType)
        {
            case WindowType.TextContent:
                contentBg.color = backgroundColor;
                CreateTextContent(contentArea);
                break;
                
            case WindowType.WebBrowser:
                contentBg.color = Color.white;
                CreateWebBrowserContent(contentArea);
                break;
                
            case WindowType.CLI:
                contentBg.color = cliBackgroundColor;
                CreateCLIContent(contentArea);
                break;
                
            case WindowType.ControlPanel:
                contentBg.color = backgroundColor;
                CreateControlPanelContent(contentArea);
                break;
                
            case WindowType.Custom:
                contentBg.color = backgroundColor;
                CreateCustomContent(contentArea);
                break;
                
            default:
                contentBg.color = backgroundColor;
                CreateTextContent(contentArea);
                break;
        }
    }

    /// <summary>
    /// Creates simple text content for the window
    /// </summary>
    private void CreateTextContent(GameObject contentArea)
    {
        if (string.IsNullOrEmpty(contentText)) return;
        
        GameObject textContent = new GameObject("TextContent");
        textContent.transform.SetParent(contentArea.transform, false);
        
        RectTransform textRect = textContent.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 10);
        textRect.offsetMax = new Vector2(-10, -10);
        
        TextMeshProUGUI contentTextComponent = textContent.AddComponent<TextMeshProUGUI>();
        contentTextComponent.text = contentText;
        contentTextComponent.color = textColor;
        contentTextComponent.fontSize = textSize;
        contentTextComponent.alignment = TextAlignmentOptions.TopLeft;
        contentTextComponent.enableWordWrapping = true;
    }
    
    /// <summary>
    /// Creates web browser content with navigation bar
    /// </summary>
    private void CreateWebBrowserContent(GameObject contentArea)
    {
        float navBarHeight = showNavigationBar ? 40f : 0f;
        float bookmarkHeight = showBookmarksBar ? 30f : 0f;
        
        // Create navigation bar if enabled
        if (showNavigationBar)
        {
            CreateBrowserNavigationBar(contentArea, navBarHeight);
        }
        
        // Create bookmarks bar if enabled
        if (showBookmarksBar)
        {
            CreateBrowserBookmarksBar(contentArea, navBarHeight, bookmarkHeight);
        }
        
        // Create web content area
        GameObject webContent = new GameObject("WebContent");
        webContent.transform.SetParent(contentArea.transform, false);
        
        RectTransform webRect = webContent.AddComponent<RectTransform>();
        webRect.anchorMin = Vector2.zero;
        webRect.anchorMax = Vector2.one;
        webRect.offsetMin = new Vector2(0, 0);
        webRect.offsetMax = new Vector2(0, -(navBarHeight + bookmarkHeight));
        
        // Placeholder for actual web content
        Image webBg = webContent.AddComponent<Image>();
        webBg.color = Color.white;
        
        // Add placeholder text showing the URL
        GameObject urlDisplay = new GameObject("URLDisplay");
        urlDisplay.transform.SetParent(webContent.transform, false);
        
        RectTransform urlRect = urlDisplay.AddComponent<RectTransform>();
        urlRect.anchorMin = new Vector2(0.5f, 0.5f);
        urlRect.anchorMax = new Vector2(0.5f, 0.5f);
        urlRect.sizeDelta = new Vector2(400, 100);
        
        TextMeshProUGUI urlText = urlDisplay.AddComponent<TextMeshProUGUI>();
        urlText.text = $"Loading: {startingURL}\n\n[Web browser simulation]";
        urlText.color = Color.gray;
        urlText.fontSize = 16;
        urlText.alignment = TextAlignmentOptions.Center;
    }
    
    /// <summary>
    /// Creates CLI content with terminal interface
    /// </summary>
    private void CreateCLIContent(GameObject contentArea)
    {
        GameObject cliContent = new GameObject("CLIContent");
        cliContent.transform.SetParent(contentArea.transform, false);
        
        RectTransform cliRect = cliContent.AddComponent<RectTransform>();
        cliRect.anchorMin = Vector2.zero;
        cliRect.anchorMax = Vector2.one;
        cliRect.offsetMin = new Vector2(10, 10);
        cliRect.offsetMax = new Vector2(-10, -10);
        
        TextMeshProUGUI cliText = cliContent.AddComponent<TextMeshProUGUI>();
        cliText.text = $"{promptPrefix} {startingDirectory}\n{promptPrefix} echo \"Terminal ready\"\nTerminal ready\n{promptPrefix} _";
        cliText.color = cliTextColor;
        cliText.fontSize = cliFontSize;
        cliText.alignment = TextAlignmentOptions.TopLeft;
        cliText.enableWordWrapping = true;
        cliText.fontStyle = FontStyles.Bold;
    }
    
    /// <summary>
    /// Creates control panel content based on category
    /// </summary>
    private void CreateControlPanelContent(GameObject contentArea)
    {
        GameObject panelContent = new GameObject("ControlPanelContent");
        panelContent.transform.SetParent(contentArea.transform, false);
        
        RectTransform panelRect = panelContent.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = new Vector2(20, 20);
        panelRect.offsetMax = new Vector2(-20, -20);
        
        TextMeshProUGUI panelText = panelContent.AddComponent<TextMeshProUGUI>();
        panelText.text = GetControlPanelContent();
        panelText.color = textColor;
        panelText.fontSize = textSize;
        panelText.alignment = TextAlignmentOptions.TopLeft;
        panelText.enableWordWrapping = true;
    }
    
    /// <summary>
    /// Creates custom content from prefab
    /// </summary>
    private void CreateCustomContent(GameObject contentArea)
    {
        if (customContentPrefab != null)
        {
            GameObject customContent = Instantiate(customContentPrefab);
            customContent.transform.SetParent(contentArea.transform, false);
            
            // Ensure custom content fills the area
            RectTransform customRect = customContent.GetComponent<RectTransform>();
            if (customRect != null)
            {
                customRect.anchorMin = Vector2.zero;
                customRect.anchorMax = Vector2.one;
                customRect.offsetMin = Vector2.zero;
                customRect.offsetMax = Vector2.zero;
            }
        }
        else
        {
            // Fallback to text content if no prefab is assigned
            CreateTextContent(contentArea);
        }
    }
    
    /// <summary>
    /// Creates browser navigation bar
    /// </summary>
    private void CreateBrowserNavigationBar(GameObject contentArea, float height)
    {
        GameObject navBar = new GameObject("NavigationBar");
        navBar.transform.SetParent(contentArea.transform, false);
        
        RectTransform navRect = navBar.AddComponent<RectTransform>();
        navRect.anchorMin = new Vector2(0, 1);
        navRect.anchorMax = new Vector2(1, 1);
        navRect.pivot = new Vector2(0.5f, 1);
        navRect.sizeDelta = new Vector2(0, height);
        navRect.anchoredPosition = Vector2.zero;
        
        Image navBg = navBar.AddComponent<Image>();
        navBg.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        
        // Add address bar placeholder
        GameObject addressBar = new GameObject("AddressBar");
        addressBar.transform.SetParent(navBar.transform, false);
        
        RectTransform addressRect = addressBar.AddComponent<RectTransform>();
        addressRect.anchorMin = new Vector2(0.1f, 0.2f);
        addressRect.anchorMax = new Vector2(0.9f, 0.8f);
        addressRect.offsetMin = Vector2.zero;
        addressRect.offsetMax = Vector2.zero;
        
        Image addressBg = addressBar.AddComponent<Image>();
        addressBg.color = Color.white;
        
        GameObject addressText = new GameObject("AddressText");
        addressText.transform.SetParent(addressBar.transform, false);
        
        RectTransform addressTextRect = addressText.AddComponent<RectTransform>();
        addressTextRect.anchorMin = Vector2.zero;
        addressTextRect.anchorMax = Vector2.one;
        addressTextRect.offsetMin = new Vector2(10, 0);
        addressTextRect.offsetMax = new Vector2(-10, 0);
        
        TextMeshProUGUI addressTextComponent = addressText.AddComponent<TextMeshProUGUI>();
        addressTextComponent.text = startingURL;
        addressTextComponent.color = Color.black;
        addressTextComponent.fontSize = 12;
        addressTextComponent.alignment = TextAlignmentOptions.MidlineLeft;
    }
    
    /// <summary>
    /// Creates browser bookmarks bar
    /// </summary>
    private void CreateBrowserBookmarksBar(GameObject contentArea, float navBarHeight, float bookmarkHeight)
    {
        GameObject bookmarkBar = new GameObject("BookmarksBar");
        bookmarkBar.transform.SetParent(contentArea.transform, false);
        
        RectTransform bookmarkRect = bookmarkBar.AddComponent<RectTransform>();
        bookmarkRect.anchorMin = new Vector2(0, 1);
        bookmarkRect.anchorMax = new Vector2(1, 1);
        bookmarkRect.pivot = new Vector2(0.5f, 1);
        bookmarkRect.sizeDelta = new Vector2(0, bookmarkHeight);
        bookmarkRect.anchoredPosition = new Vector2(0, -navBarHeight);
        
        Image bookmarkBg = bookmarkBar.AddComponent<Image>();
        bookmarkBg.color = new Color(0.95f, 0.95f, 0.95f, 1f);
        
        // Add some sample bookmarks
        string[] bookmarks = { "Home", "News", "Social", "Work" };
        float bookmarkWidth = 80f;
        for (int i = 0; i < bookmarks.Length; i++)
        {
            CreateBookmarkButton(bookmarkBar, bookmarks[i], i * bookmarkWidth + 10);
        }
    }
    
    /// <summary>
    /// Creates a single bookmark button
    /// </summary>
    private void CreateBookmarkButton(GameObject parent, string name, float xPosition)
    {
        GameObject bookmark = new GameObject($"Bookmark_{name}");
        bookmark.transform.SetParent(parent.transform, false);
        
        RectTransform bookmarkRect = bookmark.AddComponent<RectTransform>();
        bookmarkRect.anchorMin = new Vector2(0, 0);
        bookmarkRect.anchorMax = new Vector2(0, 1);
        bookmarkRect.pivot = new Vector2(0, 0.5f);
        bookmarkRect.sizeDelta = new Vector2(70, 0);
        bookmarkRect.anchoredPosition = new Vector2(xPosition, 0);
        
        Button bookmarkButton = bookmark.AddComponent<Button>();
        Image bookmarkImage = bookmark.AddComponent<Image>();
        bookmarkImage.color = new Color(0.8f, 0.8f, 0.8f, 1f);
        bookmarkButton.targetGraphic = bookmarkImage;
        
        GameObject bookmarkText = new GameObject("Text");
        bookmarkText.transform.SetParent(bookmark.transform, false);
        
        RectTransform textRect = bookmarkText.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI textComponent = bookmarkText.AddComponent<TextMeshProUGUI>();
        textComponent.text = name;
        textComponent.color = Color.black;
        textComponent.fontSize = 10;
        textComponent.alignment = TextAlignmentOptions.Center;
    }
    
    /// <summary>
    /// Gets control panel content based on category
    /// </summary>
    private string GetControlPanelContent()
    {
        string content = $"Control Panel - {panelCategory}\n\n";
        
        switch (panelCategory)
        {
            case ControlPanelCategory.System:
                content += "System Information:\n• Computer Name: DESKTOP-PC\n• Processor: Intel Core i7\n• Memory: 16 GB RAM\n• System Type: 64-bit\n\nSystem Settings:\n• Performance Options\n• Environment Variables\n• System Protection";
                break;
            case ControlPanelCategory.Network:
                content += "Network Configuration:\n• Network Adapters\n• Internet Options\n• Windows Firewall\n• Network Discovery\n\nConnection Status:\n• Ethernet: Connected\n• Wi-Fi: Available\n• Bluetooth: Enabled";
                break;
            case ControlPanelCategory.Security:
                content += "Security Center:\n• Windows Defender\n• Firewall Status\n• Automatic Updates\n• User Account Control\n\nSecurity Options:\n• BitLocker Drive Encryption\n• Certificate Manager\n• Credential Manager";
                break;
            case ControlPanelCategory.UserAccounts:
                content += "User Accounts:\n• Change Account Type\n• Manage User Accounts\n• Create Password Reset Disk\n• Configure Family Safety\n\nCurrent User:\n• Administrator\n• Password Protected: Yes\n• Last Login: Today";
                break;
            case ControlPanelCategory.Hardware:
                content += "Hardware and Devices:\n• Device Manager\n• Printers and Faxes\n• Sound Settings\n• Display Settings\n\nInstalled Hardware:\n• Graphics Card: NVIDIA GTX\n• Audio Device: Realtek HD\n• Storage: 1TB SSD";
                break;
            default:
                content += "Control panel category settings and options would appear here.";
                break;
        }
        
        if (showAdvancedOptions)
        {
            content += "\n\n[Advanced Options Enabled]";
        }
        
        return content;
    }

    /// <summary>
    /// Makes the window draggable by its title bar
    /// </summary>
    private void MakeWindowDraggable(GameObject windowContainer)
    {
        // For now, we'll rely on the basic UI interaction system
        // The window can be moved by developers implementing a custom drag handler
        // or by using the existing Unity UI drag components
        Debug.Log("Window is created. Custom drag implementation can be added later.");
    }

    // Window control methods
    private void CloseWindow(GameObject window)
    {
        Destroy(window);
    }

    private void MinimizeWindow(GameObject window)
    {
        window.SetActive(false);
        // In a full implementation, you'd add this to a taskbar or similar
        Debug.Log($"Minimized window: {applicationTitle}");
    }

    private void MaximizeWindow(GameObject window)
    {
        RectTransform windowRect = window.GetComponent<RectTransform>();
        Canvas canvas = window.GetComponentInParent<Canvas>();
        
        if (windowRect != null && canvas != null)
        {
            // Toggle between maximized and normal size
            if (windowRect.sizeDelta == windowSize)
            {
                // Maximize
                windowRect.sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
                windowRect.anchoredPosition = Vector2.zero;
            }
            else
            {
                // Restore
                windowRect.sizeDelta = windowSize;
                windowRect.anchoredPosition = Vector2.zero;
            }
        }
    }
}