using UnityEngine;

namespace BloodSample.Utilities
{
    /// <summary>
    /// Optimizes laboratory layout for better equipment visibility
    /// </summary>
    public class LaboratoryLayoutOptimizer : MonoBehaviour
    {
        [Header("Layout Optimization")]
        [SerializeField] private bool _removeObstructingWalls = true;
        [SerializeField] private bool _optimizeOnStart = true;
        
        private void Start()
        {
            if (_optimizeOnStart)
            {
                Invoke(nameof(OptimizeLaboratoryLayout), 1f); // Wait for lab generation
            }
        }
        
        /// <summary>
        /// Remove any walls that obstruct equipment visibility
        /// </summary>
        [ContextMenu("Optimize Laboratory Layout")]
        public void OptimizeLaboratoryLayout()
        {
            Debug.Log("[LaboratoryLayoutOptimizer] 🔧 Optimizing laboratory layout for equipment visibility...");
            
            if (_removeObstructingWalls)
            {
                RemoveObstructingWalls();
            }
            
            OptimizeEquipmentPositions();
            
            Debug.Log("[LaboratoryLayoutOptimizer] ✅ Laboratory layout optimized!");
        }
        
        /// <summary>
        /// Remove walls that block equipment visibility
        /// </summary>
        private void RemoveObstructingWalls()
        {
            // Find all walls that might be obstructing view
            GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            
            int removedWalls = 0;
            
            foreach (GameObject obj in allObjects)
            {
                if (obj == null) continue;
                
                // Check if it's a wall that might be obstructing
                if (IsObstructingWall(obj))
                {
                    Debug.Log($"[LaboratoryLayoutOptimizer] Removing obstructing wall: {obj.name}");
                    Destroy(obj);
                    removedWalls++;
                }
            }
            
            Debug.Log($"[LaboratoryLayoutOptimizer] Removed {removedWalls} obstructing walls");
        }
        
        /// <summary>
        /// Check if a wall is obstructing equipment visibility
        /// </summary>
        private bool IsObstructingWall(GameObject obj)
        {
            // Check if it's a wall-like object
            if (!obj.name.ToLower().Contains("wall")) return false;
            
            // Check if it's in the center area where it would obstruct view
            Vector3 pos = obj.transform.position;
            
            // Remove walls that are too close to center (within 15 units)
            bool tooCloseToCenter = (Mathf.Abs(pos.x) < 15f && Mathf.Abs(pos.z) < 15f);
            
            // Remove walls that are blocking the main view area
            bool blockingMainView = (pos.z > -5f && pos.z < 5f && Mathf.Abs(pos.x) < 20f);
            
            return tooCloseToCenter || blockingMainView;
        }
        
        /// <summary>
        /// Optimize equipment positions for better visibility
        /// </summary>
        private void OptimizeEquipmentPositions()
        {
            // Find equipment that might be hidden
            var equipment = FindObjectsByType<BloodSample.Systems.LaboratoryEquipment>(FindObjectsSortMode.None);
            var workstations = FindObjectsByType<BloodSample.Systems.Workstation>(FindObjectsSortMode.None);
            
            Debug.Log($"[LaboratoryLayoutOptimizer] Found {equipment.Length} equipment and {workstations.Length} workstations");
            
            // Ensure all equipment is in visible positions
            int repositioned = 0;
            
            foreach (var equip in equipment)
            {
                if (equip == null) continue;
                
                if (IsEquipmentHidden(equip.transform.position))
                {
                    Vector3 newPos = FindVisiblePosition(equip.transform.position);
                    equip.transform.position = newPos;
                    repositioned++;
                    Debug.Log($"[LaboratoryLayoutOptimizer] Repositioned {equip.name} to {newPos}");
                }
            }
            
            Debug.Log($"[LaboratoryLayoutOptimizer] Repositioned {repositioned} equipment for better visibility");
        }
        
        /// <summary>
        /// Check if equipment is in a hidden position
        /// </summary>
        private bool IsEquipmentHidden(Vector3 position)
        {
            // Check if equipment is behind walls or in corners
            return (Mathf.Abs(position.x) > 30f || Mathf.Abs(position.z) > 30f);
        }
        
        /// <summary>
        /// Find a visible position for equipment
        /// </summary>
        private Vector3 FindVisiblePosition(Vector3 originalPos)
        {
            // Move equipment to a more central, visible location
            Vector3 newPos = originalPos;
            
            // Clamp to visible area
            newPos.x = Mathf.Clamp(newPos.x, -20f, 20f);
            newPos.z = Mathf.Clamp(newPos.z, -20f, 20f);
            
            return newPos;
        }
        
        /// <summary>
        /// Create an optimal open laboratory layout
        /// </summary>
        [ContextMenu("Create Open Laboratory Layout")]
        public void CreateOpenLaboratoryLayout()
        {
            Debug.Log("[LaboratoryLayoutOptimizer] 🏗️ Creating optimal open laboratory layout...");
            
            // Remove all existing walls first
            RemoveAllWalls();
            
            // Create only minimal boundary walls
            CreateBoundaryWalls();
            
            Debug.Log("[LaboratoryLayoutOptimizer] ✅ Open laboratory layout created!");
        }
        
        private void RemoveAllWalls()
        {
            GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            
            foreach (GameObject obj in allObjects)
            {
                if (obj != null && obj.name.ToLower().Contains("wall"))
                {
                    Destroy(obj);
                }
            }
        }
        
        private void CreateBoundaryWalls()
        {
            // Create minimal boundary walls far from center
            var materialGen = ModernLabMaterialGenerator.Instance;
            var wallMaterial = materialGen.CreateLabWallMaterial();
            
            // Far boundary walls
            CreateWall("BoundaryNorth", new Vector3(0, 2.5f, 40f), new Vector3(80f, 5f, 1f), wallMaterial);
            CreateWall("BoundarySouth", new Vector3(0, 2.5f, -40f), new Vector3(80f, 5f, 1f), wallMaterial);
            CreateWall("BoundaryEast", new Vector3(40f, 2.5f, 0), new Vector3(1f, 5f, 80f), wallMaterial);
            CreateWall("BoundaryWest", new Vector3(-40f, 2.5f, 0), new Vector3(1f, 5f, 80f), wallMaterial);
        }
        
        private void CreateWall(string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.position = position;
            wall.transform.localScale = scale;
            wall.GetComponent<Renderer>().material = material;
            wall.transform.SetParent(transform);
        }
    }
}
