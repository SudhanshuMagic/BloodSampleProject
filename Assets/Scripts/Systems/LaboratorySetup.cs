using UnityEngine;

namespace BloodSample.Systems
{
    /// <summary>
    /// Simple laboratory setup component for scene organization
    /// </summary>
    public class LaboratorySetup : MonoBehaviour
    {
        [Header("Laboratory Configuration")]
        [SerializeField] private Vector3[] _workstationPositions;
        [SerializeField] private bool _autoSetupOnStart = false;
        
        private void Start()
        {
            if (_autoSetupOnStart)
            {
                SetupLaboratory();
            }
        }
        
        public void SetupLaboratory()
        {
            Debug.Log("[LaboratorySetup] Laboratory setup initiated");
        }
        
        public Vector3[] GetWorkstationPositions()
        {
            return _workstationPositions ?? new Vector3[0];
        }
        
        public void SetWorkstationPositions(Vector3[] positions)
        {
            _workstationPositions = positions;
        }
    }
}
