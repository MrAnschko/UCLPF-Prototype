using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class SpatializerDebug : IAudioEffectPluginGUI
{
    Vector3 focusPosition; // Vector where we think the Person is focussing.
    float mix;

    public override string Name
    {
        get { return "Aarons Spatializer Settings"; }
    }


    public override string Description
    {
        get { return "Plugin to be used in pair with the custom Spatializer for debugging purposes"; }
    }

    public override string Vendor
    {
        get { return "Aaron Rose"; }
    }

    public override bool OnGUI(IAudioEffectPlugin plugin)
    {
        plugin.GetFloatParameter("Total Mix", out mix);
        Rect r = GUILayoutUtility.GetRect(200, 150, GUILayout.ExpandWidth(true));
        GUIHelpers.DrawText(r.x + 5, r.y - 5, r.width, $"{mix}", Color.white);
        Debug.Log(mix);

        float[] buffer;
        int numsamples = 16;
        plugin.GetFloatBuffer("SourcePos", out buffer, numsamples);
        Matrix4x4 matrix = new Matrix4x4();
        for (int i = 0; i < numsamples; i++)
        {
            matrix[i] = buffer[i];
        }
        
        Debug.Log(matrix);

        return true;
    }
}
