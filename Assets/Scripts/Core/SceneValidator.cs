using UnityEngine;

namespace BloodSample.Core
{
    /// <summary>
    /// Validates the scene on startup and ensures all required components exist
    /// Provides automatic setup guarantee regardless of scene state
    /// </summary>
    public class SceneValidator : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void ValidateScene()
        {
            // Wait one frame to ensure all objects are loaded
            GameObject validator = new GameObject("SceneValidator_Temp");
            validator.AddComponent<SceneValidatorComponent>();
        }
    }
    
    public class SceneValidatorComponent : MonoBehaviour
    {
        private void Start()
        {
            // Validate after one frame
            Invoke(nameof(PerformValidation), 0.1f);
        }
        
        private void PerformValidation()
        {
            Debug.Log("🔍 SceneValidator: Checking scene setup...");
            
            // Check for essential components
            bool hasGameManager = FindObjectOfType<GameManager>() != null;
            bool hasInputManager = FindObjectOfType<InputManager>() != null;
            bool hasAutoSetup = FindObjectOfType<AutoSetupManager>() != null;
            bool hasCamera = Camera.main != null;
            
            Debug.Log($"📊 Scene Status: GameManager={hasGameManager}, InputManager={hasInputManager}, AutoSetup={hasAutoSetup}, Camera={hasCamera}");
            
            // If we're missing essential components, create emergency setup
            if (!hasGameManager || !hasInputManager || !hasAutoSetup)
            {
                CreateEmergencySetup();
            }
            
            // Cleanup this validator
            Destroy(gameObject);
        }
        
        private void CreateEmergencySetup()
        {
            Debug.Log("🚨 SceneValidator: Creating emergency laboratory setup!");
            
            GameObject emergencySetup = new GameObject("EmergencyAutoSetup");
            emergencySetup.AddComponent<AutoSetupManager>();
        }
    }
}
