using UnityEngine;
using BloodSample.Systems;
using BloodSample.Data;
using BloodSample.UI;
using BloodSample.Utilities;
using System.Collections;
using System.Collections.Generic;

namespace BloodSample.Core
{
    /// <summary>
    /// Master auto-setup manager that creates everything automatically when you press Play
    /// Just add this script to an empty GameObject in your scene and press Play!
    /// </summary>
    public class AutoSetupManager : MonoBehaviour
    {
        [Header("Auto Setup Settings")]
        [SerializeField] private bool _enableAutoSetup = true;
        [SerializeField] private bool _showSetupProgress = true;
        [SerializeField] private float _setupDelay = 0.1f;
        
        [Header("Scene Configuration")]
        [SerializeField] private int _initialBloodSamples = 5;
        [SerializeField] private int _workstationCount = 4;
        [SerializeField] private Vector3 _laboratorySize = new Vector3(20f, 5f, 15f);
        
        [Header("Generated Objects")]
        [SerializeField] private List<GameObject> _generatedObjects = new List<GameObject>();
        
        // Runtime generated prefabs and materials
        private GameObject _bloodSamplePrefab;
        private GameObject _workstationPrefab;
        private Material _bloodMaterial;
        private Material _plasmaMaterial;
        private Material _highlightMaterial;
        private Material _tableMaterial;
        
        // Core managers
        private GameManager _gameManager;
        private InputManager _inputManager;
        private UIManager _uiManager;
        private Camera _mainCamera;
        
        private void Awake()
        {
            if (_enableAutoSetup)
            {
                StartCoroutine(AutoSetupSequence());
            }
        }
        
        private IEnumerator AutoSetupSequence()
        {
            LogSetup("🚀 Starting Automatic Laboratory Setup...");
            
            yield return StartCoroutine(SetupCoreManagers());
            yield return StartCoroutine(SetupCamera());
            yield return StartCoroutine(CreateMaterials());
            yield return StartCoroutine(CreatePrefabs());
            yield return StartCoroutine(BuildLaboratoryEnvironment());
            yield return StartCoroutine(CreateInitialSamples());
            yield return StartCoroutine(SetupUI());
            yield return StartCoroutine(FinalizeSetup());
            
            LogSetup("✅ Laboratory Setup Complete! Ready to use.");
            
            // Run system test
            yield return new WaitForSeconds(1f);
            RunSystemTest();
        }
        
        private IEnumerator SetupCoreManagers()
        {
            LogSetup("📋 Setting up core managers...");
            
            // Create GameManager if it doesn't exist
            if (GameManager.Instance == null)
            {
                GameObject gmObj = new GameObject("GameManager");
                gmObj.transform.SetParent(transform);
                _gameManager = gmObj.AddComponent<GameManager>();
                _generatedObjects.Add(gmObj);
            }
            else
            {
                _gameManager = GameManager.Instance;
            }
            
            // Create InputManager if it doesn't exist
            _inputManager = FindFirstObjectByType<InputManager>();
            if (_inputManager == null)
            {
                GameObject imObj = new GameObject("InputManager");
                imObj.transform.SetParent(transform);
                _inputManager = imObj.AddComponent<InputManager>();
                _generatedObjects.Add(imObj);
            }
            
            yield return new WaitForSeconds(_setupDelay);
        }
        
        private IEnumerator SetupCamera()
        {
            LogSetup("📷 Setting up camera system...");
            
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                _mainCamera = FindFirstObjectByType<Camera>();
            }
            
            if (_mainCamera == null)
            {
                // Create camera if none exists
                GameObject camObj = new GameObject("Main Camera");
                _mainCamera = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
                _generatedObjects.Add(camObj);
            }
            
            // Add camera controller
            if (_mainCamera.GetComponent<CameraController>() == null)
            {
                _mainCamera.gameObject.AddComponent<CameraController>();
            }
            
            // Position camera
            _mainCamera.transform.position = new Vector3(0f, 4f, -6f);
            _mainCamera.transform.rotation = Quaternion.Euler(15f, 0f, 0f);
            
            yield return new WaitForSeconds(_setupDelay);
        }
        
        private IEnumerator CreateMaterials()
        {
            LogSetup("🎨 Creating materials...");
            
            // Blood Material (red, slightly transparent)
            _bloodMaterial = new Material(Shader.Find("Standard"));
            _bloodMaterial.name = "Blood_Material_Auto";
            _bloodMaterial.color = new Color(0.8f, 0.1f, 0.1f, 0.8f);
            _bloodMaterial.SetFloat("_Mode", 3); // Transparent mode
            _bloodMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            _bloodMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            _bloodMaterial.SetInt("_ZWrite", 0);
            _bloodMaterial.DisableKeyword("_ALPHATEST_ON");
            _bloodMaterial.EnableKeyword("_ALPHABLEND_ON");
            _bloodMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            _bloodMaterial.renderQueue = 3000;
            
            // Plasma Material (yellow, transparent)
            _plasmaMaterial = new Material(_bloodMaterial);
            _plasmaMaterial.name = "Plasma_Material_Auto";
            _plasmaMaterial.color = new Color(0.9f, 0.9f, 0.2f, 0.6f);
            
            // Highlight Material (bright blue, emissive)
            _highlightMaterial = new Material(Shader.Find("Standard"));
            _highlightMaterial.name = "Highlight_Material_Auto";
            _highlightMaterial.color = new Color(0.2f, 0.6f, 1f, 1f);
            _highlightMaterial.EnableKeyword("_EMISSION");
            _highlightMaterial.SetColor("_EmissionColor", new Color(0.2f, 0.6f, 1f, 1f) * 0.3f);
            
            // Table Material (brown wood-like)
            _tableMaterial = new Material(Shader.Find("Standard"));
            _tableMaterial.name = "Table_Material_Auto";
            _tableMaterial.color = new Color(0.6f, 0.4f, 0.2f, 1f);
            _tableMaterial.SetFloat("_Metallic", 0.1f);
            _tableMaterial.SetFloat("_Smoothness", 0.3f);
            
            yield return new WaitForSeconds(_setupDelay);
        }
        
        private IEnumerator CreatePrefabs()
        {
            LogSetup("🧪 Creating blood sample prefab...");
            yield return StartCoroutine(CreateBloodSamplePrefab());
            
            LogSetup("🔬 Creating workstation prefab...");
            yield return StartCoroutine(CreateWorkstationPrefab());
        }
        
        private IEnumerator CreateBloodSamplePrefab()
        {
            // Create main container
            GameObject sampleObj = new GameObject("BloodSample_Auto");
            
            // Container (test tube)
            GameObject container = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            container.transform.SetParent(sampleObj.transform);
            container.transform.localPosition = Vector3.zero;
            container.transform.localScale = new Vector3(0.3f, 0.5f, 0.3f);
            container.name = "Container";
            
            // Make container glass-like
            Renderer containerRenderer = container.GetComponent<Renderer>();
            Material glassMaterial = new Material(Shader.Find("Standard"));
            glassMaterial.color = new Color(0.9f, 0.9f, 0.9f, 0.3f);
            glassMaterial.SetFloat("_Mode", 3); // Transparent
            glassMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            glassMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            glassMaterial.SetInt("_ZWrite", 0);
            glassMaterial.DisableKeyword("_ALPHATEST_ON");
            glassMaterial.EnableKeyword("_ALPHABLEND_ON");
            glassMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            glassMaterial.renderQueue = 3000;
            containerRenderer.material = glassMaterial;
            
            // Liquid content
            GameObject liquid = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            liquid.transform.SetParent(sampleObj.transform);
            liquid.transform.localPosition = new Vector3(0f, 0.1f, 0f);
            liquid.transform.localScale = new Vector3(0.25f, 0.4f, 0.25f);
            liquid.name = "Liquid";
            liquid.GetComponent<Renderer>().material = _bloodMaterial;
            
            // Add components
            Rigidbody rb = sampleObj.AddComponent<Rigidbody>();
            rb.mass = 0.1f;
            rb.drag = 1f;
            rb.angularDrag = 5f;
            
            CapsuleCollider col = sampleObj.AddComponent<CapsuleCollider>();
            col.radius = 0.15f;
            col.height = 1f;
            
            BloodSample.Systems.BloodSample bloodSampleComponent = sampleObj.AddComponent<BloodSample.Systems.BloodSample>();
            
            // Set up the blood sample component references
            System.Reflection.FieldInfo liquidRendererField = typeof(BloodSample.Systems.BloodSample).GetField("_liquidRenderer", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (liquidRendererField != null)
            {
                liquidRendererField.SetValue(bloodSampleComponent, liquid.GetComponent<Renderer>());
            }
            
            System.Reflection.FieldInfo liquidLevelField = typeof(BloodSample.Systems.BloodSample).GetField("_liquidLevel", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (liquidLevelField != null)
            {
                liquidLevelField.SetValue(bloodSampleComponent, liquid.transform);
            }
            
            System.Reflection.FieldInfo sampleMaterialsField = typeof(BloodSample.Systems.BloodSample).GetField("_sampleMaterials", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (sampleMaterialsField != null)
            {
                Material[] materials = { _bloodMaterial, _plasmaMaterial };
                sampleMaterialsField.SetValue(bloodSampleComponent, materials);
            }
            
            System.Reflection.FieldInfo highlightMaterialField = typeof(InteractableObject).GetField("_highlightMaterial", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (highlightMaterialField != null)
            {
                highlightMaterialField.SetValue(bloodSampleComponent, _highlightMaterial);
            }
            
            _bloodSamplePrefab = sampleObj;
            sampleObj.SetActive(false); // Keep as template
            
            yield return new WaitForSeconds(_setupDelay);
        }
        
        private IEnumerator CreateWorkstationPrefab()
        {
            GameObject workstationObj = new GameObject("Workstation_Auto");
            
            // Table surface
            GameObject table = GameObject.CreatePrimitive(PrimitiveType.Cube);
            table.transform.SetParent(workstationObj.transform);
            table.transform.localPosition = Vector3.zero;
            table.transform.localScale = new Vector3(2f, 0.1f, 1f);
            table.name = "TableSurface";
            table.GetComponent<Renderer>().material = _tableMaterial;
            
            // Table legs
            for (int i = 0; i < 4; i++)
            {
                GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                leg.transform.SetParent(workstationObj.transform);
                leg.transform.localScale = new Vector3(0.1f, 0.5f, 0.1f);
                leg.transform.localPosition = new Vector3(
                    (i % 2 == 0) ? -0.8f : 0.8f,
                    -0.55f,
                    (i < 2) ? -0.3f : 0.3f
                );
                leg.name = $"Leg_{i + 1}";
                leg.GetComponent<Renderer>().material = _tableMaterial;
            }
            
            // Sample slots on table
            Transform[] sampleSlots = new Transform[3];
            for (int i = 0; i < 3; i++)
            {
                GameObject slot = new GameObject($"SampleSlot_{i + 1}");
                slot.transform.SetParent(workstationObj.transform);
                slot.transform.localPosition = new Vector3(-0.6f + (i * 0.6f), 0.6f, 0f);
                sampleSlots[i] = slot.transform;
            }
            
            // Add workstation component
            Workstation workstationComponent = workstationObj.AddComponent<Workstation>();
            
            // Set up sample slots using reflection
            System.Reflection.FieldInfo sampleSlotsField = typeof(Workstation).GetField("_sampleSlots", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (sampleSlotsField != null)
            {
                sampleSlotsField.SetValue(workstationComponent, sampleSlots);
            }
            
            System.Reflection.FieldInfo highlightMaterialField = typeof(InteractableObject).GetField("_highlightMaterial", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (highlightMaterialField != null)
            {
                highlightMaterialField.SetValue(workstationComponent, _highlightMaterial);
            }
            
            _workstationPrefab = workstationObj;
            workstationObj.SetActive(false); // Keep as template
            
            yield return new WaitForSeconds(_setupDelay);
        }
        
        private IEnumerator BuildLaboratoryEnvironment()
        {
            LogSetup("🏗️ Building laboratory environment...");
            
            // Create floor
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.transform.SetParent(transform);
            floor.transform.localScale = new Vector3(2f, 1f, 1.5f);
            floor.name = "Laboratory_Floor";
            
            Material floorMaterial = new Material(Shader.Find("Standard"));
            floorMaterial.color = new Color(0.9f, 0.9f, 0.9f, 1f);
            floor.GetComponent<Renderer>().material = floorMaterial;
            _generatedObjects.Add(floor);
            
            // Create walls
            CreateWalls();
            
            // Create lighting
            CreateLighting();
            
            // Create workstations
            Vector3[] positions = {
                new Vector3(-3f, 0f, -2f),
                new Vector3(3f, 0f, -2f),
                new Vector3(-3f, 0f, 2f),
                new Vector3(3f, 0f, 2f)
            };
            
            for (int i = 0; i < Mathf.Min(_workstationCount, positions.Length); i++)
            {
                GameObject workstation = Instantiate(_workstationPrefab, positions[i], Quaternion.identity, transform);
                workstation.SetActive(true);
                workstation.name = $"Workstation_{i + 1}";
                
                Workstation ws = workstation.GetComponent<Workstation>();
                if (ws != null)
                {
                    ws.SetWorkstationId(i);
                }
                
                _generatedObjects.Add(workstation);
            }
            
            yield return new WaitForSeconds(_setupDelay);
        }
        
        private void CreateWalls()
        {
            // Back wall
            GameObject backWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backWall.transform.SetParent(transform);
            backWall.transform.position = new Vector3(0f, 2.5f, 7.5f);
            backWall.transform.localScale = new Vector3(20f, 5f, 0.5f);
            backWall.name = "Back_Wall";
            
            Material wallMaterial = new Material(Shader.Find("Standard"));
            wallMaterial.color = new Color(0.95f, 0.95f, 0.95f, 1f);
            backWall.GetComponent<Renderer>().material = wallMaterial;
            _generatedObjects.Add(backWall);
        }
        
        private void CreateLighting()
        {
            // Main directional light
            GameObject mainLight = new GameObject("Main Light");
            mainLight.transform.SetParent(transform);
            mainLight.transform.position = new Vector3(0f, 5f, 0f);
            mainLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            
            Light light = mainLight.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            light.color = new Color(0.95f, 0.95f, 1f);
            _generatedObjects.Add(mainLight);
            
            // Ambient lighting
            RenderSettings.ambientLight = new Color(0.3f, 0.3f, 0.35f);
        }
        
        private IEnumerator CreateInitialSamples()
        {
            LogSetup($"🩸 Creating {_initialBloodSamples} initial blood samples...");
            
            for (int i = 0; i < _initialBloodSamples; i++)
            {
                Vector3 spawnPos = new Vector3(
                    Random.Range(-2f, 2f),
                    2f,
                    Random.Range(-1f, 1f)
                );
                
                GameObject sample = Instantiate(_bloodSamplePrefab, spawnPos, 
                    Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), transform);
                sample.SetActive(true);
                sample.name = $"BloodSample_{i + 1}";
                
                // Randomize sample properties
                BloodSample bs = sample.GetComponent<BloodSample>();
                if (bs != null)
                {
                    var types = System.Enum.GetValues(typeof(SampleType));
                    SampleType randomType = (SampleType)types.GetValue(Random.Range(0, types.Length));
                    bs.SetSampleType(randomType);
                    bs.SetVolume(Random.Range(2f, 8f));
                }
                
                _generatedObjects.Add(sample);
                
                if (i % 2 == 0) yield return new WaitForSeconds(_setupDelay);
            }
        }
        
        private IEnumerator SetupUI()
        {
            LogSetup("🖥️ Setting up user interface...");
            
            // Find or create UI Manager
            _uiManager = FindFirstObjectByType<UIManager>();
            if (_uiManager == null)
            {
                GameObject uiObj = new GameObject("UIManager");
                uiObj.transform.SetParent(transform);
                _uiManager = uiObj.AddComponent<UIManager>();
                _generatedObjects.Add(uiObj);
            }
            
            // Create basic canvas if none exists
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                _generatedObjects.Add(canvasObj);
            }
            
            yield return new WaitForSeconds(_setupDelay);
        }
        
        private IEnumerator FinalizeSetup()
        {
            LogSetup("🔧 Finalizing setup...");
            
            // Add debug helper
            GameObject debugObj = new GameObject("DebugHelper");
            debugObj.transform.SetParent(transform);
            DebugHelper debugHelper = debugObj.AddComponent<DebugHelper>();
            
            // Set blood sample prefab reference for debug spawning
            System.Reflection.FieldInfo samplePrefabField = typeof(DebugHelper).GetField("_samplePrefab", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (samplePrefabField != null)
            {
                samplePrefabField.SetValue(debugHelper, _bloodSamplePrefab);
            }
            
            _generatedObjects.Add(debugObj);
            
            // Add system tester
            GameObject testerObj = new GameObject("SystemTester");
            testerObj.transform.SetParent(transform);
            SystemTester tester = testerObj.AddComponent<SystemTester>();
            
            System.Reflection.FieldInfo testPrefabField = typeof(SystemTester).GetField("_testBloodSamplePrefab", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (testPrefabField != null)
            {
                testPrefabField.SetValue(tester, _bloodSamplePrefab);
            }
            
            _generatedObjects.Add(testerObj);
            
            yield return new WaitForSeconds(_setupDelay);
        }
        
        private void RunSystemTest()
        {
            SystemTester tester = FindFirstObjectByType<SystemTester>();
            if (tester != null)
            {
                LogSetup("🧪 Running system integration test...");
                tester.RunAllTestsMenu();
            }
        }
        
        private void LogSetup(string message)
        {
            if (_showSetupProgress)
            {
                Debug.Log($"[AutoSetup] {message}");
            }
        }
        
        [ContextMenu("Clear Generated Objects")]
        public void ClearGeneratedObjects()
        {
            foreach (GameObject obj in _generatedObjects)
            {
                if (obj != null)
                {
                    DestroyImmediate(obj);
                }
            }
            _generatedObjects.Clear();
            LogSetup("Cleared all generated objects");
        }
        
        [ContextMenu("Regenerate Laboratory")]
        public void RegenerateLaboratory()
        {
            ClearGeneratedObjects();
            StartCoroutine(AutoSetupSequence());
        }
    }
}
