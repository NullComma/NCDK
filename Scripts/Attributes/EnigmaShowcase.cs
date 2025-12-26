using UnityEngine;
using System.Collections.Generic;

namespace EnigmaCore
{
    [AddComponentMenu("Enigma Core/Showcase")]
    public class EnigmaShowcase : MonoBehaviour
    {
        // ========================================================================
        // 1. BUTTONS SYSTEM
        // ========================================================================
        [Header("1. Button System")]
        [SerializeField] private string _consoleMessage = "Hello World";

        [Button("Print Console Message")]
        private void PrintMessage()
        {
            Debug.Log($"[Enigma] {_consoleMessage}");
        }

        [Button("Button With Parameters", 40)]
        private void CreateCube(Vector3 position, string name, Color color)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = position;
            cube.name = string.IsNullOrEmpty(name) ? "Cube" : name;
            cube.GetComponent<Renderer>().material.color = color;
            Debug.Log($"Created {cube.name} at {position}");
        }

        // ========================================================================
        // 2. PROGRESS BARS (Fields & Properties)
        // ========================================================================
        [Space(20)]
        [Header("2. Progress Bars")]
        
        [Tooltip("Barra simples com valores fixos (0 a 100)")]
        [ProgressBar(0, 100)]
        public float SimpleHealth = 75f;

        [Tooltip("Barra usando cor customizada (RGB)")]
        [ProgressBar(0, 100, 1f, 0.3f, 0.3f)] // Vermelho
        public int ManaPoints = 30;

        [SerializeField] private float _maxStamina = 200f;

        [Tooltip("Barra dinâmica: O Max é definido pela variável '_maxStamina'")]
        [ProgressBar(0, nameof(_maxStamina), 0.3f, 0.3f, 1f)] // Azul
        public float Stamina = 150f;

        // Teste de Property + ShowInInspector + ProgressBar + ReadOnly
        [ShowInInspector, ReadOnly, ProgressBar(0, 100)]
        public float ReadOnlyPropertyBar { get; set; } = 88.5f;

        // ========================================================================
        // 3. MIN MAX SLIDERS
        // ========================================================================
        [Space(20)]
        [Header("3. MinMax Sliders")]

        [Tooltip("Slider simples para float (Range)")]
        [MinMaxSlider(0, 10)]
        public float FloatSlider = 5.5f;

        [Tooltip("Slider de intervalo para Vector2 (X=Min, Y=Max)")]
        [MinMaxSlider(0, 24)]
        public Vector2 DayNightCycle = new Vector2(8, 18);

        [Tooltip("Slider de intervalo Inteiro")]
        [MinMaxSlider(0, 100)]
        public Vector2Int LevelRange = new Vector2Int(10, 50);

        [SerializeField] private float _dynamicMin = 10;
        [SerializeField] private float _dynamicMax = 50;

        [Tooltip("Slider cujos limites dependem de outras variáveis")]
        [MinMaxSlider(nameof(_dynamicMin), nameof(_dynamicMax))]
        public float DynamicSlider = 25f;

        // ========================================================================
        // 4. INFO BOXES (Conditionals)
        // ========================================================================
        [Space(20)]
        [Header("4. Info Boxes & Logic")]

        [InfoBox("Esta mensagem sempre aparece aqui.", InfoMessageType.Info)]
        public string AlwaysVisible = "Text";

        [Space(10)]
        public bool showWarning = false;

        [InfoBox("Cuidado! Esta opção é perigosa.", InfoMessageType.Warning, nameof(showWarning))]
        public GameObject dangerousObject;

        [Space(10)]
        [MinMaxSlider(0, 10)]
        public float criticalValue = 5f;

        // Condicional baseada em Método
        private bool IsCritical() => criticalValue > 8f;

        [InfoBox("Value is too high! System might crash.", InfoMessageType.Error, nameof(IsCritical))]
        public string status = "Safe";


        // ========================================================================
        // 5. PROPERTIES & SHOW IN INSPECTOR
        // ========================================================================
        [Header("5. C# Properties")]

        [ShowInInspector]
        public string MyAutoProperty { get; set; } = "Edit Me!";

        [ShowInInspector, ReadOnly]
        public Vector3 ReadOnlyVector { get; private set; } = Vector3.up;

        // ========================================================================
        // 6. THE MEGA COMBO
        // ========================================================================
        [Space(20)]
        [Header("6. Stress Test (Combo)")]

        [SerializeField] 
        private bool _simulateError = true;

        // Combo: Property + ShowInInspector + MinMaxSlider + InfoBox Condicional
        [ShowInInspector]
        [MinMaxSlider(0, 100)]
        [InfoBox("Error Simulation is Active!", InfoMessageType.Error, nameof(_simulateError))]
        public Vector2 ComplexProperty { get; set; } = new Vector2(20, 80);

        
        // Métodos auxiliares para interagir com o inspector
        [Button("Toggle Error")]
        private void ToggleError()
        {
            _simulateError = !_simulateError;
        }

        [Button("Randomize Bars")]
        private void RandomizeValues()
        {
            SimpleHealth = Random.Range(0, 100);
            ManaPoints = Random.Range(0, 100);
            Stamina = Random.Range(0, _maxStamina);
            ReadOnlyPropertyBar = Random.Range(0, 100);
        }
    }
}