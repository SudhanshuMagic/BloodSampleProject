using UnityEngine;
using BloodSample.Core;

namespace BloodSample.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject _mainMenuPanel;
        [SerializeField] private GameObject _laboratoryPanel;
        [SerializeField] private GameObject _pausePanel;
        
        [Header("Laboratory UI")]
        [SerializeField] private GameObject _interactionPromptPanel;
        [SerializeField] private GameObject _selectedObjectPanel;
        
        [Header("Sample Info Panel")]
        [SerializeField] private GameObject _sampleInfoPanel;
        
        // Runtime text display (console-based for zero-setup)
        private string _currentInteractionPrompt = "";
        private string _currentSelectedObject = "";
        private string _currentSampleInfo = "";
        
        private InputManager _inputManager;
        private GameManager _gameManager;
        
        private void Start()
        {
            _inputManager = FindFirstObjectByType<InputManager>();
            _gameManager = GameManager.Instance;
            
            SetupEventListeners();
            InitializeUI();
        }
        
        private void SetupEventListeners()
        {
            if (_inputManager != null)
            {
                _inputManager.OnObjectSelected.AddListener(OnObjectSelected);
                _inputManager.OnObjectDeselected.AddListener(OnObjectDeselected);
            }
            
            if (_gameManager != null)
            {
                _gameManager.OnGameStateChanged.AddListener(OnGameStateChanged);
            }
            
            // Note: Button functionality handled by input system for zero-setup
        }
        
        private void InitializeUI()
        {
            ShowInteractionPrompt("");
            ShowSelectedObject("");
            HideSampleInfo();
            
            // Show appropriate panel based on game state
            OnGameStateChanged(_gameManager != null ? _gameManager.CurrentState : GameState.Laboratory);
        }
        
        private void OnGameStateChanged(GameState newState)
        {
            HideAllPanels();
            
            switch (newState)
            {
                case GameState.MainMenu:
                    ShowPanel(_mainMenuPanel);
                    break;
                    
                case GameState.Laboratory:
                    ShowPanel(_laboratoryPanel);
                    break;
                    
                case GameState.Paused:
                    ShowPanel(_pausePanel);
                    break;
            }
        }
        
        private void OnObjectSelected(GameObject selectedObject)
        {
            if (selectedObject == null) return;
            
            ShowSelectedObject(selectedObject.name);
            
            // Check if it's an interactable object
            IInteractable interactable = selectedObject.GetComponent<IInteractable>();
            if (interactable != null)
            {
                ShowInteractionPrompt(interactable.InteractionPrompt);
            }
            
            // Check if it's a blood sample
            var bloodSample = selectedObject.GetComponent<BloodSample.Systems.BloodSample>();
            if (bloodSample != null)
            {
                ShowSampleInfo(bloodSample);
            }
        }
        
        private void OnObjectDeselected()
        {
            ShowSelectedObject("");
            ShowInteractionPrompt("");
            HideSampleInfo();
        }
        
        public void ShowInteractionPrompt(string prompt)
        {
            _currentInteractionPrompt = prompt;
            if (!string.IsNullOrEmpty(prompt))
            {
                Debug.Log($"[UI] 💡 {prompt}");
            }
            
            // Show/hide panel if available
            if (_interactionPromptPanel != null)
            {
                _interactionPromptPanel.SetActive(!string.IsNullOrEmpty(prompt));
            }
        }
        
        public void ShowSelectedObject(string objectName)
        {
            _currentSelectedObject = objectName;
            if (!string.IsNullOrEmpty(objectName))
            {
                Debug.Log($"[UI] 🎯 Selected: {objectName}");
            }
            
            // Show/hide panel if available
            if (_selectedObjectPanel != null)
            {
                _selectedObjectPanel.SetActive(!string.IsNullOrEmpty(objectName));
            }
        }
        
        public void ShowSampleInfo(BloodSample.Systems.BloodSample sample)
        {
            if (sample == null) return;
            
            var data = sample.Data;
            _currentSampleInfo = $"ID: {data.sampleId} | Type: {data.sampleType} | Volume: {data.volume:F1}mL | Quality: {data.qualityScore:F0}%";
            
            Debug.Log($"[UI] 🩸 Sample Info: {_currentSampleInfo}");
            
            if (_sampleInfoPanel != null)
            {
                _sampleInfoPanel.SetActive(true);
            }
        }
        
        public void HideSampleInfo()
        {
            _currentSampleInfo = "";
            if (_sampleInfoPanel != null)
            {
                _sampleInfoPanel.SetActive(false);
            }
        }
        
        // Simple on-screen UI display using OnGUI (no packages required)
        private void OnGUI()
        {
            // Create a simple UI overlay
            GUILayout.BeginArea(new Rect(10, 10, 400, 150));
            
            // Show interaction prompt
            if (!string.IsNullOrEmpty(_currentInteractionPrompt))
            {
                GUI.backgroundColor = new Color(0.2f, 0.6f, 1f, 0.8f);
                GUILayout.Box(_currentInteractionPrompt, GUILayout.Height(30));
                GUI.backgroundColor = Color.white;
            }
            
            // Show selected object
            if (!string.IsNullOrEmpty(_currentSelectedObject))
            {
                GUI.backgroundColor = new Color(0.2f, 0.8f, 0.2f, 0.8f);
                GUILayout.Box(_currentSelectedObject, GUILayout.Height(25));
                GUI.backgroundColor = Color.white;
            }
            
            // Show sample info
            if (!string.IsNullOrEmpty(_currentSampleInfo))
            {
                GUI.backgroundColor = new Color(0.8f, 0.2f, 0.2f, 0.8f);
                GUILayout.Box(_currentSampleInfo, GUILayout.Height(25));
                GUI.backgroundColor = Color.white;
            }
            
            GUILayout.EndArea();
            
            // Show controls help in bottom right
            GUILayout.BeginArea(new Rect(Screen.width - 250, Screen.height - 120, 240, 110));
            GUI.backgroundColor = new Color(0f, 0f, 0f, 0.7f);
            GUILayout.BeginVertical("box");
            GUILayout.Label("Controls:", GUI.skin.GetStyle("label"));
            GUILayout.Label("WASD - Move Camera");
            GUILayout.Label("Mouse + RMB - Look Around");
            GUILayout.Label("LMB - Select Objects");
            GUILayout.Label("E - Interact");
            GUILayout.Label("F1 - Debug Panel");
            GUILayout.EndVertical();
            GUILayout.EndArea();
            GUI.backgroundColor = Color.white;
        }
        
        private void ShowPanel(GameObject panel)
        {
            if (panel != null)
            {
                panel.SetActive(true);
            }
        }
        
        private void HideAllPanels()
        {
            if (_mainMenuPanel != null) _mainMenuPanel.SetActive(false);
            if (_laboratoryPanel != null) _laboratoryPanel.SetActive(false);
            if (_pausePanel != null) _pausePanel.SetActive(false);
        }
        
        private void TogglePause()
        {
            if (_gameManager == null) return;
            
            if (_gameManager.CurrentState == GameState.Paused)
            {
                _gameManager.ResumeGame();
            }
            else
            {
                _gameManager.PauseGame();
            }
        }
        
        public void OnResumeButtonClicked()
        {
            _gameManager?.ResumeGame();
        }
        
        public void OnExitButtonClicked()
        {
            _gameManager?.ExitGame();
        }
    }
}
