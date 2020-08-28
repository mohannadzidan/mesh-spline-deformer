using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshSplineDeformer))]
public class MeshSplineDeformerEditor : Editor
{
    Tool previousTool = Tool.None;
   
    void OnEnable()
    {
        var deformer = target as MeshSplineDeformer;
        deformer.meshFilter = deformer.GetComponent<MeshFilter>();
        deformer.spline = deformer.GetComponent<BezierSpline>();
        deformer.spline.onBezierChange -= deformer.Deform;
        deformer.spline.onBezierChange += deformer.Deform;
        Debug.Log("Start");
    }
    void OnDisable()
    {
        var deformer = target as MeshSplineDeformer;
        Tools.current = previousTool;
        deformer.spline.onBezierChange -= deformer.Deform;

    }
    void OnSceneGUI()
    {
        var deformer = target as MeshSplineDeformer;
        int controlId = GUIUtility.GetControlID(FocusType.Passive);
        // Start treating your events

        if (deformer.editPlane)
        {
            Tools.current = Tool.None;
            // draw plane
            var bounds = deformer.baseMesh.bounds;
            var min = bounds.min;
            var max = bounds.max;
            var planeVerts = new Vector3[]{
                min,
                min + new Vector3(0, bounds.size.y, 0),
                min+new Vector3(bounds.size.x, bounds.size.y, 0),
                min+new Vector3(bounds.size.x, 0, 0)
            };
            for (int i = 0; i < planeVerts.Length; i++)
                planeVerts[i] = deformer.customPlaneQuaterinion * planeVerts[i] + deformer.customPlanePosition;

            Handles.DrawSolidRectangleWithOutline(planeVerts, new Color(1, 1, 0, 0.1f), Color.black);
            Handles.color = Color.red;
            deformer.customPlaneQuaterinion = Handles.Disc(deformer.customPlaneQuaterinion, deformer.customPlanePosition, Vector3.up, 1, true, 0.01f);
            deformer.customPlanePosition = Handles.PositionHandle(deformer.customPlanePosition, deformer.customPlaneQuaterinion);
        }
        else
        {

        }

    }

    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    static void DrawGizmoForMyScript(MeshSplineDeformer deformer, GizmoType gizmoType)
    {
        if (deformer.editPlane)
        {

            Gizmos.color = new Color(1, 1, 1, 0.5f);
            Gizmos.DrawMesh(deformer.baseMesh, deformer.transform.position, deformer.transform.rotation);

        }
    }
    //    public override void OnInspectorGUI()
    //     {
    //         serializedObject.Update();
    //         //EditorGUILayout.PropertyField(lookAtPoint);
    //         serializedObject.ApplyModifiedProperties();
    //     }
}
