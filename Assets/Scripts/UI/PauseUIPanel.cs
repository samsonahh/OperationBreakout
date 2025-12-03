using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using UnityEngine;

public class PauseUIPanel : UIPanel
{
    private protected override void Initialize()
    {

    }

    public override void CloseUI()
    {
        GameManager.Instance.ChangeGameState(GameState.Gameplay);
    }

    private void OnEnable()
    {
        InputManager.Instance.UnPause += InputManager_UnPause;
    }

    private void OnDisable()
    {
        InputManager.Instance.UnPause -= InputManager_UnPause;
    }
    
    private void InputManager_UnPause()
    {
        Debug.Log("UnPause");   
        CloseUI();
    }

    public void ReturnToMenu() => GameManager.Instance.ReturnToMenu();
    
    public void OpenSettings() => UIManager.Instance.ShowPanel(UIManager.PanelName.Settings);
}