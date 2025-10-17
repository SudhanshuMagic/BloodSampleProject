using UnityEngine;
using BloodSample.Systems;
using BloodSample.Data;
using System.Collections;

namespace BloodSample.Core
{
    /// <summary>
    /// Smart Auto-Setup that detects empty scenes and automatically creates the laboratory
    /// This version is even more intelligent and requires ZERO user interaction
    /// </summary>
    public class SmartAutoSetup : MonoBehaviour
    {
        [Header("Smart Detection")]
        [SerializeField] private bool _autoDetectEmptyScene = true;
        [SerializeField] private bool _overrideExistingSetup = false;
        
        private static bool _globalInitialized = false;
        
        private void Awake()
        {
            // This runs automatically when the script exists anywhere in the scene
            if (!_globalInitialized)
            {
                StartCoroutine(SmartInitialization());
            }
        }
        
        private IEnumerator SmartInitialization()
        {
            _globalInitialized = true;
            
            Debug.Log("🧠 <color=cyan>[SMART SETUP]</color> Analyzing scene...");
            
            yield return new WaitForEndOfFrame(); // Let scene fully load
            
            if (ShouldAutoSetup())
            {
                Debug.Log("🎯 <color=green>[SMART SETUP]</color> Empty scene detected - auto-creating laboratory!");
                yield return StartCoroutine(CreateCompleteLaboratory());
            }
            else
            {
                Debug.Log("ℹ️ <color=yellow>[SMART SETUP]</color> Scene already has content - skipping auto-setup");
            }
        }
        
        private bool ShouldAutoSetup()
        {
            // Check if scene is essentially empty and needs setup
            
            // If override is enabled, always setup
            if (_overrideExistingSetup) return true;
            
            // Check for existing laboratory components
            if (FindObjectOfType<BloodSample>() != null) return false;
            if (FindObjectOfType<Workstation>() != null) return false;
            if (FindObjectOfType<AutoSetupManager>() != null) return false;
            
            // Check for minimal scene objects (ignore cameras, lights, UI)
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            int meaningfulObjects = 0;
            
            foreach (GameObject obj in allObjects)
            {
                // Skip system objects
                if (obj.GetComponent<Camera>() != null) continue;
                if (obj.GetComponent<Light>() != null) continue;
                if (obj.GetComponent<Canvas>() != null) continue;
                if (obj.name.Contains("EventSystem")) continue;
                if (obj.name.Contains("Main Camera")) continue;
                if (obj.name.Contains("Directional Light")) continue;
                if (obj == gameObject) continue; // Skip self
                
                meaningfulObjects++;
            }
            
            Debug.Log($"🔍 <color=cyan>[SMART SETUP]</color> Found {meaningfulObjects} meaningful objects in scene");
            
            // If scene has fewer than 3 meaningful objects, it's probably empty
            return meaningfulObjects < 3;
        }
        
        private IEnumerator CreateCompleteLaboratory()
        {
            Debug.Log("🏗️ <color=green>[SMART SETUP]</color> Building laboratory from scratch...");
            
            // Create the full auto-setup system
            GameObject setupObj = new GameObject("[SMART] Laboratory Auto-Builder");
            AutoSetupManager autoSetup = setupObj.AddComponent<AutoSetupManager>();
            
            // Wait for the auto-setup to complete
            yield return new WaitForSeconds(0.5f);
            
            // Add additional smart features
            yield return StartCoroutine(AddSmartEnhancements());
            
            Debug.Log("✨ <color=magenta>[SMART SETUP]</color> Smart laboratory creation complete!");
        }
        
        private IEnumerator AddSmartEnhancements()
        {
            Debug.Log("⚡ <color=yellow>[SMART SETUP]</color> Adding smart enhancements...");
            
            // Ensure we have a proper camera setup
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                mainCam = FindObjectOfType<Camera>();
            }
            
            if (mainCam != null && mainCam.GetComponent<CameraController>() == null)
            {
                mainCam.gameObject.AddComponent<CameraController>();
                Debug.Log("📷 Smart Setup: Added camera controller");
            }
            
            // Add runtime prefab generator for dynamic content
            if (FindObjectOfType<RuntimePrefabGenerator>() == null)
            {
                GameObject generatorObj = new GameObject("[SMART] Runtime Generator");
                generatorObj.AddComponent<RuntimePrefabGenerator>();
                Debug.Log("🔧 Smart Setup: Added runtime prefab generator");
            }
            
            // Add debug helper if not present
            if (FindObjectOfType<DebugHelper>() == null)
            {
                GameObject debugObj = new GameObject("[SMART] Debug Helper");
                DebugHelper debugHelper = debugObj.AddComponent<DebugHelper>();
                
                // Auto-configure debug helper with any existing sample prefab
                var samples = FindObjectsOfType<BloodSample>();
                if (samples.Length > 0)
                {
                    var prefabField = typeof(DebugHelper).GetField("_samplePrefab", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (prefabField != null && samples[0] != null)
                    {
                        prefabField.SetValue(debugHelper, samples[0].gameObject);
                    }
                }
                
                Debug.Log("🐛 Smart Setup: Added debug helper with auto-configuration");
            }
            
            yield return new WaitForSeconds(0.1f);
        }
        
        /// <summary>
        /// Can be called manually to force smart setup
        /// </summary>
        [ContextMenu("Force Smart Setup")]
        public void ForceSmartSetup()
        {
            _overrideExistingSetup = true;
            StartCoroutine(SmartInitialization());
        }
        
        /// <summary>
        /// Static method that can be called from anywhere to ensure setup
        /// </summary>
        public static void EnsureLaboratoryExists()
        {
            if (FindObjectOfType<SmartAutoSetup>() == null)
            {
                GameObject smartObj = new GameObject("[AUTO] Smart Laboratory Setup");
                smartObj.AddComponent<SmartAutoSetup>();
                
                Debug.Log("🎯 <color=green>[SMART SETUP]</color> Smart setup injected automatically!");
            }
        }
    }
    
    /// <summary>
    /// Extension class that ensures smart setup runs even without manual GameObject creation
    /// </summary>
    public static class AutoLaboratoryEnsurer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void EnsureLabExists()
        {
            // Small delay to let scene settle
            CoroutineRunner.Instance.StartCoroutine(DelayedEnsure());
        }
        
        private static IEnumerator DelayedEnsure()
        {
            yield return new WaitForSeconds(0.1f);
            
            // Only create if no setup components exist at all
            if (Object.FindObjectOfType<AutoSetupManager>() == null && 
                Object.FindObjectOfType<SmartAutoSetup>() == null)
            {
                SmartAutoSetup.EnsureLaboratoryExists();
            }
        }
    }
    
    /// <summary>
    /// Utility class to run coroutines from static methods
    /// </summary>
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _instance;
        
        public static CoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject runnerObj = new GameObject("[SYSTEM] Coroutine Runner");
                    _instance = runnerObj.AddComponent<CoroutineRunner>();
                    DontDestroyOnLoad(runnerObj);
                }
                return _instance;
            }
        }
    }
}
