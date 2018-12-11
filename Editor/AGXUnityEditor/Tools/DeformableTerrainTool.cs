using System;
using UnityEngine;
using UnityEditor;
using AGXUnity;

namespace AGXUnityEditor.Tools
{
  [CustomTool( typeof( DeformableTerrain ) )]
  public class DeformableTerrainTool : Tool
  {
    public DeformableTerrain DeformableTerrain { get; private set; }

    public DeformableTerrainTool( DeformableTerrain deformableTerrain )
    {
      DeformableTerrain = deformableTerrain;
    }
  }
}
