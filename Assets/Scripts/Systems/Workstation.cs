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
        /// Automatically detect and place samples dropped near the workstation
        /// </summary>
        private void Update()
        {
            if (_isProcessing) return;
            
            // Check for samples near the workstation slots
            for (int i = 0; i < _sampleSlots.Length && i < _currentSamples.Length; i++)
            {
                if (_currentSamples[i] == null && _sampleSlots[i] != null)
                {
                    // Look for samples within range of this slot
                    Collider[] nearbyObjects = Physics.OverlapSphere(_sampleSlots[i].position, 1f);
                    
                    foreach (Collider col in nearbyObjects)
                    {
                        var bloodSample = col.GetComponent<BloodSample>();
                        if (bloodSample != null)
                        {
                            // Check if this sample isn't already placed in another slot
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
                                PlaceSampleInSlot(bloodSample, i);
                                break;
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Place a blood sample in a specific slot
        /// </summary>
        private void PlaceSampleInSlot(BloodSample sample, int slotIndex)
        {
            if (slotIndex >= 0 && slotIndex < _currentSamples.Length && _currentSamples[slotIndex] == null)
            {
                _currentSamples[slotIndex] = sample;
                
                // Snap the sample to the slot position
                sample.transform.position = _sampleSlots[slotIndex].position;
                sample.transform.rotation = _sampleSlots[slotIndex].rotation;
                
                // Make it kinematic so it stays in place
                Rigidbody sampleRb = sample.GetComponent<Rigidbody>();
                if (sampleRb != null)
                {
                    sampleRb.isKinematic = true;
                }
                
                Debug.Log($"[Workstation] Sample {sample.name} placed in slot {slotIndex + 1}");
                
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
    }
}
