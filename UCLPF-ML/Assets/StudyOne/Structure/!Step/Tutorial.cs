
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField]TMP_Text text;

    private void Start()
    {
        SetText(ProcessInfos.CurrentMethod);

    }

    public void SetText(ProcessInfos.InteractionMethod method)
    {
        if(method == ProcessInfos.InteractionMethod.Circle)
        {
            text.text = "You will have the short option to practice with the method.\r\n" +
                "This is the so called <u>Circle</u> method. Depending on where you look a circle with you as center will be drawn. Objects farther away from the circle circumference will be muffled. If you look up you can hear all sources unmuffled.\r\n\r\n" +
                "Follow the purple marker.\r\n\r\nGreen cylinders are sound sources. They will be invisible in the future. For Practice there is no time limit, afterwards the sources will only play for 150 Seconds.";
        }
        if (method == ProcessInfos.InteractionMethod.View)
        {
            text.text = "You will have the short option to practice with the method.\r\n" +
                "This is the so called <u>View</u> method. Imagine it like a flashlight coming from you pupils. Objects farther away from the light will be muffled.\r\n\r\n" +
                "Follow the purple marker.\r\n\r\nGreen cylinders are sound sources. They will be invisible in the future. For Practice there is no time limit, afterwards the sources will only play for 150 Seconds.";
        }

        if (method == ProcessInfos.InteractionMethod.Point)
        {
            text.text = "You will have the short option to practice with the method.\r\n" +
                "This is the so called <u>Point</u> method. Depending on where you look a point will be drawn. Objects farther away from the point will be muffled. If you look up you can hear all sources unmuffled.\r\n\r\n" +
                "Follow the purple marker.\r\n\r\nGreen cylinders are sound sources. They will be invisible in the future. For Practice there is no time limit, afterwards the sources will only play for 150 Seconds.";
        }
    }
}
