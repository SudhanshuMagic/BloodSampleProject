using UnityEngine;
using BloodSample.Systems;

namespace BloodSample.Utilities
{
    /// <summary>
    /// Generates instruction boards throughout the laboratory
    /// </summary>
    public class InstructionBoardGenerator : MonoBehaviour
    {
        [Header("Board Generation Settings")]
        [SerializeField] private bool _generateOnStart = true;
        [SerializeField] private Vector3[] _boardPositions;
        [SerializeField] private InstructionBoard.InstructionType[] _boardTypes;
        
        private ModernLabMaterialGenerator _materialGenerator;
        
        private void Start()
        {
            _materialGenerator = ModernLabMaterialGenerator.Instance;
            
            if (_generateOnStart)
            {
                GenerateInstructionBoards();
            }
        }
        
        /// <summary>
        /// Generate instruction boards at strategic locations
        /// </summary>
        public void GenerateInstructionBoards()
        {
            Debug.Log("[InstructionBoardGenerator] 📋 Creating instruction boards...");
            
            // Default positions if none specified
            if (_boardPositions == null || _boardPositions.Length == 0)
            {
                _boardPositions = new Vector3[]
                {
                    new Vector3(-15f, 2f, -10f),  // Main handling instructions
                    new Vector3(15f, 2f, -10f),   // Safety procedures
                    new Vector3(-15f, 2f, 10f),   // Processing workflow
                    new Vector3(15f, 2f, 10f),    // Quality control
                    new Vector3(0f, 2f, -20f)     // Equipment operation
                };
            }
            
            // Default board types if none specified
            if (_boardTypes == null || _boardTypes.Length == 0)
            {
                _boardTypes = new InstructionBoard.InstructionType[]
                {
                    InstructionBoard.InstructionType.BloodHandling,
                    InstructionBoard.InstructionType.SafetyProcedures,
                    InstructionBoard.InstructionType.SampleProcessing,
                    InstructionBoard.InstructionType.QualityControl,
                    InstructionBoard.InstructionType.EquipmentOperation
                };
            }
            
            // Create instruction boards
            int boardsCreated = 0;
            int maxBoards = Mathf.Min(_boardPositions.Length, _boardTypes.Length);
            
            for (int i = 0; i < maxBoards; i++)
            {
                CreateInstructionBoard(_boardPositions[i], _boardTypes[i], i + 1);
                boardsCreated++;
            }
            
            Debug.Log($"[InstructionBoardGenerator] ✅ Created {boardsCreated} instruction boards");
        }
        
        /// <summary>
        /// Create a single instruction board
        /// </summary>
        private void CreateInstructionBoard(Vector3 position, InstructionBoard.InstructionType type, int boardNumber)
        {
            // Create main board structure
            GameObject boardObj = new GameObject($"InstructionBoard_{type}_{boardNumber}");
            boardObj.transform.position = position;
            boardObj.transform.SetParent(transform);
            
            // Create board frame
            GameObject frame = CreateBoardFrame();
            frame.transform.SetParent(boardObj.transform);
            frame.transform.localPosition = Vector3.zero;
            
            // Create screen display
            GameObject screen = CreateScreenDisplay();
            screen.transform.SetParent(boardObj.transform);
            screen.transform.localPosition = new Vector3(0, 0, -0.02f);
            
            // Create mounting bracket
            GameObject bracket = CreateMountingBracket();
            bracket.transform.SetParent(boardObj.transform);
            bracket.transform.localPosition = new Vector3(0, 0, 0.1f);
            
            // Add instruction board component
            InstructionBoard instructionBoard = boardObj.AddComponent<InstructionBoard>();
            instructionBoard.SetInstructionType(type);
            
            // Set up screen materials using reflection
            SetupScreenMaterials(instructionBoard, screen);
            
            // Add Rigidbody for interaction
            var rb = boardObj.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            
            Debug.Log($"[InstructionBoardGenerator] Created {type} instruction board at {position}");
        }
        
        /// <summary>
        /// Create the main board frame
        /// </summary>
        private GameObject CreateBoardFrame()
        {
            GameObject frame = GameObject.CreatePrimitive(PrimitiveType.Cube);
            frame.name = "BoardFrame";
            frame.transform.localScale = new Vector3(2f, 1.5f, 0.1f);
            
            // Apply frame material
            var renderer = frame.GetComponent<Renderer>();
            if (renderer != null && _materialGenerator != null)
            {
                renderer.material = _materialGenerator.CreateStainlessSteelMaterial();
            }
            
            return frame;
        }
        
        /// <summary>
        /// Create the display screen area
        /// </summary>
        private GameObject CreateScreenDisplay()
        {
            GameObject screen = GameObject.CreatePrimitive(PrimitiveType.Cube);
            screen.name = "DisplayScreen";
            screen.transform.localScale = new Vector3(1.8f, 1.3f, 0.05f);
            
            // Apply screen material (default off state)
            var renderer = screen.GetComponent<Renderer>();
            if (renderer != null && _materialGenerator != null)
            {
                renderer.material = _materialGenerator.CreateComputerScreenMaterial(false);
            }
            
            // Remove collider to avoid interference
            var collider = screen.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }
            
            return screen;
        }
        
        /// <summary>
        /// Create mounting bracket for wall mounting
        /// </summary>
        private GameObject CreateMountingBracket()
        {
            GameObject bracket = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bracket.name = "MountingBracket";
            bracket.transform.localScale = new Vector3(0.3f, 0.1f, 0.3f);
            bracket.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            
            // Apply bracket material
            var renderer = bracket.GetComponent<Renderer>();
            if (renderer != null && _materialGenerator != null)
            {
                renderer.material = _materialGenerator.CreateStainlessSteelMaterial();
            }
            
            // Remove collider to avoid interference
            var collider = bracket.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }
            
            return bracket;
        }
        
        /// <summary>
        /// Setup screen materials for the instruction board
        /// </summary>
        private void SetupScreenMaterials(InstructionBoard board, GameObject screen)
        {
            if (_materialGenerator == null) return;
            
            // Create screen materials
            var screenOnMaterial = _materialGenerator.CreateComputerScreenMaterial(true);
            var screenOffMaterial = _materialGenerator.CreateComputerScreenMaterial(false);
            
            // Use reflection to set the materials
            var screenDisplayField = typeof(InstructionBoard).GetField("_screenDisplay", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var screenOnField = typeof(InstructionBoard).GetField("_screenOnMaterial", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var screenOffField = typeof(InstructionBoard).GetField("_screenOffMaterial", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (screenDisplayField != null) screenDisplayField.SetValue(board, screen);
            if (screenOnField != null) screenOnField.SetValue(board, screenOnMaterial);
            if (screenOffField != null) screenOffField.SetValue(board, screenOffMaterial);
        }
        
        /// <summary>
        /// Create a prominent main instruction board
        /// </summary>
        public void CreateMainInstructionBoard()
        {
            Vector3 prominentPosition = new Vector3(0f, 2.5f, -25f); // Front and center
            
            // Create larger main board
            GameObject mainBoardObj = new GameObject("MainInstructionBoard_BloodHandling");
            mainBoardObj.transform.position = prominentPosition;
            mainBoardObj.transform.SetParent(transform);
            
            // Create larger frame for main board
            GameObject frame = GameObject.CreatePrimitive(PrimitiveType.Cube);
            frame.name = "MainBoardFrame";
            frame.transform.SetParent(mainBoardObj.transform);
            frame.transform.localPosition = Vector3.zero;
            frame.transform.localScale = new Vector3(3f, 2f, 0.15f);
            
            // Apply professional frame material
            var frameRenderer = frame.GetComponent<Renderer>();
            if (frameRenderer != null && _materialGenerator != null)
            {
                frameRenderer.material = _materialGenerator.CreateMedicalBlueMaterial();
            }
            
            // Create larger screen
            GameObject screen = GameObject.CreatePrimitive(PrimitiveType.Cube);
            screen.name = "MainDisplayScreen";
            screen.transform.SetParent(mainBoardObj.transform);
            screen.transform.localPosition = new Vector3(0, 0, -0.08f);
            screen.transform.localScale = new Vector3(2.7f, 1.7f, 0.05f);
            
            // Add instruction board component
            InstructionBoard instructionBoard = mainBoardObj.AddComponent<InstructionBoard>();
            instructionBoard.SetInstructionType(InstructionBoard.InstructionType.BloodHandling);
            
            // Setup materials
            SetupScreenMaterials(instructionBoard, screen);
            
            // Add Rigidbody
            var rb = mainBoardObj.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            
            Debug.Log("[InstructionBoardGenerator] Created main instruction board");
        }
        
        /// <summary>
        /// Context menu for manual board generation
        /// </summary>
        [ContextMenu("Generate Instruction Boards")]
        public void GenerateInstructionBoardsMenu()
        {
            GenerateInstructionBoards();
        }
        
        [ContextMenu("Create Main Board")]
        public void CreateMainInstructionBoardMenu()
        {
            CreateMainInstructionBoard();
        }
    }
}
