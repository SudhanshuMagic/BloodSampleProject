using UnityEngine;
using BloodSample.Core;
using BloodSample.Systems;

namespace BloodSample.Utilities
{
    /// <summary>
    /// Generates modern laboratory equipment with proper materials and functionality
    /// </summary>
    public class ModernLabEquipmentGenerator : MonoBehaviour
    {
        [Header("Generation Settings")]
        [SerializeField] private bool _generateOnStart = false;
        [SerializeField] private Transform _equipmentParent;
        
        private ModernLabMaterialGenerator _materialGenerator;
        
        private void Start()
        {
            _materialGenerator = ModernLabMaterialGenerator.Instance;
            
            if (_generateOnStart)
            {
                GenerateCompleteModernLaboratory();
            }
        }
        
        /// <summary>
        /// Generate complete modern laboratory with all equipment
        /// </summary>
        public void GenerateCompleteModernLaboratory()
        {
            Debug.Log("[ModernLabEquipmentGenerator] 🏥 Creating complete modern laboratory...");
            
            // Ensure material generator is initialized
            if (_materialGenerator == null)
            {
                _materialGenerator = ModernLabMaterialGenerator.Instance;
                Debug.Log("[ModernLabEquipmentGenerator] Material generator initialized");
            }
            
            if (_materialGenerator == null)
            {
                Debug.LogError("[ModernLabEquipmentGenerator] Failed to initialize material generator!");
                return;
            }
            
            CreateLaboratoryInfrastructure();
            CreateSafetyEquipment();
            CreateProcessingEquipment();
            CreateDataManagementEquipment();
            CreateStorageEquipment();
            CreateFurniture();
            
            Debug.Log("[ModernLabEquipmentGenerator] ✅ Modern laboratory creation complete!");
        }
        
        /// <summary>
        /// Create laboratory infrastructure (walls, floor, lighting)
        /// </summary>
        private void CreateLaboratoryInfrastructure()
        {
            // Create larger floor for open laboratory
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "LabFloor";
            floor.transform.localScale = new Vector3(8f, 1f, 8f);
            floor.transform.position = Vector3.zero;
            floor.GetComponent<Renderer>().material = _materialGenerator.CreateLabFloorMaterial();
            SetParent(floor);
            
            // Create minimal perimeter walls for completely open laboratory
            CreateWall("WallNorth", new Vector3(0, 2.5f, 35f), new Vector3(70f, 5f, 1f));
            CreateWall("WallSouth", new Vector3(0, 2.5f, -35f), new Vector3(70f, 5f, 1f));
            CreateWall("WallEast", new Vector3(35f, 2.5f, 0), new Vector3(1f, 5f, 70f));
            CreateWall("WallWest", new Vector3(-35f, 2.5f, 0), new Vector3(1f, 5f, 70f));
            
            Debug.Log("[ModernLabEquipmentGenerator] Created completely open laboratory - walls moved to perimeter");
            
            // Create modern lighting
            CreateModernLighting();
        }
        
        private void CreateWall(string name, Vector3 position, Vector3 scale)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.position = position;
            wall.transform.localScale = scale;
            wall.GetComponent<Renderer>().material = _materialGenerator.CreateLabWallMaterial();
            SetParent(wall);
        }
        
        private void CreateModernLighting()
        {
            // Main overhead lighting
            for (int x = -15; x <= 15; x += 10)
            {
                for (int z = -15; z <= 15; z += 10)
                {
                    GameObject lightFixture = CreateLightFixture($"LightFixture_{x}_{z}", new Vector3(x, 4.5f, z));
                    SetParent(lightFixture);
                }
            }
        }
        
        private GameObject CreateLightFixture(string name, Vector3 position)
        {
            // Create fixture housing
            GameObject fixture = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            fixture.name = name;
            fixture.transform.position = position;
            fixture.transform.localScale = new Vector3(2f, 0.2f, 2f);
            fixture.GetComponent<Renderer>().material = _materialGenerator.CreateStainlessSteelMaterial();
            
            // Add light component
            GameObject lightObj = new GameObject("Light");
            lightObj.transform.SetParent(fixture.transform);
            lightObj.transform.localPosition = Vector3.down * 0.3f;
            
            Light lightComponent = lightObj.AddComponent<Light>();
            lightComponent.type = LightType.Point;
            lightComponent.color = new Color(0.95f, 0.95f, 1f, 1f);
            lightComponent.intensity = 2f;
            lightComponent.range = 15f;
            lightComponent.shadows = LightShadows.Soft;
            
            return fixture;
        }
        
        /// <summary>
        /// Create safety equipment
        /// </summary>
        private void CreateSafetyEquipment()
        {
            // Latex gloves box
            CreateLatexGlovesBox(new Vector3(-15f, 1f, 10f));
            
            // Alcohol sterilizer
            CreateAlcoholSterilizer(new Vector3(-12f, 1f, 10f));
            
            // Sharps disposal box
            CreateSharpsDisposalBox(new Vector3(-18f, 1f, 8f));
            
            // Light switches
            CreateLightSwitch(new Vector3(-24f, 1.5f, 15f));
            CreateLightSwitch(new Vector3(24f, 1.5f, 15f));
        }
        
        private void CreateLatexGlovesBox(Vector3 position)
        {
            GameObject gloveBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gloveBox.name = "LatexGlovesBox";
            gloveBox.transform.position = position;
            gloveBox.transform.localScale = new Vector3(0.8f, 0.4f, 0.6f);
            gloveBox.GetComponent<Renderer>().material = _materialGenerator.CreateMedicalBlueMaterial();
            
            // Add physics
            gloveBox.AddComponent<Rigidbody>().isKinematic = true;
            
            // Add functionality
            var glovesComponent = gloveBox.AddComponent<LatexGlovesBox>();
            
            SetParent(gloveBox);
        }
        
        private void CreateAlcoholSterilizer(Vector3 position)
        {
            GameObject sterilizer = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            sterilizer.name = "AlcoholSterilizer";
            sterilizer.transform.position = position;
            sterilizer.transform.localScale = new Vector3(0.5f, 0.8f, 0.5f);
            sterilizer.GetComponent<Renderer>().material = _materialGenerator.CreateSterileWhiteMaterial();
            
            // Add pump mechanism (child object)
            GameObject pump = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pump.name = "Pump";
            pump.transform.SetParent(sterilizer.transform);
            pump.transform.localPosition = new Vector3(0, 0.7f, 0);
            pump.transform.localScale = new Vector3(0.3f, 0.2f, 0.3f);
            pump.GetComponent<Renderer>().material = _materialGenerator.CreateMedicalBlueMaterial();
            
            sterilizer.AddComponent<Rigidbody>().isKinematic = true;
            var equipment = sterilizer.AddComponent<LaboratoryEquipment>();
            
            SetParent(sterilizer);
        }
        
        private void CreateSharpsDisposalBox(Vector3 position)
        {
            GameObject disposal = GameObject.CreatePrimitive(PrimitiveType.Cube);
            disposal.name = "SharpsDisposalBox";
            disposal.transform.position = position;
            disposal.transform.localScale = new Vector3(0.6f, 0.8f, 0.4f);
            disposal.GetComponent<Renderer>().material = _materialGenerator.CreateWarningOrangeMaterial();
            
            disposal.AddComponent<Rigidbody>().isKinematic = true;
            var equipment = disposal.AddComponent<LaboratoryEquipment>();
            
            SetParent(disposal);
        }
        
        private void CreateLightSwitch(Vector3 position)
        {
            GameObject switchObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            switchObj.name = "LightSwitch";
            switchObj.transform.position = position;
            switchObj.transform.localScale = new Vector3(0.2f, 0.3f, 0.1f);
            switchObj.GetComponent<Renderer>().material = _materialGenerator.CreateSterileWhiteMaterial();
            
            switchObj.AddComponent<Rigidbody>().isKinematic = true;
            var lightSwitch = switchObj.AddComponent<LightSwitch>();
            
            // Set up materials for the light switch
            var onMaterial = _materialGenerator.CreateSafetyGreenMaterial();
            var offMaterial = _materialGenerator.CreateWarningOrangeMaterial();
            lightSwitch.SetSwitchMaterials(onMaterial, offMaterial);
            
            // Find all lights to control
            var allLights = FindObjectsByType<Light>(FindObjectsSortMode.None);
            
            // Assign lights to the light switch
            if (allLights.Length > 0)
            {
                lightSwitch.SetControlledLights(allLights);
                Debug.Log($"[ModernLabEquipmentGenerator] Assigned {allLights.Length} lights to light switch");
            }
            
            SetParent(switchObj);
        }
        
        /// <summary>
        /// Create processing equipment
        /// </summary>
        private void CreateProcessingEquipment()
        {
            // Test tube racks
            CreateTestTubeRack(new Vector3(5f, 1f, 8f));
            CreateTestTubeRack(new Vector3(8f, 1f, 8f));
            
            // Receiving bench
            CreateReceivingBench(new Vector3(0f, 0.8f, 15f));
            
            // Barcode scanner
            CreateBarcodeScanner(new Vector3(2f, 1.2f, 15f));
            
            // Labelling station
            CreateLabellingStation(new Vector3(-5f, 1f, 12f));
        }
        
        private void CreateTestTubeRack(Vector3 position)
        {
            GameObject rack = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rack.name = "TestTubeRack";
            rack.transform.position = position;
            rack.transform.localScale = new Vector3(1.5f, 0.3f, 0.8f);
            rack.GetComponent<Renderer>().material = _materialGenerator.CreateStainlessSteelMaterial();
            
            // Create holes for test tubes
            for (int i = 0; i < 8; i++)
            {
                GameObject slot = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                slot.name = $"TubeSlot_{i}";
                slot.transform.SetParent(rack.transform);
                slot.transform.localPosition = new Vector3(-0.6f + (i * 0.2f), 0.2f, 0);
                slot.transform.localScale = new Vector3(0.08f, 0.3f, 0.08f);
                slot.GetComponent<Renderer>().material = _materialGenerator.CreateMedicalBlueMaterial();
            }
            
            rack.AddComponent<Rigidbody>().isKinematic = true;
            var equipment = rack.AddComponent<LaboratoryEquipment>();
            
            SetParent(rack);
        }
        
        private void CreateReceivingBench(Vector3 position)
        {
            GameObject bench = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bench.name = "ReceivingBench";
            bench.transform.position = position;
            bench.transform.localScale = new Vector3(4f, 0.1f, 1.5f);
            bench.GetComponent<Renderer>().material = _materialGenerator.CreateStainlessSteelMaterial();
            
            bench.AddComponent<Rigidbody>().isKinematic = true;
            var equipment = bench.AddComponent<LaboratoryEquipment>();
            
            SetParent(bench);
        }
        
        private void CreateBarcodeScanner(Vector3 position)
        {
            GameObject scanner = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            scanner.name = "BarcodeScanner";
            scanner.transform.position = position;
            scanner.transform.localScale = new Vector3(0.3f, 0.4f, 0.15f);
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
            
            scanner.AddComponent<Rigidbody>();
            var scannerComponent = scanner.AddComponent<BarcodeScanner>();
            
            SetParent(scanner);
        }
        
        private void CreateLabellingStation(Vector3 position)
        {
            GameObject station = GameObject.CreatePrimitive(PrimitiveType.Cube);
            station.name = "LabellingStation";
            station.transform.position = position;
            station.transform.localScale = new Vector3(1.2f, 0.8f, 0.8f);
            station.GetComponent<Renderer>().material = _materialGenerator.CreateSterileWhiteMaterial();
            
            station.AddComponent<Rigidbody>().isKinematic = true;
            var equipment = station.AddComponent<LaboratoryEquipment>();
            
            SetParent(station);
        }
        
        /// <summary>
        /// Create data management equipment
        /// </summary>
        private void CreateDataManagementEquipment()
        {
            // Computer workstation
            CreateComputerWorkstation(new Vector3(-10f, 0f, -8f));
        }
        
        private void CreateComputerWorkstation(Vector3 position)
        {
            // Desk
            GameObject desk = GameObject.CreatePrimitive(PrimitiveType.Cube);
            desk.name = "ComputerDesk";
            desk.transform.position = position + new Vector3(0, 0.4f, 0);
            desk.transform.localScale = new Vector3(2f, 0.8f, 1f);
            desk.GetComponent<Renderer>().material = _materialGenerator.CreateStainlessSteelMaterial();
            
            // Monitor
            GameObject monitor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            monitor.name = "ComputerMonitor";
            monitor.transform.position = position + new Vector3(0, 1.2f, -0.3f);
            monitor.transform.localScale = new Vector3(1.2f, 0.8f, 0.1f);
            monitor.GetComponent<Renderer>().material = _materialGenerator.CreateComputerScreenMaterial(true);
            
            // Mouse
            GameObject mouse = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            mouse.name = "ComputerMouse";
            mouse.transform.position = position + new Vector3(0.5f, 0.82f, 0.2f);
            mouse.transform.localScale = new Vector3(0.15f, 0.08f, 0.2f);
            mouse.GetComponent<Renderer>().material = _materialGenerator.CreateStainlessSteelMaterial();
            
            // Add computer functionality to monitor
            monitor.AddComponent<Rigidbody>().isKinematic = true;
            var computer = monitor.AddComponent<ComputerWorkstation>();
            
            // Add mouse grabbability
            mouse.AddComponent<Rigidbody>();
            var mouseGrabbable = mouse.AddComponent<GrabbableObject>();
            
            SetParent(desk);
            SetParent(monitor);
            SetParent(mouse);
        }
        
        /// <summary>
        /// Create storage equipment
        /// </summary>
        private void CreateStorageEquipment()
        {
            // Walk-in freezer
            CreateWalkInFreezer(new Vector3(15f, 0f, -15f));
            
            // Cabinets
            CreateCabinet(new Vector3(-20f, 1f, -5f));
            CreateCabinet(new Vector3(-20f, 1f, 0f));
            CreateCabinet(new Vector3(-20f, 1f, 5f));
            
            // Transport containers
            CreateTransportContainer(new Vector3(18f, 0.5f, 10f));
            CreateTransportContainer(new Vector3(20f, 0.5f, 10f));
        }
        
        private void CreateWalkInFreezer(Vector3 position)
        {
            // Main freezer body
            GameObject freezer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            freezer.name = "WalkInFreezer";
            freezer.transform.position = position + new Vector3(0, 1.25f, 0);
            freezer.transform.localScale = new Vector3(4f, 2.5f, 4f);
            freezer.GetComponent<Renderer>().material = _materialGenerator.CreateStainlessSteelMaterial();
            
            // Door
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
            
            freezer.AddComponent<Rigidbody>().isKinematic = true;
            var freezerComponent = freezer.AddComponent<WalkInFreezer>();
            
            SetParent(freezer);
        }
        
        private void CreateCabinet(Vector3 position)
        {
            GameObject cabinet = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabinet.name = "LaboratoryCabinet";
            cabinet.transform.position = position;
            cabinet.transform.localScale = new Vector3(1.5f, 2f, 0.8f);
            cabinet.GetComponent<Renderer>().material = _materialGenerator.CreateSterileWhiteMaterial();
            
            cabinet.AddComponent<Rigidbody>().isKinematic = true;
            var equipment = cabinet.AddComponent<LaboratoryEquipment>();
            
            SetParent(cabinet);
        }
        
        private void CreateTransportContainer(Vector3 position)
        {
            GameObject container = GameObject.CreatePrimitive(PrimitiveType.Cube);
            container.name = "TransportContainer";
            container.transform.position = position;
            container.transform.localScale = new Vector3(0.8f, 1f, 0.6f);
            container.GetComponent<Renderer>().material = _materialGenerator.CreateMedicalBlueMaterial();
            
            container.AddComponent<Rigidbody>();
            var containerGrabbable = container.AddComponent<GrabbableObject>();
            
            SetParent(container);
        }
        
        /// <summary>
        /// Create furniture
        /// </summary>
        private void CreateFurniture()
        {
            // Tables
            CreateTable(new Vector3(3f, 0f, 3f));
            CreateTable(new Vector3(-3f, 0f, 3f));
            
            // Chairs
            CreateChair(new Vector3(-10f, 0f, -7.5f)); // Computer chair
            CreateChair(new Vector3(6f, 0f, 5f));
            
            // Trays
            CreateTray(new Vector3(1f, 1.5f, 15f));
            CreateTray(new Vector3(-1f, 1.5f, 15f));
        }
        
        private void CreateTable(Vector3 position)
        {
            GameObject table = GameObject.CreatePrimitive(PrimitiveType.Cube);
            table.name = "LaboratoryTable";
            table.transform.position = position + new Vector3(0, 0.4f, 0);
            table.transform.localScale = new Vector3(2f, 0.8f, 1.2f);
            table.GetComponent<Renderer>().material = _materialGenerator.CreateStainlessSteelMaterial();
            
            table.AddComponent<Rigidbody>().isKinematic = true;
            var equipment = table.AddComponent<LaboratoryEquipment>();
            
            SetParent(table);
        }
        
        private void CreateChair(Vector3 position)
        {
            GameObject chair = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chair.name = "LaboratoryChair";
            chair.transform.position = position + new Vector3(0, 0.25f, 0);
            chair.transform.localScale = new Vector3(0.6f, 0.5f, 0.6f);
            chair.GetComponent<Renderer>().material = _materialGenerator.CreateMedicalBlueMaterial();
            
            chair.AddComponent<Rigidbody>();
            var chairGrabbable = chair.AddComponent<GrabbableObject>();
            
            SetParent(chair);
        }
        
        private void CreateTray(Vector3 position)
        {
            GameObject tray = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tray.name = "LaboratoryTray";
            tray.transform.position = position;
            tray.transform.localScale = new Vector3(0.8f, 0.05f, 0.8f);
            tray.GetComponent<Renderer>().material = _materialGenerator.CreateStainlessSteelMaterial();
            
            tray.AddComponent<Rigidbody>();
            var trayGrabbable = tray.AddComponent<GrabbableObject>();
            
            SetParent(tray);
        }
        
        private void SetParent(GameObject obj)
        {
            if (_equipmentParent != null)
            {
                obj.transform.SetParent(_equipmentParent);
            }
        }
        
        [ContextMenu("Generate Complete Modern Laboratory")]
        public void GenerateCompleteModernLaboratoryMenu()
        {
            GenerateCompleteModernLaboratory();
        }
    }
}
