using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using UnityEngine;

public class PauseUIPanel : UIPanel
{
    [SerializeField] private SceneReference _menuScene;
    
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

    public void ReturnToMenu() => GameManager.Instance.SwitchScenes(_menuScene, GameState.Title).Forget();
    
    public void OpenSettings() => UIManager.Instance.ShowPanel(UIManager.PanelName.Settings);
}