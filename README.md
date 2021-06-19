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
```cs
//convert the current scene to a stage and a json object
Stage exportedStage = StageExporter.Export();
string json = exportedStage.ToJson();

//create scene from json and load the stage additively
Stage loadedStage = Stage.FromJson(json);
StageBuilder.BuildAsync(loadedStage);
```

Can also do these operations through the right click menu in the editor:
![example](https://media.discordapp.net/attachments/784916261871550494/847988115980681256/unknown.png)

## Limitations
- Lightmaps aren't supported
- NavMeshes aren't supported
- A lot of built-in Unity components aren't supported
  - The common ones are, this limitation is due to how the components are transfered in and out of json data.
  - This can be improved by creating new `ComponentProcessor` types that handle a built-in component (user made ones are automatically covered)
