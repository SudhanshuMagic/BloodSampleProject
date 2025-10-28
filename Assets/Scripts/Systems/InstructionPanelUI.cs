using UnityEngine;

namespace BloodSample.Systems
{
    /// <summary>
    /// Legacy instruction panel - replaced by SimpleInstructionSystem
    /// This file is kept for compatibility but functionality moved to SimpleInstructionSystem
    /// </summary>
    public class InstructionPanelUI : MonoBehaviour
    {
        [Header("Legacy - Use SimpleInstructionSystem Instead")]
        [SerializeField] private bool _legacyComponent = true;
        
        private void Start()
        {
            Debug.LogWarning("[InstructionPanelUI] This component is deprecated. Please use SimpleInstructionSystem instead.");
            
            // Try to find and use SimpleInstructionSystem instead
            var simpleSystem = FindObjectOfType<SimpleInstructionSystem>();
            if (simpleSystem == null)
            {
                Debug.Log("[InstructionPanelUI] Creating SimpleInstructionSystem automatically...");
                GameObject newSystem = new GameObject("SimpleInstructionSystem_Auto");
                newSystem.AddComponent<SimpleInstructionSystem>();
            }
        }
    }
}
