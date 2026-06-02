using UnityEditor;
using UnityEngine;

public static class PuddlesDashStatusParticlesPrefabSetup
{
    private const string PrefabPath = "Assets/Prefabs/Puddles.prefab";
    private const string MaterialPath = "Assets/Materials/PuddlesDashStatusParticles.mat";

    [MenuItem("Tools/Puddles/Create Editable Dash Status Particles")]
    public static void ApplyFromMenu()
    {
        Apply();
    }

    public static void Apply()
    {
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(PrefabPath);

        try
        {
            PlayerMovement movement = prefabRoot.GetComponent<PlayerMovement>();

            if (movement == null)
            {
                movement = prefabRoot.GetComponentInChildren<PlayerMovement>();
            }

            if (movement == null)
            {
                Debug.LogError("Could not find PlayerMovement on Puddles prefab.");
                return;
            }

            Transform particleTransform = prefabRoot.transform.Find("DashStatusParticles");
            GameObject particleObject = particleTransform != null
                ? particleTransform.gameObject
                : new GameObject("DashStatusParticles");

            particleObject.transform.SetParent(prefabRoot.transform, false);
            particleObject.transform.localPosition = movement.dashStatusParticlesOffset;
            particleObject.transform.localRotation = Quaternion.identity;
            particleObject.transform.localScale = Vector3.one;

            ParticleSystem particles = particleObject.GetComponent<ParticleSystem>();

            if (particles == null)
            {
                particles = particleObject.AddComponent<ParticleSystem>();
            }

            Material material = GetOrCreateParticleMaterial();
            ConfigureParticles(particles, movement.dashReadyColor, material);

            movement.dashStatusParticleMaterial = material;
            movement.dashStatusParticles = particles;
            movement.createDashStatusParticles = false;

            EditorUtility.SetDirty(particleObject);
            EditorUtility.SetDirty(particles);
            EditorUtility.SetDirty(material);
            EditorUtility.SetDirty(movement);

            PrefabUtility.SaveAsPrefabAsset(prefabRoot, PrefabPath);
            Debug.Log("Saved DashStatusParticles child on Puddles prefab.");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }
    }

    private static Material GetOrCreateParticleMaterial()
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);

        if (material == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");

            if (shader == null)
            {
                shader = Shader.Find("Particles/Standard Unlit");
            }

            if (shader == null)
            {
                shader = Shader.Find("Sprites/Default");
            }

            material = new Material(shader)
            {
                name = "PuddlesDashStatusParticles"
            };

            AssetDatabase.CreateAsset(material, MaterialPath);
        }

        SetMaterialColor(material, Color.white);
        return material;
    }

    private static void SetMaterialColor(Material material, Color color)
    {
        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }

        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
        }

        if (material.HasProperty("_TintColor"))
        {
            material.SetColor("_TintColor", color);
        }
    }

    private static void ConfigureParticles(ParticleSystem particles, Color color, Material material)
    {
        ParticleSystem.MainModule main = particles.main;
        main.loop = true;
        main.playOnAwake = true;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.8f, 1.35f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.08f, 0.28f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.22f, 0.48f);
        main.startColor = color;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 80;

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.enabled = true;
        emission.rateOverTime = 14f;

        ParticleSystem.ShapeModule shape = particles.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.25f;
        shape.radiusThickness = 1f;

        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particles.colorOverLifetime;
        colorOverLifetime.enabled = true;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(color, 0f),
                new GradientColorKey(color, 1f)
            },
            new[]
            {
                new GradientAlphaKey(0.25f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = gradient;

        ParticleSystemRenderer renderer = particles.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.sortingOrder = 10;
        renderer.sharedMaterial = material;
    }
}
