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
        }

        void Update()
        {
            float dt = Time.deltaTime;
            GameManager.gameState.gameTime += dt;
            if (GameManager.gameState.gameTime < 2f) return;
            if(dt < 0.050f){
                int count = activeAnts.Count;

                frames.Add((count, dt));
                for (int i = 0; i < count; i++)
                {
                    activeAnts[i].UpdateSelf();
                }

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

        /**
         * Update used for coroutine algorythms
         *
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
        */

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