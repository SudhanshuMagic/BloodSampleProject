using UnityEngine;

namespace BloodSample.Core
{
    /// <summary>
    /// ULTIMATE ZERO-SETUP BOOTSTRAP
    /// This ensures the laboratory is created even in completely empty scenes
    /// Automatically detects and creates everything needed - NO EXCEPTIONS!
    /// </summary>
    [DefaultExecutionOrder(-1000)] // Run very early
    public class ZeroSetupBootstrap : MonoBehaviour
    {
        // This class exists as a failsafe but doesn't need to be manually added
        // The AutoInitializer handles everything automatically
        
        private void Awake()
        {
            // This should never run if AutoInitializer works correctly
            // But it's here as a failsafe just in case
            Debug.Log("🔄 [ZeroSetupBootstrap] Failsafe bootstrap running...");
            
            if (FindFirstObjectByType<AutoSetupManager>() == null)
            {
                Debug.Log("🚨 [ZeroSetupBootstrap] No AutoSetupManager found - creating emergency setup!");
                CreateEmergencySetup();
            }
        }
        
        private void CreateEmergencySetup()
        {
            GameObject emergencySetup = new GameObject("🚨 EMERGENCY AUTO-SETUP");
            emergencySetup.AddComponent<AutoSetupManager>();
            Debug.Log("✅ [ZeroSetupBootstrap] Emergency setup created!");
        }
    }
    
    /// <summary>
    /// Editor-time initialization to ensure scripts are properly set up
    /// This runs in the Unity Editor to prepare for automatic setup
    /// </summary>
    #if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
    public static class EditorBootstrap
    {
        static EditorBootstrap()
        {
            // This runs when Unity Editor starts
            Debug.Log("🛠️ [EditorBootstrap] Editor initialized - Auto-setup system ready!");
            
            // Subscribe to play mode changes
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }
        
        private static void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange state)
        {
            switch (state)
            {
                case UnityEditor.PlayModeStateChange.EnteredPlayMode:
                    Debug.Log("▶️ [EditorBootstrap] Entered Play Mode - Auto-setup will activate!");
                    break;
                    
                case UnityEditor.PlayModeStateChange.ExitingPlayMode:
                    Debug.Log("⏹️ [EditorBootstrap] Exiting Play Mode");
                    break;
            }
        }
    }
    #endif
}
