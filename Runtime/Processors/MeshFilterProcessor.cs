﻿using UnityEngine;

namespace Popcron.SceneStaging
{
    public class MeshFilterProcessor : ComponentProcessor<MeshFilter>
    {
        protected override void Save(Component mapObject, MeshFilter meshFilter)
        {
            mapObject.Set(nameof(meshFilter.sharedMesh), meshFilter.sharedMesh);
        }

        protected override void Load(Component mapObject, MeshFilter meshFilter)
        {
            meshFilter.sharedMesh = mapObject.Get<Mesh>(nameof(meshFilter.sharedMesh));
        }
    }
}