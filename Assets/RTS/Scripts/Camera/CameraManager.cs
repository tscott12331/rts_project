using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    GameObject MainCamera;
    [SerializeField]
    GameObject CinematicCamera;



    private void GameManager_GameStateChanged(GameState newState)
    {
        switch(newState)
        {
            case GameState.Playing:
                MainCamera.SetActive(true);
                CinematicCamera.SetActive(false);
                break;
            case GameState.MainMenu:
                MainCamera.SetActive(false);
                CinematicCamera.SetActive(true);
                break;
        }
    }

    private void OnEnable()
    {
        GameManager.GameStateChanged += GameManager_GameStateChanged;
    }
    private void OnDisable()
    {
        GameManager.GameStateChanged -= GameManager_GameStateChanged;
    }
}
