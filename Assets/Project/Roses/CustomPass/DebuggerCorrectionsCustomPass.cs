using UnityEngine;



public class DebuggerCorrectionsCustomPass : MonoBehaviour
{
    int _resolutionChangeIndex;
    Vector2Int[] _resolutions = new Vector2Int[] { new(960, 540), new(1366, 768), new(1920, 1080), new(2560, 1440), new(3840, 2160) };
    string[] _resolutionsStrings;

    private void Awake()
    {
        _resolutionsStrings = new string[_resolutions.Length];
        for (int i = 0; i < _resolutions.Length; ++i)
        {
            int width = _resolutions[i].x;
            int height = _resolutions[i].y;

            _resolutionsStrings[i] = width + "x" + height;

            if (width == Screen.width && height == Screen.height) _resolutionChangeIndex = i;
        }        
    }

    private void OnGUI()
    {
        Rect position = new Rect(16, 16, Screen.width *0.9f, Screen.height * 0.9f);
        GUIContent content = new GUIContent(CorrectionsCustomPass.LogInfo_1 + "\n" + CorrectionsCustomPass.LogInfo_2 + "\n" + CorrectionsCustomPass.LogInfo_3);
        GUIStyle style = GUI.skin.box;
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = 18;
        style.normal.textColor = Color.LerpUnclamped(Color.red, Color.yellow, 0.5f);
        GUI.Box(position, content, style);

        position.y += 128;
        position.width = 200;
        position.height = 100;
        int prev_resolutionChangeIndex = _resolutionChangeIndex;
        _resolutionChangeIndex = GUI.SelectionGrid(position, _resolutionChangeIndex, _resolutionsStrings, 1);
        if (prev_resolutionChangeIndex != _resolutionChangeIndex)
        {
            Screen.SetResolution(_resolutions[_resolutionChangeIndex].x, _resolutions[_resolutionChangeIndex].y, Screen.fullScreen);
        }
    }
}
