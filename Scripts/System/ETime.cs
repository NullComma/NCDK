using System;
using UnityEngine;

namespace EnigmaCore {
	public static class ETime {
		
		[Obsolete("This was implemented as a workaround to some Unity bug in some old version. Use Time.deltaTime directly.", false)]
		/// <summary>
		/// Delta time scaled by time scale.
		/// </summary>
		/// <returns>Delta time multiplied by time scale.</returns>
		public static float DeltaTimeScaled {
			get {
				return Time.deltaTime * Time.timeScale;
			}
		}

		/// <summary>
		/// Set timescale and invoke an event if changed.
		/// </summary>
		public static float TimeScale {
			get { return Time.timeScale; }
			set {
				var oldTimeScale = Time.timeScale;
				if (value == oldTimeScale) return;
                _onTimePaused.Invoke(value == 0f);
                Time.timeScale = value;
				_onTimeScaleChanged.Invoke(oldTimeScale, value);
			}
		}

        public static bool IsPaused
		{
			get => Mathf.Approximately(Time.timeScale, 0f);
			set => TimeScale = value ? 0f : 1f;
		}

		/// <summary>
		/// Notify time scaled changed (oldTimeScale, newTimeScale)
		/// </summary>
		public static event Action<float, float> OnTimeScaleChanged {
			add {
				_onTimeScaleChanged -= value;
				_onTimeScaleChanged += value;
			}
			remove {
				_onTimeScaleChanged -= value;
			}
		}
		private static Action<float, float> _onTimeScaleChanged = delegate { };
        
        public static event Action<bool> OnTimePaused {
            add {
                _onTimePaused -= value;
                _onTimePaused += value;
            }
            remove {
                _onTimePaused -= value;
            }
        }
        private static Action<bool> _onTimePaused = delegate { };
	}
}
