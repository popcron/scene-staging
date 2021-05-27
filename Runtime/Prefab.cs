using System;
using UnityEngine;

namespace Popcron.SceneStaging
{
    public class Prefab : MonoBehaviour
    {
        [SerializeField]
        private GameObject original;

        [SerializeField]
        private string path;

        public GameObject Original
        {
            get => original;
            set => original = value;
        }

        public string Path
        {
            get => path;
            set => path = value;
        }
    }
}