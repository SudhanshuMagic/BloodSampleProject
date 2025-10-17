using UnityEngine;
using BloodSample.Core;
using BloodSample.Systems;
using BloodSample.Data;

namespace BloodSample.Utilities
{
    /// <summary>
    /// Advanced runtime prefab generator for creating customized laboratory equipment
    /// Can be used to expand the laboratory with additional equipment types
    /// </summary>
    public class RuntimePrefabGenerator : MonoBehaviour
    {
        [Header("Generation Settings")]
        [SerializeField] private Material _defaultMaterial;
        [SerializeField] private Material _glassMaterial;
        [SerializeField] private Material _metalMaterial;
        [SerializeField] private Material _plasticMaterial;
        
        public static RuntimePrefabGenerator Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                CreateDefaultMaterials();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void CreateDefaultMaterials()
        {
            if (_defaultMaterial == null)
            {
                _defaultMaterial = new Material(Shader.Find("Standard"));
                _defaultMaterial.color = Color.white;
            }
            
            if (_glassMaterial == null)
            {
                _glassMaterial = CreateGlassMaterial();
            }
            
            if (_metalMaterial == null)
            {
                _metalMaterial = CreateMetalMaterial();
            }
            
            if (_plasticMaterial == null)
            {
                _plasticMaterial = CreatePlasticMaterial();
            }
        }
        
        public GameObject CreateCentrifuge(Vector3 position = default)
        {
            GameObject centrifuge = new GameObject("Centrifuge_Runtime");
            centrifuge.transform.position = position;
            
            // Main body (cylinder)
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            body.transform.SetParent(centrifuge.transform);
            body.transform.localPosition = Vector3.zero;
            body.transform.localScale = new Vector3(1f, 0.5f, 1f);
            body.name = "Body";
            body.GetComponent<Renderer>().material = _metalMaterial;
            
            // Lid (smaller cylinder on top)
            GameObject lid = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            lid.transform.SetParent(centrifuge.transform);
            lid.transform.localPosition = new Vector3(0f, 0.6f, 0f);
            lid.transform.localScale = new Vector3(0.9f, 0.1f, 0.9f);
            lid.name = "Lid";
            lid.GetComponent<Renderer>().material = _glassMaterial;
            
            // Sample holder inside
            GameObject holder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            holder.transform.SetParent(centrifuge.transform);
            holder.transform.localPosition = new Vector3(0f, 0.1f, 0f);
            holder.transform.localScale = new Vector3(0.6f, 0.2f, 0.6f);
            holder.name = "SampleHolder";
            holder.GetComponent<Renderer>().material = _plasticMaterial;
            
            // Add components
            centrifuge.AddComponent<BoxCollider>();
            
            // Add equipment script (you can create this)
            InteractableObject interactable = centrifuge.AddComponent<InteractableObject>();
            interactable.SetInteractionPrompt("Use Centrifuge");
            
            return centrifuge;
        }
        
        public GameObject CreateMicroscope(Vector3 position = default)
        {
            GameObject microscope = new GameObject("Microscope_Runtime");
            microscope.transform.position = position;
            
            // Base
            GameObject baseObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            baseObj.transform.SetParent(microscope.transform);
            baseObj.transform.localPosition = Vector3.zero;
            baseObj.transform.localScale = new Vector3(0.8f, 0.1f, 0.8f);
            baseObj.name = "Base";
            baseObj.GetComponent<Renderer>().material = _metalMaterial;
            
            // Stand (vertical column)
            GameObject stand = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            stand.transform.SetParent(microscope.transform);
            stand.transform.localPosition = new Vector3(0f, 0.8f, 0f);
            stand.transform.localScale = new Vector3(0.1f, 0.8f, 0.1f);
            stand.name = "Stand";
            stand.GetComponent<Renderer>().material = _metalMaterial;
            
            // Eyepiece
            GameObject eyepiece = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            eyepiece.transform.SetParent(microscope.transform);
            eyepiece.transform.localPosition = new Vector3(0f, 1.5f, 0.3f);
            eyepiece.transform.localRotation = Quaternion.Euler(45f, 0f, 0f);
            eyepiece.transform.localScale = new Vector3(0.15f, 0.3f, 0.15f);
            eyepiece.name = "Eyepiece";
            eyepiece.GetComponent<Renderer>().material = _plasticMaterial;
            
            // Objective lens
            GameObject objective = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            objective.transform.SetParent(microscope.transform);
            objective.transform.localPosition = new Vector3(0f, 0.8f, 0f);
            objective.transform.localScale = new Vector3(0.1f, 0.2f, 0.1f);
            objective.name = "Objective";
            objective.GetComponent<Renderer>().material = _glassMaterial;
            
            // Stage (sample platform)
            GameObject stage = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stage.transform.SetParent(microscope.transform);
            stage.transform.localPosition = new Vector3(0f, 0.4f, 0f);
            stage.transform.localScale = new Vector3(0.6f, 0.05f, 0.4f);
            stage.name = "Stage";
            stage.GetComponent<Renderer>().material = _metalMaterial;
            
            // Add components
            microscope.AddComponent<BoxCollider>();
            InteractableObject interactable = microscope.AddComponent<InteractableObject>();
            interactable.SetInteractionPrompt("Use Microscope");
            
            return microscope;
        }
        
        public GameObject CreateStorageUnit(Vector3 position = default)
        {
            GameObject storage = new GameObject("StorageUnit_Runtime");
            storage.transform.position = position;
            
            // Main cabinet
            GameObject cabinet = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabinet.transform.SetParent(storage.transform);
            cabinet.transform.localPosition = Vector3.zero;
            cabinet.transform.localScale = new Vector3(1.5f, 2f, 0.8f);
            cabinet.name = "Cabinet";
            cabinet.GetComponent<Renderer>().material = _metalMaterial;
            
            // Door
            GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
            door.transform.SetParent(storage.transform);
            door.transform.localPosition = new Vector3(0f, 0f, 0.41f);
            door.transform.localScale = new Vector3(1.4f, 1.9f, 0.05f);
            door.name = "Door";
            
            // Make door slightly transparent
            Material doorMaterial = new Material(_glassMaterial);
            doorMaterial.color = new Color(0.8f, 0.8f, 0.9f, 0.7f);
            door.GetComponent<Renderer>().material = doorMaterial;
            
            // Handle
            GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            handle.transform.SetParent(door.transform);
            handle.transform.localPosition = new Vector3(0.5f, 0f, 1f);
            handle.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            handle.transform.localScale = new Vector3(0.1f, 0.2f, 0.1f);
            handle.name = "Handle";
            handle.GetComponent<Renderer>().material = _metalMaterial;
            
            // Shelves inside
            for (int i = 0; i < 3; i++)
            {
                GameObject shelf = GameObject.CreatePrimitive(PrimitiveType.Cube);
                shelf.transform.SetParent(storage.transform);
                shelf.transform.localPosition = new Vector3(0f, -0.5f + (i * 0.5f), 0f);
                shelf.transform.localScale = new Vector3(1.3f, 0.02f, 0.7f);
                shelf.name = $"Shelf_{i + 1}";
                shelf.GetComponent<Renderer>().material = _metalMaterial;
                
                // Create sample slots on shelves
                for (int j = 0; j < 4; j++)
                {
                    GameObject slot = new GameObject($"Shelf_{i + 1}_Slot_{j + 1}");
                    slot.transform.SetParent(shelf.transform);
                    slot.transform.localPosition = new Vector3(-0.4f + (j * 0.27f), 0.5f, 0f);
                }
            }
            
            // Add components
            storage.AddComponent<BoxCollider>();
            InteractableObject interactable = storage.AddComponent<InteractableObject>();
            interactable.SetInteractionPrompt("Access Storage");
            
            return storage;
        }
        
        public GameObject CreateTestTubeRack(Vector3 position = default)
        {
            GameObject rack = new GameObject("TestTubeRack_Runtime");
            rack.transform.position = position;
            
            // Base
            GameObject baseObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            baseObj.transform.SetParent(rack.transform);
            baseObj.transform.localPosition = Vector3.zero;
            baseObj.transform.localScale = new Vector3(1f, 0.1f, 0.3f);
            baseObj.name = "Base";
            baseObj.GetComponent<Renderer>().material = _plasticMaterial;
            
            // Create holes for test tubes
            int rows = 2;
            int cols = 6;
            float spacing = 0.15f;
            
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    GameObject hole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    hole.transform.SetParent(rack.transform);
                    hole.transform.localPosition = new Vector3(
                        -0.375f + (c * spacing),
                        0.15f,
                        -0.075f + (r * 0.15f)
                    );
                    hole.transform.localScale = new Vector3(0.08f, 0.2f, 0.08f);
                    hole.name = $"Hole_{r + 1}_{c + 1}";
                    
                    // Make holes slightly darker
                    Material holeMaterial = new Material(_plasticMaterial);
                    holeMaterial.color = _plasticMaterial.color * 0.7f;
                    hole.GetComponent<Renderer>().material = holeMaterial;
                }
            }
            
            // Add components
            rack.AddComponent<BoxCollider>();
            InteractableObject interactable = rack.AddComponent<InteractableObject>();
            interactable.SetInteractionPrompt("Use Test Tube Rack");
            
            return rack;
        }
        
        private Material CreateGlassMaterial()
        {
            Material glass = new Material(Shader.Find("Standard"));
            glass.name = "Glass_Runtime";
            glass.color = new Color(0.9f, 0.9f, 0.95f, 0.3f);
            glass.SetFloat("_Mode", 3); // Transparent mode
            glass.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            glass.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            glass.SetInt("_ZWrite", 0);
            glass.EnableKeyword("_ALPHABLEND_ON");
            glass.renderQueue = 3000;
            glass.SetFloat("_Metallic", 0.1f);
            glass.SetFloat("_Smoothness", 0.9f);
            return glass;
        }
        
        private Material CreateMetalMaterial()
        {
            Material metal = new Material(Shader.Find("Standard"));
            metal.name = "Metal_Runtime";
            metal.color = new Color(0.7f, 0.7f, 0.8f, 1f);
            metal.SetFloat("_Metallic", 0.8f);
            metal.SetFloat("_Smoothness", 0.7f);
            return metal;
        }
        
        private Material CreatePlasticMaterial()
        {
            Material plastic = new Material(Shader.Find("Standard"));
            plastic.name = "Plastic_Runtime";
            plastic.color = new Color(0.9f, 0.9f, 0.9f, 1f);
            plastic.SetFloat("_Metallic", 0.1f);
            plastic.SetFloat("_Smoothness", 0.6f);
            return plastic;
        }
        
        [ContextMenu("Create Sample Equipment Set")]
        public void CreateSampleEquipmentSet()
        {
            Vector3 basePos = transform.position;
            
            CreateCentrifuge(basePos + new Vector3(-2f, 0f, 0f));
            CreateMicroscope(basePos + new Vector3(0f, 0f, 0f));
            CreateStorageUnit(basePos + new Vector3(2f, 0f, 0f));
            CreateTestTubeRack(basePos + new Vector3(0f, 0f, -2f));
            
            Debug.Log("Sample equipment set created!");
        }
    }
}
