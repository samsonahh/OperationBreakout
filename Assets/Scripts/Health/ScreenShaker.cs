using System;
using NaughtyAttributes;
using UnityEngine;

public class ScreenShaker : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float _shakeDuration = 0.3f;
    [SerializeField] private float _shakeAmplitude = 1f;
    [SerializeField] private float _shakeFrequency = 1f;

    [Button("Shake")]
    public void Shake()
    {
        CameraManager.Instance.CameraShaker.ShakeCamera(_shakeAmplitude, _shakeFrequency, _shakeDuration);
    }
}
