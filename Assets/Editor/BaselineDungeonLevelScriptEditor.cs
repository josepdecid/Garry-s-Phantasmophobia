using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BaselineDungeonLevelScript))]
public class BaselineDungeonLevelScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Get target level script from dungeon object
        BaselineDungeonLevelScript levelScript = (BaselineDungeonLevelScript) target;

        // Add control fields by overriding propeties
        levelScript.sizeX = EditorGUILayout.IntField("Size x", levelScript.sizeX);
        levelScript.sizeZ = EditorGUILayout.IntField("Size z", levelScript.sizeZ);

        if(GUILayout.Button("Build dungeon floor")){
            levelScript.BuildDungeonFloor();
        }

        if(GUILayout.Button("Clean up floor")){
            levelScript.CleanUpFloor();
        }

        if(GUILayout.Button("Clean up all")){
            levelScript.CleanUp();
        }
    }
}
