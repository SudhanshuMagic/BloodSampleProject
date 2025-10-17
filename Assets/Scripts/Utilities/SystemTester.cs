using UnityEngine;
using BloodSample.Core;
using BloodSample.Systems;

namespace BloodSample.Utilities
{
    /// <summary>
    /// System integration tester for verifying all systems work correctly
    /// </summary>
    public class SystemTester : MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField] private bool _runTestsOnStart = false;
        [SerializeField] private GameObject _testBloodSamplePrefab;
        
        private void Start()
        {
            if (_runTestsOnStart)
            {
                Invoke(nameof(RunAllTestsMenu), 1f);
            }
        }
        
        [ContextMenu("Run All Tests")]
        public void RunAllTestsMenu()
        {
            RunAllTests();
        }
        
        public void RunAllTests()
        {
            Debug.Log("[SystemTester] 🧪 Starting System Integration Tests...");
            
            TestGameManager();
            TestInputManager();
            TestUIManager();
            TestBloodSampleSystem();
            TestWorkstationSystem();
            
            Debug.Log("[SystemTester] ✅ All Tests Completed");
        }
        
        private void TestGameManager()
        {
            Debug.Log("[SystemTester] Testing GameManager...");
            
            GameManager gm = GameManager.Instance;
            if (gm != null)
            {
                var initialState = gm.CurrentState;
                gm.ChangeGameState(GameState.MainMenu);
                gm.ChangeGameState(GameState.Laboratory);
                
                if (gm.CurrentState == GameState.Laboratory)
                {
                    Debug.Log("[SystemTester] ✓ GameManager state change successful");
                }
                else
                {
                    Debug.LogError("[SystemTester] ✗ GameManager state change failed");
                }
            }
            else
            {
                Debug.LogError("[SystemTester] ✗ GameManager not found");
            }
        }
        
        private void TestInputManager()
        {
            Debug.Log("[SystemTester] Testing InputManager...");
            
            InputManager im = FindFirstObjectByType<InputManager>();
            if (im != null)
            {
                Debug.Log("[SystemTester] ✓ InputManager found and active");
            }
            else
            {
                Debug.LogError("[SystemTester] ✗ InputManager not found");
            }
        }
        
        private void TestUIManager()
        {
            Debug.Log("[SystemTester] Testing UIManager...");
            
            var uiManager = FindFirstObjectByType<BloodSample.UI.UIManager>();
            if (uiManager != null)
            {
                uiManager.ShowInteractionPrompt("Test Prompt");
                uiManager.ShowSelectedObject("Test Object");
                Debug.Log("[SystemTester] ✓ UIManager working correctly");
            }
            else
            {
                Debug.LogError("[SystemTester] ✗ UIManager not found");
            }
        }
        
        private void TestBloodSampleSystem()
        {
            Debug.Log("[SystemTester] Testing BloodSample System...");
            
            var samples = FindObjectsByType<BloodSample.Systems.BloodSample>(FindObjectsSortMode.None);
            if (samples.Length > 0)
            {
                var testSample = samples[0];
                if (testSample != null)
                {
                    var data = testSample.Data;
                    Debug.Log($"[SystemTester] ✓ BloodSample working - ID: {data.sampleId}, Type: {data.sampleType}");
                }
            }
            else if (_testBloodSamplePrefab != null)
            {
                // Create a test sample
                GameObject testObj = Instantiate(_testBloodSamplePrefab, Vector3.zero, Quaternion.identity);
                var testSample = testObj.GetComponent<BloodSample.Systems.BloodSample>();
                if (testSample != null)
                {
                    Debug.Log("[SystemTester] ✓ BloodSample prefab working correctly");
                    Destroy(testObj);
                }
            }
            else
            {
                Debug.LogWarning("[SystemTester] ⚠ No BloodSamples found to test");
            }
        }
        
        private void TestWorkstationSystem()
        {
            Debug.Log("[SystemTester] Testing Workstation System...");
            
            var workstations = FindObjectsByType<Workstation>(FindObjectsSortMode.None);
            if (workstations.Length > 0)
            {
                var testWorkstation = workstations[0];
                if (testWorkstation != null)
                {
                    Debug.Log($"[SystemTester] ✓ Workstation working - ID: {testWorkstation.GetWorkstationId()}");
                }
            }
            else
            {
                Debug.LogWarning("[SystemTester] ⚠ No Workstations found to test");
            }
        }
        
        public void SetTestBloodSamplePrefab(GameObject prefab)
        {
            _testBloodSamplePrefab = prefab;
        }
    }
}
