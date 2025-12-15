using System;
using TMPro;
using UnityEngine;

public class ResultsPanel : MonoBehaviour, IInitializable
{
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private TMP_Text _titleText;
    
    public void Initialize()
    {
        _levelManager.OnTimerDepleted += LevelManager_OnTimerDepleted;
        _levelManager.OnWin += LevelManager_OnWin;
    }

    private void OnEnable()
    {
        // play some start anim
    }

    private void OnDestroy()
    {
        _levelManager.OnTimerDepleted -= LevelManager_OnTimerDepleted;
        _levelManager.OnWin -= LevelManager_OnWin;
    }

    private void LevelManager_OnTimerDepleted()
    {
        gameObject.SetActive(true);
        _titleText.text = "You Lose!";
    }

    private void LevelManager_OnWin()
    {
        gameObject.SetActive(true);
        _titleText.text = "You Win!";
    }

    public void Restart() => GameManager.Instance.ReloadScene(GameState.Gameplay);

    public void ReturnToMenu() => GameManager.Instance.ReturnToMenu();
}
