using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlainColorCreator : EditorWindow {

    Color targetColor = Color.white;
    new string name = "white";
    int size = 16;

    [MenuItem("Window/PTexture Creator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<PlainColorCreator>("Simple Texture Creator");
    }

    private void OnGUI()
    {
        name = EditorGUILayout.TextField("File name - ", name);
        size = EditorGUILayout.IntField("texture size -", size);
        targetColor = EditorGUILayout.ColorField("Texture Color", targetColor);

        if(GUILayout.Button("Create Texture"))
        {
            TextureCreator.SaveTexture(TextureCreator.ColorToTexture(targetColor, size), 
                "/Textures", name);
        }
    }

}
