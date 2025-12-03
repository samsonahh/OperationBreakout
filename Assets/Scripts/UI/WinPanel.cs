using System;
using UnityEngine;

public class WinPanel : MonoBehaviour, IInitializable
{
    public void Initialize()
    {
        // sub to some win event
    }

    private void OnEnable()
    {
        // play some start anim
    }

    private void OnDestroy()
    {
        // unsub from win event
    }

    public void Restart() => GameManager.Instance.ReloadScene(GameState.Gameplay);

    public void ReturnToMenu() => GameManager.Instance.ReturnToMenu();
}
