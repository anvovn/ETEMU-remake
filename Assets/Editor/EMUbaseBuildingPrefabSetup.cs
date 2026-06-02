using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EMUbaseBuildingPrefabSetup
{
    private const string PrefabPath = "Assets/Prefabs/EMUbase.prefab";
    private const string DetailRootName = "BuildingDetails";

    [MenuItem("Tools/EMUbase/Build Exterior Details")]
    public static void ApplyFromMenu()
    {
        Apply();
    }

    public static void Apply()
    {
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(PrefabPath);

        try
        {
            Transform existingDetails = prefabRoot.transform.Find(DetailRootName);

            if (existingDetails != null)
            {
                Object.DestroyImmediate(existingDetails.gameObject);
            }

            Materials materials = CreateMaterials();
            Transform detailsRoot = new GameObject(DetailRootName).transform;
            detailsRoot.SetParent(prefabRoot.transform, false);
            detailsRoot.localPosition = Vector3.zero;
            detailsRoot.localRotation = Quaternion.identity;
            detailsRoot.localScale = Vector3.one;

            AddMainEntrance(detailsRoot, materials);
            AddWindowRows(detailsRoot, materials);
            AddRoofTrim(detailsRoot, materials);
            AddCornerColumns(detailsRoot, materials);

            PrefabUtility.SaveAsPrefabAsset(prefabRoot, PrefabPath);
            Debug.Log("Saved EMUbase building exterior details.");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }
    }

    private static Materials CreateMaterials()
    {
        return new Materials
        {
            Window = GetOrCreateMaterial("Assets/Materials/EMUbaseWindow.mat", new Color(0.08f, 0.24f, 0.36f), 0.25f),
            Door = GetOrCreateMaterial("Assets/Materials/EMUbaseDoor.mat", new Color(0.16f, 0.1f, 0.06f), 0f),
            Trim = GetOrCreateMaterial("Assets/Materials/EMUbaseTrim.mat", new Color(0.72f, 0.72f, 0.66f), 0f),
            Sign = GetOrCreateMaterial("Assets/Materials/EMUbaseSign.mat", new Color(0.02f, 0.34f, 0.18f), 0f)
        };
    }

    private static Material GetOrCreateMaterial(string path, Color color, float metallic)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

        if (material == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");

            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            material = new Material(shader)
            {
                name = System.IO.Path.GetFileNameWithoutExtension(path)
            };

            AssetDatabase.CreateAsset(material, path);
        }

        SetColor(material, color);

        if (material.HasProperty("_Metallic"))
        {
            material.SetFloat("_Metallic", metallic);
        }

        if (material.HasProperty("_Smoothness"))
        {
            material.SetFloat("_Smoothness", 0.55f);
        }

        EditorUtility.SetDirty(material);
        return material;
    }

    private static void SetColor(Material material, Color color)
    {
        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }

        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
        }
    }

    private static void AddMainEntrance(Transform parent, Materials materials)
    {
        AddCube(parent, "FrontEntranceDoor", new Vector3(0f, -1.2f, -176.5f), new Vector3(8f, 0.42f, 0.25f), materials.Door);
        AddCube(parent, "FrontEntranceFrameTop", new Vector3(0f, -0.9f, -176.7f), new Vector3(11f, 0.08f, 0.35f), materials.Trim);
        AddCube(parent, "FrontEntranceFrameLeft", new Vector3(-5.8f, -1.18f, -176.7f), new Vector3(0.45f, 0.48f, 0.35f), materials.Trim);
        AddCube(parent, "FrontEntranceFrameRight", new Vector3(5.8f, -1.18f, -176.7f), new Vector3(0.45f, 0.48f, 0.35f), materials.Trim);
        AddCube(parent, "EMUbaseSign", new Vector3(0f, -0.55f, -176.75f), new Vector3(18f, 0.16f, 0.3f), materials.Sign);
    }

    private static void AddWindowRows(Transform parent, Materials materials)
    {
        float[] levels = { -0.65f, -0.15f, 0.35f, 0.85f };
        float[] frontXs = { -10f, 10f };

        foreach (float y in levels)
        {
            foreach (float x in frontXs)
            {
                AddCube(parent, $"FrontWindow_{x}_{y}", new Vector3(x, y, -176.55f), new Vector3(5.2f, 0.14f, 0.18f), materials.Window);
            }
        }

        AddSideWindowStrip(parent, materials.Window, 14.75f, true, new[] { -145f, -105f, -65f, -25f, 15f, 55f, 95f });
        AddSideWindowStrip(parent, materials.Window, -14.75f, true, new[] { -145f, -105f, -65f, -25f, 15f, 55f, 95f });
        AddSideWindowStrip(parent, materials.Window, 105.2f, false, new[] { -175f, -130f, -85f, -40f });
        AddSideWindowStrip(parent, materials.Window, -157.8f, false, new[] { -80f, -35f, 10f, 55f });
    }

    private static void AddSideWindowStrip(Transform parent, Material material, float fixedCoordinate, bool onXFace, IEnumerable<float> runCoordinates)
    {
        float[] levels = { -0.55f, 0.05f, 0.65f };

        foreach (float y in levels)
        {
            foreach (float coordinate in runCoordinates)
            {
                Vector3 position = onXFace
                    ? new Vector3(fixedCoordinate, y, coordinate)
                    : new Vector3(coordinate, y, fixedCoordinate);
                Vector3 scale = onXFace
                    ? new Vector3(0.18f, 0.13f, 7f)
                    : new Vector3(7f, 0.13f, 0.18f);

                AddCube(parent, $"SideWindow_{fixedCoordinate}_{coordinate}_{y}", position, scale, material);
            }
        }
    }

    private static void AddRoofTrim(Transform parent, Materials materials)
    {
        AddCube(parent, "FrontRoofTrim", new Vector3(0f, 1.54f, -176.55f), new Vector3(31f, 0.08f, 0.35f), materials.Trim);
        AddCube(parent, "BackRoofTrim", new Vector3(0f, 1.54f, 129.5f), new Vector3(31f, 0.08f, 0.35f), materials.Trim);
        AddCube(parent, "LeftRoofTrim", new Vector3(-14.75f, 1.54f, -23.4f), new Vector3(0.35f, 0.08f, 306f), materials.Trim);
        AddCube(parent, "RightRoofTrim", new Vector3(14.75f, 1.54f, -23.4f), new Vector3(0.35f, 0.08f, 306f), materials.Trim);
    }

    private static void AddCornerColumns(Transform parent, Materials materials)
    {
        Vector3[] positions =
        {
            new Vector3(-14.9f, 0f, -176.7f),
            new Vector3(14.9f, 0f, -176.7f),
            new Vector3(-14.9f, 0f, 129.6f),
            new Vector3(14.9f, 0f, 129.6f)
        };

        foreach (Vector3 position in positions)
        {
            AddCube(parent, $"CornerColumn_{position.x}_{position.z}", position, new Vector3(0.65f, 2.9f, 0.65f), materials.Trim);
        }
    }

    private static void AddCube(Transform parent, string name, Vector3 localPosition, Vector3 localScale, Material material)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(parent, false);
        cube.transform.localPosition = localPosition;
        cube.transform.localRotation = Quaternion.identity;
        cube.transform.localScale = localScale;

        Object.DestroyImmediate(cube.GetComponent<BoxCollider>());

        MeshRenderer renderer = cube.GetComponent<MeshRenderer>();
        renderer.sharedMaterial = material;
    }

    private struct Materials
    {
        public Material Window;
        public Material Door;
        public Material Trim;
        public Material Sign;
    }
}
