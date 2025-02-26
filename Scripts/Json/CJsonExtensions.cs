#if NEWTONSOFT_JSON
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
#if JSON_FOR_UNITY_CONVERTERS
using Newtonsoft.Json.UnityConverters;
using Newtonsoft.Json.UnityConverters.Math;
#endif
using UnityEngine;
using UnityEngine.Scripting;

namespace EnigmaCore {
    [Preserve]
	public static class CJsonExtensions {
	
		public static readonly JsonSerializerSettings DefaultSettings = new JsonSerializerSettings {
			NullValueHandling = NullValueHandling.Ignore,
			MissingMemberHandling = MissingMemberHandling.Ignore,
            Error = (sender, e) => {
                Debug.LogError(e.ErrorContext.Error);
                e.ErrorContext.Handled = true;
            },
			Formatting = Formatting.Indented,
			Converters = new JsonConverter[] {
				new StringEnumConverter(),
				new VersionConverter(),
				#if JSON_FOR_UNITY_CONVERTERS
				new Vector2Converter(),
				new Vector3Converter(),
				new Vector2IntConverter(),
				new Vector3IntConverter(),
				#endif
            },
			#if JSON_FOR_UNITY_CONVERTERS
			ContractResolver = new UnityTypeContractResolver(),
			#endif
		};
		
	}
}
#endif