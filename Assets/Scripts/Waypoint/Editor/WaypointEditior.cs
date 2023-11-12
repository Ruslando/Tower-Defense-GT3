using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Waypoint))]
public class WaypointEditior : Editor
{
   Waypoint Waypoint => target as Waypoint;

   private void OnSceneGUI()
   {
      Handles.color = Color.red;
      for (int i = 0; i < Waypoint.Points.Length; i++)
      {
         EditorGUI.BeginChangeCheck();
         
         //create handles
         Vector3 currentWaypointPoint = Waypoint.CurrentPosition + Waypoint.Points[i];
         Vector3 newWaypointPoint = Handles.FreeMoveHandle(currentWaypointPoint, 0.7f, 
            new Vector3(0.3f, 0.3f, 0.3f), Handles.SphereHandleCap);
         
         //create text
         GUIStyle textStyle = new GUIStyle();
         textStyle.fontStyle = FontStyle.Bold;
         textStyle.fontSize = 16;
         textStyle.normal.textColor = Color.yellow;
         Vector3 textAllignment = Vector3.down * 0.35f + Vector3.right * 0.35f;
         Handles.Label(Waypoint.CurrentPosition + Waypoint.Points[i] + textAllignment, $"{i + 1}", textStyle);

         EditorGUI.EndChangeCheck();

         if (EditorGUI.EndChangeCheck())
         {
            Undo.RecordObject(target, "Free Move Handle");
            Waypoint.Points[i] = newWaypointPoint - Waypoint.CurrentPosition;
         }
      }
   }
}
