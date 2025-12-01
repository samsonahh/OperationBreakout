using System;

public class SettingsUIPanel : UIPanel
{
    private protected override void Initialize()
    {
        
    }

    private void OnEnable()
    {
        InputManager.Instance.UnPause += InputManager_UnPause;
    }

    private void OnDisable()
    {
        InputManager.Instance.UnPause -= InputManager_UnPause;
    }

    public override void CloseUI()
    {
        if (GameManager.Instance.CurrentGameState == GameState.Title)
        {
            UIManager.Instance.HideAllPanels();
            return;
        }
        
        UIManager.Instance.ShowPanel(UIManager.PanelName.PauseMenu);
    }
    
    private void InputManager_UnPause()
    {
        CloseUI();
    }
}