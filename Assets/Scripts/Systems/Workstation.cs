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
    }
}
