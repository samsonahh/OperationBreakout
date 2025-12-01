using System;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.ChangeGameState(GameState.Title);
    }

    public void StartGame()
    {
        GameManager.Instance.StartGame();
    }
    
    public void OpenSettings() => UIManager.Instance.ShowPanel(UIManager.PanelName.Settings);
}
