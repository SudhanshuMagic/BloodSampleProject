using UnityEngine;

namespace BloodSample.Core
{
    /// <summary>
    /// Place this script on ANY GameObject in the scene and it will automatically trigger the setup
    /// Alternative method if the bootstrap doesn't work - just add to any object
    /// </summary>
    public class AutoSetupTrigger : MonoBehaviour
    {
        [Header("Auto Trigger Settings")]
        [SerializeField] private bool _triggerOnStart = true;
        [SerializeField] private bool _destroyAfterTrigger = true;
        
        private void Awake()
        {
            if (_triggerOnStart)
            {
                TriggerAutoSetup();
            }
        }
        
        [ContextMenu("Trigger Auto Setup")]
        public void TriggerAutoSetup()
        {
            // Check if auto-setup already exists
            AutoSetupManager existingSetup = FindObjectOfType<AutoSetupManager>();
            
            if (existingSetup == null)
            {
                Debug.Log("🎯 AutoSetupTrigger: Creating laboratory setup...");
                
                // Create the auto-setup on this same GameObject or a new one
                GameObject setupObj = gameObject;
                if (gameObject.GetComponent<AutoSetupManager>() == null)
                {
                    setupObj.AddComponent<AutoSetupManager>();
                }
            }
            else
            {
                Debug.Log("✅ AutoSetupTrigger: Laboratory setup already exists!");
            }
            
            // Optionally destroy this trigger after use
            if (_destroyAfterTrigger)
            {
                Destroy(this); // Only destroy this component, not the whole GameObject
            }
        }
    }
}
