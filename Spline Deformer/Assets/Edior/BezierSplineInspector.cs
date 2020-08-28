using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierSpline))]
public class BezierSplineInspector : Editor
{

    private const int stepsPerCurve = 10;
    private const float directionScale = 0.5f;
    private const float mainBezierHandleSize = 0.08f;
    private const float pickSize = 0.06f;

    private static Color[] modeColors = {
        Color.white,
        Color.yellow,
        Color.cyan
    };

    private BezierSpline spline;
    private Transform handleTransform;
    private Quaternion handleRotation;
    private int selectedIndex = -1;
    private static GUIStyle ToggleButtonStyleNormal = null;
    private static GUIStyle ToggleButtonStyleToggled = null;
    public override void OnInspectorGUI()
    {
        if (ToggleButtonStyleNormal == null)
        {
            ToggleButtonStyleNormal = "Button";
            ToggleButtonStyleToggled = "Button";
            ToggleButtonStyleToggled.normal.background = ToggleButtonStyleNormal.focused.background;
        }

        spline = target as BezierSpline;
        if (GUILayout.Button(EditorGUIUtility.IconContent("MoveTool"), spline.editMode == BezierSpline.SplineEditMode.Position ? ToggleButtonStyleToggled : ToggleButtonStyleNormal))
        {
            spline.editMode = BezierSpline.SplineEditMode.Position;
        }
        if (GUILayout.Button(EditorGUIUtility.IconContent("RotateTool"), spline.editMode == BezierSpline.SplineEditMode.Rotation ? ToggleButtonStyleToggled : ToggleButtonStyleNormal))
        {
            spline.editMode = BezierSpline.SplineEditMode.Rotation;

        }
        EditorGUI.BeginChangeCheck();
        bool loop = EditorGUILayout.Toggle("Loop", spline.Loop);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Toggle Loop");
            EditorUtility.SetDirty(spline);
            spline.Loop = loop;
        }
        if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount)
        {
            DrawSelectedPointInspector();
        }
        if (GUILayout.Button("Add Curve"))
        {
            Undo.RecordObject(spline, "Add Curve");
            spline.AddCurve();
            EditorUtility.SetDirty(spline);
        }
    }

    private void DrawSelectedPointInspector()
    {
        GUILayout.Label("Selected Point");

        EditorGUI.BeginChangeCheck();
        var point = EditorGUILayout.Vector3Field("Position", spline.GetControlPoint(selectedIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Move Point");
            EditorUtility.SetDirty(spline);
            spline.SetControlPoint(selectedIndex, point);
        }


        EditorGUI.BeginChangeCheck();
        BezierSpline.BezierControlPointMode mode = (BezierSpline.BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Change Point Mode");
            spline.SetControlPointMode(selectedIndex, mode);
            EditorUtility.SetDirty(spline);
        }
    }

    private void OnSceneGUI()
    {
        spline = target as BezierSpline;
        handleTransform = spline.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ?
            handleTransform.rotation : Quaternion.identity;

        Vector3 p0 = ShowPoint(0);
        for (int i = 1; i < spline.ControlPointCount; i += 3)
        {
            Vector3 p1 = ShowPoint(i);
            Vector3 p2 = ShowPoint(i + 1);
            Vector3 p3 = ShowPoint(i + 2);

            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);

            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
            p0 = p3;
        }
        //ShowDirections();
    }

    private void ShowDirections()
    {
        Handles.color = Color.green;
        Vector3 point = spline.GetPoint(0f);
        Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);
        int steps = stepsPerCurve * spline.CurveCount;
        for (int i = 1; i <= steps; i++)
        {
            point = spline.GetPoint(i / (float)steps);
            Handles.DrawLine(point, point + spline.GetDirection(i / (float)steps) * directionScale);
        }
    }
    private Vector3 ShowPoint(int index)
    {
        Vector3 point = handleTransform.TransformPoint(spline.GetControlPoint(index));
        bool isMainControl = index % 3 == 0;
        float handleSize = HandleUtility.GetHandleSize(point);
        float size = handleSize * (isMainControl ? 2 : 1);
        Handles.color = modeColors[(int)spline.GetControlPointMode(index)];
        if (Handles.Button(point, handleRotation, size * mainBezierHandleSize, size * pickSize, Handles.SphereHandleCap))
        {
            selectedIndex = index;
            Repaint();
        }
        if (selectedIndex == index)
        {
            if (spline.editMode == BezierSpline.SplineEditMode.Position)
            {
                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, handleRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(spline, "Move Point");
                    EditorUtility.SetDirty(spline);
                    spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
                }
            }
            else if (spline.editMode == BezierSpline.SplineEditMode.Rotation && selectedIndex % 3 == 0)
            {
                Vector3 tangent = spline.GetDirection(index / (float)spline.ControlPointCount);
                var rotation = spline.GetControlRotation(index);
                EditorGUI.BeginChangeCheck();
                rotation = Handles.FreeRotateHandle(rotation, point, handleSize);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(spline, "Rotate Point");
                    EditorUtility.SetDirty(spline);
                    spline.SetControlRotation(index, rotation);
                }
            }


        }
        return point;
    }
}