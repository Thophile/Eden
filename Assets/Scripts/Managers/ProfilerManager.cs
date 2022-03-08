using Assets.Scripts.Model;
using Assets.Scripts.MonoBehaviours;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class ProfilerManager : GameManager
    {
        static readonly string algorythm = nameof(UpdateAntsProxiedQueued);
        static readonly string savePath = "/Profiling_" + algorythm + ".csv";
        public List<(int, float)> frames = new List<(int, float)>();

        void Start()
        {
            GameManager.gameState = new GameState();
            isPaused = false;
            StartCoroutine(algorythm);
        }

        void Update()
        {
            float dt = Time.deltaTime;
            GameManager.gameState.gameTime += dt;
            if (GameManager.gameState.gameTime < 2f) return;

            if (dt < 0.05f)
            {
                frames.Add((activeAnts.Count, dt));

                pheroDecayTimer += dt;
                if (pheroDecayTimer > pheroDecayDelay)
                {
                    pheroDecayTimer -= pheroDecayDelay;
                    gameState.pheromonesMap.DecayMarkers();
                }
            }
            else
            {

                FileInfo fileInfo = new FileInfo(Application.persistentDataPath + savePath);
                using (TextWriter writer = new StreamWriter(fileInfo.Open(FileMode.Create)))
                {
                    foreach (var tuple in frames)
                    {
                        writer.WriteLine(tuple.Item1 + ";" + tuple.Item2);
                    }
                }
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }

        public IEnumerator UpdateAntsProxied()
        {
            while (true)
            {
                foreach (Ant ant in activeAnts)
                {
                    ant.UpdateSelf();
                }
                yield return null;
            }
        }

        public IEnumerator UpdateAntsProxiedQueued()
        {
            while (true)
            {
                foreach (Ant ant in antsToUpdate)
                {
                    ant.UpdateSelf();
                }
                antsToUpdate.Clear();
                yield return null;
            }
        }
    }
}