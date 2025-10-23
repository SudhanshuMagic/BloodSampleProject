using UnityEngine;

namespace BloodSample.Utilities
{
    /// <summary>
    /// Generates modern laboratory materials and aesthetics
    /// </summary>
    public class ModernLabMaterialGenerator : MonoBehaviour
    {
        [Header("Modern Lab Colors")]
        [SerializeField] private Color _sterileWhite = new Color(0.95f, 0.95f, 1f, 1f);
        [SerializeField] private Color _medicalBlue = new Color(0.2f, 0.4f, 0.8f, 1f);
        [SerializeField] private Color _safetyGreen = new Color(0.2f, 0.8f, 0.3f, 1f);
        [SerializeField] private Color _warningOrange = new Color(1f, 0.6f, 0.1f, 1f);
        [SerializeField] private Color _stainlessSteel = new Color(0.7f, 0.7f, 0.8f, 1f);
        
        private static ModernLabMaterialGenerator _instance;
        public static ModernLabMaterialGenerator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<ModernLabMaterialGenerator>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject("ModernLabMaterialGenerator");
                        _instance = obj.AddComponent<ModernLabMaterialGenerator>();
                    }
                }
                return _instance;
            }
        }
        
        /// <summary>
        /// Create sterile white material for clean surfaces
        /// </summary>
        public Material CreateSterileWhiteMaterial()
        {
            Material material = new Material(Shader.Find("Standard"));
            material.name = "Modern_SterileWhite";
            material.color = _sterileWhite;
            material.SetFloat("_Metallic", 0.1f);
            material.SetFloat("_Smoothness", 0.9f);
            return material;
        }
        
        /// <summary>
        /// Create stainless steel material for equipment
        /// </summary>
        public Material CreateStainlessSteelMaterial()
        {
            Material material = new Material(Shader.Find("Standard"));
            material.name = "Modern_StainlessSteel";
            material.color = _stainlessSteel;
            material.SetFloat("_Metallic", 0.8f);
            material.SetFloat("_Smoothness", 0.95f);
            return material;
        }
        
        /// <summary>
        /// Create medical blue material for accents
        /// </summary>
        public Material CreateMedicalBlueMaterial()
        {
            Material material = new Material(Shader.Find("Standard"));
            material.name = "Modern_MedicalBlue";
            material.color = _medicalBlue;
            material.SetFloat("_Metallic", 0.2f);
            material.SetFloat("_Smoothness", 0.8f);
            return material;
        }
        
        /// <summary>
        /// Create safety green material for indicators
        /// </summary>
        public Material CreateSafetyGreenMaterial()
        {
            Material material = new Material(Shader.Find("Standard"));
            material.name = "Modern_SafetyGreen";
            material.color = _safetyGreen;
            material.SetFloat("_Metallic", 0f);
            material.SetFloat("_Smoothness", 0.6f);
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", _safetyGreen * 0.3f);
            return material;
        }
        
        /// <summary>
        /// Create warning orange material for hazards
        /// </summary>
        public Material CreateWarningOrangeMaterial()
        {
            Material material = new Material(Shader.Find("Standard"));
            material.name = "Modern_WarningOrange";
            material.color = _warningOrange;
            material.SetFloat("_Metallic", 0f);
            material.SetFloat("_Smoothness", 0.7f);
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", _warningOrange * 0.2f);
            return material;
        }
        
        /// <summary>
        /// Create transparent glass material
        /// </summary>
        public Material CreateGlassMaterial()
        {
            Material material = new Material(Shader.Find("Standard"));
            material.name = "Modern_Glass";
            material.color = new Color(0.9f, 0.95f, 1f, 0.3f);
            material.SetFloat("_Mode", 3); // Transparent mode
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
            material.SetFloat("_Metallic", 0f);
            material.SetFloat("_Smoothness", 1f);
            return material;
        }
        
        /// <summary>
        /// Create computer screen material
        /// </summary>
        public Material CreateComputerScreenMaterial(bool isOn = true)
        {
            Material material = new Material(Shader.Find("Standard"));
            material.name = isOn ? "Modern_ScreenOn" : "Modern_ScreenOff";
            
            if (isOn)
            {
                material.color = new Color(0.1f, 0.2f, 0.3f, 1f);
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", new Color(0.2f, 0.4f, 0.8f, 1f));
            }
            else
            {
                material.color = new Color(0.1f, 0.1f, 0.1f, 1f);
            }
            
            material.SetFloat("_Metallic", 0f);
            material.SetFloat("_Smoothness", 0.9f);
            return material;
        }
        
        /// <summary>
        /// Create guidance marker material with glow
        /// </summary>
        public Material CreateGuidanceMarkerMaterial()
        {
            Material material = new Material(Shader.Find("Standard"));
            material.name = "Modern_GuidanceMarker";
            material.color = new Color(0.2f, 0.8f, 1f, 0.8f);
            material.SetFloat("_Mode", 3); // Transparent mode
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
            
            // Add emission for glow effect
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", new Color(0.3f, 1f, 1.5f, 1f));
            
            material.SetFloat("_Metallic", 0f);
            material.SetFloat("_Smoothness", 0.9f);
            return material;
        }
        
        /// <summary>
        /// Create floor material for laboratory
        /// </summary>
        public Material CreateLabFloorMaterial()
        {
            Material material = new Material(Shader.Find("Standard"));
            material.name = "Modern_LabFloor";
            material.color = new Color(0.9f, 0.9f, 0.95f, 1f);
            material.SetFloat("_Metallic", 0f);
            material.SetFloat("_Smoothness", 0.8f);
            
            // Add subtle grid pattern using normal map
            // In a real implementation, you'd load a texture here
            
            return material;
        }
        
        /// <summary>
        /// Create wall material for laboratory
        /// </summary>
        public Material CreateLabWallMaterial()
        {
            Material material = new Material(Shader.Find("Standard"));
            material.name = "Modern_LabWall";
            material.color = _sterileWhite;
            material.SetFloat("_Metallic", 0f);
            material.SetFloat("_Smoothness", 0.4f);
            return material;
        }
        
        /// <summary>
        /// Apply modern laboratory lighting setup
        /// </summary>
        public void SetupModernLighting()
        {
            // Set ambient lighting for sterile environment
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.95f, 0.95f, 1f, 1f);
            RenderSettings.ambientEquatorColor = new Color(0.9f, 0.9f, 0.95f, 1f);
            RenderSettings.ambientGroundColor = new Color(0.8f, 0.8f, 0.85f, 1f);
            
            // Ensure proper lighting for medical environment
            RenderSettings.fog = false; // Clear air in sterile environment
            
            Debug.Log("[ModernLabMaterialGenerator] Modern laboratory lighting configured");
        }
        
        /// <summary>
        /// Create all standard laboratory materials at once
        /// </summary>
        public void GenerateAllLaboratoryMaterials()
        {
            Debug.Log("[ModernLabMaterialGenerator] Generating complete modern laboratory material set...");
            
            CreateSterileWhiteMaterial();
            CreateStainlessSteelMaterial();
            CreateMedicalBlueMaterial();
            CreateSafetyGreenMaterial();
            CreateWarningOrangeMaterial();
            CreateGlassMaterial();
            CreateComputerScreenMaterial(true);
            CreateComputerScreenMaterial(false);
            CreateGuidanceMarkerMaterial();
            CreateLabFloorMaterial();
            CreateLabWallMaterial();
            
            SetupModernLighting();
            
            Debug.Log("[ModernLabMaterialGenerator] ✅ Modern laboratory materials generated successfully!");
        }
    }
}
