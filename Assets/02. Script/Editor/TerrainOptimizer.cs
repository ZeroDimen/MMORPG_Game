using UnityEngine;
using UnityEditor;

public class TerrainOptimizer : EditorWindow
{
    // 최적화 수치 — 필요하면 창에서 바꿀 수 있게 필드로 둠
    private float pixelError = 10f;
    private bool drawInstanced = true;
    private float treeDistance = 300f;
    private float detailDistance = 80f;
    private float billboardStart = 50f;
    private UnityEngine.Rendering.ShadowCastingMode shadowMode
        = UnityEngine.Rendering.ShadowCastingMode.On; // On = OneSided

    [MenuItem("Tools/Terrain Optimizer")]
    public static void ShowWindow()
    {
        GetWindow<TerrainOptimizer>("Terrain Optimizer");
    }

    private void OnGUI()
    {
        GUILayout.Label("모든 Terrain에 적용할 값", EditorStyles.boldLabel);

        pixelError     = EditorGUILayout.FloatField("Pixel Error", pixelError);
        drawInstanced  = EditorGUILayout.Toggle("Draw Instanced", drawInstanced);
        treeDistance   = EditorGUILayout.FloatField("Tree Distance", treeDistance);
        detailDistance = EditorGUILayout.FloatField("Detail Distance", detailDistance);
        billboardStart = EditorGUILayout.FloatField("Billboard Start", billboardStart);
        shadowMode     = (UnityEngine.Rendering.ShadowCastingMode)
            EditorGUILayout.EnumPopup("Shadow Casting Mode", shadowMode);

        GUILayout.Space(10);

        if (GUILayout.Button("씬의 모든 Terrain에 적용"))
        {
            ApplyToAll();
        }
    }

    private void ApplyToAll()
    {
        // 씬에 있는 모든 Terrain 찾기 (비활성 포함하려면 Resources 방식 사용)
        Terrain[] terrains = FindObjectsByType<Terrain>(FindObjectsSortMode.None);

        if (terrains.Length == 0)
        {
            Debug.LogWarning("씬에서 Terrain을 찾지 못했습니다.");
            return;
        }

        int count = 0;
        foreach (var t in terrains)
        {
            // Undo 등록 — Ctrl+Z로 되돌릴 수 있게
            Undo.RecordObject(t, "Optimize Terrain");

            t.heightmapPixelError = pixelError;
            t.drawInstanced       = drawInstanced;
            t.treeDistance        = treeDistance;
            t.detailObjectDistance = detailDistance;
            t.treeBillboardDistance = billboardStart;
            t.shadowCastingMode   = shadowMode;

            EditorUtility.SetDirty(t); // 변경사항 저장되도록 표시
            count++;
        }

        Debug.Log($"[TerrainOptimizer] {count}개의 Terrain에 최적화 설정을 적용했습니다.");
    }
}