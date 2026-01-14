using System.Runtime.InteropServices;
using System.Xml.Linq;
using UnityEditor;
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
        return true;
    }
}
