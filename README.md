# Scene Staging
Utility for working with Unity Scenes as JSON objects, perfect for loading Scenes at runtime and allowing for modability.

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
