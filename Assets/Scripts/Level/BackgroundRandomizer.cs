using System;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundRandomizer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private List<Sprite> _backgroundSprites = new();
    
    private void Start()
    {
        _spriteRenderer.sprite = _backgroundSprites[UnityEngine.Random.Range(0, _backgroundSprites.Count)];
    }
}
