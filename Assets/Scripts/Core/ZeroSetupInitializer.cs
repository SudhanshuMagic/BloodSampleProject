using UnityEngine;
using UnityEngine.SceneManagement;

namespace BloodSample.Core
{
    /// <summary>
    /// ZERO SETUP REQUIRED! This runs automatically when you press Play.
    /// No GameObject creation, no script attachment needed - just press Play!
    /// </summary>
    public static class ZeroSetupInitializer
    {
        private static bool _hasInitialized = false;
        
        /// <summary>
        /// This method runs AUTOMATICALLY when you press Play - no setup required!
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void AutoInitializeLaboratory()
        {
            // Prevent multiple initializations
            if (_hasInitialized) return;
            _hasInitialized = true;
            
            Debug.Log("🚀 <color=cyan>[ZERO SETUP]</color> Auto-initializing Blood Sample Laboratory...");
            
            // Check if we're in a scene that needs auto-setup
            Scene currentScene = SceneManager.GetActiveScene();
            
            // Only auto-setup if no AutoSetupManager already exists
            if (Object.FindObjectOfType<AutoSetupManager>() == null)
            {
                CreateAutoSetupManager();
            }
            
            Debug.Log("✅ <color=green>[ZERO SETUP]</color> Laboratory initialization complete!");
        }
        
        private static void CreateAutoSetupManager()
        {
            // Create the setup manager automatically
            GameObject setupManager = new GameObject("[AUTO] Laboratory Setup Manager");
            
            // Add the auto setup component
            AutoSetupManager autoSetup = setupManager.AddComponent<AutoSetupManager>();
            
            // Configure for automatic execution
            var enableAutoSetupField = typeof(AutoSetupManager).GetField("_enableAutoSetup", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (enableAutoSetupField != null)
            {
                enableAutoSetupField.SetValue(autoSetup, true);
            }
            
            var showProgressField = typeof(AutoSetupManager).GetField("_showSetupProgress", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (showProgressField != null)
            {
                showProgressField.SetValue(autoSetup, true);
            }
            
            Debug.Log("🎯 <color=yellow>[ZERO SETUP]</color> Auto-setup manager created and activated!");
        }
        
        /// <summary>
        /// Alternative method that runs even earlier in the initialization process
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void EarlyInitialization()
        {
            Debug.Log("⚡ <color=magenta>[ZERO SETUP]</color> Early initialization - preparing systems...");
            
            // Set up any early configurations here
            Application.targetFrameRate = 60;
            
            // Ensure quality settings are appropriate
            QualitySettings.vSyncCount = 1;
        }
    }
}
