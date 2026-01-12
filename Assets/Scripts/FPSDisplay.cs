using UnityEngine;
using TMPro;

public class FPSDisplay : MonoBehaviour
{
    public TMP_Text fpsText;
    float deltaTime;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        int fpsInt = Mathf.CeilToInt(fps);

        fpsText.text = "FPS: " + fpsInt;

        // ===== ĐỔI MÀU THEO FPS =====
        if (fpsInt < 20)
        {
            fpsText.color = Color.red;
        }
        else if (fpsInt < 30)
        {
            fpsText.color = Color.yellow;
        }
        else
        {
            fpsText.color = Color.green;
        }
        // ============================
    }
}
