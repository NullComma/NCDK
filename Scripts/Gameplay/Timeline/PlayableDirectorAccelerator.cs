using UnityEngine;
using UnityEngine.Playables;

namespace NullCore.Gameplay.Timeline
{
    /// <summary>
    /// Gamified cutscene accelerator. Accelerates playback rate each time a button is pressed ("mashed") and decays back over time.
    /// Works with Gamepad, Keyboard, and Mouse as per EInput extensions.
    /// </summary>
    [RequireComponent(typeof(PlayableDirector))]
    public class PlayableDirectorAccelerator : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Optional. Se vazio, vai tentar pegar do próprio GameObject.")]
        [SerializeField] private PlayableDirector _director;

        [Header("Settings - Minigame")]
        [Tooltip("Velocidade base padrão (normalmente 1).")]
        [SerializeField] private float _baseSpeed = 1f;
        
        [Tooltip("Velocidade máxima que o PlayableDirector alcança ao apertar os botões repetidamente.")]
        [SerializeField] private float _maxSpeed = 3.5f;
        
        [Tooltip("O quanto de velocidade adiciona a cada vez que o jogador aperta um botão em OnPressed, gamificando o aumento de velocidade.")]
        [SerializeField] private float _speedIncrementPerPress = 0.5f;
        
        [Tooltip("A taxa de velocidade que ele perde por segundo quando não está apertando nenhum botão.")]
        [SerializeField] private float _decayRatePerSecond = 1.5f;

        private float _currentSpeed;

        private void Awake()
        {
            if (!TryGetComponent(out _director))
            {
                Debug.LogError("PlayableDirectorAccelerator: No PlayableDirector found on this GameObject.");
                enabled = false;
                return;
            }
            
            _currentSpeed = _baseSpeed;
        }

        private void OnEnable()
        {
            _currentSpeed = _baseSpeed;
            ApplySpeed(_currentSpeed);
        }

        private void OnDisable()
        {
            ApplySpeed(_baseSpeed);
        }

        private void Update()
        {
            if (_director.state != PlayState.Playing)
            {
                // Reseta a velocidade silenciosamente se a cutscene terminar ou não estiver no state correto
                if (_currentSpeed != _baseSpeed)
                {
                    _currentSpeed = _baseSpeed;
                }
                return;
            }

            // Checa se algum botão (mouse, teclado, gamepad) foi apertado neste frame (minigame)
            if (EInput.IsAnyButtonPressed())
            {
                _currentSpeed += _speedIncrementPerPress;
                if (_currentSpeed > _maxSpeed)
                {
                    _currentSpeed = _maxSpeed;
                }
            }
            else
            {
                // Se currentSpeed é maior que a base, começa a decair de volta pro padrão
                if (_currentSpeed > _baseSpeed)
                {
                    _currentSpeed -= _decayRatePerSecond * Time.deltaTime;
                    if (_currentSpeed < _baseSpeed)
                    {
                        _currentSpeed = _baseSpeed;
                    }
                }
            }

            // Aplica a velocidade atualizada
            ApplySpeed(_currentSpeed);
        }

        private void ApplySpeed(float speed)
        {
            if (_director.playableGraph.IsValid())
            {
                if (_director.playableGraph.GetRootPlayableCount() > 0)
                {
                    _director.playableGraph.GetRootPlayable(0).SetSpeed(speed);
                }
            }
        }
    }
}
