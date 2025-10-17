using UnityEngine;
using UnityEngine.Events;

namespace BloodSample.Core
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game State")]
        [SerializeField] private GameState _currentState = GameState.MainMenu;
        
        [Header("Events")]
        public UnityEvent<GameState> OnGameStateChanged;
        
        public static GameManager Instance { get; private set; }
        
        public GameState CurrentState => _currentState;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGame();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeGame()
        {
            Application.targetFrameRate = 60;
            
            // Initialize other managers
            FindFirstObjectByType<InputManager>()?.Initialize();
            
            ChangeGameState(GameState.Laboratory);
        }
        
        public void ChangeGameState(GameState newState)
        {
            if (_currentState == newState) return;
            
            GameState previousState = _currentState;
            _currentState = newState;
            
            Debug.Log($"Game State Changed: {previousState} -> {newState}");
            OnGameStateChanged?.Invoke(newState);
        }
        
        public void PauseGame()
        {
            Time.timeScale = 0f;
            ChangeGameState(GameState.Paused);
        }
        
        public void ResumeGame()
        {
            Time.timeScale = 1f;
            ChangeGameState(GameState.Laboratory);
        }
        
        public void ExitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
    
    public enum GameState
    {
        MainMenu,
        Laboratory,
        Testing,
        Paused,
        Results
    }
}
