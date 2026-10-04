using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 라이트맵 베이크 준비: Static 플래그 진단/적용 + Adaptive Probe Volumes 설정.
/// 콘솔 로그는 MCP 경유 시 첫 줄만 읽히므로 전부 한 줄로 출력한다.
/// </summary>
public static class DungeonBakePrep
{
    // ------------------------------------------------------------------
    // A. 진단
    // ------------------------------------------------------------------
    [MenuItem("Tools/Dungeon/A. Diagnose Bake Readiness")]
    public static void Diagnose()
    {
        var renderers = Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        int contributeGI = 0, missingUV2 = 0, noMesh = 0;
        var flagCombos = new Dictionary<StaticEditorFlags, int>();
        var badModels = new Dictionary<string, int>();

        foreach (var r in renderers)
        {
            var flags = GameObjectUtility.GetStaticEditorFlags(r.gameObject);
            if (!flagCombos.ContainsKey(flags)) flagCombos[flags] = 0;
            flagCombos[flags]++;

            if ((flags & StaticEditorFlags.ContributeGI) == 0) continue;
            contributeGI++;

            var mf = r.GetComponent<MeshFilter>();
            var mesh = mf != null ? mf.sharedMesh : null;
            if (mesh == null) { noMesh++; continue; }

            if (!mesh.HasVertexAttribute(UnityEngine.Rendering.VertexAttribute.TexCoord1))
            {
                missingUV2++;
                string path = AssetDatabase.GetAssetPath(mesh);
                if (string.IsNullOrEmpty(path)) path = "(생성 메시)";
                if (!badModels.ContainsKey(path)) badModels[path] = 0;
                badModels[path]++;
            }
        }

        string combos = string.Join(" , ", flagCombos.OrderByDescending(k => k.Value)
            .Take(8).Select(k => $"[{k.Key}]x{k.Value}"));
        string bad = badModels.Count == 0
            ? "없음"
            : string.Join(" , ", badModels.OrderByDescending(k => k.Value).Take(12).Select(k => $"{k.Key}x{k.Value}"));

        // 씬의 Torch 오브젝트 상태
        var torches = Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Where(r => r.gameObject.name.StartsWith("Torch")).ToArray();
        int torchGI = torches.Count(r => (GameObjectUtility.GetStaticEditorFlags(r.gameObject) & StaticEditorFlags.ContributeGI) != 0);

        Debug.Log($"[DIAG] MeshRenderer={renderers.Length} ContributeGI={contributeGI} noMesh={noMesh} missingUV2={missingUV2}" +
                  $" || Torch오브젝트={torches.Length} 그중ContributeGI={torchGI}" +
                  $" || 플래그조합: {combos}" +
                  $" || UV2없는 메시: {bad}");
    }

    // ------------------------------------------------------------------
    // B. Torch를 Contribute GI로 (본체만, 파티클/라이트 자식 제외)
    // ------------------------------------------------------------------
    const StaticEditorFlags TorchFlags =
        StaticEditorFlags.ContributeGI |
        StaticEditorFlags.BatchingStatic |
        StaticEditorFlags.OccluderStatic |
        StaticEditorFlags.OccludeeStatic |
        StaticEditorFlags.ReflectionProbeStatic;

    [MenuItem("Tools/Dungeon/B. Mark Torches Contribute GI")]
    public static void MarkTorches()
    {
        var torches = Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Where(r => r.gameObject.name.StartsWith("Torch"))
            .Select(r => r.gameObject)
            .ToArray();

        Undo.SetCurrentGroupName("Mark torches contribute GI");
        int undoGroup = Undo.GetCurrentGroup();

        int changed = 0;
        foreach (var go in torches)
        {
            var before = GameObjectUtility.GetStaticEditorFlags(go);
            if (before == TorchFlags) continue;
            Undo.RecordObject(go, "Static flags");
            // 자기 자신에만 적용 (자식 Flame / HeatDistortion / 라이트는 건드리지 않음)
            GameObjectUtility.SetStaticEditorFlags(go, TorchFlags);

            // MeshRenderer가 라이트맵을 받도록
            var r = go.GetComponent<MeshRenderer>();
            if (r != null)
            {
                Undo.RecordObject(r, "Receive GI");
                r.receiveGI = ReceiveGI.Lightmaps;
                r.scaleInLightmap = 1f;
                EditorUtility.SetDirty(r);
            }
            EditorUtility.SetDirty(go);
            changed++;
        }

        // torch.fbx 라이트맵 UV 생성 켜기
        string importerMsg = EnsureLightmapUVs("Assets/99. Resources/Mega Fantasy Props Pack/FBX/torch.fbx");

        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkAllScenesDirty();
        Debug.Log($"[TORCH-STATIC] 대상={torches.Length} 변경={changed} || importer: {importerMsg}");
    }

    static string EnsureLightmapUVs(string fbxPath)
    {
        var imp = AssetImporter.GetAtPath(fbxPath) as ModelImporter;
        if (imp == null) return $"{fbxPath} -> ModelImporter 아님/없음";
        if (imp.generateSecondaryUV) return $"{fbxPath} -> 이미 켜져 있음";
        imp.generateSecondaryUV = true;
        imp.SaveAndReimport();
        return $"{fbxPath} -> Generate Lightmap UVs 켜고 재임포트";
    }

    /// <summary>
    /// Contribute GI인데 UV2가 없는 메시의 원본 모델에 Generate Lightmap UVs를 켜고 재임포트한다.
    /// </summary>
    [MenuItem("Tools/Dungeon/A2. Enable Lightmap UVs on GI Meshes")]
    public static void EnableLightmapUVsOnGIMeshes()
    {
        var paths = new HashSet<string>();
        foreach (var r in Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if ((GameObjectUtility.GetStaticEditorFlags(r.gameObject) & StaticEditorFlags.ContributeGI) == 0) continue;
            var mf = r.GetComponent<MeshFilter>();
            var mesh = mf != null ? mf.sharedMesh : null;
            if (mesh == null) continue;
            if (mesh.HasVertexAttribute(UnityEngine.Rendering.VertexAttribute.TexCoord1)) continue;
            var p = AssetDatabase.GetAssetPath(mesh);
            if (!string.IsNullOrEmpty(p)) paths.Add(p);
        }

        var done = new List<string>();
        var skipped = new List<string>();

        AssetDatabase.StartAssetEditing();
        try
        {
            foreach (var p in paths)
            {
                var imp = AssetImporter.GetAtPath(p) as ModelImporter;
                if (imp == null) { skipped.Add($"{System.IO.Path.GetFileName(p)}(ModelImporter아님)"); continue; }
                if (imp.generateSecondaryUV) { skipped.Add($"{System.IO.Path.GetFileName(p)}(이미ON)"); continue; }
                imp.generateSecondaryUV = true;
                imp.secondaryUVHardAngle = 88f;
                imp.secondaryUVAngleDistortion = 8f;
                imp.secondaryUVAreaDistortion = 15f;
                imp.secondaryUVPackMargin = 4f;
                imp.SaveAndReimport();
                done.Add(System.IO.Path.GetFileName(p));
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }
        AssetDatabase.Refresh();

        Debug.Log($"[LIGHTMAP-UV] 대상모델={paths.Count} 켜짐={done.Count} [{string.Join(", ", done)}] 건너뜀={skipped.Count} [{string.Join(", ", skipped)}]");
    }


    // ------------------------------------------------------------------
    // C. Adaptive Probe Volumes
    // ------------------------------------------------------------------
    [MenuItem("Tools/Dungeon/C. Setup Adaptive Probe Volumes")]
    public static void SetupAPV()
    {
        // 1) URP 에셋들을 APV로 전환 (setter가 internal이라 SerializedObject로 접근)
        var sb = new List<string>();
        foreach (var guid in AssetDatabase.FindAssets("t:UniversalRenderPipelineAsset"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!path.StartsWith("Assets/Settings/")) continue;   // 프로젝트에서 실제로 쓰는 것만
            var asset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(path);
            if (asset == null) continue;

            var before = asset.lightProbeSystem;
            var so = new SerializedObject(asset);
            var prop = so.FindProperty("m_LightProbeSystem");
            if (prop == null)
            {
                var it = so.GetIterator();
                while (it.NextVisible(true))
                {
                    if (it.name.ToLower().Contains("lightprobesystem")) { prop = so.FindProperty(it.propertyPath); break; }
                }
            }
            if (prop == null)
            {
                sb.Add($"{System.IO.Path.GetFileNameWithoutExtension(path)}:프로퍼티못찾음");
                continue;
            }
            prop.intValue = (int)LightProbeSystem.ProbeVolumes;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(asset);
            sb.Add($"{System.IO.Path.GetFileNameWithoutExtension(path)}:{before}->{asset.lightProbeSystem}");
        }
        AssetDatabase.SaveAssets();

        // 2) 씬에 글로벌 Probe Volume 배치
        var existing = Object.FindObjectsByType<ProbeVolume>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        ProbeVolume pv;
        string pvMsg;
        if (existing.Length > 0)
        {
            pv = existing[0];
            pvMsg = $"기존 {existing.Length}개 재사용";
        }
        else
        {
            var go = new GameObject("Adaptive Probe Volume - Dungeon");
            Undo.RegisterCreatedObjectUndo(go, "Create Probe Volume");
            pv = Undo.AddComponent<ProbeVolume>(go);
            pvMsg = "신규 생성";
        }

        Undo.RecordObject(pv, "Configure Probe Volume");
        pv.mode = ProbeVolume.Mode.Scene;   // 씬의 모든 Contribute GI 오브젝트를 자동으로 감쌈
        pv.overridesSubdivLevels = false;
        pv.fillEmptySpaces = false;
        EditorUtility.SetDirty(pv);

        EditorSceneManager.MarkAllScenesDirty();
        Debug.Log($"[APV] URP에셋: {string.Join(" , ", sb)} || ProbeVolume: {pvMsg} mode={pv.mode} size={pv.size}");
    }

    [MenuItem("Tools/Dungeon/D. Verify APV")]
    public static void VerifyAPV()
    {
        var urp = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        var pvs = Object.FindObjectsByType<ProbeVolume>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        string list = string.Join(" , ", pvs.Select(p => $"{p.name}(mode={p.mode},size={p.size},active={p.isActiveAndEnabled})"));

        var perScene = Object.FindObjectsByType<ProbeVolumePerSceneData>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        Debug.Log($"[APV-VERIFY] 활성URP={(urp != null ? urp.name : "null")} lightProbeSystem={(urp != null ? urp.lightProbeSystem.ToString() : "-")}" +
                  $" || ProbeVolume {pvs.Length}개: {list}" +
                  $" || PerSceneData={perScene.Length}");
    }


    [MenuItem("Tools/Dungeon/E. Torch Brightness Stats")]
    public static void BrightnessStats()
    {
        var all = Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var baked = all.Where(l => l.lightmapBakeType == LightmapBakeType.Baked).ToArray();
        var flick = all.Where(l => l.name == "Flicker light").ToArray();

        var groups = baked.GroupBy(l => Mathf.Round(l.intensity * 10f) / 10f)
                          .OrderByDescending(g => g.Count()).Take(8)
                          .Select(g => $"{g.Key:0.#}x{g.Count()}");

        var rangeGroups = baked.GroupBy(l => Mathf.Round(l.range * 10f) / 10f)
                               .OrderByDescending(g => g.Count()).Take(6)
                               .Select(g => $"R{g.Key:0.#}x{g.Count()}");

        var fGroups = flick.GroupBy(l => Mathf.Round(l.intensity * 10f) / 10f)
                           .OrderByDescending(g => g.Count()).Take(6)
                           .Select(g => $"{g.Key:0.#}x{g.Count()}");

        float bMin = baked.Length > 0 ? baked.Min(l => l.bounceIntensity) : -1f;
        float bMax = baked.Length > 0 ? baked.Max(l => l.bounceIntensity) : -1f;

        string lsInfo = "-";
        LightingSettings ls;
        if (Lightmapping.TryGetLightingSettings(out ls) && ls != null)
            lsInfo = $"{ls.name} indirectScale={ls.indirectScale:0.##} albedoBoost={ls.albedoBoost:0.##} lightmapResolution={ls.lightmapResolution:0.#}";

        Debug.Log($"[BRIGHT] baked={baked.Length} intensity: {string.Join(" , ", groups)}" +
                  $" || range: {string.Join(" , ", rangeGroups)}" +
                  $" || flicker={flick.Length} intensity: {string.Join(" , ", fGroups)}" +
                  $" || bounceIntensity={bMin:0.##}~{bMax:0.##} || LightingSettings: {lsInfo}");
    }


    /// <summary>
    /// Flicker light 자식을 가진 Baked 라이트의 intensity에만 배율을 적용한다.
    /// 실시간 Flicker 라이트는 건드리지 않으므로 깜빡임 진폭은 그대로 유지된다.
    /// 주의: 누적 적용된다. 두 번 실행하면 4배.
    /// </summary>
    const float BakedIntensityMultiplier = 2f;

    [MenuItem("Tools/Dungeon/F. Multiply Baked Torch Intensity x2")]
    public static void MultiplyBakedIntensity()
    {
        var candidates = Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Where(l => l.lightmapBakeType == LightmapBakeType.Baked)
            .Where(l => l.transform.Find("Flicker light") != null)
            .ToArray();

        // Range가 큰 건 횜불이 아닌 방 조명이므로 제외한다.
        var targets = candidates.Where(l => l.range <= 10f).ToArray();
        var excluded = candidates.Where(l => l.range > 10f).ToArray();

        if (targets.Length == 0)
        {
            Debug.LogWarning("[BRIGHTEN] 대상 라이트가 없음. Main 씬이 열려 있는지 확인.");
            return;
        }

        Undo.SetCurrentGroupName("Brighten baked torch lights");
        int undoGroup = Undo.GetCurrentGroup();

        float beforeMin = targets.Min(l => l.intensity);
        float beforeMax = targets.Max(l => l.intensity);

        foreach (var l in targets)
        {
            Undo.RecordObject(l, "Brighten baked light");
            l.intensity *= BakedIntensityMultiplier;
            EditorUtility.SetDirty(l);
        }

        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkAllScenesDirty();

        string exList = excluded.Length == 0 ? "없음" : string.Join(", ", excluded.Select(l => $"{l.name}(R{l.range:0.#},I{l.intensity:0.#})"));

        Debug.Log($"[BRIGHTEN] x{BakedIntensityMultiplier} 적용={targets.Length} " +
                  $"|| before {beforeMin:0.##}~{beforeMax:0.##} -> after {targets.Min(l => l.intensity):0.##}~{targets.Max(l => l.intensity):0.##} " +
                  $"|| Flicker 라이트 변경없음 || 제외(Range>10): {exList}");
    }


    [MenuItem("Tools/Dungeon/G. Verify Bake Result")]
    public static void VerifyBake()
    {
        int maps = LightmapSettings.lightmaps != null ? LightmapSettings.lightmaps.Length : 0;

        var gi = Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Where(r => (GameObjectUtility.GetStaticEditorFlags(r.gameObject) & StaticEditorFlags.ContributeGI) != 0)
            .ToArray();
        int mapped = gi.Count(r => r.lightmapIndex >= 0 && r.lightmapIndex < 65534);

        var torches = gi.Where(r => r.gameObject.name.StartsWith("Torch")).ToArray();
        int torchMapped = torches.Count(r => r.lightmapIndex >= 0 && r.lightmapIndex < 65534);

        var prv = UnityEngine.Rendering.ProbeReferenceVolume.instance;
                        int perScene = Object.FindObjectsByType<ProbeVolumePerSceneData>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length;
        string apv = prv != null ? $"initialized={prv.isInitialized} perSceneData={perScene}" : $"null perSceneData={perScene}";

        Debug.Log($"[BAKE-VERIFY] lightmaps={maps} || ContributeGI={gi.Length} 라이트맵받음={mapped} 안받음={gi.Length - mapped}" +
                  $" || Torch={torches.Length} 라이트맵받음={torchMapped} || APV {apv}");
    }


    [MenuItem("Tools/Dungeon/H. Inspect Unmapped Renderers")]
    public static void InspectUnmapped()
    {
        var gi = Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Where(r => (GameObjectUtility.GetStaticEditorFlags(r.gameObject) & StaticEditorFlags.ContributeGI) != 0)
            .ToArray();

        var unmapped = gi.Where(r => r.lightmapIndex < 0 || r.lightmapIndex >= 65534).ToArray();

        var byIndex = unmapped.GroupBy(r => r.lightmapIndex).Select(g => $"idx{g.Key}x{g.Count()}");
        var byReceive = unmapped.GroupBy(r => r.receiveGI).Select(g => $"{g.Key}x{g.Count()}");
        var byScale = unmapped.GroupBy(r => Mathf.Round(r.scaleInLightmap * 100f) / 100f).OrderByDescending(g => g.Count()).Take(5).Select(g => $"scale{g.Key:0.##}x{g.Count()}");
        var byActive = unmapped.GroupBy(r => r.gameObject.activeInHierarchy).Select(g => $"active{g.Key}x{g.Count()}");
        var byName = unmapped.GroupBy(r => r.gameObject.name).OrderByDescending(g => g.Count()).Take(10).Select(g => $"{g.Key}x{g.Count()}");

        // 매핑된 횜불과 안된 횜불 비교
        var torches = gi.Where(r => r.gameObject.name.StartsWith("Torch")).ToArray();
        var tOk = torches.Where(r => r.lightmapIndex >= 0 && r.lightmapIndex < 65534).ToArray();
        var tNo = torches.Where(r => r.lightmapIndex < 0 || r.lightmapIndex >= 65534).ToArray();
        string tCmp = $"OK={tOk.Length}(scale {(tOk.Length > 0 ? tOk.Min(r => r.scaleInLightmap) : -1):0.##}~{(tOk.Length > 0 ? tOk.Max(r => r.scaleInLightmap) : -1):0.##}) " +
                      $"NO={tNo.Length}(scale {(tNo.Length > 0 ? tNo.Min(r => r.scaleInLightmap) : -1):0.##}~{(tNo.Length > 0 ? tNo.Max(r => r.scaleInLightmap) : -1):0.##}, receiveGI {string.Join("/", tNo.GroupBy(r => r.receiveGI).Select(g => g.Key + "x" + g.Count()))})";

        Debug.Log($"[UNMAPPED] 총={unmapped.Length} || lightmapIndex: {string.Join(" , ", byIndex)}" +
                  $" || receiveGI: {string.Join(" , ", byReceive)} || {string.Join(" , ", byScale)} || {string.Join(" , ", byActive)}" +
                  $" || 이름: {string.Join(" , ", byName)} || Torch비교: {tCmp}");
    }


    [MenuItem("Tools/Dungeon/I. Find Inactive GI Roots")]
    public static void FindInactiveRoots()
    {
        var gi = Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Where(r => (GameObjectUtility.GetStaticEditorFlags(r.gameObject) & StaticEditorFlags.ContributeGI) != 0)
            .Where(r => !r.gameObject.activeInHierarchy)
            .ToArray();

        var roots = new Dictionary<string, int>();
        foreach (var r in gi)
        {
            // 꺼져 있는 최상위 조상을 찾는다
            Transform highestOff = null;
            for (var t = r.transform; t != null; t = t.parent)
                if (!t.gameObject.activeSelf) highestOff = t;

            string key = highestOff != null ? Path2(highestOff) : $"(자기자신만 off) {Path2(r.transform)}";
            if (!roots.ContainsKey(key)) roots[key] = 0;
            roots[key]++;
        }

        var top = roots.OrderByDescending(k => k.Value).Take(15).Select(k => $"{k.Key} -> {k.Value}개");
        Debug.Log($"[INACTIVE-GI] 비활성 ContributeGI 렌더러={gi.Length} || 꺼진 부모: {string.Join("  ||  ", top)}");
    }

    static string Path2(Transform t)
    {
        string p = t.name;
        while (t.parent != null) { t = t.parent; p = t.name + "/" + p; }
        return p;
    }
}
