using UnityEngine;
using System.Collections;

namespace BloodSample.Systems
{
    /// <summary>
    /// Simple instruction system for displaying blood sample handling instructions
    /// Uses console output and keyboard controls (no UI dependencies required)
    /// </summary>
    public class SimpleInstructionSystem : MonoBehaviour
    {
        [Header("Instruction Settings")]
        [SerializeField] private bool _showOnStart = true;
        [SerializeField] private bool _autoAdvance = false;
        [SerializeField] private float _autoAdvanceDelay = 10f;
        
        private int _currentStep = 0;
        private InstructionStep[] _instructionSteps;
        private bool _instructionsActive = true;
        
        [System.Serializable]
        public class InstructionStep
        {
            public string title;
            [TextArea(4, 8)]
            public string content;
            public string stepNumber;
        }
        
        private void Awake()
        {
            InitializeInstructionSteps();
        }
        
        private void Start()
        {
            if (_showOnStart)
            {
                StartCoroutine(ShowWelcomeInstructions());
            }
        }
        
        private void InitializeInstructionSteps()
        {
            _instructionSteps = new InstructionStep[]
            {
                new InstructionStep
                {
                    title = "WELCOME TO BLOOD HANDLING LAB",
                    content = "This training teaches proper blood sample handling procedures and safety protocols.\n\n" +
                             "MOVEMENT CONTROLS:\n" +
                             "• WASD - Move around laboratory\n" +
                             "• Mouse - Look around in all directions\n" +
                             "• Left Click - Select objects\n" +
                             "• Right Click + Drag - Move equipment\n\n" +
                             "INSTRUCTION CONTROLS:\n" +
                             "• TAB - Next instruction step\n" +
                             "• SHIFT+TAB - Previous instruction step\n" +
                             "• G - Show/Hide instructions\n" +
                             "• H - Repeat current instruction",
                    stepNumber = "WELCOME"
                },
                new InstructionStep
                {
                    title = "STEP 1: SAFETY FIRST",
                    content = "Before handling any blood samples, ensure proper safety measures:\n\n" +
                             "SAFETY CHECKLIST:\n" +
                             "• Put on latex gloves (located near workstation)\n" +
                             "• Use alcohol sterilizer for hand sanitization\n" +
                             "• Ensure work area is clean and sterile\n" +
                             "• Check sample integrity before processing\n" +
                             "• Verify all equipment is functioning properly\n\n" +
                             "REMEMBER: Safety is the top priority in blood sample handling!",
                    stepNumber = "STEP 1 OF 7"
                },
                new InstructionStep
                {
                    title = "STEP 2: SAMPLE IDENTIFICATION",
                    content = "Proper sample identification is crucial for accurate results:\n\n" +
                             "IDENTIFICATION PROCESS:\n" +
                             "• Use the barcode scanner to scan sample ID\n" +
                             "• Verify patient information matches request\n" +
                             "• Check sample expiration date and time\n" +
                             "• Ensure sample volume is adequate (minimum 5.0mL)\n" +
                             "• Record sample details in laboratory system\n" +
                             "• Double-check all information before proceeding",
                    stepNumber = "STEP 2 OF 7"
                },
                new InstructionStep
                {
                    title = "STEP 3: WORKSTATION PREPARATION",
                    content = "Prepare your workstation for optimal sample processing:\n\n" +
                             "PREPARATION STEPS:\n" +
                             "• Select the appropriate workstation for your samples\n" +
                             "• Ensure work surface is clean and disinfected\n" +
                             "• Check equipment calibration status\n" +
                             "• Prepare all necessary processing equipment\n" +
                             "• Verify all systems are operational and ready\n" +
                             "• Organize samples for efficient workflow",
                    stepNumber = "STEP 3 OF 7"
                },
                new InstructionStep
                {
                    title = "STEP 4: SAMPLE PLACEMENT",
                    content = "Place blood samples correctly in the workstation:\n\n" +
                             "PLACEMENT PROCEDURE:\n" +
                             "• Right-click and drag blood samples from the rack\n" +
                             "• Move samples toward the blue circular slots on workstation\n" +
                             "• Drop samples near slots - they will automatically snap into position\n" +
                             "• Ensure proper sample orientation and secure placement\n" +
                             "• Maximum of 3 samples can be processed per workstation\n" +
                             "• Verify all samples are properly seated before processing",
                    stepNumber = "STEP 4 OF 7"
                },
                new InstructionStep
                {
                    title = "STEP 5: SAMPLE PROCESSING",
                    content = "Process samples following established laboratory protocols:\n\n" +
                             "PROCESSING STEPS:\n" +
                             "• Press 'E' key while at workstation to start manual processing\n" +
                             "• OR wait 3 seconds for automatic processing to begin\n" +
                             "• Processing time is approximately 5 seconds per sample\n" +
                             "• Monitor processing status and do not disturb samples\n" +
                             "• Wait for completion confirmation before removing samples\n" +
                             "• Note any processing anomalies or errors",
                    stepNumber = "STEP 5 OF 7"
                },
                new InstructionStep
                {
                    title = "STEP 6: QUALITY CONTROL",
                    content = "Verify sample quality and processing results:\n\n" +
                             "QUALITY CHECKS:\n" +
                             "• Check processing completion status\n" +
                             "• Verify sample quality score is above 80%\n" +
                             "• Review all processing results for accuracy\n" +
                             "• Document any anomalies or quality issues\n" +
                             "• Prepare samples for storage or further analysis\n" +
                             "• Complete quality control documentation",
                    stepNumber = "STEP 6 OF 7"
                },
                new InstructionStep
                {
                    title = "STEP 7: STORAGE & DOCUMENTATION",
                    content = "Complete the blood handling process properly:\n\n" +
                             "FINAL STEPS:\n" +
                             "• Store processed samples in the walk-in freezer\n" +
                             "• Update computer records with all results\n" +
                             "• Complete all required laboratory documentation\n" +
                             "• Clean and sanitize all equipment used\n" +
                             "• Remove gloves and wash hands thoroughly\n" +
                             "• File all paperwork and quality control records\n\n" +
                             "CONGRATULATIONS! You have completed the blood handling training successfully!",
                    stepNumber = "STEP 7 OF 7"
                }
            };
        }
        
        private IEnumerator ShowWelcomeInstructions()
        {
            yield return new WaitForSeconds(2f); // Wait for scene to load
            ShowCurrentInstruction();
        }
        
        private void Update()
        {
            if (!_instructionsActive) return;
            
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
                ShowCurrentInstruction();
            }
        }
        
        public void ShowNextStep()
        {
            if (_currentStep < _instructionSteps.Length - 1)
            {
                _currentStep++;
                ShowCurrentInstruction();
            }
            else
            {
                Debug.Log("=== BLOOD HANDLING TRAINING COMPLETE ===");
                Debug.Log("You have completed all instruction steps!");
                Debug.Log("Press G to toggle instructions or H to review current step.");
            }
        }
        
        public void ShowPreviousStep()
        {
            if (_currentStep > 0)
            {
                _currentStep--;
                ShowCurrentInstruction();
            }
        }
        
        public void ShowCurrentInstruction()
        {
            if (_currentStep >= 0 && _currentStep < _instructionSteps.Length)
            {
                var step = _instructionSteps[_currentStep];
                
                Debug.Log("=====================================");
                Debug.Log($"📋 {step.title}");
                Debug.Log($"🔢 {step.stepNumber}");
                Debug.Log("=====================================");
                Debug.Log(step.content);
                Debug.Log("=====================================");
                Debug.Log("NAVIGATION: TAB (Next) | SHIFT+TAB (Previous) | G (Hide/Show) | H (Repeat)");
                Debug.Log("=====================================");
            }
        }
        
        public void ToggleInstructions()
        {
            _instructionsActive = !_instructionsActive;
            
            if (_instructionsActive)
            {
                Debug.Log("📋 INSTRUCTIONS ENABLED - Press TAB for next step, G to hide");
                ShowCurrentInstruction();
            }
            else
            {
                Debug.Log("📋 INSTRUCTIONS HIDDEN - Press G to show instructions again");
            }
        }
        
        public void SkipToEnd()
        {
            _currentStep = _instructionSteps.Length - 1;
            ShowCurrentInstruction();
        }
        
        public void ResetInstructions()
        {
            _currentStep = 0;
            _instructionsActive = true;
            ShowCurrentInstruction();
        }
        
        // Public methods for external control
        [ContextMenu("Show Current Instruction")]
        public void ShowCurrentInstructionMenu()
        {
            ShowCurrentInstruction();
        }
        
        [ContextMenu("Next Step")]
        public void NextStepMenu()
        {
            ShowNextStep();
        }
        
        [ContextMenu("Previous Step")]
        public void PreviousStepMenu()
        {
            ShowPreviousStep();
        }
        
        [ContextMenu("Reset Instructions")]
        public void ResetInstructionsMenu()
        {
            ResetInstructions();
        }
        
        // Get current step info for other systems
        public string GetCurrentStepTitle()
        {
            if (_currentStep >= 0 && _currentStep < _instructionSteps.Length)
            {
                return _instructionSteps[_currentStep].title;
            }
            return "";
        }
        
        public string GetCurrentStepNumber()
        {
            if (_currentStep >= 0 && _currentStep < _instructionSteps.Length)
            {
                return _instructionSteps[_currentStep].stepNumber;
            }
            return "";
        }
        
        public int GetCurrentStepIndex()
        {
            return _currentStep;
        }
        
        public int GetTotalSteps()
        {
            return _instructionSteps.Length;
        }
    }
}
