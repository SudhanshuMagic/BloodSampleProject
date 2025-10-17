using UnityEngine;

namespace BloodSample.Core
{
    /// <summary>
    /// Automatically creates and runs the laboratory setup when Unity starts
    /// NO MANUAL SETUP REQUIRED - Just press Play!
    /// </summary>
    public static class AutoBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoInitialize()
        {
            // Check if we're in a scene that should auto-setup
            if (ShouldAutoSetup())
            {
                // Create the auto-setup manager automatically
                GameObject autoSetupObj = new GameObject("AutoSetup_Bootstrap");
                AutoSetupManager setupManager = autoSetupObj.AddComponent<AutoSetupManager>();
                
                // Make it persistent and mark as DontDestroyOnLoad if needed
                Object.DontDestroyOnLoad(autoSetupObj);
                
                Debug.Log("🚀 AutoBootstrap: Laboratory setup will begin automatically!");
            }
        }
        
        private static bool ShouldAutoSetup()
        {
            // Auto-setup in any scene that doesn't already have core managers
            GameManager existingGameManager = Object.FindObjectOfType<GameManager>();
            AutoSetupManager existingAutoSetup = Object.FindObjectOfType<AutoSetupManager>();
            
            // Only auto-setup if we don't already have these components
            return existingGameManager == null && existingAutoSetup == null;
        }
    }
    
    /// <summary>
    /// Alternative bootstrap that runs when entering Play mode
    /// This ensures setup happens even if the above method doesn't trigger
    /// </summary>
    [System.Serializable]
    public class PlayModeBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnEnterPlayMode()
        {
            // Double-check that we have auto-setup running
            AutoSetupManager existingSetup = Object.FindObjectOfType<AutoSetupManager>();
            
            if (existingSetup == null)
            {
                // Create backup auto-setup
                GameObject autoSetupObj = new GameObject("AutoSetup_PlayMode");
                autoSetupObj.AddComponent<AutoSetupManager>();
                
                Debug.Log("🔄 PlayModeBootstrap: Creating fallback laboratory setup!");
            }
        }
    }
}
