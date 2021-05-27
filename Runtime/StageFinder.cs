using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Popcron.SceneStaging
{
    public class StageFinder
    {
        /// <summary>
        /// Loads all stages from the directory of the game and its sub directories recursively.
        /// </summary>
        public static async Task<List<Stage>> LoadFromApplication()
        {
#if UNITY_EDITOR
            string directory = Application.dataPath;
#else
            string directory = Directory.GetParent(Application.dataPath).FullName;
#endif
            return await LoadFromDirectory(directory);
        }

        /// <summary>
        /// Loads all stages from this specific directory and its sub directories recursively.
        /// </summary>
        public static async Task<List<Stage>> LoadFromDirectory(string directory)
        {
            List<Stage> stages = new List<Stage>();
            string[] files = Directory.GetFiles(directory, $"*{Stage.Extension}", SearchOption.AllDirectories);
            int filesCount = files.Length;
            for (int i = filesCount - 1; i >= 0; i--)
            {
                try
                {
                    string pathToMap = files[i];
                    using (StreamReader reader = File.OpenText(pathToMap))
                    {
                        string fileText = await reader.ReadToEndAsync();
                        Stage stage = await Conversion.FromJson(fileText);
                        if (stage is not null)
                        {
                            stages.Add(stage);
                        }
                    }
                }
                catch { }
            }

            return stages;
        }
    }
}
