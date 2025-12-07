using Eflatun.SceneReference;
using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public enum GameState
    {
        Title,
        Loading,
        Gameplay,
        Paused,
        Results,
    }

public class GameManager : Singleton<GameManager>
{
    [Header("Editor Bootstrap States")] 
    [SerializeField, SerializedDictionary("Scene", "State")]
    private SerializedDictionary<SceneReference, GameState> _startingStates = new();
    
    [field: SerializeField, ReadOnly] public GameState CurrentGameState { get; private set; }

    /// <summary>
    /// Action that is invoked when the game state is changed.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>GameState newState</c>: The state that was changed to.</description></item>
    /// </list>
    /// </remarks>
    public event Action<GameState> OnGameStateChanged = delegate { };

    [Header("References")] [SerializeField]
    private SceneReference _titleScene;
    
    [SerializeField] private SceneReference _mainScene;
    
    public float DefaultFixedDeltaTime { get; private set; }
    private float _previousTimeScale = 1f;
    
    private float _impactFramesTimeScale;
    private float _impactFramesDuration;
    private float _impactFramesRemainingTime;
    private Coroutine _impactFramesCoroutine;

    private protected override void Awake()
    {
        base.Awake();
        
#if UNITY_EDITOR
        Dictionary<string, GameState> startingStates = new();
        foreach (var kvp in _startingStates)
            startingStates.Add(kvp.Key.Name, kvp.Value);
        if(startingStates.TryGetValue(GetCurrentScene().Name, out GameState targetState))
            ChangeGameState(targetState, true);
        else
            ChangeGameState(GameState.Gameplay, true);
#endif
    }

    private void Start()
    {
        DefaultFixedDeltaTime = Time.fixedDeltaTime;
    }

    /// <summary>
    /// Changes the current game state to the specified new state.
    /// Will not change if the new state is the same as the current state unless 'force' is true.
    /// </summary>
    /// <param name="newState"></param>
    /// <param name="force">Whether to rechange to the same state.</param>
    public void ChangeGameState(GameState newState, bool force = false)
    {
        if (CurrentGameState == newState && !force)
            return;

        CurrentGameState = newState;
        OnGameStateEnter(newState);

        OnGameStateChanged.Invoke(newState);
    }

    /// <summary>
    /// Called when after the game state has changed.
    /// Handle any UI updates or game logic that should occur when entering a new game state.
    /// </summary>
    /// <param name="newState"></param>
    private void OnGameStateEnter(GameState newState)
    {
        switch (newState)
        {
            case GameState.Title:
                SetTimeScale(1f);
                UIManager.Instance.HideAllPanels();
                InputManager.Instance.EnableUIActions();
                break;
            case GameState.Loading:
                SetTimeScale(0f);
                InputManager.Instance.DisableAllActions();
                break;
            case GameState.Gameplay:
                SetTimeScale(1f);
                UIManager.Instance.HideAllPanels();
                InputManager.Instance.EnablePlayerActions();
                break;
            case GameState.Paused:
                SetTimeScale(0f, true);
                UIManager.Instance.ShowPanel(UIManager.PanelName.PauseMenu);
                break;
            case GameState.Results:
                SetTimeScale(0f);
                InputManager.Instance.EnableUIActions();
                break;
        }
    }

    /// <summary>
    /// Asynchronously switches to the specified scene and changes the game state afterwards.
    /// While waiting for the scene to load, the game state is set to Loading.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="afterState"></param>
    /// <returns></returns>
    public async UniTask SwitchScenes(SceneReference scene, GameState afterState)
    {
        ChangeGameState(GameState.Loading);
        await UIManager.Instance.LoadingPanel.FadeIn();

        await SceneManager.LoadSceneAsync(scene.Name, LoadSceneMode.Single);
        
        ChangeGameState(afterState);

        await UIManager.Instance.LoadingPanel.FadeOut();
    }

    /// <summary>
    /// TODO in the future because of loading from saved point.
    /// </summary>
    public void StartGame()
    {
        SwitchScenes(_mainScene, GameState.Gameplay).Forget();
    }

    /// <summary>
    /// Reloads the scene by switching to the current scene and changing the game state afterwards.
    /// </summary>
    /// <param name="afterState"></param>
    public void ReloadScene(GameState afterState) => SwitchScenes(GetCurrentScene(), afterState).Forget();

    /// <summary>
    /// Helper method to switch back to the title scene and set the game state to Title.
    /// </summary>
    public void ReturnToMenu() => SwitchScenes(_titleScene, GameState.Title).Forget();

    public SceneReference GetCurrentScene() => SceneReference.FromScenePath(SceneManager.GetActiveScene().path);

    /// <summary>
    /// Quits the game properly based on the platform.
    /// </summary>
    public static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
    
    /// <summary>
    /// Sets the timescale of the game.
    /// You can specify whether to save the previous timescale value.
    /// </summary>
    /// <param name="timeScale">The new timescale value.</param>
    /// <param name="trackPrevious">Whether to save the previous timescale value.</param>
    public void SetTimeScale(float timeScale, bool trackPrevious = false)
    {
        _previousTimeScale = trackPrevious ? Time.timeScale : 1f;
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = DefaultFixedDeltaTime * timeScale;
    }
    
    /// <summary>
    /// Starts the impact frames with the specified timescale and duration.
    /// </summary>
    /// <param name="timeScale">The timescale of the impact frames.</param>
    /// <param name="duration">The duration of the impact frames.</param>
    public void StartImpactFrames(float timeScale, float duration)
    {
        if (duration <= 0) return;

        if(_impactFramesCoroutine != null)
        {
            _impactFramesRemainingTime = Mathf.Max(_impactFramesRemainingTime, duration);
            return;
        }
        
        _impactFramesRemainingTime = duration;
        _impactFramesCoroutine = StartCoroutine(ImpactFramesCoroutine(timeScale));
    }
    
    /// <summary>
    /// Coroutine that handles the impact frames of the weapon.
    /// </summary>
    /// <param name="timeScale">The timescale of the impact frames.</param>
    private IEnumerator ImpactFramesCoroutine(float timeScale)
    {
        SetTimeScale(timeScale);

        while (_impactFramesRemainingTime > 0f)
        {
            if (CurrentGameState == GameState.Gameplay)
                _impactFramesRemainingTime -= Time.unscaledDeltaTime; // only increment if playing

            yield return null;
        }
        _impactFramesRemainingTime = 0f;

        SetTimeScale(1);
        _impactFramesCoroutine = null;
    }
}