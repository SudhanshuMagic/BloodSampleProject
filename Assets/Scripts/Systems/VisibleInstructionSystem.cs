using UnityEngine;

namespace BloodSample.Systems
{
    /// <summary>
    /// Visible 3D instruction system using TextMesh (no UI packages required)
    /// Creates floating instruction text visible in the 3D scene
    /// </summary>
    public class VisibleInstructionSystem : MonoBehaviour
    {
        [Header("Instruction Display Settings")]
        [SerializeField] private bool _showOnStart = true;
        [SerializeField] private Vector3 _instructionPosition = new Vector3(0, 3, 2);
        [SerializeField] private float _textSize = 0.1f;
        [SerializeField] private Color _textColor = Color.white;
        [SerializeField] private Color _titleColor = Color.yellow;
        
        private GameObject _instructionPanel;
        private TextMesh _titleTextMesh;
        private TextMesh _contentTextMesh;
        private TextMesh _navigationTextMesh;
        private int _currentStep = 0;
        private InstructionStep[] _instructionSteps;
        private bool _instructionsVisible = true;
        
        [System.Serializable]
        public class InstructionStep
        {
            public string title;
            [TextArea(4, 8)]
            public string content;
            public string stepInfo;
        }
        
        private void Awake()
        {
            InitializeInstructionSteps();
            CreateVisibleInstructionPanel();
        }
        
        private void Start()
        {
            if (_showOnStart)
            {
                ShowInstructions();
                Invoke(nameof(ShowWelcomeMessage), 2f);
            }
            else
            {
                HideInstructions();
            }
        }
        
        private void InitializeInstructionSteps()
        {
            _instructionSteps = new InstructionStep[]
            {
                new InstructionStep
                {
                    title = "WELCOME TO BLOOD HANDLING LAB",
                    content = "This training teaches proper blood sample handling procedures.\n\n" +
                             "CONTROLS:\n" +
                             "• WASD - Move around laboratory\n" +
                             "• Mouse - Look around in all directions\n" +
                             "• Left Click - Select objects\n" +
                             "• Right Click + Drag - Move equipment\n\n" +
                             "INSTRUCTIONS:\n" +
                             "• TAB - Next step    • SHIFT+TAB - Previous step\n" +
                             "• G - Hide/Show      • H - Repeat current step",
                    stepInfo = "WELCOME - Press TAB to continue"
                },
                new InstructionStep
                {
                    title = "STEP 1: SAFETY FIRST",
                    content = "Before handling blood samples:\n\n" +
                             "• Put on latex gloves\n" +
                             "• Use alcohol sterilizer\n" +
                             "• Ensure work area is clean\n" +
                             "• Check sample integrity\n" +
                             "• Verify equipment is functioning\n\n" +
                             "Safety is the top priority!",
                    stepInfo = "STEP 1 OF 7 - Press TAB to continue"
                },
                new InstructionStep
                {
                    title = "STEP 2: SAMPLE IDENTIFICATION",
                    content = "Proper sample identification:\n\n" +
                             "• Use barcode scanner to scan sample ID\n" +
                             "• Verify patient information matches\n" +
                             "• Check sample expiration date\n" +
                             "• Ensure sample volume (min 5.0mL)\n" +
                             "• Record details in laboratory system",
                    stepInfo = "STEP 2 OF 7 - Press TAB to continue"
                },
                new InstructionStep
                {
                    title = "STEP 3: WORKSTATION PREPARATION",
                    content = "Prepare your workstation:\n\n" +
                             "• Select appropriate workstation\n" +
                             "• Ensure clean work surface\n" +
                             "• Check equipment calibration\n" +
                             "• Prepare processing equipment\n" +
                             "• Verify all systems operational",
                    stepInfo = "STEP 3 OF 7 - Press TAB to continue"
                },
                new InstructionStep
                {
                    title = "STEP 4: SAMPLE PLACEMENT",
                    content = "Place samples in workstation:\n\n" +
                             "• Right-click and drag samples from rack\n" +
                             "• Drop ANYWHERE on workstation surface\n" +
                             "• Auto-snap to nearest available slot\n" +
                             "• Sample liquid turns RED when verified\n" +
                             "• Computer screen shows 'SAMPLE VERIFIED'\n" +
                             "• Maximum 3 samples per workstation",
                    stepInfo = "STEP 4 OF 7 - Press TAB to continue"
                },
                new InstructionStep
                {
                    title = "STEP 5: SAMPLE PROCESSING",
                    content = "Process samples following protocol:\n\n" +
                             "• Press 'E' at workstation to start manually\n" +
                             "• OR wait 3 seconds for auto-processing\n" +
                             "• Processing takes ~5 seconds\n" +
                             "• Monitor status, do not disturb samples\n" +
                             "• Wait for completion confirmation",
                    stepInfo = "STEP 5 OF 7 - Press TAB to continue"
                },
                new InstructionStep
                {
                    title = "STEP 6: QUALITY CONTROL",
                    content = "Verify sample quality:\n\n" +
                             "• Check processing completion status\n" +
                             "• Verify quality score above 80%\n" +
                             "• Review processing results\n" +
                             "• Document any anomalies\n" +
                             "• Prepare for storage or analysis",
                    stepInfo = "STEP 6 OF 7 - Press TAB to continue"
                },
                new InstructionStep
                {
                    title = "STEP 7: STORAGE & DOCUMENTATION",
                    content = "Complete the process:\n\n" +
                             "• Store processed samples in freezer\n" +
                             "• Update computer records\n" +
                             "• Complete required documentation\n" +
                             "• Clean and sanitize equipment\n" +
                             "• Remove gloves and wash hands\n\n" +
                             "TRAINING COMPLETE!",
                    stepInfo = "STEP 7 OF 7 - Training Complete!"
                }
            };
        }
        
        private void CreateVisibleInstructionPanel()
        {
            // Create main instruction panel object
            _instructionPanel = new GameObject("VisibleInstructionPanel");
            _instructionPanel.transform.SetParent(transform);
            _instructionPanel.transform.position = _instructionPosition;
            
            // Create background panel (simple cube)
            GameObject background = GameObject.CreatePrimitive(PrimitiveType.Cube);
            background.name = "InstructionBackground";
            background.transform.SetParent(_instructionPanel.transform);
            background.transform.localPosition = Vector3.zero;
            background.transform.localScale = new Vector3(6f, 4f, 0.1f);
            
            // Style the background
            Renderer bgRenderer = background.GetComponent<Renderer>();
            bgRenderer.material = new Material(Shader.Find("Standard"));
            bgRenderer.material.color = new Color(0.1f, 0.2f, 0.4f, 0.8f);
            bgRenderer.material.SetFloat("_Mode", 3); // Transparent mode
            bgRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            bgRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            bgRenderer.material.SetInt("_ZWrite", 0);
            bgRenderer.material.DisableKeyword("_ALPHATEST_ON");
            bgRenderer.material.EnableKeyword("_ALPHABLEND_ON");
            bgRenderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            bgRenderer.material.renderQueue = 3000;
            
            // Remove collider from background
            Destroy(background.GetComponent<Collider>());
            
            // Create title text
            CreateTitleText();
            
            // Create content text
            CreateContentText();
            
            // Create navigation text
            CreateNavigationText();
        }
        
        private void CreateTitleText()
        {
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(_instructionPanel.transform);
            titleObj.transform.localPosition = new Vector3(0, 1.5f, -0.1f);
            titleObj.transform.localRotation = Quaternion.identity;
            
            _titleTextMesh = titleObj.AddComponent<TextMesh>();
            _titleTextMesh.text = "WELCOME TO BLOOD HANDLING LAB";
            _titleTextMesh.fontSize = 20;
            _titleTextMesh.color = _titleColor;
            _titleTextMesh.anchor = TextAnchor.MiddleCenter;
            _titleTextMesh.alignment = TextAlignment.Center;
            _titleTextMesh.characterSize = _textSize;
        }
        
        private void CreateContentText()
        {
            GameObject contentObj = new GameObject("ContentText");
            contentObj.transform.SetParent(_instructionPanel.transform);
            contentObj.transform.localPosition = new Vector3(0, 0.2f, -0.1f);
            contentObj.transform.localRotation = Quaternion.identity;
            
            _contentTextMesh = contentObj.AddComponent<TextMesh>();
            _contentTextMesh.text = "Loading instructions...";
            _contentTextMesh.fontSize = 12;
            _contentTextMesh.color = _textColor;
            _contentTextMesh.anchor = TextAnchor.MiddleCenter;
            _contentTextMesh.alignment = TextAlignment.Center;
            _contentTextMesh.characterSize = _textSize * 0.8f;
        }
        
        private void CreateNavigationText()
        {
            GameObject navObj = new GameObject("NavigationText");
            navObj.transform.SetParent(_instructionPanel.transform);
            navObj.transform.localPosition = new Vector3(0, -1.5f, -0.1f);
            navObj.transform.localRotation = Quaternion.identity;
            
            _navigationTextMesh = navObj.AddComponent<TextMesh>();
            _navigationTextMesh.text = "TAB: Next | SHIFT+TAB: Previous | G: Hide/Show | H: Repeat";
            _navigationTextMesh.fontSize = 10;
            _navigationTextMesh.color = Color.cyan;
            _navigationTextMesh.anchor = TextAnchor.MiddleCenter;
            _navigationTextMesh.alignment = TextAlignment.Center;
            _navigationTextMesh.characterSize = _textSize * 0.6f;
        }
        
        private void Update()
        {
            // Handle keyboard shortcuts
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    ShowPreviousStep();
                }
                else
                {
                    ShowNextStep();
                }
            }
            else if (Input.GetKeyDown(KeyCode.G))
            {
                ToggleInstructions();
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                UpdateStepDisplay();
            }
            
            // Make instruction panel face the player camera
            if (_instructionPanel != null && Camera.main != null)
            {
                Vector3 directionToCamera = Camera.main.transform.position - _instructionPanel.transform.position;
                directionToCamera.y = 0; // Keep it upright
                if (directionToCamera != Vector3.zero)
                {
                    _instructionPanel.transform.rotation = Quaternion.LookRotation(-directionToCamera);
                }
            }
        }
        
        private void ShowWelcomeMessage()
        {
            UpdateStepDisplay();
            Debug.Log("📋 VISIBLE INSTRUCTIONS: Look for the floating instruction panel in the scene!");
            Debug.Log("Use TAB to navigate through 7 detailed blood handling steps.");
        }
        
        public void ShowNextStep()
        {
            if (_currentStep < _instructionSteps.Length - 1)
            {
                _currentStep++;
                UpdateStepDisplay();
            }
            else
            {
                Debug.Log("🎉 TRAINING COMPLETE! All instruction steps finished.");
            }
        }
        
        public void ShowPreviousStep()
        {
            if (_currentStep > 0)
            {
                _currentStep--;
                UpdateStepDisplay();
            }
        }
        
        public void ToggleInstructions()
        {
            _instructionsVisible = !_instructionsVisible;
            
            if (_instructionPanel != null)
            {
                _instructionPanel.SetActive(_instructionsVisible);
            }
            
            if (_instructionsVisible)
            {
                Debug.Log("📋 INSTRUCTIONS SHOWN - Floating panel is now visible");
                UpdateStepDisplay();
            }
            else
            {
                Debug.Log("📋 INSTRUCTIONS HIDDEN - Press G to show again");
            }
        }
        
        public void ShowInstructions()
        {
            _instructionsVisible = true;
            if (_instructionPanel != null)
            {
                _instructionPanel.SetActive(true);
            }
        }
        
        public void HideInstructions()
        {
            _instructionsVisible = false;
            if (_instructionPanel != null)
            {
                _instructionPanel.SetActive(false);
            }
        }
        
        private void UpdateStepDisplay()
        {
            if (_currentStep >= 0 && _currentStep < _instructionSteps.Length)
            {
                var step = _instructionSteps[_currentStep];
                
                if (_titleTextMesh != null) _titleTextMesh.text = step.title;
                if (_contentTextMesh != null) _contentTextMesh.text = step.content;
                if (_navigationTextMesh != null) 
                {
                    _navigationTextMesh.text = $"{step.stepInfo}\nTAB: Next | SHIFT+TAB: Previous | G: Hide/Show | H: Repeat";
                }
            }
        }
        
        // Public methods for external control
        [ContextMenu("Show Current Step")]
        public void ShowCurrentStepMenu()
        {
            UpdateStepDisplay();
        }
        
        [ContextMenu("Next Step")]
        public void NextStepMenu()
        {
            ShowNextStep();
        }
        
        [ContextMenu("Toggle Visibility")]
        public void ToggleVisibilityMenu()
        {
            ToggleInstructions();
        }
    }
}
