using UnityEngine;
using BloodSample.Core;
using BloodSample.Systems;

namespace BloodSample.Utilities
{
    /// <summary>
    /// Debug helper for runtime debugging and testing
    /// </summary>
    public class DebugHelper : MonoBehaviour
    {
        [Header("Debug Settings")]
        [SerializeField] private bool _showDebugGUI = true;
        [SerializeField] private KeyCode _toggleGUIKey = KeyCode.F1;
        [SerializeField] private KeyCode _spawnSampleKey = KeyCode.F2;
        
        [Header("Spawn Settings")]
        [SerializeField] private GameObject _samplePrefab;
        [SerializeField] private float _spawnDistance = 2f;
        
        private GameManager _gameManager;
        private InputManager _inputManager;
        private Camera _playerCamera;
        private bool _guiVisible = true;
        
        private void Start()
        {
            _gameManager = GameManager.Instance;
            _inputManager = FindFirstObjectByType<InputManager>();
            _playerCamera = Camera.main;
            
            _guiVisible = _showDebugGUI;
        }
        
        private void Update()
        {
            HandleDebugInput();
        }
        
        private void HandleDebugInput()
        {
            if (Input.GetKeyDown(_toggleGUIKey))
            {
                _guiVisible = !_guiVisible;
            }
            
            if (Input.GetKeyDown(_spawnSampleKey) && _samplePrefab != null)
            {
                SpawnSampleAtCamera();
            }
        }
        
        private void OnGUI()
        {
            if (!_guiVisible) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 300, 400));
            GUILayout.BeginVertical("Box");
            
            GUILayout.Label("Debug Panel", GUI.skin.GetStyle("Label"));
            GUILayout.Space(10);
            
            // Game State Info
            GUILayout.Label("=== Game State ===");
            if (_gameManager != null)
            {
                GUILayout.Label($"State: {_gameManager.CurrentState}");
                GUILayout.Label($"Time Scale: {Time.timeScale:F1}");
            }
            
            GUILayout.Space(10);
            
            // Input Info
            GUILayout.Label("=== Input System ===");
            if (_inputManager != null)
            {
                GUILayout.Label($"Selected: {(_inputManager.SelectedObject?.name ?? "None")}");
                GUILayout.Label($"Has Selection: {_inputManager.HasSelection}");
            }
            
            GUILayout.Space(10);
            
            // Quick Actions
            GUILayout.Label("=== Quick Actions ===");
            
            if (GUILayout.Button("Spawn Sample") && _samplePrefab != null)
            {
                SpawnSampleAtCamera();
            }
            
            if (GUILayout.Button("Clear Scene Samples"))
            {
                ClearAllSamples();
            }
            
            GUILayout.Space(10);
            
            // Scene Stats
            GUILayout.Label("=== Scene Stats ===");
            var samples = FindObjectsByType<BloodSample.Systems.BloodSample>(FindObjectsSortMode.None);
            var workstations = FindObjectsByType<Workstation>(FindObjectsSortMode.None);
            
            GUILayout.Label($"Blood Samples: {samples.Length}");
            GUILayout.Label($"Workstations: {workstations.Length}");
            
            GUILayout.Space(10);
            
            // Controls Help
            GUILayout.Label("=== Controls ===");
            GUILayout.Label($"Toggle Debug: {_toggleGUIKey}");
            GUILayout.Label($"Spawn Sample: {_spawnSampleKey}");
            GUILayout.Label("WASD: Move Camera");
            GUILayout.Label("Mouse: Look Around (Hold RMB)");
            GUILayout.Label("E: Interact");
            GUILayout.Label("LMB: Select Object");
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        
        private void SpawnSampleAtCamera()
        {
            if (_samplePrefab == null || _playerCamera == null) return;
            
            Vector3 spawnPosition = _playerCamera.transform.position + _playerCamera.transform.forward * _spawnDistance;
            GameObject newSample = Instantiate(_samplePrefab, spawnPosition, Quaternion.identity);
            
            Debug.Log($"[DebugHelper] Spawned blood sample at {spawnPosition}");
        }
        
        private void ClearAllSamples()
        {
            var samples = FindObjectsByType<BloodSample.Systems.BloodSample>(FindObjectsSortMode.None);
            foreach (var sample in samples)
            {
                if (sample != null)
                {
                    DestroyImmediate(sample.gameObject);
                }
            }
            
            Debug.Log($"[DebugHelper] Cleared {samples.Length} blood samples from scene");
        }
        
        public void SetSamplePrefab(GameObject prefab)
        {
            _samplePrefab = prefab;
        }
    }
}
