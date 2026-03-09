namespace Mod;

using HarmonyLib;
using Mod.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

[CreateOnStart]
public class DebugInfo : MonoBehaviour
{
    public enum InfoType
    {
        Space,
        Version,
        UnityVersion,
        Scene,
        LastScene,
        PendingScene,
        Position,
        Rotation,
        Gravity,
        Activated,
        Time,
        TimeUnscaled,
        FPS,
        MinFPS,
        MaxFPS,
    }

    public List<(TMP_Text t, InfoType i)> texts = [];
    public List<float> frames = [];
    public List<(int, float)> minFPS = [];
    public List<(int, float)> maxFPS = [];
    public Canvas canvas;

    public void Start()
    {
        canvas = Builder.Canvas();
        DontDestroyOnLoad(canvas.gameObject);

        AddText(InfoType.UnityVersion);
        AddText(InfoType.Version);
        AddText(InfoType.Scene);
        AddText(InfoType.LastScene);
        AddText(InfoType.PendingScene);

        AddText(InfoType.Space);

        AddText(InfoType.FPS);
        AddText(InfoType.MinFPS);
        AddText(InfoType.MaxFPS);

        AddText(InfoType.Space);

        AddText(InfoType.Position);
        AddText(InfoType.Rotation);

        AddText(InfoType.Space);

        AddText(InfoType.Gravity);
        AddText(InfoType.Activated);

        AddText(InfoType.Space);

        AddText(InfoType.Time);
        AddText(InfoType.TimeUnscaled);
    }

    public void Update()
    {
        UpdateFps();
        UpdateDebug();

        if (LevelSelector.Showing || (!NewMovement.Instance || !NewMovement.Instance.gameObject.activeInHierarchy))
        {
            foreach (var (t, _) in texts)
                t.gameObject.SetActive(false);
            return;
        }

        foreach (var (t, i) in texts)
        {
            t.gameObject.SetActive(true);
            if (i == InfoType.Space)
                t.text = "";
            else
                t.text = $"{i}: {GetText(i)}";
            t.color = GetColor(i);
        }
    }

    public void UpdateDebug()
    {
        Debug.developerConsoleEnabled = true;
        Debug.developerConsoleVisible = true;
    }

    public void UpdateFps()
    {
        frames.Add(Time.unscaledTime);

        foreach (var frame in frames.ToList())
            if (Time.unscaledTime - frame >= 1)
                frames.Remove(frame);

        minFPS.Add((frames.Count, Time.unscaledTime));

        maxFPS.Add((frames.Count, Time.unscaledTime));

        foreach (var fps in minFPS.ToList())
            if (Time.unscaledTime - fps.Item2 > 10)
                minFPS.Remove(fps);

        foreach (var fps in maxFPS.ToList())
            if (Time.unscaledTime - fps.Item2 > 10)
                maxFPS.Remove(fps);
    }

    public string GetText(InfoType type)
    {
        switch (type)
        {
            case InfoType.Position:
                return NewMovement.Instance ? NewMovement.Instance.transform.position.ToString() : "null";
            case InfoType.Rotation:
                return NewMovement.Instance ? NewMovement.Instance.transform.eulerAngles.ToString() : "null";
            case InfoType.Time:
                return Time.time.ToString();
            case InfoType.TimeUnscaled:
                return Time.unscaledTime.ToString();
            case InfoType.Space:
                return "";
            case InfoType.Version:
                return Application.version;
            case InfoType.UnityVersion:
                return Application.unityVersion;
            case InfoType.FPS:
                return frames.Count.ToString();
            case InfoType.MinFPS:
                return minFPS.Min(x => x.Item1).ToString();
            case InfoType.MaxFPS:
                return maxFPS.Max(x => x.Item1).ToString();
            case InfoType.Gravity:
                return Physics.gravity.ToString();
            case InfoType.Activated:
                return NewMovement.Instance ? NewMovement.Instance.activated.ToString() : "null";
            case InfoType.Scene:
                return SceneHelper.CurrentScene;
            case InfoType.LastScene:
                return SceneHelper.LastScene;
            case InfoType.PendingScene:
                return SceneHelper.PendingScene;
        }
        return "Null";
    }

    public Color GetColor(InfoType type)
    {
        switch (type)
        {
            case InfoType.FPS:
                bool isOkay = frames.Count > 80;
                bool isSuffering = frames.Count < 30;
                if (isOkay) return new Color(0, 1, 0, 1);
                if (isSuffering) return new Color(1, 0, 0, 1);
                return new Color(1, 1, 1, 1);
            case InfoType.MinFPS:
                int c = minFPS.Min(x => x.Item1);
                isOkay = c > 80;
                isSuffering = c < 30;
                if (isOkay) return new Color(0, 1, 0, 1);
                if (isSuffering) return new Color(1, 0, 0, 1);
                return new Color(1, 1, 1, 1);
            case InfoType.MaxFPS:
                c = maxFPS.Max(x => x.Item1);
                isOkay = c > 80;
                isSuffering = c < 30;
                if (isOkay) return new Color(0, 1, 0, 1);
                if (isSuffering) return new Color(1, 0, 0, 1);
                return new Color(1, 1, 1, 1);
            case InfoType.Activated:
                if (NewMovement.Instance)
                    if (!NewMovement.Instance.activated)
                        return new Color(1, 0, 0, 1);
                return new Color(1, 1, 1, 1);
            default:
                return new Color(1, 1, 1, 1);
        }
        return new Color(1, 1, 1, 1);
    }

    public void AddText(InfoType type)
    {
        TMP_Text text = Builder.Text(canvas.gameObject, new Vector2(0, 1080 - texts.Count * 20), new Vector2(500, 20), 20, type.ToString());
        texts.Add((text, type));
    }
}

[HarmonyPatch(typeof(Debug), nameof(Debug.isDebugBuild), MethodType.Getter)]
public static class DebugPatch
{
    public static bool Prefix(ref bool __result)
    {
        __result = true;
        return false;
    }
}

[HarmonyPatch]
public static class DebugPatchesInGeneral
{
    [HarmonyPatch(typeof(Debug), nameof(Debug.ClearDeveloperConsole))]
    public static bool Prefix() { return false; }
}