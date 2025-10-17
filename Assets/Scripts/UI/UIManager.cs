using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
        [SerializeField] private TextMeshProUGUI _interactionPromptText;
        [SerializeField] private TextMeshProUGUI _selectedObjectText;
        [SerializeField] private Button _pauseButton;
        
        [Header("Sample Info Panel")]
        [SerializeField] private GameObject _sampleInfoPanel;
        [SerializeField] private TextMeshProUGUI _sampleIdText;
        [SerializeField] private TextMeshProUGUI _sampleTypeText;
        [SerializeField] private TextMeshProUGUI _sampleVolumeText;
        [SerializeField] private TextMeshProUGUI _sampleQualityText;
        
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
            
            if (_pauseButton != null)
            {
                _pauseButton.onClick.AddListener(TogglePause);
            }
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
            if (_interactionPromptText != null)
            {
                _interactionPromptText.text = prompt;
                _interactionPromptText.gameObject.SetActive(!string.IsNullOrEmpty(prompt));
            }
        }
        
        public void ShowSelectedObject(string objectName)
        {
            if (_selectedObjectText != null)
            {
                _selectedObjectText.text = string.IsNullOrEmpty(objectName) ? "" : $"Selected: {objectName}";
            }
        }
        
        public void ShowSampleInfo(BloodSample.Systems.BloodSample sample)
        {
            if (_sampleInfoPanel == null || sample == null) return;
            
            _sampleInfoPanel.SetActive(true);
            
            var data = sample.Data;
            if (_sampleIdText != null) _sampleIdText.text = $"ID: {data.sampleId}";
            if (_sampleTypeText != null) _sampleTypeText.text = $"Type: {data.sampleType}";
            if (_sampleVolumeText != null) _sampleVolumeText.text = $"Volume: {data.volume:F1}mL";
            if (_sampleQualityText != null) _sampleQualityText.text = $"Quality: {data.qualityScore:F0}%";
        }
        
        public void HideSampleInfo()
        {
            if (_sampleInfoPanel != null)
            {
                _sampleInfoPanel.SetActive(false);
            }
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
