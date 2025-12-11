using UnityEngine;


public enum GameState {
    MainMenu,
    PauseMenu,
    Playing,
}

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public delegate void GameStateChangedHandler(GameState state);
    public static event GameStateChangedHandler GameStateChanged;

    public delegate void PauseStateChangedHandler(bool paused);
    public static event PauseStateChangedHandler PauseStateChanged;

    public delegate void GameBeganHandler();
    public static event GameBeganHandler GameBegan;

    public Transform PlayerStartPoint;
    public Transform EnemyStartPoint;

    public Canvas PauseCanvas;



    private bool paused = false;
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

        // initialize players
        player = new(ObjectOwner.Player, PlayerStartPoint);
        enemy = new(ObjectOwner.Enemy, EnemyStartPoint);

        // send game begin event
        GameBegan?.Invoke();
    }

    private void SetPauseState(bool paused) {
        this.paused = paused;

        if(paused) {
            Time.timeScale = 0.0f;
            PauseCanvas.gameObject.SetActive(true);
        } else {
            Time.timeScale = 1.0f;
            PauseCanvas.gameObject.SetActive(false);
        }

        PauseStateChanged?.Invoke(paused);
    }



    public void HandleResumeClicked() {
        SetPauseState(false);
    }

    public void HandleQuitClicked() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void HandlePlayAgainClicked() {
        UnitManager.Instance.ResetManager();
        StructureManager.Instance.ResetManager();

        GameBegan?.Invoke();
    }



    private void Player_PlayerLost(ObjectOwner owner) {
        SetPauseState(true);
        Debug.Log($"{owner} lost");
    }

    private void InputManager_KeyDown(Keybind action) {
        switch(action) {
            case Keybind.Pause:
                SetPauseState(!paused);
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
