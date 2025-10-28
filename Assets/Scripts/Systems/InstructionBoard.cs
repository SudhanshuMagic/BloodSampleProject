using UnityEngine;
using BloodSample.Core;

namespace BloodSample.Systems
{
    /// <summary>
    /// Interactive instruction board for blood sample handling procedures
    /// </summary>
    public class InstructionBoard : InteractableObject
    {
        [Header("Instruction Board Settings")]
        [SerializeField] private InstructionType _boardType = InstructionType.BloodHandling;
        [SerializeField] private bool _showOnStart = false;
        [SerializeField] private float _displayDuration = 10f;
        
        [Header("Visual Components")]
        [SerializeField] private GameObject _screenDisplay;
        [SerializeField] private Material _screenOnMaterial;
        [SerializeField] private Material _screenOffMaterial;
        
        private bool _isDisplaying = false;
        private static InstructionBoard _currentlyDisplaying;
        
        public enum InstructionType
        {
            BloodHandling,
            SafetyProcedures,
            SampleProcessing,
            QualityControl,
            EquipmentOperation
        }
        
        protected override void Awake()
        {
            base.Awake();
            SetInteractionPrompt("View Sample Handling Instructions");
            
            if (_showOnStart)
            {
                Invoke(nameof(ShowInstructions), 1f);
            }
        }
        
        public override void OnInteract()
        {
            base.OnInteract();
            
            if (_isDisplaying)
            {
                HideInstructions();
            }
            else
            {
                ShowInstructions();
            }
        }
        
        /// <summary>
        /// Display blood sample handling instructions
        /// </summary>
        public void ShowInstructions()
        {
            // Hide any other instruction board that might be displaying
            if (_currentlyDisplaying != null && _currentlyDisplaying != this)
            {
                _currentlyDisplaying.HideInstructions();
            }
            
            _currentlyDisplaying = this;
            _isDisplaying = true;
            
            // Update visual display
            UpdateScreenDisplay(true);
            
            // Display instructions based on board type
            DisplayInstructionText();
            
            SetInteractionPrompt("Hide Instructions");
            
            // Auto-hide after duration (if set)
            if (_displayDuration > 0)
            {
                Invoke(nameof(HideInstructions), _displayDuration);
            }
            
            Debug.Log($"[InstructionBoard] Displaying {_boardType} instructions");
        }
        
        /// <summary>
        /// Hide the instruction display
        /// </summary>
        public void HideInstructions()
        {
            _isDisplaying = false;
            
            if (_currentlyDisplaying == this)
            {
                _currentlyDisplaying = null;
            }
            
            // Update visual display
            UpdateScreenDisplay(false);
            
            SetInteractionPrompt("View Sample Handling Instructions");
            
            // Cancel auto-hide if active
            CancelInvoke(nameof(HideInstructions));
            
            Debug.Log("[InstructionBoard] Instructions hidden");
        }
        
        /// <summary>
        /// Display the appropriate instruction text
        /// </summary>
        private void DisplayInstructionText()
        {
            string instructions = GetInstructionsForType(_boardType);
            
            Debug.Log("=== BLOOD SAMPLE HANDLING INSTRUCTIONS ===");
            Debug.Log(instructions);
            Debug.Log("==========================================");
        }
        
        /// <summary>
        /// Get instruction text based on board type
        /// </summary>
        private string GetInstructionsForType(InstructionType type)
        {
            switch (type)
            {
                case InstructionType.BloodHandling:
                    return GetBloodHandlingInstructions();
                    
                case InstructionType.SafetyProcedures:
                    return GetSafetyInstructions();
                    
                case InstructionType.SampleProcessing:
                    return GetProcessingInstructions();
                    
                case InstructionType.QualityControl:
                    return GetQualityControlInstructions();
                    
                case InstructionType.EquipmentOperation:
                    return GetEquipmentInstructions();
                    
                default:
                    return GetBloodHandlingInstructions();
            }
        }
        
        /// <summary>
        /// Comprehensive blood sample handling instructions
        /// </summary>
        private string GetBloodHandlingInstructions()
        {
            return @"
🩸 BLOOD SAMPLE HANDLING PROCEDURES

📋 PRE-COLLECTION PREPARATION:
• Verify patient identity and requisition forms
• Ensure proper labeling materials are available
• Check collection tubes for expiration dates
• Wash hands and wear appropriate PPE

🧤 SAFETY REQUIREMENTS:
• Always wear disposable gloves
• Use safety-engineered needles and devices
• Never recap used needles
• Dispose of sharps in designated containers

🩸 COLLECTION PROCEDURES:
1. Select appropriate collection site
2. Clean site with alcohol pad (70% isopropyl)
3. Allow to air dry completely
4. Perform venipuncture using sterile technique
5. Fill tubes in correct order of draw

📝 LABELING REQUIREMENTS:
• Label tubes immediately after collection
• Include: Patient name, ID, date, time
• Use barcode scanning when available
• Verify accuracy before transport

🔬 POST-COLLECTION HANDLING:
• Invert tubes gently (5-10 times for EDTA)
• Transport at appropriate temperature
• Process within specified time limits
• Store according to test requirements

⚠️ CRITICAL SAFETY REMINDERS:
• Report all needle stick injuries immediately
• Follow standard precautions at all times
• Use proper waste disposal procedures
• Maintain chain of custody documentation

🌡️ STORAGE CONDITIONS:
• Room Temperature (20-25°C): Most routine tests
• Refrigerated (2-8°C): Labile analytes
• Frozen (-20°C): Long-term storage
• Never freeze whole blood samples

⏰ TIME-SENSITIVE GUIDELINES:
• Process STAT samples immediately
• Complete routine tests within 24 hours
• Separate serum/plasma within 2 hours
• Document all processing times

For questions or emergencies, contact Laboratory Supervisor immediately.";
        }
        
        /// <summary>
        /// Laboratory safety procedures
        /// </summary>
        private string GetSafetyInstructions()
        {
            return @"
🦺 LABORATORY SAFETY PROCEDURES

⚠️ PERSONAL PROTECTIVE EQUIPMENT (PPE):
• Wear lab coats at all times in work areas
• Use disposable gloves for all sample handling
• Safety glasses required for splash-risk procedures
• Closed-toe shoes mandatory (no sandals)

🧽 HYGIENE AND DECONTAMINATION:
• Wash hands before and after patient contact
• Use alcohol-based sanitizer between samples
• Disinfect work surfaces with 10% bleach solution
• Clean spills immediately with appropriate materials

🗂️ SPECIMEN HANDLING SAFETY:
• Treat all specimens as potentially infectious
• Use biological safety cabinet when required
• Never pipette by mouth - use mechanical devices
• Avoid aerosol generation during processing

🔥 EMERGENCY PROCEDURES:
• Eye wash stations: Flush for 15 minutes minimum
• Fire extinguishers: Know locations and types
• Spill kits: Available for chemical and biological spills
• Emergency shower: Use for large chemical exposures

☣️ BIOHAZARD WASTE MANAGEMENT:
• Red bags: Contaminated materials and PPE
• Sharps containers: Needles, scalpels, broken glass
• Yellow containers: Pathological waste
• Never overfill containers (¾ full maximum)

📞 EMERGENCY CONTACTS:
• Laboratory Director: Ext. 2800
• Safety Officer: Ext. 2850
• Poison Control: 1-800-222-1222
• Emergency Services: 911

Remember: When in doubt, ask for help!";
        }
        
        /// <summary>
        /// Sample processing workflow instructions
        /// </summary>
        private string GetProcessingInstructions()
        {
            return @"
🔬 SAMPLE PROCESSING WORKFLOW

1️⃣ SAMPLE RECEPTION:
• Verify sample integrity and labeling
• Check temperature during transport
• Inspect for hemolysis or contamination
• Log receipt time in laboratory system

2️⃣ CENTRIFUGATION PROCEDURES:
• Balance tubes in centrifuge
• Use appropriate speed and time settings
• Standard: 3000-4000 RPM for 10 minutes
• Check for complete separation

3️⃣ SERUM/PLASMA SEPARATION:
• Use sterile technique for aliquoting
• Avoid disturbing cellular components
• Transfer to appropriate storage tubes
• Label aliquots with source information

4️⃣ ANALYSIS PREPARATION:
• Bring samples to room temperature
• Mix gently before testing
• Use appropriate controls and calibrators
• Follow instrument-specific protocols

5️⃣ QUALITY CONTROL:
• Run controls with each batch
• Check calibration before analysis
• Monitor for systematic errors
• Document all QC results

6️⃣ RESULT REPORTING:
• Review results for clinical correlation
• Verify critical values with repeat testing
• Report within established turnaround times
• Maintain result confidentiality

📊 WORKSTATION ORGANIZATION:
• Keep clean and dirty areas separate
• Use designated areas for different processes
• Maintain adequate lighting and ventilation
• Store reagents at proper temperatures";
        }
        
        /// <summary>
        /// Quality control procedures
        /// </summary>
        private string GetQualityControlInstructions()
        {
            return @"
✅ QUALITY CONTROL PROCEDURES

🎯 DAILY QC REQUIREMENTS:
• Run controls at beginning of each shift
• Verify instrument calibration
• Check reagent expiration dates
• Review environmental conditions

📈 CONTROL SAMPLE GUIDELINES:
• Use controls that span the analytical range
• Run normal and abnormal level controls
• Document all control results
• Investigate out-of-range values immediately

🔍 ANALYTICAL MONITORING:
• Review Westgard rules for control evaluation
• Monitor trends in control performance
• Establish corrective action procedures
• Maintain QC charts and documentation

📝 DOCUMENTATION REQUIREMENTS:
• Record all QC activities
• Document corrective actions taken
• Maintain temperature logs
• Keep maintenance records current

⚡ CORRECTIVE ACTION PROCEDURES:
• Stop testing if controls fail acceptance criteria
• Investigate root cause of problems
• Repeat controls after corrective action
• Document resolution before resuming testing

🎚️ CALIBRATION MANAGEMENT:
• Perform calibrations per manufacturer guidelines
• Use traceable reference materials
• Verify calibration with independent samples
• Maintain calibration records

📊 PROFICIENCY TESTING:
• Participate in external QA programs
• Review proficiency testing results
• Implement improvements when needed
• Maintain PT documentation

Remember: Quality is everyone's responsibility!";
        }
        
        /// <summary>
        /// Equipment operation instructions
        /// </summary>
        private string GetEquipmentInstructions()
        {
            return @"
⚙️ LABORATORY EQUIPMENT OPERATION

🔬 MICROSCOPE OPERATION:
• Clean objectives before use
• Start with lowest magnification
• Use proper illumination settings
• Store covered when not in use

🌀 CENTRIFUGE PROCEDURES:
• Balance samples before spinning
• Secure rotor properly
• Never open while spinning
• Clean after each use

❄️ REFRIGERATOR/FREEZER MANAGEMENT:
• Monitor temperatures daily
• Keep temperature logs current
• Report temperature excursions immediately
• Organize samples systematically

🖥️ COMPUTER SYSTEM USAGE:
• Log in with personal credentials
• Never share passwords
• Back up data regularly
• Report system issues promptly

🧪 ANALYTICAL INSTRUMENTS:
• Follow startup procedures daily
• Perform required maintenance
• Use only approved reagents
• Shut down properly at end of shift

📊 BARCODE SCANNER OPERATION:
• Ensure clean scan window
• Hold steady during scanning
• Verify successful reads
• Report scanning problems

🔧 MAINTENANCE SCHEDULES:
• Daily: Basic cleaning and QC
• Weekly: Performance verification
• Monthly: Detailed maintenance
• Annually: Professional service

⚠️ EQUIPMENT PROBLEMS:
• Never attempt unauthorized repairs
• Tag equipment 'Out of Service'
• Contact technical support
• Document all issues

Always refer to manufacturer instructions for specific procedures.";
        }
        
        /// <summary>
        /// Update the visual display screen
        /// </summary>
        private void UpdateScreenDisplay(bool isOn)
        {
            if (_screenDisplay != null)
            {
                var renderer = _screenDisplay.GetComponent<Renderer>();
                if (renderer != null && _screenOnMaterial != null && _screenOffMaterial != null)
                {
                    renderer.material = isOn ? _screenOnMaterial : _screenOffMaterial;
                }
            }
        }
        
        /// <summary>
        /// Set the type of instructions this board displays
        /// </summary>
        public void SetInstructionType(InstructionType type)
        {
            _boardType = type;
            SetInteractionPrompt($"View {type} Instructions");
        }
        
        /// <summary>
        /// Check if instructions are currently being displayed
        /// </summary>
        public bool IsDisplaying()
        {
            return _isDisplaying;
        }
    }
}
