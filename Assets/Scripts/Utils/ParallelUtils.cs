using System;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class ParallelUtils
    {
		public delegate void DelegateFor(int i);
		public delegate void DelegateProcess(int from, int to);

		public static void For(int from, int to, DelegateFor delFor)
		{
			DelegateProcess process = delegate (int chunkStart, int chunkEnd) {
				for (int i = chunkStart; i < chunkEnd; ++i)
					delFor(i);
			};

			int cores = Environment.ProcessorCount;
			int chunks = (to - from) / cores;

			IAsyncResult[] asyncResults = new IAsyncResult[cores];

			int end = 0;
			for (int i = 0; i < cores; ++i)
			{
				int start = i * chunks;
				end = Math.Min(start + chunks, to);
				asyncResults[i] = process.BeginInvoke(start, end, null, null);
			}

			for (int i = end; i < to; ++i)
				delFor(i);

			for (int i = 0; i < cores; ++i)
				process.EndInvoke(asyncResults[i]);
		}

		public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask)
        {
			var results = new NativeArray<RaycastHit>(1, Allocator.TempJob);

			var commands = new NativeArray<RaycastCommand>(1, Allocator.TempJob);

			commands[0] = new RaycastCommand(origin, direction, distance:maxDistance, layerMask:layerMask);

			JobHandle handle = RaycastCommand.ScheduleBatch(commands, results, 1, default(JobHandle));

			handle.Complete();

			// Copy the result. If batchedHit.collider is null there was no hit
			hitInfo = results[0];

			// Dispose the buffers
			results.Dispose();
			commands.Dispose();

			return hitInfo.collider != null;
			
		}
		public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo)
		{
			var results = new NativeArray<RaycastHit>(1, Allocator.TempJob);

			var commands = new NativeArray<RaycastCommand>(1, Allocator.TempJob);

			commands[0] = new RaycastCommand(origin, direction);

			JobHandle handle = RaycastCommand.ScheduleBatch(commands, results, 1, default(JobHandle));

			handle.Complete();

			hitInfo = results[0];

			// Dispose the buffers
			results.Dispose();
			commands.Dispose();

			return hitInfo.collider != null;
		}
	}
}