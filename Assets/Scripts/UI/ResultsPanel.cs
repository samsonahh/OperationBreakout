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
    }

    private void OnEnable()
    {
        // play some start anim
    }

    private void OnDestroy()
    {
        _levelManager.OnTimerDepleted -= LevelManager_OnTimerDepleted;
    }

    private void LevelManager_OnTimerDepleted()
    {
        gameObject.SetActive(true);
        _titleText.text = "You Lose!";
    }

    public void Restart() => GameManager.Instance.ReloadScene(GameState.Gameplay);

    public void ReturnToMenu() => GameManager.Instance.ReturnToMenu();
}
