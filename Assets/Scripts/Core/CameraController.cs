using UnityEngine;

namespace BloodSample.Core
{
    public class CameraController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _rotationSpeed = 2f;
        [SerializeField] private float _zoomSpeed = 2f;
        
        [Header("Constraints")]
        [SerializeField] private float _minZoom = 2f;
        [SerializeField] private float _maxZoom = 10f;
        [SerializeField] private Vector2 _pitchLimits = new Vector2(-30f, 60f);
        
        [Header("Input Settings")]
        [SerializeField] private bool _invertMouseY = false;
        [SerializeField] private KeyCode _rotateKey = KeyCode.Mouse1;
        
        private Camera _camera;
        private Vector3 _lastMousePosition;
        private float _currentPitch;
        private float _currentYaw;
        
        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _currentPitch = transform.eulerAngles.x;
            _currentYaw = transform.eulerAngles.y;
        }
        
        private void Update()
        {
            HandleMovement();
            HandleRotation();
            HandleZoom();
        }
        
        private void HandleMovement()
        {
            Vector3 moveDirection = Vector3.zero;
            
            // WASD movement
            if (Input.GetKey(KeyCode.W)) moveDirection += transform.forward;
            if (Input.GetKey(KeyCode.S)) moveDirection -= transform.forward;
            if (Input.GetKey(KeyCode.A)) moveDirection -= transform.right;
            if (Input.GetKey(KeyCode.D)) moveDirection += transform.right;
            if (Input.GetKey(KeyCode.Q)) moveDirection += Vector3.up;
            if (Input.GetKey(KeyCode.E)) moveDirection -= Vector3.up;
            
            // Apply movement
            if (moveDirection.magnitude > 0.1f)
            {
                moveDirection.Normalize();
                transform.Translate(moveDirection * _moveSpeed * Time.deltaTime, Space.World);
            }
        }
        
        private void HandleRotation()
        {
            if (Input.GetKey(_rotateKey))
            {
                Vector3 mouseDelta = Input.mousePosition - _lastMousePosition;
                
                _currentYaw += mouseDelta.x * _rotationSpeed * Time.deltaTime;
                _currentPitch -= mouseDelta.y * _rotationSpeed * Time.deltaTime * (_invertMouseY ? -1f : 1f);
                
                _currentPitch = Mathf.Clamp(_currentPitch, _pitchLimits.x, _pitchLimits.y);
                
                transform.rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);
                
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            
            _lastMousePosition = Input.mousePosition;
        }
        
        private void HandleZoom()
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            
            if (Mathf.Abs(scrollInput) > 0.01f)
            {
                if (_camera.orthographic)
                {
                    _camera.orthographicSize -= scrollInput * _zoomSpeed;
                    _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, _minZoom, _maxZoom);
                }
                else
                {
                    float currentFOV = _camera.fieldOfView;
                    currentFOV -= scrollInput * _zoomSpeed * 10f;
                    _camera.fieldOfView = Mathf.Clamp(currentFOV, 15f, 90f);
                }
            }
        }
        
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }
        
        public void SetRotation(Vector3 eulerAngles)
        {
            _currentPitch = eulerAngles.x;
            _currentYaw = eulerAngles.y;
            transform.rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);
        }
        
        public void FocusOnObject(Transform target, Vector3 offset)
        {
            if (target == null) return;
            
            Vector3 targetPosition = target.position + offset;
            transform.position = targetPosition;
            transform.LookAt(target);
            
            Vector3 lookRotation = transform.eulerAngles;
            _currentPitch = lookRotation.x;
            _currentYaw = lookRotation.y;
        }
    }
}
