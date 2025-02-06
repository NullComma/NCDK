using System;
using EnigmaCore.UI;
using UnityEngine;

#if FMOD
using FMODUnity;
#endif


namespace EnigmaCore {
	public class UISoundsBankSO : ScriptableObject
	{
		#if FMOD
		[Header("Default Sounds")]
		[SerializeField] protected EventReference _soundSelect;
		public EventReference SoundSelect => this._soundSelect;
		
		[SerializeField] protected EventReference _soundSubmit;
		public EventReference SoundSubmit => this._soundSubmit;
		
		[SerializeField] protected EventReference _soundCancel;
		public EventReference SoundCancel => this._soundCancel;

		[SerializeField] protected EventReference _soundOpenMenu;
		public EventReference SoundOpenMenu => this._soundOpenMenu;

		[SerializeField] protected EventReference _soundCloseMenu;
		public EventReference SoundCloseMenu => this._soundCloseMenu;
		#endif

	}
	
}