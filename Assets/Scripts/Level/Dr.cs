using System;
using UnityEngine;

public class Dr : MonoBehaviour
{
    private MapGenerator _mapGenerator;
    
    private Collider2D _collider;
    
    private bool _isPickedUp = false;
    private Transform _target;

    private void Awake()
    {
        _mapGenerator = FindObjectOfType<MapGenerator>();
        if(_mapGenerator == null) Debug.LogError("Map generator not found");
        
        _collider = GetComponent<Collider2D>();
    }

    private void LateUpdate()
    {
        if (!_isPickedUp)
            return;
        
        transform.position = _target.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isPickedUp)
            return;

        if (!other.TryGetComponent(out PlayerController player))
            return;
        
        _isPickedUp = true;
        _target = other.transform;
        _collider.enabled = false;
        
        _mapGenerator.OpenWinCondition();
    }
}
