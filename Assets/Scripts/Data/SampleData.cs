using UnityEngine;
using System;

namespace BloodSample.Data
{
    [System.Serializable]
    public class SampleData
    {
        [Header("Sample Identification")]
        public string sampleId;
        public string patientId;
        public DateTime collectionDate;
        public SampleType sampleType;
        
        [Header("Sample Properties")]
        public float volume; // in mL
        public float temperature; // in Celsius
        public SampleState currentState;
        public BloodType bloodType;
        
        [Header("Quality Control")]
        public bool isContaminated;
        public bool isExpired;
        public float qualityScore; // 0-100
        
        [Header("Processing History")]
        public ProcessingStep[] processingHistory;
        
        public SampleData()
        {
            sampleId = GenerateSampleId();
            collectionDate = DateTime.Now;
            sampleType = SampleType.WholeBlood;
            volume = 5.0f;
            temperature = 37.0f; // Normal body temperature
            currentState = SampleState.Fresh;
            bloodType = BloodType.Unknown;
            isContaminated = false;
            isExpired = false;
            qualityScore = 100f;
            processingHistory = new ProcessingStep[0];
        }
        
        private string GenerateSampleId()
        {
            return $"BS-{DateTime.Now:yyyyMMdd}-{UnityEngine.Random.Range(1000, 9999)}";
        }
        
        public bool IsViable()
        {
            return !isContaminated && !isExpired && qualityScore > 50f;
        }
        
        public void AddProcessingStep(ProcessingStep step)
        {
            ProcessingStep[] newHistory = new ProcessingStep[processingHistory.Length + 1];
            System.Array.Copy(processingHistory, newHistory, processingHistory.Length);
            newHistory[processingHistory.Length] = step;
            processingHistory = newHistory;
        }
    }
    
    [System.Serializable]
    public struct ProcessingStep
    {
        public string stepName;
        public DateTime timestamp;
        public string notes;
        
        public ProcessingStep(string name, string stepNotes = "")
        {
            stepName = name;
            timestamp = DateTime.Now;
            notes = stepNotes;
        }
    }
    
    public enum SampleType
    {
        WholeBlood,
        Plasma,
        Serum,
        RedBloodCells,
        WhiteBloodCells,
        Platelets
    }
    
    public enum SampleState
    {
        Fresh,
        Refrigerated,
        Frozen,
        Processing,
        Analyzed,
        Disposed
    }
    
    public enum BloodType
    {
        Unknown,
        A_Positive,
        A_Negative,
        B_Positive,
        B_Negative,
        AB_Positive,
        AB_Negative,
        O_Positive,
        O_Negative
    }
}
