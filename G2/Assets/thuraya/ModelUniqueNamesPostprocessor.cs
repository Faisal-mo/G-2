using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class ModelUniqueNamesPostprocessor : AssetPostprocessor
{
    void OnPostprocessModel(GameObject go)
    {
        string prefix = Path.GetFileNameWithoutExtension(assetPath);

        var seenGo = new HashSet<string>();
        var seenMesh = new HashSet<string>();

        foreach (var t in go.GetComponentsInChildren<Transform>(true))
        {
            string newName = $"{prefix}_{t.name}";
            if (!seenGo.Add(newName))
                newName = $"{newName}_{t.GetInstanceID()}";
            t.name = newName;
        }

        foreach (var mf in go.GetComponentsInChildren<MeshFilter>(true))
        {
            var m = mf.sharedMesh;
            if (!m) continue;
            string newName = $"{prefix}_{m.name}";
            if (!seenMesh.Add(newName))
                newName = $"{newName}_{m.GetInstanceID()}";
            m.name = newName;
        }

        foreach (var smr in go.GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            var m = smr.sharedMesh;
            if (!m) continue;
            string newName = $"{prefix}_{m.name}";
            if (!seenMesh.Add(newName))
                newName = $"{newName}_{m.GetInstanceID()}";
            m.name = newName;
        }
    }
}
