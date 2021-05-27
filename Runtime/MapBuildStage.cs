namespace Popcron.SceneStaging
{
    public enum StageBuildStep
    {
        Initializing,
        CreatingProps,
        ParentObjects,
        CreateComponents,
        LoadComponents,
        FinishedBuilding,
        Inactive
    }
}
