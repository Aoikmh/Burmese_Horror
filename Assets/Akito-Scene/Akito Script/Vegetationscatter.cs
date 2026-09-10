using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Attach to an empty "Decorations" child under a road segment root.
/// Fill in your tree/bush prefabs, set the area size, then right-click
/// this component's header (or the gear icon) and choose "Scatter Now".
/// Re-run it any time to get a fresh random arrangement.
/// </summary>
public class VegetationScatter : MonoBehaviour
{
    [Header("Prefabs to scatter (mix a few types for variety)")]
    public GameObject[] prefabs;

    [Header("Area size (centered on this object)")]
    public float areaWidth = 20f;
    public float areaLength = 40f;

    [Tooltip("Empty strip left and right of Z-axis center kept clear so nothing spawns on the road itself.")]
    public float roadClearWidth = 6f;

    [Header("Amount")]
    public int count = 30;

    [Header("Variation")]
    public Vector2 scaleRange = new Vector2(0.8f, 1.3f);
    public bool randomizeYRotation = true;

    [ContextMenu("Scatter Now")]
    public void Scatter()
    {
        // Clear anything from a previous scatter first
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            DestroyImmediate(transform.GetChild(i).gameObject);
#else
            Destroy(transform.GetChild(i).gameObject);
#endif
        }

        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogWarning("[VegetationScatter] No prefabs assigned.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            float x = Random.Range(-areaWidth / 2f, areaWidth / 2f);

            // Push anything that landed inside the road's clear strip out to the nearest edge of it
            if (Mathf.Abs(x) < roadClearWidth / 2f)
            {
                x = (x < 0 ? -1f : 1f) * (roadClearWidth / 2f + Random.Range(0f, (areaWidth / 2f) - (roadClearWidth / 2f)));
            }

            float z = Random.Range(-areaLength / 2f, areaLength / 2f);

            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];

#if UNITY_EDITOR
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, transform);
#else
            GameObject instance = Instantiate(prefab, transform);
#endif
            instance.transform.localPosition = new Vector3(x, 0f, z);

            if (randomizeYRotation)
                instance.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            float scale = Random.Range(scaleRange.x, scaleRange.y);
            instance.transform.localScale = Vector3.one * scale;
        }
    }
}