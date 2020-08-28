using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BezierSpline))]
public class MeshSplineDeformer : MonoBehaviour
{
    public bool forceDeform;
    public bool flip;
    [HideInInspector] public bool editPlane; // custom planes in development
    public Mesh baseMesh;
    public Axis axis;
    public float heightScale = 1;
    public float bumb = 1;
    [HideInInspector] public MeshFilter meshFilter;
    [HideInInspector] public BezierSpline spline;

    public enum Axis { X, Z };
    Mesh __internalBaseMesh;
    Axis __internalAxis;

    public Quaternion customPlaneQuaterinion;
    public Vector3 customPlanePosition;
    // Start is called before the first frame update

    public ComputeShader compute;

    void Start()
    {


    }
    void CloneMesh()
    {
        var meshClone = new Mesh();
        meshClone.vertices = baseMesh.vertices;
        meshClone.uv = baseMesh.uv;
        meshClone.triangles = baseMesh.triangles;
        meshClone.name = baseMesh.name + "(Procedural)";
        meshClone.RecalculateNormals();
        meshFilter.sharedMesh = meshClone;
    }


    public void Deform()
    {
     
        if (meshFilter.sharedMesh == null) return;
        Vector3[] verts = baseMesh.vertices;
        for (int i = 0; i < verts.Length; i++)
        {
            float sVal = GetSplineValue(verts[i]);
            Vector3 splinePoint;
            var splineNormal = SplineNormal(sVal, out splinePoint);
            var rotation = spline.GetRotation(sVal);
            var up = rotation * Vector3.up;
            float offset = axis == Axis.X ? verts[i].z : verts[i].x;
            verts[i] = splineNormal * offset * bumb + splinePoint + verts[i].y * heightScale * up;
        }
        meshFilter.sharedMesh.vertices = verts;
        meshFilter.sharedMesh.RecalculateNormals();
        meshFilter.sharedMesh.RecalculateBounds();
    }
    Vector3 SplineNormal(float val)
    {
        var point = spline.GetPoint(val);
        var dir = spline.GetVelocity(val);
        var projection = Vector3.ProjectOnPlane(dir, Vector3.up);
        if (flip) return new Vector3(projection.z, 0, -projection.x).normalized;
        return new Vector3(-projection.z, 0, projection.x).normalized;
    }
    Vector3 SplineNormal(float val, out Vector3 point)
    {
        point = transform.InverseTransformPoint(spline.GetPoint(val));
        var dir = spline.GetVelocity(val);
        var projection = Vector3.ProjectOnPlane(dir, Vector3.up);
        if (flip) return new Vector3(projection.z, 0, -projection.x).normalized;
        return new Vector3(-projection.z, 0, projection.x).normalized;
    }

    void OnDrawGizmos()
    {
        if (Selection.activeGameObject != this.gameObject) return;
        Gizmos.color = Color.red;
        var point = spline.GetPoint(1);
        Gizmos.DrawLine(point, point + SplineNormal(1));
        for (float f = 0; f <= 1f; f += 0.05f)
        {
            point = spline.GetPoint(f);
            var rotation = spline.GetRotation(f);
            Gizmos.DrawLine(point, point + rotation * SplineNormal(f));
        }


    }
    float GetSplineValue(Vector3 position)
    {
        if (axis == Axis.X)
        {
            float min = baseMesh.bounds.min.x;
            float length = baseMesh.bounds.size.x;
            return (position.x - min) / length;
        }
        else
        {
            float min = baseMesh.bounds.min.z;
            float length = baseMesh.bounds.size.z;
            return (position.z - min) / length;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
    void OnValidate()
    {
        if (forceDeform)
        {
            forceDeform = false;
            Deform();
        }
        if (baseMesh != __internalBaseMesh)
        {
            __internalBaseMesh = baseMesh;
            CloneMesh();
            Deform();
        }
        if (axis != __internalAxis)
        {
            __internalAxis = axis;
            Deform();
        }
    }



}
