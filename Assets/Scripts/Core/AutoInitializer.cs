using UnityEngine;
using UnityEngine.SceneManagement;

namespace BloodSample.Core
{
    /// <summary>
    /// ZERO-SETUP AUTOMATIC INITIALIZER
    /// This script automatically runs when Unity starts - NO MANUAL SETUP REQUIRED!
    /// Just import the scripts and press Play - everything happens automatically!
    /// </summary>
    public static class AutoInitializer
    {
        private static bool _hasInitialized = false;
        
        /// <summary>
        /// This method runs automatically when Unity starts - NO user action required!
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeOnLoad()
        {
            if (_hasInitialized) return;
            
            Debug.Log("🚀 [AutoInitializer] Starting ZERO-SETUP automatic initialization...");
            
            // Subscribe to scene loaded event to ensure we run after scene is ready
            SceneManager.sceneLoaded += OnSceneLoaded;
            _hasInitialized = true;
        }
        
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Only run in Play mode and for main scenes (not loading screens, etc.)
            if (!Application.isPlaying) return;
            
            Debug.Log($"🎬 [AutoInitializer] Scene '{scene.name}' loaded - checking for auto-setup...");
            
            // Check if we already have a setup manager
            AutoSetupManager existingSetup = Object.FindObjectOfType<AutoSetupManager>();
            if (existingSetup != null)
            {
                Debug.Log("✅ [AutoInitializer] AutoSetupManager already exists - skipping creation");
                return;
            }
            
            // Check if scene already has laboratory elements
            if (HasExistingLaboratorySetup())
            {
                Debug.Log("✅ [AutoInitializer] Laboratory already set up - skipping auto-setup");
                return;
            }
            
            // Create and initialize the automatic setup
            CreateAutoSetupManager();
        }
        
        private static bool HasExistingLaboratorySetup()
        {
            // Check for existing core managers or laboratory elements
            return Object.FindObjectOfType<GameManager>() != null ||
                   Object.FindObjectOfType<LaboratorySetup>() != null ||
                   Object.FindObjectsOfType<BloodSample.Systems.BloodSample>().Length > 0;
        }
        
        private static void CreateAutoSetupManager()
        {
            Debug.Log("🏗️ [AutoInitializer] Creating automatic setup manager...");
            
            // Create the setup manager GameObject
            GameObject autoSetupObj = new GameObject("🧪 AUTO-SETUP MANAGER (Generated)");
            
            // Add the AutoSetupManager component
            AutoSetupManager setupManager = autoSetupObj.AddComponent<AutoSetupManager>();
            
            // Enable auto setup (it should be enabled by default, but ensure it)
            var enableAutoSetupField = typeof(AutoSetupManager).GetField("_enableAutoSetup", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (enableAutoSetupField != null)
            {
                enableAutoSetupField.SetValue(setupManager, true);
            }
            
            // Set up progress display
            var showProgressField = typeof(AutoSetupManager).GetField("_showSetupProgress", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (showProgressField != null)
            {
                showProgressField.SetValue(setupManager, true);
            }
            
            Debug.Log("✨ [AutoInitializer] AUTO-SETUP MANAGER CREATED! Laboratory will build automatically!");
            Debug.Log("🎮 [AutoInitializer] NO FURTHER ACTION NEEDED - Just wait for setup to complete!");
        }
        
        /// <summary>
        /// Alternative initialization method that can be called manually if needed
        /// But normally this runs automatically!
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInitialization()
        {
            // Double-check initialization after scene is fully loaded
            if (!Application.isPlaying) return;
            
            // Wait a frame then check if setup is running
            CoroutineRunner.StartCoroutine(CheckSetupAfterFrame());
        }
        
        private static System.Collections.IEnumerator CheckSetupAfterFrame()
        {
            yield return null; // Wait one frame
            
            AutoSetupManager setupManager = Object.FindObjectOfType<AutoSetupManager>();
            if (setupManager == null && !HasExistingLaboratorySetup())
            {
                Debug.LogWarning("⚠️ [AutoInitializer] Setup manager not found - creating fallback setup...");
                CreateAutoSetupManager();
            }
            else if (setupManager != null)
            {
                Debug.Log("✅ [AutoInitializer] Setup manager confirmed active!");
            }
            else
            {
                Debug.Log("✅ [AutoInitializer] Laboratory already exists - setup not needed");
            }
        }
    }
    
    /// <summary>
    /// Helper class to run coroutines from static context
    /// </summary>
    internal class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _instance;
        
        public static CoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject runnerObj = new GameObject("CoroutineRunner");
                    _instance = runnerObj.AddComponent<CoroutineRunner>();
                    Object.DontDestroyOnLoad(runnerObj);
                }
                return _instance;
            }
        }
        
        public static Coroutine StartCoroutine(System.Collections.IEnumerator routine)
        {
            return Instance.StartCoroutine(routine);
        }
    }
}
