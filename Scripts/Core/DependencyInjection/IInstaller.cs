using UnityEngine.Scripting;

namespace NullCore
{
    /// <summary>
    /// Interface for modules that register dependencies into the static NullCore.ServiceLocator.
    /// </summary>
    public interface IInstaller
    {
        /// <summary>
        /// The execution priority. Lower values run first (e.g., Core = -1000, Game = 1000).
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Performs the registration of services using NullCore.ServiceLocator.
        /// </summary>
        void Install();
    }
}