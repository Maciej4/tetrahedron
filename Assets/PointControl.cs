﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointControl : MonoBehaviour {
	public Transform alpha;
	public Transform beta;
	public Transform charlie;
	public Transform delta;

	public float side0 = 0.1f;
	public float side1 = 0.1f;
	public float side2 = 0.1f;
	public float side3 = 0.1f;
	public float side4 = 0.1f;
	public float side5 = 0.1f;

    public class Tetrahedron {
        public Vector3 A { get; set; }
        public Vector3 B { get; set; }
        public Vector3 C { get; set; }
        public Vector3 D { get; set; }
        public Tetrahedron() {
            A = new Vector3(0.0f, 0.0f, 0.0f);
            B = new Vector3(0.0f, 0.0f, 0.0f);
            C = new Vector3(0.0f, 0.0f, 0.0f);
            D = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }

    // Use this for initialization
    void Start () {

	}

    Tetrahedron edge_input(float AsB_m, float AsC_m, float AsD_m, float BsC_m, float BsD_m, float CsD_m)
    {
        float AsB_m2 = AsB_m * AsB_m;
        float AsC_m2 = AsC_m * AsC_m;
        float AsD_m2 = AsD_m * AsD_m;
        float BsC_m2 = BsC_m * BsC_m;
        float BsD_m2 = BsD_m * BsD_m;
        float CsD_m2 = CsD_m * CsD_m;
        float qx = AsB_m;
        float rx = (AsB_m2 + AsC_m2 - BsC_m2) / (2.0f * AsB_m);
        float ry = Mathf.Sqrt(AsC_m2 - rx * rx);
        float sx = (AsB_m2 + AsD_m2 - BsD_m2) / (2.0f * AsB_m);
        float sy = (BsD_m2 - (sx - qx) * (sx - qx) - CsD_m2 + (sx - rx) * (sx - rx) + ry * ry) / (2 * ry);
        float sz = Mathf.Sqrt(AsD_m2 - sx * sx - sy * sy);
        Tetrahedron t = new Tetrahedron();
        t.A = new Vector3(0.0f, 0.0f, 0.0f);
        t.B = new Vector3(qx, 0.0f, 0.0f);
        t.C = new Vector3(rx, ry, 0.0f);
        t.D = new Vector3(sx, sy, sz);
        return t;
    }

    // Update is called once per frame
    void Update () {
        Tetrahedron t = edge_input(side0, side1, side2, side3, side4, side5);
        alpha.transform.localPosition = t.A;
        beta.transform.localPosition = t.B;
        charlie.transform.localPosition = t.C;
        delta.transform.localPosition = t.D;
    }
}
