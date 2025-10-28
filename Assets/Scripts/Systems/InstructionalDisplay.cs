using UnityEngine;
using BloodSample.Core;

namespace BloodSample.Systems
{
    /// <summary>
    /// Instructional display screen for blood sample handling procedures
    /// </summary>
    public class InstructionalDisplay : InteractableObject
    {
        [Header("Display Settings")]
        [SerializeField] private DisplayContent _currentContent = DisplayContent.BloodSampleHandling;
        [SerializeField] private bool _isDisplayOn = true;
        [SerializeField] private float _textSize = 0.08f;
        
        [Header("Display Materials")]
        [SerializeField] private Material _screenOnMaterial;
        [SerializeField] private Material _screenOffMaterial;
        [SerializeField] private Renderer _screenRenderer;
        
        private string[] _bloodHandlingSteps = {
            "BLOOD SAMPLE HANDLING PROCEDURE",
            "",
            "1. SAFETY FIRST",
            "   • Put on latex gloves",
            "   • Use alcohol sterilizer",
            "   • Check sample integrity",
            "",
            "2. SAMPLE IDENTIFICATION", 
            "   • Scan barcode on sample",
            "   • Verify sample ID",
            "   • Check expiration date",
            "",
            "3. WORKSTATION PREPARATION",
            "   • Select appropriate workstation",
            "   • Ensure clean work surface",
            "   • Prepare processing equipment",
            "",
            "4. SAMPLE PROCESSING",
            "   • Place sample in workstation slot",
            "   • Press E to start processing",
            "   • Wait for completion (5 seconds)",
            "",
            "5. STORAGE & TRANSPORT",
            "   • Use transport container",
            "   • Store in walk-in freezer",
            "   • Update computer records",
            "",
            "6. QUALITY CONTROL",
            "   • Verify sample quality",
            "   • Check processing results",
            "   • Complete documentation"
        };
        
        private string[] _workstationOperationSteps = {
            "WORKSTATION OPERATION GUIDE",
            "",
            "CONTROLS:",
            "• WASD - Move around laboratory",
            "• Mouse + RMB - Look around",
            "• Left Click - Select objects",
            "• E Key - Interact with equipment",
            "",
            "WORKSTATION USAGE:",
            "1. Approach any workstation",
            "2. Left-click to select it",
            "3. Press E to interact",
            "4. Place samples in slots",
            "5. Process samples automatically",
            "",
            "EQUIPMENT LOCATIONS:",
            "• Barcode Scanner - For sample ID",
            "• Latex Gloves - Safety equipment",
            "• Alcohol Sterilizer - Cleaning",
            "• Walk-in Freezer - Sample storage",
            "• Computer System - Data entry",
            "",
            "TIP: Follow the glowing markers",
            "for guided laboratory tour!"
        };
        
        private string[] _qualityControlSteps = {
            "QUALITY CONTROL PROCEDURES",
            "",
            "SAMPLE QUALITY CHECKS:",
            "• Volume: Minimum 5.0mL required",
            "• Quality Score: Must be >80%",
            "• Temperature: Check storage temp",
            "• Contamination: Visual inspection",
            "",
            "PROCESSING VERIFICATION:",
            "• Workstation calibration check",
            "• Equipment status verification",
            "• Result validation",
            "",
            "DOCUMENTATION:",
            "• Sample ID and type recorded",
            "• Processing time logged",
            "• Quality metrics documented",
            "• Storage location updated",
            "",
            "TROUBLESHOOTING:",
            "• Low quality samples - Reprocess",
            "• Equipment issues - Use alternate",
            "• System errors - Check computer"
        };
        
        public enum DisplayContent
        {
            BloodSampleHandling,
            WorkstationOperation,
            QualityControl
        }
        
        protected override void Awake()
        {
            base.Awake();
            SetInteractionPrompt("View Instructions");
            UpdateDisplay();
        }
        
        public override void OnInteract()
        {
            base.OnInteract();
            
            if (_isDisplayOn)
            {
                CycleContent();
            }
            else
            {
                TurnOnDisplay();
            }
        }
        
        private void CycleContent()
        {
            // Cycle through different instruction sets
            _currentContent = (DisplayContent)(((int)_currentContent + 1) % 3);
            UpdateDisplay();
            
            string contentName = _currentContent.ToString();
            Debug.Log($"[InstructionalDisplay] Switched to: {contentName}");
            SetInteractionPrompt($"View Instructions - {contentName}");
        }
        
        private void TurnOnDisplay()
        {
            _isDisplayOn = true;
            UpdateDisplay();
            SetInteractionPrompt("Cycle Instructions");
        }
        
        private void TurnOffDisplay()
        {
            _isDisplayOn = false;
            UpdateDisplay();
            SetInteractionPrompt("Turn On Display");
        }
        
        private void UpdateDisplay()
        {
            if (_screenRenderer != null)
            {
                _screenRenderer.material = _isDisplayOn ? _screenOnMaterial : _screenOffMaterial;
            }
            
            // Display current content in console for now
            // In a full implementation, this would render to a texture on the screen
            if (_isDisplayOn)
            {
                DisplayInstructions();
            }
        }
        
        private void DisplayInstructions()
        {
            string[] instructions = GetCurrentInstructions();
            
            Debug.Log("=== INSTRUCTIONAL DISPLAY ===");
            foreach (string line in instructions)
            {
                Debug.Log(line);
            }
            Debug.Log("============================");
        }
        
        private string[] GetCurrentInstructions()
        {
            switch (_currentContent)
            {
                case DisplayContent.BloodSampleHandling:
                    return _bloodHandlingSteps;
                case DisplayContent.WorkstationOperation:
                    return _workstationOperationSteps;
                case DisplayContent.QualityControl:
                    return _qualityControlSteps;
                default:
                    return _bloodHandlingSteps;
            }
        }
        
        /// <summary>
        /// Set the display materials for on/off states
        /// </summary>
        public void SetDisplayMaterials(Material onMaterial, Material offMaterial)
        {
            _screenOnMaterial = onMaterial;
            _screenOffMaterial = offMaterial;
            
            if (_screenRenderer == null)
            {
                _screenRenderer = GetComponent<Renderer>();
            }
            
            UpdateDisplay();
        }
        
        /// <summary>
        /// Set specific content to display
        /// </summary>
        public void SetDisplayContent(DisplayContent content)
        {
            _currentContent = content;
            UpdateDisplay();
        }
        
        /// <summary>
        /// Add custom instruction set
        /// </summary>
        public void SetCustomInstructions(string[] instructions)
        {
            _bloodHandlingSteps = instructions;
            if (_currentContent == DisplayContent.BloodSampleHandling)
            {
                UpdateDisplay();
            }
        }
        
        [ContextMenu("Show Blood Sample Handling")]
        public void ShowBloodSampleHandling()
        {
            SetDisplayContent(DisplayContent.BloodSampleHandling);
        }
        
        [ContextMenu("Show Workstation Operation")]
        public void ShowWorkstationOperation()
        {
            SetDisplayContent(DisplayContent.WorkstationOperation);
        }
        
        [ContextMenu("Show Quality Control")]
        public void ShowQualityControl()
        {
            SetDisplayContent(DisplayContent.QualityControl);
        }
        
        [ContextMenu("Cycle Content")]
        public void CycleContentMenu()
        {
            CycleContent();
        }
    }
}
