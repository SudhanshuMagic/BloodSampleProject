namespace BloodSample.Core
{
    public interface IInteractable
    {
        bool CanInteract { get; }
        string InteractionPrompt { get; }
        
        void OnSelect();
        void OnDeselect();
        void OnInteract();
        void OnInteractionEnd();
    }
}
