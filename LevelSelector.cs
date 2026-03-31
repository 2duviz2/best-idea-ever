namespace Mod;

using Mod.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mod.Helpers;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.Linq;
using UnityEngine.EventSystems;

[CreateOnStart]
public class LevelSelector : MonoBehaviour
{
    public Canvas canvas;
    public List<TMP_Text> texts = [];

    public static List<string> levels = [
        "Level 0-1", "Level 0-2", "Level 0-3", "Level 0-4", "Level 0-5", "Level 0-S", "Level 0-E",
        "Level 1-1", "Level 1-2", "Level 1-3", "Level 1-4", "Level 1-S", "Level 1-E",
        "Level 2-1", "Level 2-2", "Level 2-3", "Level 2-4", "Level 2-S",
        "Level 3-1", "Level 3-2",
        "Level 4-1", "Level 4-2", "Level 4-3", "Level 4-4", "Level 4-S",
        "Level 5-1", "Level 5-2", "Level 5-3", "Level 5-4", "Level 5-S",
        "Level 6-1", "Level 6-2",
        "Level 7-1", "Level 7-2", "Level 7-3", "Level 7-4", "Level 7-S",
        "Level 8-1", "Level 8-2", "Level 8-3", "Level 8-4",
        "Level P-1", "Level P-2",
        ];

    public int currentSelected = 0;
    public static bool Showing = false;

    public void Start()
    {
        canvas = Builder.Canvas();

        DontDestroyOnLoad(canvas);

        levels = AddressableKeysGrabber.MainAddressablesLocator.GetAllLocationsOfType(typeof(SceneInstance)).Select(location => location.PrimaryKey).ToList();

        CreateTexts();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && (!NewMovement.Instance || !NewMovement.Instance.activated || OptionsManager.Instance.paused))
            Showing = !Showing;

        if (!Showing)
        {
            foreach (var text in texts)
                text.rectTransform.anchoredPosition = new Vector2(-300, text.rectTransform.anchoredPosition.y);
            return;
        }

        int i = 0;
        foreach (var text in texts)
        {
            float x = text.rectTransform.anchoredPosition.x;
            text.rectTransform.anchoredPosition = new Vector2(x - (x - (currentSelected == i ? 30 : 0)) / 5f, text.rectTransform.anchoredPosition.y);
            if (currentSelected == i) text.color = new Color(1, 0, 0, 1);
            else text.color = new Color(1, 1, 1, 1);
            i++;
        }

        currentSelected += (int)Mathf.Clamp(-Input.mouseScrollDelta.y * 2, -1, 1);
        if (currentSelected < 0) currentSelected = texts.Count - 1;
        if (currentSelected > texts.Count - 1) currentSelected = 0;

        if (Input.GetMouseButtonDown(0) && Time.time > 2)
        {
            SceneHelper.LoadScene(levels[currentSelected]);
            Showing = false;
        }
    }

    public void CreateTexts()
    {
        float yOffset = 0;

        int i = 0;
        foreach (var level in levels)
        {
            int storedI = i;

            TMP_Text text = Builder.Text(canvas.gameObject, new Vector2(0, yOffset), new Vector2(300, 20), 20, level);
            text.raycastTarget = true;
            texts.Add(text);

            EventTrigger et = text.gameObject.AddComponent<EventTrigger>();
            et.triggers = [];

            EventTrigger.Entry entry = new()
            {
                eventID = EventTriggerType.PointerEnter
            };

            entry.callback.AddListener((data) => { currentSelected = storedI; });
            et.triggers.Add(entry);

            yOffset -= 20;
            i++;
        }
    }
}