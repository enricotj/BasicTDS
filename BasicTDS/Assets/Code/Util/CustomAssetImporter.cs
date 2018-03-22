using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

internal sealed class CustomAssetImporter : AssetPostprocessor
{
    public int spritePixelsPerUnit = 64;
    public FilterMode filterMode = FilterMode.Point;

    void OnPreprocessTexture()
    {
        TextureImporter textureImporter = (TextureImporter)assetImporter;
        //textureImporter.spritePixelsPerUnit = spritePixelsPerUnit; 
        textureImporter.filterMode = filterMode;
    }
    
}
#endif