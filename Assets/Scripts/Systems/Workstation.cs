using UnityEngine;
using BloodSample.Core;

namespace BloodSample.Systems
{
    /// <summary>
    /// Laboratory workstation for processing blood samples
    /// </summary>
    public class Workstation : InteractableObject
    {
        [Header("Workstation Configuration")]
        [SerializeField] private int _workstationId;
        [SerializeField] private Transform[] _sampleSlots;
        [SerializeField] private float _processingTime = 5f;
        
        private BloodSample[] _currentSamples;
        private bool _isProcessing = false;
        
        protected override void Awake()
        {
            base.Awake();
            _currentSamples = new BloodSample[_sampleSlots?.Length ?? 0];
            SetInteractionPrompt($"Use Workstation {_workstationId + 1}");
        }
        
        public override void OnInteract()
        {
            base.OnInteract();
            
            if (_isProcessing)
            {
                Debug.Log($"[Workstation] Workstation {_workstationId + 1} is currently processing samples");
                return;
            }
            
            Debug.Log($"[Workstation] Interacting with Workstation {_workstationId + 1}");
            ProcessSamples();
        }
        
        private void ProcessSamples()
        {
            int sampleCount = 0;
            for (int i = 0; i < _currentSamples.Length; i++)
            {
                if (_currentSamples[i] != null)
                {
                    sampleCount++;
                }
            }
            
            if (sampleCount > 0)
            {
                Debug.Log($"[Workstation] Processing {sampleCount} samples...");
                _isProcessing = true;
                Invoke(nameof(CompleteProcessing), _processingTime);
            }
            else
            {
                Debug.Log($"[Workstation] No samples to process at Workstation {_workstationId + 1}");
            }
        }
        
        private void CompleteProcessing()
        {
            _isProcessing = false;
            Debug.Log($"[Workstation] Sample processing complete at Workstation {_workstationId + 1}");
        }
        
        public bool CanPlaceSample()
        {
            return !_isProcessing && HasEmptySlot();
        }
        
        private bool HasEmptySlot()
        {
            for (int i = 0; i < _currentSamples.Length; i++)
            {
                if (_currentSamples[i] == null)
                {
                    return true;
                }
            }
            return false;
        }
        
        public void SetWorkstationId(int id)
        {
            _workstationId = id;
            SetInteractionPrompt($"Use Workstation {_workstationId + 1}");
        }
        
        public int GetWorkstationId()
        {
            return _workstationId;
        }
        
        public Transform[] GetSampleSlots()
        {
            return _sampleSlots;
        }
        
        /// <summary>
        /// Automatically detect and place samples dropped anywhere on the workstation
        /// </summary>
        private void Update()
        {
            if (_isProcessing) return;
            
            // Check for samples anywhere on the workstation surface
            Vector3 workstationCenter = transform.position + Vector3.up * 0.5f; // Above the table surface
            Collider[] nearbyObjects = Physics.OverlapBox(
                workstationCenter, 
                new Vector3(1.2f, 0.3f, 0.7f), // Covers entire workstation surface
                transform.rotation
            );
            
            foreach (Collider col in nearbyObjects)
            {
                var bloodSample = col.GetComponent<BloodSample>();
                if (bloodSample != null)
                {
                    // Check if this sample isn't already placed in a slot
                    bool alreadyPlaced = false;
                    for (int j = 0; j < _currentSamples.Length; j++)
                    {
                        if (_currentSamples[j] == bloodSample)
                        {
                            alreadyPlaced = true;
                            break;
                        }
                    }
                    
                    if (!alreadyPlaced)
                    {
                        // Check if sample is moving too fast (just dropped)
                        Rigidbody sampleRb = bloodSample.GetComponent<Rigidbody>();
                        if (sampleRb != null && sampleRb.velocity.magnitude > 0.5f)
                        {
                            // Sample is still moving, wait a bit
                            continue;
                        }
                        
                        // Find the next available slot
                        int availableSlot = FindNextAvailableSlot();
                        if (availableSlot >= 0)
                        {
                            PlaceSampleInSlot(bloodSample, availableSlot);
                            Debug.Log($"📦 [Workstation] Sample detected anywhere on workstation surface - auto-assigning to slot {availableSlot + 1}");
                        }
                        else
                        {
                            Debug.LogWarning("[Workstation] No available slots - workstation is full");
                        }
                        break; // Only process one sample per frame
                    }
                }
            }
        }
        
        /// <summary>
        /// Find the next available slot for sample placement
        /// </summary>
        private int FindNextAvailableSlot()
        {
            for (int i = 0; i < _currentSamples.Length; i++)
            {
                if (_currentSamples[i] == null)
                {
                    return i;
                }
            }
            return -1; // No available slots
        }
        
        /// <summary>
        /// Place a blood sample in a specific slot
        /// </summary>
        private void PlaceSampleInSlot(BloodSample sample, int slotIndex)
        {
            if (slotIndex >= 0 && slotIndex < _currentSamples.Length && _currentSamples[slotIndex] == null)
            {
                _currentSamples[slotIndex] = sample;
                
                // Snap the sample to the slot position with slight adjustment for visibility
                sample.transform.position = _sampleSlots[slotIndex].position + Vector3.up * 0.1f;
                sample.transform.rotation = _sampleSlots[slotIndex].rotation;
                
                // Make it kinematic so it stays in place and reset velocities
                Rigidbody sampleRb = sample.GetComponent<Rigidbody>();
                if (sampleRb != null)
                {
                    sampleRb.velocity = Vector3.zero;
                    sampleRb.angularVelocity = Vector3.zero;
                    sampleRb.isKinematic = true;
                }
                
                // Change sample color to red to indicate verification
                ChangeSampleColorToRed(sample);
                
                // Notify computer screen about sample verification
                NotifyComputerScreenSampleVerified(sample);
                
                Debug.Log($"🔴 [VERIFICATION SUCCESS] Sample {sample.name} verified on middle workstation!");
                Debug.Log($"✅ Visual: Blood liquid turned RED | 💻 Computer: 'SAMPLE VERIFIED' message displayed");
                
                // Check if we can start processing
                CheckForAutoProcessing();
            }
        }
        
        /// <summary>
        /// Check if we should automatically start processing
        /// </summary>
        private void CheckForAutoProcessing()
        {
            int samplesPlaced = 0;
            for (int i = 0; i < _currentSamples.Length; i++)
            {
                if (_currentSamples[i] != null) samplesPlaced++;
            }
            
            if (samplesPlaced > 0)
            {
                Debug.Log($"[Workstation] {samplesPlaced} samples ready for processing. Press E to process or wait for auto-processing.");
                
                // Auto-process after a short delay if user doesn't manually trigger
                if (!_isProcessing)
                {
                    Invoke(nameof(AutoProcessSamples), 3f);
                }
            }
        }
        
        /// <summary>
        /// Automatically process samples if user hasn't manually started
        /// </summary>
        private void AutoProcessSamples()
        {
            if (!_isProcessing)
            {
                Debug.Log("[Workstation] Auto-processing samples...");
                ProcessSamples();
            }
        }
        
        /// <summary>
        /// Remove a sample from its slot (for manual removal)
        /// </summary>
        public void RemoveSampleFromSlot(BloodSample sample)
        {
            for (int i = 0; i < _currentSamples.Length; i++)
            {
                if (_currentSamples[i] == sample)
                {
                    _currentSamples[i] = null;
                    
                    // Re-enable physics
                    Rigidbody sampleRb = sample.GetComponent<Rigidbody>();
                    if (sampleRb != null)
                    {
                        sampleRb.isKinematic = false;
                    }
                    
                    Debug.Log($"[Workstation] Sample {sample.name} removed from slot {i + 1}");
                    break;
                }
            }
        }
        
        /// <summary>
        /// Change the blood sample color to red to indicate verification
        /// </summary>
        private void ChangeSampleColorToRed(BloodSample sample)
        {
            // Find the liquid component (blood inside the tube)
            Transform liquidChild = sample.transform.Find(sample.name + "_Liquid");
            if (liquidChild != null)
            {
                Renderer liquidRenderer = liquidChild.GetComponent<Renderer>();
                if (liquidRenderer != null)
                {
                    // Create red material for verified blood
                    Material redMaterial = new Material(Shader.Find("Standard"));
                    redMaterial.name = "VerifiedBlood_Material";
                    redMaterial.color = new Color(0.8f, 0.1f, 0.1f, 1f); // Bright red
                    redMaterial.SetFloat("_Metallic", 0.2f);
                    redMaterial.SetFloat("_Smoothness", 0.6f);
                    
                    liquidRenderer.material = redMaterial;
                    Debug.Log($"🔴 [Workstation] SUCCESS: {sample.name} liquid changed to BRIGHT RED - Sample verified!");
                }
            }
        }
        
        [ContextMenu("Test Verification System")]
        public void TestVerificationSystem()
        {
            Debug.Log("[Workstation] Testing verification system...");
            
            // Find a sample to test with
            var testSample = FindFirstObjectByType<BloodSample>();
            if (testSample != null)
            {
                ChangeSampleColorToRed(testSample);
                NotifyComputerScreenSampleVerified(testSample);
                Debug.Log("[Workstation] Verification system test completed");
            }
            else
            {
                Debug.LogWarning("[Workstation] No blood sample found for testing");
            }
        }
        
        [ContextMenu("Show Detection Area")]
        public void ShowDetectionArea()
        {
            Vector3 workstationCenter = transform.position + Vector3.up * 0.5f;
            Vector3 boxSize = new Vector3(1.2f, 0.3f, 0.7f);
            
            Debug.Log($"[Workstation] Detection area covers entire workstation surface:");
            Debug.Log($"Center: {workstationCenter}");
            Debug.Log($"Size: {boxSize} (Width: {boxSize.x * 2}m, Height: {boxSize.y * 2}m, Depth: {boxSize.z * 2}m)");
            Debug.Log("Drop blood samples ANYWHERE within this area for automatic verification!");
        }
        
        /// <summary>
        /// Notify the computer screen about sample verification
        /// </summary>
        private void NotifyComputerScreenSampleVerified(BloodSample sample)
        {
            // Find the computer screen in the scene
            var computerMonitor = FindFirstObjectByType<InstructionalDisplay>();
            if (computerMonitor != null)
            {
                // Update the computer screen with verification message
                computerMonitor.ShowVerificationMessage($"SAMPLE VERIFIED\n\nSample ID: {sample.name}\nWorkstation: Central (Middle)\nStatus: VERIFIED\nTime: {System.DateTime.Now:HH:mm:ss}\n\nReady for processing...");
                Debug.Log($"💻 [Computer Monitor] Displaying verification message for {sample.name} on central workstation");
            }
            else
            {
                Debug.LogWarning("[Workstation] No computer screen found to display verification message");
            }
        }
    }
}
