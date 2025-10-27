using UnityEngine;

namespace BloodSample.Utilities
{
    /// <summary>
    /// Simple utility to remove obstructing walls for better equipment visibility
    /// </summary>
    public class WallRemover : MonoBehaviour
    {
        [Header("Wall Removal Settings")]
        [SerializeField] private bool _removeOnStart = true;
        [SerializeField] private float _centralAreaRadius = 15f;
        
        private void Start()
        {
            if (_removeOnStart)
            {
                Invoke(nameof(RemoveCentralWalls), 2f); // Wait for lab generation
            }
        }
        
        /// <summary>
        /// Remove any walls in the central laboratory area
        /// </summary>
        [ContextMenu("Remove Central Walls")]
        public void RemoveCentralWalls()
        {
            Debug.Log("[WallRemover] 🔧 Removing central walls for better equipment visibility...");
            
            int removedCount = 0;
            
            // Find all GameObjects that might be walls
            GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            
            foreach (GameObject obj in allObjects)
            {
                if (obj == null) continue;
                
                // Check if it's a wall object
                if (IsWallObject(obj))
                {
                    Vector3 pos = obj.transform.position;
                    float distanceFromCenter = Vector3.Distance(pos, Vector3.zero);
                    
                    // Remove walls within the central area
                    if (distanceFromCenter < _centralAreaRadius)
                    {
                        Debug.Log($"[WallRemover] Removing central wall: {obj.name} at {pos}");
                        Destroy(obj);
                        removedCount++;
                    }
                }
            }
            
            Debug.Log($"[WallRemover] ✅ Removed {removedCount} central walls");
        }
        
        /// <summary>
        /// Check if an object is a wall
        /// </summary>
        private bool IsWallObject(GameObject obj)
        {
            string name = obj.name.ToLower();
            
            // Check for wall-like names
            if (name.Contains("wall") || name.Contains("barrier") || name.Contains("partition"))
            {
                return true;
            }
            
            // Check for wall-like properties (large, thin objects)
            Vector3 scale = obj.transform.localScale;
            bool isThin = (scale.x > scale.y * 2 && scale.z < scale.x * 0.5f) || 
                          (scale.z > scale.y * 2 && scale.x < scale.z * 0.5f);
            bool isLarge = scale.x > 10f || scale.z > 10f;
            
            return isThin && isLarge;
        }
        
        /// <summary>
        /// Remove all walls except perimeter boundary walls
        /// </summary>
        [ContextMenu("Remove All Interior Walls")]
        public void RemoveAllInteriorWalls()
        {
            Debug.Log("[WallRemover] 🔧 Removing all interior walls...");
            
            int removedCount = 0;
            GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            
            foreach (GameObject obj in allObjects)
            {
                if (obj == null) continue;
                
                if (IsWallObject(obj))
                {
                    Vector3 pos = obj.transform.position;
                    
                    // Keep only far perimeter walls
                    bool isPerimeterWall = (Mathf.Abs(pos.x) > 30f || Mathf.Abs(pos.z) > 30f);
                    
                    if (!isPerimeterWall)
                    {
                        Debug.Log($"[WallRemover] Removing interior wall: {obj.name}");
                        Destroy(obj);
                        removedCount++;
                    }
                }
            }
            
            Debug.Log($"[WallRemover] ✅ Removed {removedCount} interior walls");
        }
        
        /// <summary>
        /// Create a completely open laboratory space
        /// </summary>
        [ContextMenu("Create Open Laboratory")]
        public void CreateOpenLaboratory()
        {
            Debug.Log("[WallRemover] 🏗️ Creating completely open laboratory...");
            
            // Remove all interior walls
            RemoveAllInteriorWalls();
            
            // Expand floor if needed
            GameObject floor = GameObject.Find("LabFloor");
            if (floor != null)
            {
                floor.transform.localScale = new Vector3(10f, 1f, 10f);
                Debug.Log("[WallRemover] Expanded laboratory floor");
            }
            
            Debug.Log("[WallRemover] ✅ Open laboratory created!");
        }
    }
}
