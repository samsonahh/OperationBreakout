using System;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Handles the level's score. Each enemy should give the player score.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [ShowInInspector, ReadOnly] public int Score {get; private set;}
    public event Action<int> OnScoreAdded = delegate { };
    
    /// <summary>
    /// Adds score to the score manager.
    /// </summary>
    public void AddScore(int score)
    {
        Score += score;
        OnScoreAdded?.Invoke(score);
    }

    [Button("Add 5000 score")]
    private void Add5000Score()
    {
        AddScore(5000);
    }
}
