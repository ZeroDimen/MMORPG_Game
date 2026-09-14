using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 횃불 라이트를 "약한 Baked 라이트 + 짧은 Realtime 델타 라이트" 구조로 변환한다.
///
/// 변환 전:  Point light (Light + FireFlicker)          intensity = base ± variation
/// 변환 후:  Point light (Light, Baked)                 intensity = base - variation  (고정, 라이트맵에 구움)
///           └ Flicker light (Light, Realtime + FireFlicker)  intensity = 0 ~ 2*variation (흔들리는 차이분)
///
/// 두 라이트의 합이 원래 밝기 범위와 정확히 일치한다.
/// </summary>
public static class TorchLightBakeSetup
{
    const string FlickerChildName = "Flicker light";

    // 델타 라이트의 Range = 원래 Range * 이 비율 (벽 뚫는 빛샘을 줄이기 위해 짧게)
    const float FlickerRangeRatio = 0.6f;
    const float FlickerRangeMin = 2.5f;

    [MenuItem("Tools/Dungeon/1. Setup Torch Baked + Flicker")]
    public static void SetupTorchLights()
    {
        var flickers = Object.FindObjectsByType<FireFlicker>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        Undo.SetCurrentGroupName("Torch Baked + Flicker Setup");
        int undoGroup = Undo.GetCurrentGroup();

        int converted = 0;
        int skippedNoLight = 0;
        int alreadyDone = 0;
        var sample = new StringBuilder();

        foreach (var ff in flickers)
        {
            if (ff == null) continue;
            var go = ff.gameObject;

            // 이미 변환된 델타 라이트는 건너뛴다 (재실행 안전)
            if (go.name == FlickerChildName)
            {
                alreadyDone++;
                continue;
            }

            var baseLight = go.GetComponent<Light>();
            if (baseLight == null)
            {
                skippedNoLight++;
                continue;
            }

            float bI = ff.baseIntensity;
            float vI = ff.intensityVariation;
            float bR = ff.baseRange;
            float speed = ff.flickerSpeed;
            Color col = baseLight.color;

            // --- 1) 원본 라이트를 약한 Baked 라이트로 ---
            Undo.RecordObject(baseLight, "Bake torch light");
            baseLight.type = LightType.Point;
            baseLight.lightmapBakeType = LightmapBakeType.Baked;
            baseLight.intensity = Mathf.Max(0f, bI - vI);
            baseLight.range = bR;
            baseLight.shadows = LightShadows.Soft;
            EditorUtility.SetDirty(baseLight);

            // --- 2) 원본에서 FireFlicker 제거 (Baked 라이트는 런타임에 렌더링되지 않으므로 의미 없음) ---
            Undo.DestroyObjectImmediate(ff);

            // --- 3) 델타 라이트 자식 생성 ---
            var existing = go.transform.Find(FlickerChildName);
            GameObject child;
            if (existing != null)
            {
                child = existing.gameObject;
            }
            else
            {
                child = new GameObject(FlickerChildName);
                Undo.RegisterCreatedObjectUndo(child, "Create Flicker light");
                Undo.SetTransformParent(child.transform, go.transform, "Parent Flicker light");
            }

            child.layer = go.layer;
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;
            child.transform.localScale = Vector3.one;

            float fRange = Mathf.Max(FlickerRangeMin, bR * FlickerRangeRatio);

            var fLight = child.GetComponent<Light>();
            if (fLight == null) fLight = Undo.AddComponent<Light>(child);
            Undo.RecordObject(fLight, "Configure flicker light");
            fLight.type = LightType.Point;
            fLight.color = col;
            fLight.lightmapBakeType = LightmapBakeType.Realtime;
            fLight.shadows = LightShadows.None;
            fLight.range = fRange;
            fLight.intensity = vI;
            fLight.renderMode = LightRenderMode.Auto;

            // URP 추가 라이트 데이터
            if (child.GetComponent<UniversalAdditionalLightData>() == null)
                Undo.AddComponent<UniversalAdditionalLightData>(child);

            // FireFlicker는 [RequireComponent(typeof(Light))] 이므로 Light 추가 이후에 붙인다
            var fFlicker = child.GetComponent<FireFlicker>();
            if (fFlicker == null) fFlicker = Undo.AddComponent<FireFlicker>(child);
            Undo.RecordObject(fFlicker, "Configure flicker");
            fFlicker.baseIntensity = vI;          // 0 ~ 2*vI 로 흔들림
            fFlicker.intensityVariation = vI;
            fFlicker.baseRange = fRange;
            fFlicker.rangeVariation = 0f;         // Range 고정 (라이트 컬링 재계산 방지)
            fFlicker.flickerSpeed = speed;

            EditorUtility.SetDirty(child);
            converted++;

            if (converted <= 3)
            {
                sample.AppendLine(
                    $"  {GetPath(go.transform)}  |  baked intensity={baseLight.intensity:0.###} range={bR:0.###} " +
                    $"/ flicker {0f}~{vI * 2f:0.###} range={fRange:0.###} speed={speed:0.###}");
            }
        }

        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkAllScenesDirty();

        Debug.Log(
            $"[TorchLightBakeSetup] 변환 완료: {converted}개\n" +
            $"  건너뜀(이미 델타 라이트): {alreadyDone}, 건너뜀(Light 없음): {skippedNoLight}\n" +
            $"샘플:\n{sample}");
    }

    [MenuItem("Tools/Dungeon/2. Set PC_Renderer to Forward+")]
    public static void SetForwardPlus()
    {
        var sb = new StringBuilder();
        foreach (var guid in AssetDatabase.FindAssets("t:UniversalRendererData"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var data = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(path);
            if (data == null) continue;

            if (path.Contains("PC_Renderer"))
            {
                var before = data.renderingMode;
                Undo.RecordObject(data, "Set Forward+");
                data.renderingMode = RenderingMode.ForwardPlus;
                EditorUtility.SetDirty(data);
                sb.AppendLine($"  [변경] {path}: {before} -> {data.renderingMode}");
            }
            else
            {
                sb.AppendLine($"  [유지] {path}: {data.renderingMode}");
            }
        }
        AssetDatabase.SaveAssets();
        Debug.Log($"[TorchLightBakeSetup] Rendering Path\n{sb}");
    }

    [MenuItem("Tools/Dungeon/3. Report Torch Light State")]
    public static void Report()
    {
        var lights = Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int baked = 0, realtime = 0, mixed = 0, withFlicker = 0;
        float minBaked = float.MaxValue, maxBaked = float.MinValue;

        foreach (var l in lights)
        {
            switch (l.lightmapBakeType)
            {
                case LightmapBakeType.Baked: baked++; break;
                case LightmapBakeType.Realtime: realtime++; break;
                case LightmapBakeType.Mixed: mixed++; break;
            }
            if (l.GetComponent<FireFlicker>() != null) withFlicker++;
            if (l.lightmapBakeType == LightmapBakeType.Baked && l.name != FlickerChildName)
            {
                minBaked = Mathf.Min(minBaked, l.intensity);
                maxBaked = Mathf.Max(maxBaked, l.intensity);
            }
        }

        Debug.Log(
            $"[TorchLightBakeSetup] 라이트 총 {lights.Length}개 | Baked {baked} / Realtime {realtime} / Mixed {mixed}\n" +
            $"  FireFlicker 붙은 라이트: {withFlicker}\n" +
            $"  Baked intensity 범위: {minBaked:0.###} ~ {maxBaked:0.###}");
    }

    static string GetPath(Transform t)
    {
        string p = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            p = t.name + "/" + p;
        }
        return p;
    }
}
