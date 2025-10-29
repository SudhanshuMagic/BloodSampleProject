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
        [SerializeField] private float _mouseSensitivity = 3f;
        [SerializeField] private float _maxLookAngle = 90f;
        [SerializeField] private bool _invertMouseY = false;
        [SerializeField] private bool _lockCursor = true;
        
        [Header("Equipment Dragging Settings")]
        [SerializeField] private float _dragDistance = 2f;
        [SerializeField] private LayerMask _draggableLayerMask = -1;
        [SerializeField] private float _dragSmoothness = 15f;
        [SerializeField] private float _dragHeightOffset = 0.5f;
        
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
            Debug.Log("[InputManager] Initializing input system...");
            
            // Try to find the player camera if not assigned
            _playerCamera = Camera.main;
            if (_playerCamera == null)
            {
                _playerCamera = FindFirstObjectByType<Camera>();
            }
            
            if (_playerCamera != null)
            {
                Debug.Log($"[InputManager] Camera assigned: {_playerCamera.name}");
            }
            else
            {
                Debug.LogWarning("[InputManager] No camera found. Mouse look will not work.");
            }
            
            // Initialize cursor state
            if (_lockCursor && _isMouseLookEnabled)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            
            // Initialize rotation tracking
            if (_playerCamera != null)
            {
                _currentXRotation = _playerCamera.transform.localEulerAngles.x;
                // Handle Unity's 0-360 angle representation
                if (_currentXRotation > 180f)
                {
                    _currentXRotation -= 360f;
                }
            }
        }
        
        private void Start()
        {
            Initialize();
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
            
            // Lock cursor for better mouse look experience
            if (_lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            
            // Get mouse movement
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;
            
            if (_invertMouseY) mouseY = -mouseY;
            
            // Apply horizontal rotation (Y-axis) - unlimited 360 degree rotation
            _playerCamera.transform.Rotate(Vector3.up * mouseX, Space.World);
            
            // Apply vertical rotation (X-axis) with clamping
            _currentXRotation -= mouseY;
            _currentXRotation = Mathf.Clamp(_currentXRotation, -_maxLookAngle, _maxLookAngle);
            
            // Set the camera's local X rotation directly
            Vector3 currentEuler = _playerCamera.transform.localEulerAngles;
            _playerCamera.transform.localEulerAngles = new Vector3(_currentXRotation, currentEuler.y, 0f);
            
            // Allow escape key to unlock cursor
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _isMouseLookEnabled = false;
            }
            
            // Click to re-enable mouse look after escape
            if (Input.GetMouseButtonDown(0) && !_isMouseLookEnabled)
            {
                _isMouseLookEnabled = true;
            }
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
                    
                    // Calculate a more intuitive offset for dragging
                    Vector3 screenPoint = _playerCamera.WorldToScreenPoint(_draggedObject.transform.position);
                    Vector3 currentMouseWorld = _playerCamera.ScreenToWorldPoint(new Vector3(
                        Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
                    _dragOffset = _draggedObject.transform.position - currentMouseWorld;
                    
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
            
            // Create a plane at the drag distance from camera with height offset
            Vector3 targetPosition = _playerCamera.ScreenToWorldPoint(new Vector3(
                mousePosition.x, 
                mousePosition.y, 
                _dragDistance
            ));
            
            // Add height offset to keep object visible above surfaces
            targetPosition += Vector3.up * _dragHeightOffset;
            
            // Apply offset and smooth movement with improved interpolation
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
                // Reset physics forces and enable proper drop behavior
                if (_draggedRigidbody != null)
                {
                    // Stop any accumulated velocity to prevent flying away
                    _draggedRigidbody.velocity = Vector3.zero;
                    _draggedRigidbody.angularVelocity = Vector3.zero;
                    
                    // Re-enable physics with gentle settling
                    _draggedRigidbody.isKinematic = false;
                    
                    // Add slight downward velocity for natural drop
                    _draggedRigidbody.velocity = new Vector3(0, -2f, 0);
                }
                
                Debug.Log($"[InputManager] Stopped dragging: {_draggedObject.name} - Ready for workstation detection");
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
            Debug.Log($"[InputManager] Mouse sensitivity set to: {_mouseSensitivity}");
        }
        
        /// <summary>
        /// Toggle cursor lock state
        /// </summary>
        public void ToggleCursorLock()
        {
            _lockCursor = !_lockCursor;
            
            if (_lockCursor && _isMouseLookEnabled)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Debug.Log("[InputManager] Cursor locked for mouse look");
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Debug.Log("[InputManager] Cursor unlocked");
            }
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
            _dragDistance = Mathf.Clamp(distance, 0.5f, 10f);
            Debug.Log($"[InputManager] Drag distance set to: {_dragDistance}");
        }
        
        /// <summary>
        /// Set the drag height offset
        /// </summary>
        public void SetDragHeightOffset(float heightOffset)
        {
            _dragHeightOffset = Mathf.Clamp(heightOffset, 0f, 2f);
            Debug.Log($"[InputManager] Drag height offset set to: {_dragHeightOffset}");
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
                    