# Project Structure Documentation

## Folder Organization

### Assets/Scripts/
Organized by system functionality following single responsibility principle:

#### Core/
- **GameManager.cs**: Main game state and flow control
- **SceneManager.cs**: Scene loading and management
- **InputManager.cs**: Input handling and controls
- **CameraController.cs**: Camera movement and controls

#### Systems/
- **SampleManager.cs**: Blood sample tracking and lifecycle
- **InventorySystem.cs**: Equipment and supply management
- **WorkflowEngine.cs**: Procedure guidance and validation
- **DataManager.cs**: Save/load and data persistence

#### Equipment/
- **BaseEquipment.cs**: Abstract base class for all equipment
- **Centrifuge.cs**: Centrifuge operation and control
- **Microscope.cs**: Microscope interaction system
- **StorageUnit.cs**: Sample storage and temperature control
- **TestingEquipment.cs**: Various testing device controllers

#### UI/
- **UIManager.cs**: Main UI coordinator
- **InventoryUI.cs**: Inventory display and management
- **WorkflowUI.cs**: Step-by-step procedure display
- **ResultsUI.cs**: Test results and reporting

#### Data/
- **SampleData.cs**: Blood sample data structures
- **TestResult.cs**: Test result data models
- **EquipmentState.cs**: Equipment state tracking
- **UserProgress.cs**: User progress and achievements

#### Utilities/
- **ObjectPooler.cs**: Object pooling for performance
- **AudioManager.cs**: Sound effect management
- **Extensions.cs**: Utility extension methods
- **Constants.cs**: Game constants and configuration

## Asset Organization

### Prefabs/
Reusable game objects organized by category:
- **Equipment/**: All lab equipment prefabs
- **Samples/**: Blood sample containers and types
- **UI/**: UI panels and components
- **Environment/**: Lab furniture and environment objects

### Materials/
PBR materials organized by usage:
- **Equipment/**: Metallic, glass, plastic materials for equipment
- **Environment/**: Lab surfaces, walls, lighting materials
- **UI/**: UI element materials and effects

### Models/
3D models organized by category:
- **Equipment/**: Lab equipment 3D models
- **Environment/**: Lab architecture and furniture
- **Samples/**: Sample containers and related objects

### Data/ScriptableObjects/
Configuration files for:
- Equipment specifications
- Test procedures
- Sample types
- Workflow definitions

## Naming Conventions
- **Classes**: PascalCase (e.g., SampleManager)
- **Methods**: PascalCase (e.g., ProcessSample)
- **Variables**: camelCase (e.g., sampleCount)
- **Private Fields**: _camelCase (e.g., _isProcessing)
- **Constants**: UPPER_CASE (e.g., MAX_SAMPLES)
- **Prefabs**: Descriptive names (e.g., Centrifuge_Model_01)
- **Scenes**: Descriptive names (e.g., MainLaboratory)
