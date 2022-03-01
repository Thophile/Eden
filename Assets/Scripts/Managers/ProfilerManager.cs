using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class ProfilerManager : GameManager
    {
        static readonly string savePath = "/Profiling.csv";
        public List<(int, float)> frames = new List<(int, float)>();

        void Start()
        {
            GameManager.gameState = new GameState();
            isPaused = false;
            StartCoroutine(nameof(UpdateAnts));
        }

        void Update()
        {
            float dt = Time.deltaTime;
            GameManager.gameState.gameTime += dt;
            if (GameManager.gameState.gameTime < 2f) return;

            if (dt < 0.050f)
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
                using (TextWriter writer = new StreamWriter(fileInfo.Open(FileMode.Truncate)))
                {
                    foreach (var tuple in frames)
                    {
                        writer.WriteLine(tuple.Item1 + ";" + tuple.Item2);
                    }
                }
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }

        public new IEnumerator UpdateAnts()
        {
            while (true)
            {
                for (int i = 0; i < activeAnts.Count; i++)
                {
                    activeAnts[i].UpdateSelf();
                }
                yield return null;
            }
        }

        public IEnumerator UpdateAntsBatched()
        {
            int BATCH_NB = 5;
            int batchOffset = 0;
            while (true)
            {
                for (int i = 0; i < activeAnts.Count / BATCH_NB; i++)
                {
                    activeAnts[i * BATCH_NB + batchOffset].UpdateSelf();
                }
                batchOffset = Mathf.Min((batchOffset + 1) % BATCH_NB, activeAnts.Count / BATCH_NB);
                yield return null;
            }
        }

        public IEnumerator UpdateAntsLimitedResources()
        {
            Stopwatch watch = new Stopwatch();
            int MAX_MILLIS = 3;
            watch.Start();
            for (int i = 0; ; i++)
            {
                if (watch.ElapsedMilliseconds > MAX_MILLIS)
                {
                    watch.Reset();
                    yield return null;
                    watch.Start();
                }
                if (i > activeAnts.Count - 1)
                {
                    i = -1;
                }
                else if (activeAnts[i] != null)
                {
                    activeAnts[i].UpdateSelf();
                }
            }
        }

        public IEnumerator UpdateAntsLimitedResourcesWithMaxCall()
        {
            Stopwatch watch = new Stopwatch();
            int MAX_MILLIS = 3;
            int loopCounter = 0;
            watch.Start();
            for (int i = 0; ; i++)
            {

                if (watch.ElapsedMilliseconds > MAX_MILLIS || loopCounter >= activeAnts.Count)
                {
                    watch.Reset();
                    yield return null;
                    loopCounter = 0;
                    watch.Start();
                }
                if (i > activeAnts.Count - 1)
                {
                    i = -1;
                }
                else if (activeAnts[i] != null)
                {
                    activeAnts[i].UpdateSelf();
                    loopCounter += 1;
                }
            }

        }
    }
}