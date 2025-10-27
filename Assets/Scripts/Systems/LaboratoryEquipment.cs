using UnityEngine;
using BloodSample.Core;

namespace BloodSample.Systems
{
    /// <summary>
    /// Modern laboratory equipment types for immersive environment
    /// </summary>
    public enum EquipmentType
    {
        WalkInFreezer,
        LightFixture,
        LightSwitch,
        LatexGlovesBox,
        AlcoholSterilizer,
        SharpsDisposalBox,
        TestTubeRack,
        ReceivingBench,
        Tray,
        Table,
        Chair,
        ComputerMonitor,
        Cabinet,
        Mouse,
        BarcodeScanner,
        LabellingStation,
        TransportContainer
    }

    /// <summary>
    /// Base class for all laboratory equipment
    /// </summary>
    public class LaboratoryEquipment : InteractableObject
    {
        [Header("Equipment Settings")]
        [SerializeField] protected EquipmentType _equipmentType;
        [SerializeField] protected bool _isOperational = true;
        [SerializeField] protected string _equipmentId;
        
        [Header("Status Indicators")]
        [SerializeField] protected GameObject _statusLight;
        [SerializeField] protected Material _operationalMaterial;
        [SerializeField] protected Material _nonOperationalMaterial;
        
        public EquipmentType Type => _equipmentType;
        public bool IsOperational => _isOperational;
        public string EquipmentId => _equipmentId;
        
        protected override void Awake()
        {
            base.Awake();
            if (string.IsNullOrEmpty(_equipmentId))
            {
                _equipmentId = $"{_equipmentType}_{System.DateTime.Now.Ticks}";
            }
            
            SetInteractionPrompt($"Use {_equipmentType}");
            UpdateStatusIndicator();
        }
        
        public override void OnInteract()
        {
            base.OnInteract();
            
            if (!_isOperational)
            {
                Debug.Log($"[{_equipmentType}] Equipment is not operational");
                return;
            }
            
            PerformEquipmentAction();
        }
        
        protected virtual void PerformEquipmentAction()
        {
            Debug.Log($"[{_equipmentType}] Equipment activated - {_equipmentId}");
        }
        
        protected virtual void UpdateStatusIndicator()
        {
            if (_statusLight != null)
            {
                var renderer = _statusLight.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = _isOperational ? _operationalMaterial : _nonOperationalMaterial;
                }
            }
        }
        
        public virtual void SetOperationalStatus(bool operational)
        {
            _isOperational = operational;
            UpdateStatusIndicator();
        }
    }

    /// <summary>
    /// Walk-in freezer for sample storage
    /// </summary>
    public class WalkInFreezer : LaboratoryEquipment
    {
        [Header("Freezer Settings")]
        [SerializeField] private float _temperature = -20f;
        [SerializeField] private bool _doorOpen = false;
        [SerializeField] private Transform _door;
        [SerializeField] private Light _interiorLight;
        
        protected override void Awake()
        {
            base.Awake();
            _equipmentType = EquipmentType.WalkInFreezer;
            SetInteractionPrompt("Open/Close Freezer Door");
        }
        
        protected override void PerformEquipmentAction()
        {
            base.PerformEquipmentAction();
            ToggleDoor();
        }
        
        private void ToggleDoor()
        {
            _doorOpen = !_doorOpen;
            
            if (_door != null)
            {
                Vector3 targetRotation = _doorOpen ? new Vector3(0, -90, 0) : Vector3.zero;
                _door.localRotation = Quaternion.Euler(targetRotation);
            }
            
            if (_interiorLight != null)
            {
                _interiorLight.enabled = _doorOpen;
            }
            
            Debug.Log($"[WalkInFreezer] Door {(_doorOpen ? "opened" : "closed")} - Temperature: {_temperature}°C");
            SetInteractionPrompt(_doorOpen ? "Close Freezer Door" : "Open Freezer Door");
        }
    }

    /// <summary>
    /// Light switch for laboratory lighting control
    /// </summary>
    public class LightSwitch : LaboratoryEquipment
    {
        [Header("Light Control")]
        [SerializeField] private Light[] _controlledLights;
        [SerializeField] private bool _lightsOn = true;
        [SerializeField] private Material _onMaterial;
        [SerializeField] private Material _offMaterial;
        
        protected override void Awake()
        {
            base.Awake();
            _equipmentType = EquipmentType.LightSwitch;
            SetInteractionPrompt("Toggle Lights");
            UpdateLightStatus();
        }
        
        protected override void PerformEquipmentAction()
        {
            base.PerformEquipmentAction();
            ToggleLights();
        }
        
        private void ToggleLights()
        {
            _lightsOn = !_lightsOn;
            UpdateLightStatus();
            
            Debug.Log($"[LightSwitch] Laboratory lights {(_lightsOn ? "turned on" : "turned off")}");
        }
        
        private void UpdateLightStatus()
        {
            // Check if _controlledLights is initialized
            if (_controlledLights == null || _controlledLights.Length == 0)
            {
                return; // Skip if no lights are assigned yet
            }
            
            foreach (var light in _controlledLights)
            {
                if (light != null)
                {
                    light.enabled = _lightsOn;
                }
            }
            
            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                // Only update material if both materials are assigned
                if (_onMaterial != null && _offMaterial != null)
                {
                    renderer.material = _lightsOn ? _onMaterial : _offMaterial;
                }
            }
        }
        
        /// <summary>
        /// Set the lights that this switch will control
        /// </summary>
        public void SetControlledLights(Light[] lights)
        {
            _controlledLights = lights;
            UpdateLightStatus(); // Update the lights immediately
        }
        
        /// <summary>
        /// Set the materials for on/off states
        /// </summary>
        public void SetSwitchMaterials(Material onMaterial, Material offMaterial)
        {
            _onMaterial = onMaterial;
            _offMaterial = offMaterial;
            UpdateLightStatus(); // Update the switch appearance immediately
        }
    }

    /// <summary>
    /// Latex gloves dispenser
    /// </summary>
    public class LatexGlovesBox : LaboratoryEquipment
    {
        [Header("Gloves Settings")]
        [SerializeField] private int _glovesCount = 100;
        [SerializeField] private GameObject _glovePrefab;
        [SerializeField] private Transform _dispensePoint;
        
        protected override void Awake()
        {
            base.Awake();
            _equipmentType = EquipmentType.LatexGlovesBox;
            SetInteractionPrompt($"Take Gloves ({_glovesCount} remaining)");
        }
        
        protected override void PerformEquipmentAction()
        {
            base.PerformEquipmentAction();
            DispenseGloves();
        }
        
        private void DispenseGloves()
        {
            if (_glovesCount <= 0)
            {
                Debug.Log("[LatexGlovesBox] No gloves remaining");
                SetInteractionPrompt("Empty - Refill Required");
                return;
            }
            
            _glovesCount--;
            
            if (_glovePrefab != null && _dispensePoint != null)
            {
                GameObject gloves = Instantiate(_glovePrefab, _dispensePoint.position, _dispensePoint.rotation);
                gloves.name = "DisposableGloves";
                
                // Add physics for realistic dropping
                var rb = gloves.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(Vector3.down * 2f, ForceMode.Impulse);
                }
            }
            
            Debug.Log($"[LatexGlovesBox] Gloves dispensed - {_glovesCount} remaining");
            SetInteractionPrompt($"Take Gloves ({_glovesCount} remaining)");
        }
    }

    /// <summary>
    /// Computer workstation for laboratory data management
    /// </summary>
    public class ComputerWorkstation : LaboratoryEquipment
    {
        [Header("Computer Settings")]
        [SerializeField] private bool _isLoggedIn = false;
        [SerializeField] private string _currentUser = "";
        [SerializeField] private Material _screenOnMaterial;
        [SerializeField] private Material _screenOffMaterial;
        [SerializeField] private Renderer _screenRenderer;
        
        protected override void Awake()
        {
            base.Awake();
            _equipmentType = EquipmentType.ComputerMonitor;
            SetInteractionPrompt("Use Computer");
            UpdateScreenDisplay();
        }
        
        protected override void PerformEquipmentAction()
        {
            base.PerformEquipmentAction();
            
            if (!_isLoggedIn)
            {
                LoginToSystem();
            }
            else
            {
                AccessLabSystem();
            }
        }
        
        private void LoginToSystem()
        {
            _isLoggedIn = true;
            _currentUser = "Lab_User_" + System.DateTime.Now.ToString("HHmm");
            
            Debug.Log($"[ComputerWorkstation] Logged in as {_currentUser}");
            SetInteractionPrompt("Access Lab Management System");
            UpdateScreenDisplay();
        }
        
        private void AccessLabSystem()
        {
            Debug.Log("[ComputerWorkstation] Accessing Laboratory Management System...");
            Debug.Log("• Sample tracking database");
            Debug.Log("• Equipment status monitoring");
            Debug.Log("• Quality control protocols");
            Debug.Log("• Reporting system");
        }
        
        private void UpdateScreenDisplay()
        {
            if (_screenRenderer != null)
            {
                _screenRenderer.material = _isLoggedIn ? _screenOnMaterial : _screenOffMaterial;
            }
        }
    }

    /// <summary>
    /// Barcode scanner for sample identification
    /// </summary>
    public class BarcodeScanner : GrabbableObject
    {
        [Header("Scanner Settings")]
        [SerializeField] private bool _isActive = true;
        [SerializeField] private Light _scannerLight;
        [SerializeField] private AudioSource _beepSound;
        
        protected override void Awake()
        {
            base.Awake();
            SetInteractionPrompt("Pick up Barcode Scanner");
        }
        
        public override void OnInteract()
        {
            if (!IsGrabbed)
            {
                base.OnInteract();
            }
            else
            {
                ScanForSamples();
            }
        }
        
        private void ScanForSamples()
        {
            if (!_isActive) return;
            
            // Look for samples in front of scanner
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))
            {
                var sample = hit.collider.GetComponent<BloodSample>();
                if (sample != null)
                {
                    ScanSample(sample);
                }
            }
        }
        
        private void ScanSample(BloodSample sample)
        {
            // Visual feedback
            if (_scannerLight != null)
            {
                _scannerLight.enabled = true;
                Invoke(nameof(TurnOffScannerLight), 0.5f);
            }
            
            // Audio feedback
            if (_beepSound != null)
            {
                _beepSound.Play();
            }
            
            var data = sample.Data;
            Debug.Log($"[BarcodeScanner] Sample scanned: {data.sampleId}");
            Debug.Log($"• Type: {data.sampleType}");
            Debug.Log($"• Volume: {data.volume:F1}mL");
            Debug.Log($"• Quality: {data.qualityScore:F0}%");
        }
        
        private void TurnOffScannerLight()
        {
            if (_scannerLight != null)
            {
                _scannerLight.enabled = false;
            }
        }
    }
}
