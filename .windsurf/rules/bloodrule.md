---
trigger: manual
---

Follow these Unity C# coding standards:
- Use PascalCase for classes/public methods, camelCase for private variables
- Prefix private fields with underscore (_fieldName)
- Use [SerializeField] for Inspector-exposed private fields
- Cache GetComponent<>() calls in Awake/Start, never in Update/LateUpdate
- Put physics code in FixedUpdate, rendering in Update
- Keep classes focused on single responsibility
- Use meaningful, descriptive names
- Add XML documentation comments (///) for public APIs
- Implement proper null checking before accessing components
- Use object pooling for frequently instantiated objects