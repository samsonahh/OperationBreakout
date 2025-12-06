using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class HitNumber : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text _text;

    [Header("Anim Config")] 
    [SerializeField] private float _floatUpDuration = 0.5f;
    [SerializeField] private float _floatUpDistance = 1f;
    [SerializeField] private Ease _floatUpTween = Ease.Linear;
    [SerializeField] private float _fadeAwayDuration = 0.5f;
    [SerializeField] private Ease _fadeAwayTween = Ease.Linear;
    
    public void Init(float damageNumber)
    {
        _text.text = $"{Mathf.RoundToInt(damageNumber)}";
        FadeUpAndAway().Forget();
    }

    private async UniTask FadeUpAndAway()
    {
        // float up first
        await transform.DOMoveY(
            transform.position.y + _floatUpDistance,
            _floatUpDuration
        ).SetEase(_floatUpTween);
        
        await _text.DOFade(0, _fadeAwayDuration).SetEase(_fadeAwayTween);
        
        Destroy(gameObject);
    }
    
    [Button("Test")]
    public void Test()
    {
        Init(100f);
    }
}
