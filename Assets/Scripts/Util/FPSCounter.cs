using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public float avgFrameRate;
    public Text display_Text;

    IEnumerator Start()
    {
        GUI.depth = 2;
        while (true)
        {
            if (Time.timeScale == 1)
            {
                yield return new WaitForSeconds(0.05f);
                avgFrameRate = (1 / Time.deltaTime);
                display_Text.text = "FPS: " + (Mathf.Round(avgFrameRate));
            }
            else
            {
                display_Text.text = "Pause";
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}