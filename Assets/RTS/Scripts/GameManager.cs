using TMPro;
using UnityEngine;


public enum GameState {
    MainMenu,
    Paused,
    Playing,
    GameOver,
}

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public delegate void GameStateChangedHandler(GameState state);
    public static event GameStateChangedHandler GameStateChanged;

    public delegate void GameBeganHandler();
    public static event GameBeganHandler GameBegan;

    public Transform PlayerStartPoint;
    public Transform EnemyStartPoint;

    public Canvas PauseCanvas;
    public Canvas GameOverCanvas;
    public Canvas MainMenuCanvas;



    private GameState State;

    private Player player;
    private Player enemy;

    void Start()
    {
        // load resource deposits
        ResourceDepositManager.Instance.LoadResourceDeposits();

        // load trainable units
        UnitManager.Instance.LoadTrainableUnits();

        // load placeable structures and their previews
        StructureManager.Instance.LoadPlaceableStructures();
        StructureManager.Instance.SetOwnerPlacementAreas(PlayerStartPoint, EnemyStartPoint);

        // initialize players
        player = new(ObjectOwner.Player, PlayerStartPoint);
        enemy = new(ObjectOwner.Enemy, EnemyStartPoint);

        SetGameState(GameState.MainMenu);
    }

    private void SetGameState(GameState newState)
    {
        switch(newState)
        {
            case GameState.Paused:
                HandlePauseState();
                break;
            case GameState.Playing:
                HandlePlayingState();
                break;
            case GameState.GameOver:
                HandleGameOverState();
                break;
            case GameState.MainMenu:
                HandleMainMenuState();
                break;
        }

        GameStateChanged?.Invoke(State);
    }


    private void HandlePauseState()
    {
        SetTimeState(true);
        PauseCanvas.gameObject.SetActive(true);

        State = GameState.Paused;
    }

    private void HandlePlayingState()
    {

        SetTimeState(false);

        // disable panels from potential previous states
        switch(State)
        {
            case GameState.Paused:
                PauseCanvas.gameObject.SetActive(false);
                break;
            case GameState.MainMenu:
                MainMenuCanvas.gameObject.SetActive(false);
                break;
            case GameState.GameOver:
                GameOverCanvas.gameObject.SetActive(false);
                break;
        }

        State = GameState.Playing;
    }

    private void HandleGameOverState()
    {
        SetTimeState(true);
        GameOverCanvas.gameObject.SetActive(true);

        State = GameState.GameOver;
    }

    private void HandleMainMenuState()
    {
        SetTimeState(false);
        MainMenuCanvas.gameObject.SetActive(true);

        switch(State)
        {
            case GameState.Paused:
                // quit mid game
                PauseCanvas.gameObject.SetActive(false);
                break;
            case GameState.GameOver:
                GameOverCanvas.gameObject.SetActive(false);
                break;
        }

        State = GameState.MainMenu;
    }



    private void SetTimeState(bool paused)
    {
        Time.timeScale = paused ? 0.0f : 1.0f;
    }


    public void HandleResumeClicked() {
        SetGameState(GameState.Playing);
    }

    public void HandleQuitClicked() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void HandlePlayClicked() {
        SetGameState(GameState.Playing);

        GameBegan?.Invoke();
    }

    public void HandleMainMenuClicked()
    {
        SetGameState(GameState.MainMenu);
    }



    private void Player_PlayerLost(ObjectOwner owner) {
        var panel = GameOverCanvas.transform.Find("Panel");
        var textEl = panel.Find("WinLoseText");
        var textCmp = textEl.GetComponent<TMP_Text>();
        textCmp.SetText($"You {(owner == ObjectOwner.Player ? "Lose" : "Win")}!");

        SetGameState(GameState.GameOver);
    }

    private void InputManager_KeyDown(Keybind action) {
        switch(action) {
            case Keybind.Pause:
                GameState newState = State;
                if(State == GameState.Playing)
                {
                    newState = GameState.Paused;
                } else if(State == GameState.Paused)
                {
                    newState = GameState.Playing;
                }
                SetGameState(newState);
                break;
        }
    }

    private void OnEnable() {
        Player.PlayerLost += Player_PlayerLost;

        InputManager.KeyDown += InputManager_KeyDown;
    }

    private void OnDisable() {
        Player.PlayerLost -= Player_PlayerLost;

        InputManager.KeyDown -= InputManager_KeyDown;
    }
}
