using System.Collections.Generic;

namespace Popcron.SceneStaging
{
    /// <summary>
    /// Used for parsing variables of this component after initial load.
    /// </summary>
    public interface IOnLoaded
    {
        void Loaded(List<Variable> variables);
    }
}