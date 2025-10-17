using UnityEngine;
using UnityEngine.Events;

namespace BloodSample.Core
{
    public class InteractableObject : MonoBehaviour, IInteractable
    {
        [Header("Interaction Settings")]
        [SerializeField] private bool _canInteract = true;
        [SerializeField] private string _interactionPrompt = "Press E to interact";
        [SerializeField] private Material _highlightMaterial;
        
        [Header("Events")]
        public UnityEvent OnSelected;
        public UnityEvent OnDeselected;
        public UnityEvent OnInteracted;
        
        private Renderer _renderer;
        private Material _originalMaterial;
        private bool _isSelected;
        
        public bool CanInteract => _canInteract;
        public string InteractionPrompt => _interactionPrompt;
        public bool IsSelected => _isSelected;
        
        protected virtual void Awake()
        {
            _renderer = GetComponent<Renderer>();
            if (_renderer != null)
            {
                _originalMaterial = _renderer.material;
            }
        }
        
        public virtual void OnSelect()
        {
            if (!_canInteract) return;
            
            _isSelected = true;
            ApplyHighlight();
            OnSelected?.Invoke();
        }
        
        public virtual void OnDeselect()
        {
            _isSelected = false;
            RemoveHighlight();
            OnDeselected?.Invoke();
        }
        
        public virtual void OnInteract()
        {
            if (!_canInteract) return;
            
            OnInteracted?.Invoke();
            Debug.Log($"Interacted with {gameObject.name}");
        }
        
        public virtual void OnInteractionEnd()
        {
            // Override in derived classes for specific behavior
        }
        
        private void ApplyHighlight()
        {
            if (_renderer != null && _highlightMaterial != null)
            {
                _renderer.material = _highlightMaterial;
            }
        }
        
        private void RemoveHighlight()
        {
            if (_renderer != null && _originalMaterial != null)
            {
                _renderer.material = _originalMaterial;
            }
        }
        
        public void SetInteractable(bool canInteract)
        {
            _canInteract = canInteract;
        }
        
        public void SetInteractionPrompt(string prompt)
        {
            _interactionPrompt = prompt;
        }
    }
}
