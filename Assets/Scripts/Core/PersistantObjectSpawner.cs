using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class PersistantObjectSpawner : MonoBehaviour
    {
        static bool hasSpawned = false;

        [SerializeField] GameObject persistantObjectPrefab = null;

        void Awake ()
        {
            if (hasSpawned) { return; }
            SpawnPersistantObjects ();
            hasSpawned = true;
        }

        void SpawnPersistantObjects ()
        {
            GameObject persistantObject = Instantiate (persistantObjectPrefab);
            DontDestroyOnLoad (persistantObject);
        }
    } 
}
