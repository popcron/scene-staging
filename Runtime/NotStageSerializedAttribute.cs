using System;

namespace Popcron.SceneStaging
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
    public class NotStageSerializedAttribute : Attribute
    {
        
    }
}