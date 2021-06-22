using System;

namespace Popcron.SceneStaging
{
    public class NoProcessorFound : Exception
    {
        public NoProcessorFound(string message) : base(message)
        {

        }
    }
}