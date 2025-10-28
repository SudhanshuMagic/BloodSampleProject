using UnityEngine;
using BloodSample.Core;
using BloodSample.Systems;

namespace BloodSample.Utilities
{
    /// <summary>
    /// Simplified laboratory generator for focused blood sample handling experience
    /// Creates only essential equipment as per storyboard requirements
    /// </summary>
    public class SimplifiedLabGenerator : MonoBehaviour
    {
        [Header("Simplified Lab Settings")]
        [SerializeField] private bool _generateOnStart = true;
        [SerializeField] private int _bloodSampleCount = 5;
        
        private ModernLabMaterialGenerator _materialGenerator;
        
        private void Start()
        {
            if (_generateOnStart)
            {
                _materialGenerator = ModernLabMaterialGenerator.Instance;
                GenerateSimplifiedLaboratory();
            }
        }
        
        /// <summary>
        /// Generate simplified laboratory with only essential equipment
        /// </summary>
        public void GenerateSimplifiedLaboratory()
        {
            // Clean up any existing containers first
            RemoveUnwantedContainers();
            
            // Ensure material generator is initialized
            if (_materialGenerator == null)
            {
                _materialGenerator = ModernLabMaterialGenerator.Instance;
            }
            
            CreateBasicInfrastructure();
            CreateSingleWorkstation();
            CreateInstructionalComputerMonitor();
            CreateWalkInFreezer();
            // Light fixtures removed - they were not looking good
            // CreateLightFixtures();
            CreateBloodSampleRack();
            CreateBarcodeScanner();
            CreateInstructionPanel();
            
            Debug.Log("[SimplifiedLabGenerator] ✅ Simplified laboratory creation complete!");
        }
        
        /// <summary>
        /// Create basic floor and minimal walls
        /// </summary>
        private void CreateBasicInfrastructure()
        {
            // Create floor
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "LabFloor";
            floor.transform.localScale = new Vector3(4f, 1f, 4f);
            floor.transform.position = Vector3.zero;
            floor.GetComponent<Renderer>().material = _materialGenerator.CreateLabFloorMaterial();
            floor.transform.SetParent(transform);
            
            // Create minimal perimeter walls (far away for open feel)
            CreateWall("WallNorth", new Vector3(0, 2.5f, 20f), new Vector3(40f, 5f, 1f));
            CreateWall("WallSouth", new Vector3(0, 2.5f, -20f), new Vector3(40f, 5f, 1f));
            CreateWall("WallEast", new Vector3(20f, 2.5f, 0), new Vector3(1f, 5f, 40f));
            CreateWall("WallWest", new Vector3(-20f, 2.5f, 0), new Vector3(1f, 5f, 40f));
            
            Debug.Log("[SimplifiedLabGenerator] Basic infrastructure created");
        }
        
        private void CreateWall(string name, Vector3 position, Vector3 scale)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.position = position;
            wall.transform.localScale = scale;
            wall.GetComponent<Renderer>().material = _materialGenerator.CreateLabWallMaterial();
            wall.transform.SetParent(transform);
        }
        
        /// <summary>
        /// Create single workstation at center
        /// </summary>
        private void CreateSingleWorkstation()
        {
            Vector3 workstationPosition = new Vector3(0f, 0f, -2f);
            
            // Create workstation table
            GameObject table = GameObject.CreatePrimitive(PrimitiveType.Cube);
            table.name = "WorkstationTable";
            table.transform.position = workstationPosition + new Vector3(0, 0.4f, 0);
            table.transform.localScale = new Vector3(2f, 0.8f, 1f);
            table.GetComponent<Renderer>().material = _materialGenerator.CreateStainlessSteelMaterial();
            
            // Create table legs
            for (int i = 0; i < 4; i++)
            {
                GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                leg.name = $"TableLeg_{i + 1}";
                leg.transform.SetParent(table.transform);
                leg.transform.localScale = new Vector3(0.05f, 0.5f, 0.05f);
                leg.transform.localPosition = new Vector3(
                    (i % 2 == 0) ? -0.4f : 0.4f,
                    -0.5f,
                    (i < 2) ? -0.25f : 0.25f
                );
                leg.GetComponent<Renderer>().material = _materialGenerator.CreateStainlessSteelMaterial();
            }
            
            // Create sample slots with visual indicators
            Transform[] sampleSlots = new Transform[3];
            for (int i = 0; i < 3; i++)
            {
                GameObject slot = new GameObject($"SampleSlot_{i + 1}");
                slot.transform.SetParent(table.transform);
                slot.transform.localPosition = new Vector3(-0.3f + (i * 0.3f), 0.5f, 0f);
                
                // Add visual slot indicator
                GameObject slotIndicator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                slotIndicator.name = $"SlotIndicator_{i + 1}";
                slotIndicator.transform.SetParent(slot.transform);
                slotIndicator.transform.localPosition = Vector3.zero;
                slotIndicator.transform.localScale = new Vector3(0.2f, 0.02f, 0.2f);
                slotIndicator.GetComponent<Renderer>().material = _materialGenerator.CreateMedicalBlueMaterial();
                
                // Remove collider from indicator (visual only)
                Destroy(slotIndicator.GetComponent<Collider>());
                
                sampleSlots[i] = slot.transform;
            }
            
            // Add workstation component
            table.AddComponent<Rigidbody>().isKinematic = true;
            Workstation workstationComponent = table.AddComponent<Workstation>();
            
            // Set up sample slots using reflection
            var sampleSlotsField = typeof(Workstation).GetField("_sampleSlots", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (sampleSlotsField != null)
            {
                sampleSlotsField.SetValue(workstationComponent, sampleSlots);
            }
            
            table.transform.SetParent(transform);
            
            Debug.Log("[SimplifiedLabGenerator] Single workstation created at center");
        }
        
        /// <summary>
        /// Create computer monitor displaying blood sample handling steps
        /// </summary>
        private void CreateInstructionalComputerMonitor()
        {
            Vector3 monitorPosition = new Vector3(3f, 0f, -2f);
            
            // Create desk for monitor
            GameObject desk = GameObject.CreatePrimitive(PrimitiveType.Cube);
            desk.name = "MonitorDesk";
            desk.transform.position = monitorPosition + new Vector3(0, 0.4f, 0);
            desk.transform.localScale = new Vector3(1.5f, 0.8f, 0.8f);
            desk.GetComponent<Renderer>().material = _materialGenerator.CreateStainlessSteelMaterial();
            
            // Create monitor screen
            GameObject monitor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            monitor.name = "InstructionalMonitor";
            monitor.transform.position = monitorPosition + new Vector3(0, 1.2f, -0.2f);
            monitor.transform.localScale = new Vector3(1.2f, 0.8f, 0.1f);
            
            // Set up monitor materials
            var screenOnMaterial = _materialGenerator.CreateComputerScreenMaterial(true);
            var screenOffMaterial = _materialGenerator.CreateComputerScreenMaterial(false);
            monitor.GetComponent<Renderer>().material = screenOnMaterial;
            
            // Add instructional display component
            monitor.AddComponent<Rigidbody>().isKinematic = true;
            var displayComponent = monitor.AddComponent<InstructionalDisplay>();
            displayComponent.SetDisplayMaterials(screenOnMaterial, screenOffMaterial);
            displayComponent.SetDisplayContent(InstructionalDisplay.DisplayContent.BloodSampleHandling);
            
            // Parent to desk
            monitor.transform.SetParent(desk.transform);
            desk.transform.SetParent(transform);
            
            Debug.Log("[SimplifiedLabGenerator] Instructional computer monitor created");
        }
        
        /// <summary>
        /// Create walk-in freezer for sample storage
        /// </summary>
        private void CreateWalkInFreezer()
        {
            Vector3 freezerPosition = new Vector3(-8f, 0f, 5f);
            
            // Main freezer body
            GameObject freezer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            freezer.name = "WalkInFreezer";
            freezer.transform.position = freezerPosition + new Vector3(0, 1.25f, 0);
            freezer.transform.localScale = new Vector3(3f, 2.5f, 3f);
            freezer.GetComponent<Renderer>().material = _materialGenerator.CreateStainlessSteelMaterial();
            
            // Freezer door
            GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
            door.name = "FreezerDoor";
            door.transform.SetParent(freezer.transform);
            door.transform.localPosition = new Vector3(0.4f, 0, 0.5f);
            door.transform.localScale = new Vector3(0.2f, 0.8f, 0.05f);
            door.GetComponent<Renderer>().material = _materialGenerator.CreateStainlessSteelMaterial();
            
            // Interior light
            GameObject interiorLight = new GameObject("InteriorLight");
            interiorLight.transform.SetParent(freezer.transform);
            interiorLight.transform.localPosition = new Vector3(0, 0.4f, 0);
            
            Light light = interiorLight.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = Color.white;
            light.intensity = 1.5f;
            light.range = 5f;
            light.enabled = false;
            
            // Add freezer functionality
            freezer.AddComponent<Rigidbody>().isKinematic = true;
            var freezerComponent = freezer.AddComponent<WalkInFreezer>();
            
            freezer.transform.SetParent(transform);
            
            Debug.Log("[SimplifiedLabGenerator] Walk-in freezer created");
        }
        
        /// <summary>
        /// Create light fixtures for laboratory illumination
        /// REMOVED - Light fixtures were not looking good in the scene
        /// </summary>
        /*
        private void CreateLightFixtures()
        {
            // Create 3 overhead light fixtures
            Vector3[] lightPositions = {
                new Vector3(-5f, 4f, 0f),
                new Vector3(0f, 4f, 0f),
                new Vector3(5f, 4f, 0f)
            };
            
            for (int i = 0; i < lightPositions.Length; i++)
            {
                CreateLightFixture($"LightFixture_{i + 1}", lightPositions[i]);
            }
            
            Debug.Log($"[SimplifiedLabGenerator] Created {lightPositions.Length} light fixtures");
        }
        */
        
        /*
        private void CreateLightFixture(string name, Vector3 position)
        {
            // Create fixture housing
            GameObject fixture = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            fixture.name = name;
            fixture.transform.position = position;
            fixture.transform.localScale = new Vector3(1.5f, 0.2f, 1.5f);
            fixture.GetComponent<Renderer>().material = _materialGenerator.CreateStainlessSteelMaterial();
            
            // Add light component
            GameObject lightObj = new GameObject("Light");
            lightObj.transform.SetParent(fixture.transform);
            lightObj.transform.localPosition = Vector3.down * 0.3f;
            
            Light lightComponent = lightObj.AddComponent<Light>();
            lightComponent.type = LightType.Point;
            lightComponent.color = new Color(0.95f, 0.95f, 1f, 1f);
            lightComponent.intensity = 2f;
            lightComponent.range = 12f;
            lightComponent.shadows = LightShadows.Soft;
            
            fixture.transform.SetParent(transform);
        }
        */
        
        /// <summary>
        /// Create blood sample rack with samples
        /// </summary>
        private void CreateBloodSampleRack()
        {
            Vector3 rackPosition = new Vector3(-3f, 0f, -2f);
            
            // Create rack structure
            GameObject rack = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rack.name = "BloodSampleRack";
            rack.transform.position = rackPosition + new Vector3(0, 1f, 0);
            rack.transform.localScale = new Vector3(1.5f, 0.3f, 0.8f);
            rack.GetComponent<Renderer>().material = _materialGenerator.CreateStainlessSteelMaterial();
            
            // Create sample holders in the rack
            for (int i = 0; i < _bloodSampleCount; i++)
            {
                Vector3 samplePosition = rackPosition + new Vector3(-0.5f + (i * 0.25f), 1.3f, 0);
                CreateBloodSample($"BloodSample_{i + 1}", samplePosition);
            }
            
            rack.transform.SetParent(transform);
            
            Debug.Log($"[SimplifiedLabGenerator] Blood sample rack created with {_bloodSampleCount} samples");
        }
        
        private void CreateBloodSample(string name, Vector3 position)
        {
            // Create sample tube
            GameObject sampleTube = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            sampleTube.name = name;
            sampleTube.transform.position = position;
            sampleTube.transform.localScale = new Vector3(0.15f, 0.4f, 0.15f);
            sampleTube.GetComponent<Renderer>().material = _materialGenerator.CreateGlassMaterial();
            
            // Create blood liquid inside
            GameObject liquid = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            liquid.name = name + "_Liquid";
            liquid.transform.SetParent(sampleTube.transform);
            liquid.transform.localPosition = new Vector3(0, -0.2f, 0);
            liquid.transform.localScale = new Vector3(0.8f, 0.6f, 0.8f);
            liquid.GetComponent<Renderer>().material = _materialGenerator.CreateWarningOrangeMaterial(); // Red-ish for blood
            
            // Add sample functionality with proper physics
            Rigidbody rb = sampleTube.AddComponent<Rigidbody>();
            rb.mass = 0.5f; // Light enough to grab easily
            rb.drag = 2f; // Some air resistance for realistic movement
            rb.angularDrag = 3f; // Prevent excessive spinning
            
            var sampleComponent = sampleTube.AddComponent<BloodSample.Systems.BloodSample>();
            
            // Make it grabbable by adding GrabbableObject component
            var grabbable = sampleTube.AddComponent<BloodSample.Core.GrabbableObject>();
            
            // Add a slight upward force to make it sit nicely in the rack
            rb.useGravity = true;
            
            sampleTube.transform.SetParent(transform);
        }
        
        /// <summary>
        /// Create barcode scanner for sample identification
        /// </summary>
        private void CreateBarcodeScanner()
        {
            Vector3 scannerPosition = new Vector3(1f, 1.2f, -2f);
            
            // Create scanner body
            GameObject scanner = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            scanner.name = "BarcodeScanner";
            scanner.transform.position = scannerPosition;
            scanner.transform.localScale = new Vector3(0.25f, 0.3f, 0.15f);
            scanner.GetComponent<Renderer>().material = _materialGenerator.CreateStainlessSteelMaterial();
            
            // Add scanner light
            GameObject scannerLight = new GameObject("ScannerLight");
            scannerLight.transform.SetParent(scanner.transform);
            scannerLight.transform.localPosition = new Vector3(0, -0.3f, 0.2f);
            
            Light light = scannerLight.AddComponent<Light>();
            light.type = LightType.Spot;
            light.color = Color.red;
            light.intensity = 2f;
            light.range = 3f;
            light.spotAngle = 30f;
            light.enabled = false;
            
            // Add scanner functionality
            scanner.AddComponent<Rigidbody>();
            var scannerComponent = scanner.AddComponent<BarcodeScanner>();
            
            scanner.transform.SetParent(transform);
            
            Debug.Log("[SimplifiedLabGenerator] Barcode scanner created");
        }
        
        /// <summary>
        /// Create visible instruction system for blood sample handling procedures
        /// </summary>
        private void CreateInstructionPanel()
        {
            GameObject instructionSystemObj = new GameObject("VisibleInstructionSystem");
            instructionSystemObj.transform.SetParent(transform);
            
            var instructionSystem = instructionSystemObj.AddComponent<VisibleInstructionSystem>();
            
            Debug.Log("[SimplifiedLabGenerator] Visible instruction system created");
        }
        
        [ContextMenu("Generate Simplified Laboratory")]
        public void GenerateSimplifiedLaboratoryMenu()
        {
            GenerateSimplifiedLaboratory();
        }
        
        /// <summary>
        /// Remove unwanted container objects from the scene
        /// </summary>
        private void RemoveUnwantedContainers()
        {
            // Find all GameObjects with "Container" or "Transport" in the name
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            int removedCount = 0;
            
            foreach (GameObject obj in allObjects)
            {
                if (obj.name.Contains("Container") || 
                    obj.name.Contains("Transport") || 
                    obj.name.Contains("BloodSample_Auto"))
                {
                    Debug.Log($"[SimplifiedLabGenerator] Removing unwanted container: {obj.name}");
                    DestroyImmediate(obj);
                    removedCount++;
                }
            }
            
            if (removedCount > 0)
            {
                Debug.Log($"[SimplifiedLabGenerator] Removed {removedCount} unwanted container objects");
            }
        }
        
        [ContextMenu("Remove Containers")]
        public void RemoveContainersMenu()
        {
            RemoveUnwantedContainers();
        }
        
        [ContextMenu("Clear Laboratory")]
        public void ClearLaboratory()
        {
            // Remove all child objects
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
            
            Debug.Log("[SimplifiedLabGenerator] Laboratory cleared");
        }
    }
}
