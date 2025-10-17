using UnityEngine;
using BloodSample.Core;
using BloodSample.Data;

namespace BloodSample.Systems
{
    public class BloodSample : GrabbableObject
    {
        [Header("Sample Visualization")]
        [SerializeField] private Material[] _sampleMaterials;
        [SerializeField] private Renderer _liquidRenderer;
        [SerializeField] private Transform _liquidLevel;
        
        [Header("Sample Data")]
        [SerializeField] private SampleData _sampleData;
        
        public SampleData Data => _sampleData;
        
        protected override void Awake()
        {
            base.Awake();
            
            if (_sampleData == null)
            {
                _sampleData = new SampleData();
            }
            
            UpdateVisuals();
            SetInteractionPrompt($"Sample: {_sampleData.sampleId}");
        }
        
        private void Start()
        {
            // Add initial processing step
            _sampleData.AddProcessingStep(new ProcessingStep("Sample Created", "Initial blood sample creation"));
        }
        
        public override void OnInteract()
        {
            base.OnInteract();
            DisplaySampleInfo();
        }
        
        private void DisplaySampleInfo()
        {
            Debug.Log($"=== Sample Information ===");
            Debug.Log($"ID: {_sampleData.sampleId}");
            Debug.Log($"Type: {_sampleData.sampleType}");
            Debug.Log($"Volume: {_sampleData.volume}mL");
            Debug.Log($"State: {_sampleData.currentState}");
            Debug.Log($"Quality: {_sampleData.qualityScore}%");
            Debug.Log($"Viable: {(_sampleData.IsViable() ? "Yes" : "No")}");
        }
        
        private void UpdateVisuals()
        {
            if (_liquidRenderer != null && _sampleMaterials != null && _sampleMaterials.Length > 0)
            {
                int materialIndex = (int)_sampleData.sampleType % _sampleMaterials.Length;
                _liquidRenderer.material = _sampleMaterials[materialIndex];
            }
            
            if (_liquidLevel != null)
            {
                // Scale liquid level based on volume (assuming max volume of 10mL)
                float normalizedVolume = Mathf.Clamp01(_sampleData.volume / 10f);
                Vector3 scale = _liquidLevel.localScale;
                scale.y = normalizedVolume;
                _liquidLevel.localScale = scale;
            }
        }
        
        public void ProcessSample(ProcessingStep step)
        {
            _sampleData.AddProcessingStep(step);
            
            // Update state based on processing
            switch (step.stepName.ToLower())
            {
                case "centrifuge":
                    if (_sampleData.sampleType == SampleType.WholeBlood)
                    {
                        _sampleData.sampleType = SampleType.Plasma;
                        _sampleData.currentState = SampleState.Processing;
                    }
                    break;
                    
                case "refrigerate":
                    _sampleData.currentState = SampleState.Refrigerated;
                    _sampleData.temperature = 4f; // Refrigeration temperature
                    break;
                    
                case "freeze":
                    _sampleData.currentState = SampleState.Frozen;
                    _sampleData.temperature = -20f; // Freezer temperature
                    break;
                    
                case "analyze":
                    _sampleData.currentState = SampleState.Analyzed;
                    break;
            }
            
            UpdateVisuals();
            Debug.Log($"Sample {_sampleData.sampleId} processed: {step.stepName}");
        }
        
        public void SetSampleType(SampleType newType)
        {
            _sampleData.sampleType = newType;
            UpdateVisuals();
        }
        
        public void SetVolume(float newVolume)
        {
            _sampleData.volume = Mathf.Clamp(newVolume, 0f, 10f);
            UpdateVisuals();
        }
        
        public void ContaminateSample()
        {
            _sampleData.isContaminated = true;
            _sampleData.qualityScore *= 0.1f; // Drastically reduce quality
            Debug.LogWarning($"Sample {_sampleData.sampleId} has been contaminated!");
        }
        
        public bool CanBeProcessed()
        {
            return _sampleData.IsViable() && _sampleData.currentState != SampleState.Disposed;
        }
    }
}
