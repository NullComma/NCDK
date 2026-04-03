using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace NullCore.UI
{
    public class InputPromptUI : MonoBehaviour, IPointerClickHandler
    {
        [Header("References")]
        [SerializeField] private InputActionReference _actionReference;
        [SerializeField] private NullCore.Input.InputIconMap _iconMap;
        [SerializeField] private Image _promptImage;

        [Header("Events")]
        [SerializeField] private CUnityEventString _onUpdateLabel;
        [SerializeField] private bool _clickToTrigger = false;

        private NullCore.Input.InputIconBinding _currentBinding;
        private Sprite[] _currentIcons;
        private bool _hasSetup;
        private float _animationTimer;
        private int _currentFrame;

        private void OnEnable()
        {
            if (_actionReference != null && _actionReference.action != null)
            {
                _actionReference.action.Enable();
            }

            EInput.OnDeviceChanged += OnDeviceChanged;
            EInput.OnGamepadLayoutChanged += OnGamepadLayoutChanged;
            
            // Initial update
            UpdatePrompt(EInput.CurrentDevice);
        }

        private void OnDisable()
        {
            EInput.OnDeviceChanged -= OnDeviceChanged;
            EInput.OnGamepadLayoutChanged -= OnGamepadLayoutChanged;
        }
        
        private void OnDeviceChanged(InputDeviceType newDevice)
        {
            UpdatePrompt(newDevice);
        }

        private void OnGamepadLayoutChanged()
        {
            // Just refresh whatever device we are currently on
            UpdatePrompt(EInput.CurrentDevice);
        }

        private void Update()
        {
            if (!_hasSetup || _currentIcons == null || _currentIcons.Length <= 1) return;

            _animationTimer += Time.unscaledDeltaTime;
            float timePerFrame = 1f / _currentBinding.DefaultFPS;
            
            if (_animationTimer >= timePerFrame)
            {
                _animationTimer -= timePerFrame;
                _currentFrame = (_currentFrame + 1) % _currentIcons.Length;
                _promptImage.sprite = _currentIcons[_currentFrame];
            }
        }

        public void UpdatePrompt(InputDeviceType activeDeviceType)
        {
            if (_actionReference == null || _actionReference.action == null || _iconMap == null || _promptImage == null)
            {
                return;
            }

            bool foundMap = false;

            // Iterate over all bindings for the assigned action
            foreach (var binding in _actionReference.action.bindings)
            {
                bool matchesDevice = false;
                
                if (activeDeviceType == InputDeviceType.Gamepad && binding.effectivePath.StartsWith("<Gamepad>"))
                {
                    matchesDevice = true;
                }
                else if (activeDeviceType == InputDeviceType.MouseAndKeyboard && 
                        (binding.effectivePath.StartsWith("<Keyboard>") || binding.effectivePath.StartsWith("<Mouse>")))
                {
                    matchesDevice = true;
                }

                if (matchesDevice)
                {
                    if (_iconMap.TryGetBinding(binding.effectivePath, out _currentBinding))
                    {
                        foundMap = true;
                        break;
                    }
                }
            }
            
            // Fallback: If no generic wildcard match, just pick any mapped that exists
            if (!foundMap)
            {
                Debug.Log($"[InputPromptUI] No device-specific match found for '{_actionReference.action.name}', trying fallback...");
                foreach (var binding in _actionReference.action.bindings)
                {
                    if (_iconMap.TryGetBinding(binding.effectivePath, out _currentBinding))
                    {
                        foundMap = true;
                        break;
                    }
                }
            }

            if (foundMap)
            {
                // Resolve correct array based on current device and preferences
                var resolvedIcons = ResolveIconsArray(activeDeviceType, _currentBinding);

                if (resolvedIcons != null && resolvedIcons.Length > 0)
                {
                    _hasSetup = true;
                    
                    // Only reset animation if the icons actually changed
                    if (_currentIcons != resolvedIcons)
                    {
                        _currentIcons = resolvedIcons;
                        _currentFrame = 0;
                        _animationTimer = 0f;
                    }
                    
                    _promptImage.sprite = _currentIcons[_currentFrame];
                    _promptImage.enabled = true;
                }
                else
                {
                    _hasSetup = false;
                    _currentIcons = null;
                    _promptImage.enabled = false;
                }
            }
            else
            {
                _hasSetup = false;
                _currentIcons = null;
                _promptImage.enabled = false;
            }
        }

        private Sprite[] ResolveIconsArray(InputDeviceType deviceType, NullCore.Input.InputIconBinding binding)
        {
            if (deviceType == InputDeviceType.MouseAndKeyboard)
                return binding.keyboardMouseIcons;
                
            var padLayout = EInput.GetResolvedGamepadLayout();
            Sprite[] result = null;

            switch (padLayout)
            {
                case GamepadLayoutType.Nintendo:
                    result = binding.nintendoIcons;
                    break;
                case GamepadLayoutType.PlayStation:
                    result = binding.playStationIcons;
                    break;
                case GamepadLayoutType.Xbox:
                    result = binding.xboxIcons;
                    break;
            }

            // Fallback to Xbox if Nintendo/PlayStation is selected but array is empty
            if (result == null || result.Length == 0)
            {
                result = binding.xboxIcons;
            }

            return result;
        }

        public void SetLabelText(string newText)
        {
            _onUpdateLabel?.Invoke(newText);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_clickToTrigger || eventData.button != PointerEventData.InputButton.Left) return;
            if (EInput.CurrentDevice != InputDeviceType.MouseAndKeyboard) return;
            if (_actionReference == null || _actionReference.action == null) return;

            var action = _actionReference.action;
            foreach (var control in action.controls)
            {
                if (control.device is Keyboard || control.device is Mouse)
                {
                    // Simulate a press and release
                    InputSystem.QueueDeltaStateEvent(control, 1f);
                    InputSystem.QueueDeltaStateEvent(control, 0f);
                    break;
                }
            }
        }
    }
}
