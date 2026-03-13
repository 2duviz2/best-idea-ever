namespace Mod.UI;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class Builder
{
    public static TMP_FontAsset font;

    public static void LoadFont() => font = Plugin.Ass<TMP_FontAsset>("Assets/Fonts/VCR_OSD_MONO_UI.asset");

    public static Canvas Canvas(float width = 1920, float height = 1080)
    {
        GameObject canvasObj = new GameObject("RuntimeCanvas");
        canvasObj.layer = LayerMask.NameToLayer("UI");

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.pixelPerfect = true;
        canvas.sortingOrder = 30000;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(width, height);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        Plugin.LogInfo("Canvas created!");

        return canvas;
    }

    public static TMP_Text Text(GameObject container, Vector2 position, Vector2 sizeDelta, float size = 20, string defaultText = "Ella jura")
    {
        GameObject obj = new GameObject("Text");
        obj.transform.SetParent(container.transform, false);

        RectTransform rect = obj.GetOrAddComponent<RectTransform>();
        rect.ApplyDefaults();
        rect.position = position;
        rect.sizeDelta = sizeDelta;

        if (!font) LoadFont();
        
        TMP_Text Text = obj.AddComponent<TextMeshProUGUI>();
        Text.text = defaultText;
        Text.font = font;
        Text.fontSize = size;
        Text.alignment = TextAlignmentOptions.TopLeft;
        Text.raycastTarget = false;

        return Text;
    }

    extension(RectTransform rect)
    {
        /// <summary> Sets both anchors and pivots to (0, 1) </summary>
        public void ApplyDefaults()
        {
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
        }
    }
}
