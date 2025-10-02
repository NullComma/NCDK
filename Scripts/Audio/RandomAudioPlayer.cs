using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace EnigmaCore.Audio {
	/// <summary>
	/// Random audio player that tries to not repeat the played audio as much as possible.
	/// </summary>
	public class RandomAudioPlayer : MonoBehaviour {

		[SerializeField] AudioSource _audioSource;
		[NonSerialized] AudioClip[] _audios;
		[NonSerialized] readonly Queue<AudioClip> _lastPlayedAudios = new ();
		
		public void SetAudioEvents(AudioClip[] clips) {
			if (Equals(clips, _audios)) return;
			_lastPlayedAudios.Clear();
			_audios = clips;
		}

		public void PlayAudio() {
			var audioClip = GetRandomFromListNotRepeating();
			if (audioClip == null) return;
			_lastPlayedAudios.Enqueue(audioClip);
			_audioSource.clip = audioClip;
			_audioSource.Play();
		}

		AudioClip GetRandomFromListNotRepeating() {
			if (_audios == null) return null;
			int count = _audios.Length;
			if (count == 1) return _audios[0];
			if (_lastPlayedAudios.Count >= count * 0.5f) {
				_lastPlayedAudios.Dequeue();
			}
			return _audios.Except(_lastPlayedAudios).CRandomElement();
		}
	}
}
