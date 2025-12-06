using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// The UIManager is a singleton that controls persistent UI like the pause and settings menu.
/// Gameplay specific UI has not been developed, so this class needs some work.
/// Right now, gameplay specific UI can live inside scenes and have no need to be persistent.
/// </summary>
public class UIManager : Singleton<UIManager>
{
    public enum PanelName
    {
        PauseMenu,
        Settings,
    }

    [Header("Panels")]
    [SerializeField, SerializedDictionary("Panel Name", "Panel")]
    private SerializedDictionary<PanelName, UIPanel> _panels = new SerializedDictionary<PanelName, UIPanel>();
    [field: SerializeField, ReadOnly] public UIPanel CurrentPanel { get; private set; }

    [field: Header("Loading")]
    [field: SerializeField] public LoadingPanel LoadingPanel { get; private set; }

    /// <summary>
    /// Action that is invoked when the current panel is changed.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>UIPanel newPanel</c>: The new panel that was switched to.</description></item>
    /// </list>
    /// </remarks>
    public event Action<UIPanel> OnPanelChanged = delegate { };
    /// <summary>
    /// Action that is invoked when UI under the UIManager is closed.
    /// </summary>
    public event Action OnUIClose = delegate { };

    private protected override void Awake()
    {
        base.Awake();

        InitializePanels();
    }
    
    private void InitializePanels()
    {
        foreach (UIPanel panel in _panels.Values)
            panel.Init(this);
    }

    /// <summary>
    /// Shows the specified panel, hiding the current one if it exists.
    /// </summary>
    /// <param name="panelName"></param>
    public void ShowPanel(PanelName panelName)
    {
        UIPanel panel = _panels[panelName];

        if (CurrentPanel == panel)
            return;

        if (CurrentPanel != null && CurrentPanel != panel)
            CurrentPanel.Hide();

        CurrentPanel = panel;
        CurrentPanel.Show();

        OnPanelChanged.Invoke(CurrentPanel);

        InputManager.Instance.EnableUIActions();
    }

    /// <summary>
    /// Essentially closes the UI by hiding all panels and resetting the current panel.
    /// </summary>
    public void HideAllPanels()
    {
        foreach (UIPanel panel in _panels.Values)
            panel.Hide();

        CurrentPanel = null;

        OnUIClose.Invoke();
    }
}