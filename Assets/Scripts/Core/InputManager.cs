using UnityEngine;
using UnityEngine.Events;

namespace BloodSample.Core
{
    public class InputManager : MonoBehaviour
    {
        [Header("Input Settings")]
        [SerializeField] private LayerMask _interactableLayerMask = 1;
        [SerializeField] private float _maxInteractionDistance = 5f;
        
        [Header("Events")]
        public UnityEvent<Vector3> OnPointerClick;
        public UnityEvent<GameObject> OnObjectSelected;
        public UnityEvent OnObjectDeselected;
        
        private Camera _playerCamera;
        private GameObject _selectedObject;
        private IInteractable _currentInteractable;
        
        public GameObject SelectedObject => _selectedObject;
        public bool HasSelection => _selectedObject != null;
        
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
            HandleMouseInput();
            HandleKeyboardInput();
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
    }
}
                    