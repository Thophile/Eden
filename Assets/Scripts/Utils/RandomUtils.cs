using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public abstract class RandomUtils
    {

        public static T WeightedRandom<T>(List<(T, int)> weigthList, int weigthSum, bool autoScale = true)
        {
            if (autoScale && weigthSum < 100) weigthSum = 100;
            float dice = Random.Range(0, weigthSum);
            foreach (var tuple in weigthList)
            {
                if (tuple.Item2 >= dice)
                {
                    return tuple.Item1;
                }
            }
            return default(T);
        }
    }
}