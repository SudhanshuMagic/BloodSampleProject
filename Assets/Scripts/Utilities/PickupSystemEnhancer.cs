using UnityEngine;
using BloodSample.Core;
using BloodSample.Systems;

namespace BloodSample.Utilities
{
    /// <summary>
    /// Enhances existing equipment to make them grabbable
    /// Run this once to convert InteractableObjects to GrabbableObjects
    /// </summary>
    public class PickupSystemEnhancer : MonoBehaviour
    {
        [Header("Enhancement Settings")]
        [SerializeField] private bool _enhanceOnStart = true;
        [SerializeField] private bool _makeWorkstationsGrabbable = false; // Heavy objects
        [SerializeField] private bool _makeEquipmentGrabbable = true;
        
        [Header("Grab Settings for Enhanced Objects")]
        [SerializeField] private float _grabDistance = 2f;
        [SerializeField] private float _grabForce = 1000f;
        [SerializeField] private float _grabDamping = 5f;
        
        private void Start()
        {
            if (_enhanceOnStart)
            {
                Invoke(nameof(EnhanceAllEquipment), 1f); // Wait for auto-setup to complete
            }
        }
        
        [ContextMenu("Enhance All Equipment for Pickup")]
        public void EnhanceAllEquipment()
        {
            Debug.Log("[PickupEnhancer] 🔧 Enhancing equipment for pickup...");
            
            int enhanced = 0;
            
            // Find all InteractableObjects that aren't already GrabbableObjects
            var interactables = FindObjectsByType<InteractableObject>(FindObjectsSortMode.None);
            
            foreach (var obj in interactables)
            {
                // Skip if object is null or destroyed
                if (obj == null) continue;
                
                // Skip if already grabbable
                if (obj is GrabbableObject) continue;
                
                // Skip workstations if disabled
                if (obj is Workstation && !_makeWorkstationsGrabbable) continue;
                
                // Check if object has Rigidbody (required for grabbing)
                var rb = obj.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    // Add Rigidbody if missing
                    rb = obj.gameObject.AddComponent<Rigidbody>();
                    rb.mass = 1f;
                    rb.drag = 1f;
                    rb.angularDrag = 5f;
                }
                
                // Convert to GrabbableObject
                if (ConvertToGrabbable(obj))
                {
                    enhanced++;
                    // Check if object still exists before accessing its name
                    if (obj != null)
                    {
                        Debug.Log($"[PickupEnhancer] ✅ Enhanced {obj.name} for pickup");
                    }
                    else
                    {
                        Debug.Log($"[PickupEnhancer] ✅ Enhanced object for pickup (object reference lost)");
                    }
                }
            }
            
            Debug.Log($"[PickupEnhancer] 🎯 Enhanced {enhanced} objects for pickup!");
        }
        
        private bool ConvertToGrabbable(InteractableObject original)
        {
            try
            {
                // Add GrabbableObject component
                var grabbable = original.gameObject.AddComponent<GrabbableObject>();
                
                // Configure grab settings using reflection to set private fields
                SetGrabSettings(grabbable);
                
                // Copy interaction settings from original
                CopyInteractionSettings(original, grabbable);
                
                // Remove original InteractableObject component
                DestroyImmediate(original);
                
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[PickupEnhancer] Failed to enhance {original.name}: {e.Message}");
                return false;
            }
        }
        
        private void SetGrabSettings(GrabbableObject grabbable)
        {
            // Use reflection to set private fields
            var type = typeof(GrabbableObject);
            
            var grabDistanceField = type.GetField("_grabDistance", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            grabDistanceField?.SetValue(grabbable, _grabDistance);
            
            var grabForceField = type.GetField("_grabForce", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            grabForceField?.SetValue(grabbable, _grabForce);
            
            var grabDampingField = type.GetField("_grabDamping", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            grabDampingField?.SetValue(grabbable, _grabDamping);
        }
        
        private void CopyInteractionSettings(InteractableObject from, GrabbableObject to)
        {
            // Copy basic interaction settings
            to.SetInteractionPrompt(from.InteractionPrompt);
            
            // Copy events
            to.OnSelected = from.OnSelected;
            to.OnDeselected = from.OnDeselected;
            to.OnInteracted = from.OnInteracted;
        }
        
        [ContextMenu("Make All Blood Samples Extra Grabbable")]
        public void EnhanceBloodSamples()
        {
            var samples = FindObjectsByType<BloodSample.Systems.BloodSample>(FindObjectsSortMode.None);
            
            foreach (var sample in samples)
            {
                // Blood samples are already GrabbableObjects, just enhance their settings
                SetGrabSettings(sample);
                Debug.Log($"[PickupEnhancer] 🩸 Enhanced blood sample: {sample.name}");
            }
            
            Debug.Log($"[PickupEnhancer] Enhanced {samples.Length} blood samples!");
        }
        
        [ContextMenu("Create Grabbable Test Objects")]
        public void CreateTestObjects()
        {
            // Create some test objects for pickup testing
            for (int i = 0; i < 3; i++)
            {
                GameObject testObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                testObj.name = $"GrabbableTestCube_{i + 1}";
                testObj.transform.position = transform.position + new Vector3(i * 2f, 1f, 0f);
                
                // Add Rigidbody
                var rb = testObj.AddComponent<Rigidbody>();
                rb.mass = 0.5f;
                
                // Add GrabbableObject
                var grabbable = testObj.AddComponent<GrabbableObject>();
                SetGrabSettings(grabbable);
                
                // Set interaction prompt
                grabbable.SetInteractionPrompt($"Grab Test Cube {i + 1}");
                
                Debug.Log($"[PickupEnhancer] 🧪 Created test object: {testObj.name}");
            }
        }
    }
}
