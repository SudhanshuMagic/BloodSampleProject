using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BloodSample.Core;

namespace BloodSample.Systems
{
    /// <summary>
    /// Immersive guidance system for laboratory navigation and interaction
    /// </summary>
    public class GuidanceSystem : MonoBehaviour
    {
        [Header("Guidance Settings")]
        [SerializeField] private bool _enableGuidance = true;
        [SerializeField] private float _markerGlowSpeed = 2f;
        [SerializeField] private float _markerScale = 1.5f;
        
        [Header("Marker Materials")]
        [SerializeField] private Material _guidanceMarkerMaterial;
        [SerializeField] private Material _completedMarkerMaterial;
        [SerializeField] private Material _activeMarkerMaterial;
        
        [Header("UI Messages")]
        [SerializeField] private bool _showWelcomeMessages = true;
        
        private Dictionary<string, GameObject> _activeMarkers = new Dictionary<string, GameObject>();
        private List<string> _completedSteps = new List<string>();
        private string _currentStep = "";
        
        public static GuidanceSystem Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            if (_enableGuidance)
            {
                StartCoroutine(InitializeGuidanceSequence());
            }
        }
        
        private IEnumerator InitializeGuidanceSequence()
        {
            yield return new WaitForSeconds(2f); // Wait for scene setup
            
            if (_showWelcomeMessages)
            {
                ShowWelcomeMessage();
            }
            
            yield return new WaitForSeconds(3f);
            StartWorkstationGuidance();
        }
        
        private void ShowWelcomeMessage()
        {
            Debug.Log("=== WELCOME TO THE SAMPLING LABORATORY ===");
            Debug.Log("🔬 Notice your surroundings:");
            Debug.Log("• Blood sample rack with sample tubes");
            Debug.Log("• Single workstation for processing");
            Debug.Log("• Computer monitor with handling instructions");
            Debug.Log("• Barcode scanner for sample identification");
            Debug.Log("• Walk-in freezer for sample storage");
            Debug.Log("• Light fixtures for proper illumination");
            Debug.Log("");
            Debug.Log("🎮 Controls:");
            Debug.Log("• WASD - Move around the laboratory");
            Debug.Log("• Mouse - Look around (hold right-click)");
            Debug.Log("• Left-click - Select objects");
            Debug.Log("• E - Interact with equipment");
            Debug.Log("• F1 - Debug panel");
            Debug.Log("=====================================");
        }
        
        public void StartWorkstationGuidance()
        {
            _currentStep = "workstation_approach";
            
            Debug.Log("📍 GUIDANCE: Moving to Workstation");
            Debug.Log("When ready, move towards the workstation.");
            Debug.Log("Look for the glowing marker to guide you.");
            
            // Find the first workstation
            var workstation = FindFirstObjectByType<Workstation>();
            if (workstation != null)
            {
                CreateGuidanceMarker("workstation_marker", workstation.transform, "Click to proceed with workstation");
            }
        }
        
        public void CreateGuidanceMarker(string markerId, Transform target, string message)
        {
            if (!_enableGuidance) return;
            
            // Remove existing marker if it exists
            RemoveGuidanceMarker(markerId);
            
            // Create marker GameObject
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = $"GuidanceMarker_{markerId}";
            marker.transform.position = target.position + Vector3.up * 2f;
            marker.transform.localScale = Vector3.one * _markerScale;
            
            // Remove collider (visual only)
            Destroy(marker.GetComponent<Collider>());
            
            // Set material
            var renderer = marker.GetComponent<Renderer>();
            if (renderer != null && _guidanceMarkerMaterial != null)
            {
                renderer.material = _guidanceMarkerMaterial;
            }
            
            // Add glowing animation
            var glowComponent = marker.AddComponent<GuidanceMarkerGlow>();
            glowComponent.Initialize(_markerGlowSpeed, _activeMarkerMaterial);
            
            // Add interaction detection
            var markerInteraction = marker.AddComponent<GuidanceMarkerInteraction>();
            markerInteraction.Initialize(markerId, message, this);
            
            _activeMarkers[markerId] = marker;
            
            Debug.Log($"🔆 Guidance marker created: {message}");
        }
        
        public void RemoveGuidanceMarker(string markerId)
        {
            if (_activeMarkers.ContainsKey(markerId))
            {
                if (_activeMarkers[markerId] != null)
                {
                    Destroy(_activeMarkers[markerId]);
                }
                _activeMarkers.Remove(markerId);
            }
        }
        
        public void OnMarkerInteracted(string markerId)
        {
            switch (markerId)
            {
                case "workstation_marker":
                    OnWorkstationReached();
                    break;
                    
                case "equipment_tour_marker":
                    OnEquipmentTourStarted();
                    break;
                    
                case "sample_processing_marker":
                    OnSampleProcessingStarted();
                    break;
            }
        }
        
        private void OnWorkstationReached()
        {
            RemoveGuidanceMarker("workstation_marker");
            _completedSteps.Add("workstation_approach");
            
            Debug.Log("✅ Excellent! You've reached the workstation.");
            Debug.Log("🔬 This is where you'll process blood samples.");
            Debug.Log("🎯 Next: Let's explore the laboratory equipment.");
            
            StartCoroutine(StartEquipmentTour());
        }
        
        private IEnumerator StartEquipmentTour()
        {
            yield return new WaitForSeconds(2f);
            
            _currentStep = "equipment_tour";
            
            Debug.Log("📍 GUIDANCE: Laboratory Equipment Tour");
            Debug.Log("Explore the modern laboratory equipment:");
            
            // Create markers for key equipment
            CreateEquipmentMarkers();
        }
        
        private void CreateEquipmentMarkers()
        {
            // Find and mark key equipment
            var freezer = FindFirstObjectByType<WalkInFreezer>();
            if (freezer != null)
            {
                CreateGuidanceMarker("freezer_marker", freezer.transform, "Walk-in Freezer - Sample Storage");
            }
            
            var computer = FindFirstObjectByType<ComputerWorkstation>();
            if (computer != null)
            {
                CreateGuidanceMarker("computer_marker", computer.transform, "Computer System - Data Management");
            }
            
            var gloveBox = FindFirstObjectByType<LatexGlovesBox>();
            if (gloveBox != null)
            {
                CreateGuidanceMarker("gloves_marker", gloveBox.transform, "Safety Equipment - Protective Gloves");
            }
            
            var scanner = FindFirstObjectByType<BarcodeScanner>();
            if (scanner != null)
            {
                CreateGuidanceMarker("scanner_marker", scanner.transform, "Barcode Scanner - Sample Identification");
            }
            
            var monitor = GameObject.Find("InstructionalMonitor");
            if (monitor != null)
            {
                CreateGuidanceMarker("monitor_marker", monitor.transform, "Computer Monitor - Blood Sample Handling Steps");
            }
            
            Debug.Log("🔆 Multiple guidance markers created for equipment tour");
            Debug.Log("Click on any glowing marker to learn about that equipment");
            Debug.Log("📺 Check the instructional displays for detailed procedures!");
        }
        
        private void OnEquipmentTourStarted()
        {
            Debug.Log("🔬 Great! You're exploring the laboratory equipment.");
            Debug.Log("Each piece of equipment serves a specific purpose in sample processing.");
        }
        
        private void OnSampleProcessingStarted()
        {
            Debug.Log("🧪 Starting sample processing workflow...");
            Debug.Log("This is where the real laboratory work begins!");
        }
        
        public void CompleteCurrentStep()
        {
            if (!string.IsNullOrEmpty(_currentStep))
            {
                _completedSteps.Add(_currentStep);
                Debug.Log($"✅ Step completed: {_currentStep}");
            }
        }
        
        public bool IsStepCompleted(string stepName)
        {
            return _completedSteps.Contains(stepName);
        }
        
        public void ResetGuidance()
        {
            // Clear all markers
            foreach (var marker in _activeMarkers.Values)
            {
                if (marker != null)
                {
                    Destroy(marker);
                }
            }
            
            _activeMarkers.Clear();
            _completedSteps.Clear();
            _currentStep = "";
            
            Debug.Log("🔄 Guidance system reset");
        }
        
        [ContextMenu("Skip to Equipment Tour")]
        public void SkipToEquipmentTour()
        {
            RemoveGuidanceMarker("workstation_marker");
            StartCoroutine(StartEquipmentTour());
        }
        
        [ContextMenu("Show All Markers")]
        public void ShowAllEquipmentMarkers()
        {
            CreateEquipmentMarkers();
        }
    }

    /// <summary>
    /// Glowing animation component for guidance markers
    /// </summary>
    public class GuidanceMarkerGlow : MonoBehaviour
    {
        private float _glowSpeed = 2f;
        private Material _glowMaterial;
        private Renderer _renderer;
        private Color _originalColor;
        
        public void Initialize(float speed, Material material)
        {
            _glowSpeed = speed;
            _glowMaterial = material;
            _renderer = GetComponent<Renderer>();
            
            if (_renderer != null && _glowMaterial != null)
            {
                _renderer.material = _glowMaterial;
                _originalColor = _renderer.material.color;
            }
        }
        
        private void Update()
        {
            if (_renderer != null)
            {
                float glow = Mathf.Sin(Time.time * _glowSpeed) * 0.5f + 0.5f;
                Color glowColor = Color.Lerp(_originalColor, Color.white, glow);
                _renderer.material.color = glowColor;
                
                // Add floating animation
                transform.position += Vector3.up * Mathf.Sin(Time.time * _glowSpeed * 0.5f) * 0.01f;
            }
        }
    }

    /// <summary>
    /// Interaction handler for guidance markers
    /// </summary>
    public class GuidanceMarkerInteraction : MonoBehaviour, IInteractable
    {
        private string _markerId;
        private string _message;
        private GuidanceSystem _guidanceSystem;
        
        public bool CanInteract => true;
        public string InteractionPrompt => _message;
        public bool IsSelected { get; private set; }
        
        public void Initialize(string id, string message, GuidanceSystem system)
        {
            _markerId = id;
            _message = message;
            _guidanceSystem = system;
        }
        
        public void OnInteract()
        {
            Debug.Log($"🎯 Marker interaction: {_message}");
            _guidanceSystem?.OnMarkerInteracted(_markerId);
        }
        
        public void OnSelect()
        {
            IsSelected = true;
            // Add visual feedback for selection
            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.cyan;
            }
        }
        
        public void OnDeselect()
        {
            IsSelected = false;
            // Restore original color
            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.white;
            }
        }
        
        public void OnInteractionEnd()
        {
            // Optional cleanup
        }
    }
}
