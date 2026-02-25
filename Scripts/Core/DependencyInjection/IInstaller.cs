using UnityEngine.Scripting;

namespace EnigmaCore.DependencyInjection
{
    /// <summary>
    /// Interface for modules that register dependencies into the static DIContainer.
    /// </summary>
    public interface IInstaller
    {
        /// <summary>
        /// The execution priority. Lower values run first (e.g., Core = -1000, Game = 1000).
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Performs the registration of services using DIContainer.
        /// </summary>
        void Install();
    }
}