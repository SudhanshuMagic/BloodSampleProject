using UnityEngine;
using UnityEngine.Events;

namespace BloodSample.Core
{
    public class InputManager : MonoBehaviour
    {
        [Header("Input Settings")]
        [SerializeField] private LayerMask _interactableLayerMask = -1;
        [SerializeField] private float _maxInteractionDistance = 10f;
        
        [Header("Mouse Look Settings")]
        [SerializeField] private float _mouseSensitivity = 2f;
        [SerializeField] private float _maxLookAngle = 80f;
        [SerializeField] private bool _invertMouseY = false;
        
        [Header("Equipment Dragging Settings")]
        [SerializeField] private float _dragDistance = 5f;
        [SerializeField] private LayerMask _draggableLayerMask = -1;
        [SerializeField] private float _dragSmoothness = 10f;
        
        [Header("Events")]
        public UnityEvent<Vector3> OnPointerClick;
        public UnityEvent<GameObject> OnObjectSelected;
        public UnityEvent OnObjectDeselected;
        
        private Camera _playerCamera;
        private GameObject _selectedObject;
        private IInteractable _currentInteractable;
        
        // Mouse look variables
        private float _currentXRotation = 0f;
        private bool _isMouseLookEnabled = true;
        
        // Equipment dragging variables
        private GameObject _draggedObject;
        private Vector3 _dragOffset;
        private bool _isDragging = false;
        private Rigidbody _draggedRigidbody;
        
        public GameObject SelectedObject => _selectedObject;
        public bool HasSelection => _selectedObject != null;
        public bool IsMouseLookEnabled => _isMouseLookEnabled;
        public bool IsDragging => _isDragging;
        
        private void Awake()
        {
            // Initialize UnityEvents to prevent null reference exceptions
            if (OnPointerClick == null) OnPointerClick = new UnityEvent<Vector3>();
            if (OnObjectSelected == null) OnObjectSelected = new UnityEvent<GameObject>();
            if (OnObjectDeselected == null) OnObjectDeselected = new UnityEvent();
        }
        
        public void Initialize()
        {
            _playerCamera = Camera.main;
            if (_playerCamera == null)
            {
                _playerCamera = FindFirstObjectByType<Camera>();
            }
        }
        
        private void Update()
        {
            HandleInput();
        }
        
        private void HandleInput()
        {
            HandleMouseLook();
            HandleMouseInput();
            HandleEquipmentDragging();
            HandleKeyboardInput();
        }
        
        private void HandleMouseLook()
        {
            if (!_isMouseLookEnabled || _playerCamera == null) return;
            
            // Get mouse movement
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;
            
            if (_invertMouseY) mouseY = -mouseY;
            
            // Rotate camera horizontally (Y-axis rotation)
            _playerCamera.transform.Rotate(Vector3.up * mouseX);
            
            // Rotate camera vertically (X-axis rotation with clamping)
            _currentXRotation -= mouseY;
            _currentXRotation = Mathf.Clamp(_currentXRotation, -_maxLookAngle, _maxLookAngle);
            
            // Apply vertical rotation to camera
            Vector3 currentRotation = _playerCamera.transform.localEulerAngles;
            _playerCamera.transform.localEulerAngles = new Vector3(_currentXRotation, currentRotation.y, currentRotation.z);
        }
        
        private void HandleMouseInput()
        {
            // Ensure camera is available
            if (_playerCamera == null)
            {
                _playerCamera = Camera.main ?? FindFirstObjectByType<Camera>();
                if (_playerCamera == null) return; // No camera found, skip mouse input
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePosition = Input.mousePosition;
                Ray ray = _playerCamera.ScreenPointToRay(mousePosition);
                
                OnPointerClick?.Invoke(mousePosition);
                
                if (Physics.Raycast(ray, out RaycastHit hit, _maxInteractionDistance, _interactableLayerMask))
                {
                    SelectObject(hit.collider.gameObject);
                }
                else
                {
                    DeselectObject();
                }
            }
            
            if (Input.GetMouseButtonUp(0) && _currentInteractable != null)
            {
                _currentInteractable.OnInteractionEnd();
            }
        }
        
        private void HandleEquipmentDragging()
        {
            if (_playerCamera == null) return;
            
            // Start dragging with right mouse button
            if (Input.GetMouseButtonDown(1) && !_isDragging)
            {
                StartDragging();
            }
            
            // Continue dragging
            if (Input.GetMouseButton(1) && _isDragging && _draggedObject != null)
            {
                ContinueDragging();
            }
            
            // Stop dragging
            if (Input.GetMouseButtonUp(1) && _isDragging)
            {
                StopDragging();
            }
        }
        
        private void StartDragging()
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = _playerCamera.ScreenPointToRay(mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, _maxInteractionDistance, _draggableLayerMask))
            {
                GameObject hitObject = hit.collider.gameObject;
                
                // Check if object is draggable (has Rigidbody or is grabbable)
                Rigidbody rb = hitObject.GetComponent<Rigidbody>();
                var grabbable = hitObject.GetComponent<GrabbableObject>();
                
                if (rb != null || grabbable != null)
                {
                    _draggedObject = hitObject;
                    _draggedRigidbody = rb;
                    _isDragging = true;
                    
                    // Calculate offset from object center to hit point
                    _dragOffset = _draggedObject.transform.position - hit.point;
                    
                    // Disable mouse look while dragging
                    _isMouseLookEnabled = false;
                    
                    // Make object kinematic while dragging for smooth movement
                    if (_draggedRigidbody != null)
                    {
                        _draggedRigidbody.isKinematic = true;
                    }
                    
                    Debug.Log($"[InputManager] Started dragging: {_draggedObject.name}");
                }
            }
        }
        
        private void ContinueDragging()
        {
            Vector3 mousePosition = Input.mousePosition;
            
            // Create a plane at the drag distance from camera
            Vector3 targetPosition = _playerCamera.ScreenToWorldPoint(new Vector3(
                mousePosition.x, 
                mousePosition.y, 
                _dragDistance
            ));
            
            // Apply offset and smooth movement
            Vector3 newPosition = targetPosition + _dragOffset;
            _draggedObject.transform.position = Vector3.Lerp(
                _draggedObject.transform.position, 
                newPosition, 
                Time.deltaTime * _dragSmoothness
            );
        }
        
        private void StopDragging()
        {
            if (_draggedObject != null)
            {
                // Re-enable physics if object had Rigidbody
                if (_draggedRigidbody != null)
                {
                    _draggedRigidbody.isKinematic = false;
                }
                
                Debug.Log($"[InputManager] Stopped dragging: {_draggedObject.name}");
            }
            
            _draggedObject = null;
            _draggedRigidbody = null;
            _isDragging = false;
            _isMouseLookEnabled = true; // Re-enable mouse look
        }
        
        private void HandleKeyboardInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                DeselectObject();
            }
            
            if (Input.GetKeyDown(KeyCode.E) && _currentInteractable != null)
            {
                _currentInteractable.OnInteract();
            }
        }
        
        private void SelectObject(GameObject obj)
        {
            if (_selectedObject == obj) return;
            
            DeselectObject();
            
            _selectedObject = obj;
            _currentInteractable = obj.GetComponent<IInteractable>();
            
            if (_currentInteractable != null)
            {
                _currentInteractable.OnSelect();
            }
            
            OnObjectSelected?.Invoke(obj);
            Debug.Log($"Selected: {obj.name}");
        }
        
        private void DeselectObject()
        {
            if (_selectedObject == null) return;
            
            if (_currentInteractable != null)
            {
                _currentInteractable.OnDeselect();
                _currentInteractable = null;
            }
            
            OnObjectDeselected?.Invoke();
            Debug.Log($"Deselected: {_selectedObject.name}");
            _selectedObject = null;
        }
        
        public Vector3 GetMouseWorldPosition()
        {
            // Ensure camera is available
            if (_playerCamera == null)
            {
                _playerCamera = Camera.main ?? FindFirstObjectByType<Camera>();
                if (_playerCamera == null) return Vector3.zero; // No camera found
            }
            
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = _playerCamera.ScreenPointToRay(mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                return hit.point;
            }
            
            return Vector3.zero;
        }
        
        /// <summary>
        /// Enable or disable mouse look functionality
        /// </summary>
        public void SetMouseLookEnabled(bool enabled)
        {
            _isMouseLookEnabled = enabled;
        }
        
        /// <summary>
        /// Set mouse sensitivity for look controls
        /// </summary>
        public void SetMouseSensitivity(float sensitivity)
        {
            _mouseSensitivity = Mathf.Clamp(sensitivity, 0.1f, 10f);
        }
        
        /// <summary>
        /// Force stop any current dragging operation
        /// </summary>
        public void ForceStopDragging()
        {
            if (_isDragging)
            {
                StopDragging();
            }
        }
        
        /// <summary>
        /// Set the drag distance for equipment dragging
        /// </summary>
        public void SetDragDistance(float distance)
        {
            _dragDistance = Mathf.Clamp(distance, 1f, 20f);
        }
        
        /// <summary>
        /// Get current mouse world position for UI or other systems
        /// </summary>
        public Vector3 GetCurrentMouseWorldPosition()
        {
            if (_playerCamera == null) return Vector3.zero;
            
            Vector3 mousePosition = Input.mousePosition;
            return _playerCamera.ScreenToWorldPoint(new Vector3(
                mousePosition.x, 
                mousePosition.y, 
                _dragDistance
            ));
        }
    }
}
                    