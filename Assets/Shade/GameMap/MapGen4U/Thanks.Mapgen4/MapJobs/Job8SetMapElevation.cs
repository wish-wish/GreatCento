﻿using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

#if Use_Double_Float
using Float = System.Double;
#else
using Float = System.Single;
#endif

namespace Assets.MapJobs
{

    unsafe struct Job8SetMapElevation : IJobParallelFor
    {
        // TODO: V should probably depend on the slope, or elevation, 
        // or maybe it should be 0.95 in mountainous areas and 0.99 elsewhere
        const float V = 0.95f; // reduce elevation in valleys

        public int numRegions;

        [NativeDisableUnsafePtrRestriction] public int* _triangles;
        [NativeDisableUnsafePtrRestriction] public Float* r_elevation;
        [NativeDisableUnsafePtrRestriction] public Float* r_rainfall;
        [NativeDisableUnsafePtrRestriction] public Float* t_elevation;

        [WriteOnly] public NativeArray<Vector2> uv;

        public void Execute(int index)
        {
            uv[index] = getV3(index);
        }

        private Vector2 getV3(int i)
        {
            if (i < numRegions)
            {
                return new Vector2((float)r_elevation[i], (float)r_rainfall[i]);
            }
            else
            {
                var t = i - numRegions;
                var x = V * t_elevation[t];
                var s0 = 3 * t;
                int r1 = s_begin_r(s0),
                    r2 = s_begin_r(s0 + 1),
                    r3 = s_begin_r(s0 + 2);
                var y = 1f / 3 * (r_rainfall[r1] + r_rainfall[r2] + r_rainfall[r3]);
                return new Vector2((float)x, (float)y);
            }
        }
        private int s_begin_r(int s) { return _triangles[s]; }
    }
}
