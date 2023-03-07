﻿using System;
using System.Collections.Generic;
using Unity.Collections;

using Float = System.Single;
using Float2 = UnityEngine.Vector2;

public partial class DualMesh
{
    public class Graph
    {
        public Float2[] _r_vertex { get; set; }
        public int[] _triangles { get; set; }
        public int[] _halfedges { get; set; }
        public int numBoundaryRegions { get; set; }
        public int numSolidSides { get; set; }
    }

    /* Internals */
    public NativeArray<int> _halfedges { get; set; }// s_opposite_s
    public NativeArray<int> _triangles { get; set; }// s_begin_r
    public NativeArray<Float2> _r_vertex { get; set; }

    public NativeArray<int> _r_in_s { get; set; }
    public NativeArray<Float2> _t_vertex { get; set; }
    public NativeArray<Float> s_length { get; set; }

    public int numBoundaryRegions { get; private set; }
    public int numSides { get; private set; }//_triangles.Length;
    public int numRegions { get; private set; }//_r_vertex.Length;
    public int numTriangles { get; private set; }//numSides / 3;
    public int numSolidSides { get; private set; }
    public int numSolidRegions { get; private set; }
    public int numSolidTriangles { get; private set; }


    public DualMesh(Graph graph)
    {
        numBoundaryRegions = graph.numBoundaryRegions;

        _halfedges = new NativeArray<int>(graph._halfedges, Allocator.Persistent);
        _triangles = new NativeArray<int>(graph._triangles, Allocator.Persistent);
        _r_vertex = new NativeArray<Float2>(graph._r_vertex, Allocator.Persistent);

        numSides = _triangles.Length;
        numRegions = _r_vertex.Length;
        numTriangles = numSides / 3;
        numSolidSides = graph.numSolidSides;
        numSolidRegions = numRegions - 1; // TODO: only if there are ghosts
        numSolidTriangles = numSolidSides / 3;

        _t_vertex = new NativeArray<Float2>(numTriangles, Allocator.Persistent);
        _r_in_s = new NativeArray<int>(numRegions, Allocator.Persistent);
        s_length = new NativeArray<Float>(numSides, Allocator.Persistent);

        _update();
    }

    public void Dispose()
    {
        _halfedges.Dispose();
        _triangles.Dispose();
        _r_vertex.Dispose();

        _t_vertex.Dispose();
        _r_in_s.Dispose();
        s_length.Dispose();
    }

    private void _update()
    {
        var _r_in_s = this._r_in_s;
        var _t_vertex = this._t_vertex;
        var s_length = this.s_length;

        // Construct an index for finding sides connected to a region
        for (var s = 0; s < _triangles.Length; s++)
        {
            var endpoint = _triangles[s_next_s(s)];
            if (_r_in_s[endpoint] == 0 || _halfedges[s] == -1)
            {
                _r_in_s[endpoint] = s;
            }
        }

        // Construct triangle coordinates
        for (var s = 0; s < _triangles.Length; s += 3)
        {
            int t = s / 3;
            Float2
                a = _r_vertex[_triangles[s]],
                b = _r_vertex[_triangles[s + 1]],
                c = _r_vertex[_triangles[s + 2]];
            var vt = _t_vertex[t];
            if (s_ghost(s))
            {
                // ghost triangle center is just outside the unpaired side
                Float dx = b[0] - a[0], dy = b[1] - a[1];
                var scale = 10 / Math.Sqrt(dx * dx + dy * dy); // go 10units away from side
                vt[0] = (Float)(0.5 * (a[0] + b[0]) + dy * scale);
                vt[1] = (Float)(0.5 * (a[1] + b[1]) - dx * scale);
            }
            else
            {
                // solid triangle center is at the centroid
                vt[0] = (a[0] + b[0] + c[0]) / 3;
                vt[1] = (a[1] + b[1] + c[1]) / 3;
            }
            _t_vertex[t] = vt;
        }

        for (var s = 0; s < numSides; s++)
        {
            int r1 = s_begin_r(s),
                r2 = s_end_r(s);
            Float
                dx = r_x(r1) - r_x(r2),
                dy = r_y(r1) - r_y(r2);
            s_length[s] = (Float)Math.Sqrt(dx * dx + dy * dy);
        }
    }

    public Float r_x(int r) { return _r_vertex[r][0]; }
    public Float r_y(int r) { return _r_vertex[r][1]; }
    //public Float t_x(int t) { return _t_vertex[t][0]; }
    //public Float t_y(int t) { return _t_vertex[t][1]; }
    //public Float[] r_pos(int r) { var tmp = new Float[2]; tmp[0] = r_x(r); tmp[1] = r_y(r); return tmp; }
    //public Float[] t_pos(int t) { var tmp = new Float[2]; tmp[0] = t_x(t); tmp[1] = t_y(t); return tmp; }

    public int s_begin_r(int s) { return _triangles[s]; }
    public int s_end_r(int s) { return _triangles[s_next_s(s)]; }
    public int s_inner_t(int s) { return s_to_t(s); }
    public int s_outer_t(int s) { return s_to_t(_halfedges[s]); }
    public int s_opposite_s(int s) { return _halfedges[s]; }

    public static int s_to_t(int s) { return s / 3; }
    public static int s_prev_s(int s) { return (s % 3 == 0) ? s + 2 : s - 1; }
    public static int s_next_s(int s) { return (s % 3 == 2) ? s - 2 : s + 1; }

    public int[] t_circulate_s(int t) { var out_s = new int[3]; for (var i = 0; i < 3; i++) { out_s[i] = 3 * t + i; } return out_s; }
    public int[] t_circulate_r(int t) { var out_r = new int[3]; for (var i = 0; i < 3; i++) { out_r[i] = _triangles[3 * t + i]; } return out_r; }
    public int[] t_circulate_t(int t) { var out_t = new int[3]; for (var i = 0; i < 3; i++) { out_t[i] = s_outer_t(3 * t + i); } return out_t; }
    public int[] r_circulate_s(int r)
    {
        int s0 = _r_in_s[r];
        int incoming = s0;
        var out_s = new List<int>();
        do
        {
            out_s.Add(_halfedges[incoming]);
            var outgoing = s_next_s(incoming);
            incoming = _halfedges[outgoing];
        } while (incoming != -1 && incoming != s0);
        return out_s.ToArray();
    }

    public int[] r_circulate_r(int r)
    {
        int s0 = _r_in_s[r];
        int incoming = s0;
        var out_r = new List<int>();
        do
        {
            out_r.Add(s_begin_r(incoming));
            var outgoing = s_next_s(incoming);
            incoming = _halfedges[outgoing];
        } while (incoming != -1 && incoming != s0);
        return out_r.ToArray();
    }

    public int[] r_circulate_t(int r)
    {
        int s0 = _r_in_s[r];
        int incoming = s0;
        var out_t = new List<int>();
        do
        {
            out_t.Add(s_to_t(incoming));
            var outgoing = s_next_s(incoming);
            incoming = _halfedges[outgoing];
        } while (incoming != -1 && incoming != s0);
        return out_t.ToArray();
    }

    public int ghost_r() { return numRegions - 1; }
    public bool s_ghost(int s) { return s >= numSolidSides; }
    public bool r_ghost(int r) { return r == numRegions - 1; }
    public bool t_ghost(int t) { return s_ghost(3 * t); }
    public bool s_boundary(int s) { return s_ghost(s) && (s % 3 == 0); }
    public bool r_boundary(int r) { return r < numBoundaryRegions; }

}
