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
        const int BATCH_NB = 5;
        int batchOffset = 0;

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
            if (dt < 0.050f)
            {
                int count = activeAnts.Count;

                frames.Add((count, dt));
                for (int i = 0; i < count / BATCH_NB; i++)
                {
                    activeAnts[i * BATCH_NB + batchOffset].UpdateSelf();
                }
                //UnityEngine.Debug.Log("Count: " + count + "; Batch size:" + count / BATCH_NB + "; Batch: " + batchOffset + "/" + BATCH_NB);
                batchOffset = Mathf.Min((batchOffset + 1) % BATCH_NB, count / BATCH_NB);


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
    }
}