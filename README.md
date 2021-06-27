![Usage](https://cdn.discordapp.com/attachments/377316629220032523/843959972492476466/unknown.png)

# Scene Staging
Utility for working with Unity Scenes as JSON objects, perfect for loading Scenes at runtime and allowing for modability.

*Note: This is a WIP package, so there might be some issues and inconsistencies but the project is actively being used by me in some of my projects so rest assured that it has a baseline :p*


## Installation
Use the + inside the Package Manager window and add this URL:
```json
"com.popcron.scene-staging": "https://github.com/popcron/scene-staging.git"
```
Or add it to your `packages.json` file manually (located inside the project's Packages folder).

## Example
Below is an example that converts the current active scene to JSON, and then loads the scene from the JSON string.
```cs
//convert the current scene to a stage and a json object
Stage exportedStage = StageExporter.Export();
string json = exportedStage.ToJson();

//create scene from json and load the stage additively
Stage loadedStage = Stage.FromJson(json);
StageBuilder.BuildAsync(loadedStage);
```

The resulting JSON string looks like [this](https://gdl.space/ikeqaduheq.json).

## Processors
Processors in this package describe a class that handles the conversion of a unity component from and to a stage component type. Included are the required Transform and GameObject processors, as well as a generic processor that handles anything else that is a component (including user made MonoBehaviour types). They must also be registered via the `ComponentProcessor.RegistereProcessor<T>()` method, a handy trick that can help expedite this is by registering inside a method with the `RuntimeInitializeOnLoadMethod` attribute (as seen in the example below).

Extending the functionality can be done by creating a new class that inherits from a ComponentProcessor, letting you decide what gets saved and loaded.

<details>
  <summary>Example for a SpriteRenderer</summary>
  
```cs
using Popcron.SceneStaging;
using UnityEngine;
using Component = Popcron.SceneStaging.Component;

public class SpriteRendererProcessor : ComponentProcessor<SpriteRenderer>
{
    [RuntimeInitializeOnLoadMethod]
    private static void AutoRegister()
    {
        RegisterProcessor<SpriteRendererProcessor>();
    }

	  protected override void Load(Component component, SpriteRenderer spriteRenderer)
    {
        component.Set("sprite", spriteRenderer.sprite);
        component.Set("color", spriteRenderer.color);
    }

    protected override void Save(Component component, SpriteRenderer spriteRenderer)
    {
        spriteRenderer.sprite = component.Get<Sprite>("sprite");
        spriteRenderer.color = component.Get<Color>("color");
    }
}
```
</details>

## Prefabs
When prefabs are found in the scene, they are saved as is (as if it isn't a prefab at all). This behaviour mimics what Unity actually does when a scene is opened in the game. Applying a `Prefab` component onto prefabs will change this behaviour, by saving the information about the prefab so that it can be instantiated later on.

## Samples
Included in the package are 2 samples:
- Converting all on Play
  - This sample will convert all scenes assets into a StageAsset asset into the project, this asset will contain an instance of a Stage which can then be converted to a JSON string using `stageAsset.ToJson()`
- Converter examples
  - This sample contains a list of various custom processors for some of the built-in Unity components.

## Limitations
- Lightmaps aren't supported
- Prefab overrides aren't supported
- References to objects inside the hierarchy of the prefab won't be saved
  - Only if the prefab has a `Prefab` component as described [here](#Prefabs)
- IL2CPP stripping unusued Unity properties
  - This can be mitigated by implementing custom processors or with the preservation approaches [here](https://docs.unity3d.com/Manual/ManagedCodeStripping.html)