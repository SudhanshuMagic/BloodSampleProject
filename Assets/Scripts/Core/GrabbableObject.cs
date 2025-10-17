using UnityEngine;

namespace BloodSample.Core
{
    [RequireComponent(typeof(Rigidbody))]
    public class GrabbableObject : InteractableObject
    {
        [Header("Grab Settings")]
        [SerializeField] private float _grabDistance = 2f;
        [SerializeField] private float _grabForce = 1000f;
        [SerializeField] private float _grabDamping = 5f;
        [SerializeField] private bool _freezeRotationWhenGrabbed = true;
        
        private Rigidbody _rigidbody;
        private bool _isGrabbed;
        private Camera _playerCamera;
        private Vector3 _grabOffset;
        private RigidbodyConstraints _originalConstraints;
        
        public bool IsGrabbed => _isGrabbed;
        
        protected override void Awake()
        {
            base.Awake();
            _rigidbody = GetComponent<Rigidbody>();
            _playerCamera = Camera.main;
            _originalConstraints = _rigidbody.constraints;
        }
        
        public override void OnInteract()
        {
            base.OnInteract();
            
            if (!_isGrabbed)
            {
                GrabObject();
            }
            else
            {
                ReleaseObject();
            }
        }
        
        public override void OnInteractionEnd()
        {
            if (_isGrabbed)
            {
                ReleaseObject();
            }
        }
        
        private void Update()
        {
            if (_isGrabbed)
            {
                UpdateGrabbedObject();
            }
        }
        
        private void GrabObject()
        {
            _isGrabbed = true;
            
            // Calculate grab offset
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            _grabOffset = transform.position - mouseWorldPos;
            
            // Modify physics
            _rigidbody.useGravity = false;
            if (_freezeRotationWhenGrabbed)
            {
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }
            
            SetInteractionPrompt("Release object");
            Debug.Log($"Grabbed {gameObject.name}");
        }
        
        private void ReleaseObject()
        {
            _isGrabbed = false;
            
            // Restore physics
            _rigidbody.useGravity = true;
            _rigidbody.constraints = _originalConstraints;
            
            SetInteractionPrompt("Press E to grab");
            Debug.Log($"Released {gameObject.name}");
        }
        
        private void UpdateGrabbedObject()
        {
            Vector3 targetPosition = GetMouseWorldPosition() + _grabOffset;
            Vector3 direction = targetPosition - transform.position;
            
            // Apply force to move towards target position
            _rigidbody.AddForce(direction * _grabForce - _rigidbody.velocity * _grabDamping);
            
            // Limit distance from camera
            float distanceFromCamera = Vector3.Distance(transform.position, _playerCamera.transform.position);
            if (distanceFromCamera > _grabDistance)
            {
                Vector3 directionFromCamera = (transform.position - _playerCamera.transform.position).normalized;
                transform.position = _playerCamera.transform.position + directionFromCamera * _grabDistance;
            }
        }
        
        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = _grabDistance;
            return _playerCamera.ScreenToWorldPoint(mousePosition);
        }
        
        public override void OnDeselect()
        {
            base.OnDeselect();
            if (_isGrabbed)
            {
                ReleaseObject();
            }
        }
    }
}
