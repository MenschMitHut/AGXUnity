using System;
using UnityEngine;
using AGXUnity.Utils;

namespace AGXUnity
{
  [AddComponentMenu( "AGXUnity/Deformable Terrain" )]
  [DisallowMultipleComponent]
  public class DeformableTerrain : ScriptComponent
  {
    private Terrain m_terrain = null;

    public Terrain Terrain
    {
      get
      {
        if ( m_terrain == null )
          m_terrain = GetComponent<Terrain>();
        return m_terrain;
      }
    }

    [HideInInspector]
    public TerrainData TerrainData
    {
      get { return Terrain?.terrainData; }
    }

    public agxTerrain.Terrain Native { get; private set; } = null;

    protected override bool Initialize()
    {
      if ( TerrainData == null ) {
        Debug.LogWarning( "Unable to find Terrain component in game object.", gameObject );
        return false;
      }

      var heights = HeightFieldUtils.FindHeights( TerrainData );
      var elementSize = TerrainData.size.x / Convert.ToSingle( heights.ResolutionX );
      Native = new agxTerrain.Terrain( (uint)heights.ResolutionX,
                                       (uint)heights.ResolutionY,
                                       elementSize,
                                       heights.Heights,
                                       false,
                                       20.0f );
      Native.setTransform( HeightFieldUtils.CalculateNativeOffset( transform, TerrainData ) );

      GetSimulation().add( Native );

      return true;
    }

    protected override void OnDestroy()
    {
      if ( GetSimulation() != null )
        GetSimulation().remove( Native );
    }
  }
}
