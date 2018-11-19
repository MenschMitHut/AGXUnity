using AGXUnity.Utils;
using UnityEngine;

namespace AGXUnity.Collide
{
  /// <summary>
  /// Height field object to be used with Unity "Terrain".
  /// </summary>
  [AddComponentMenu( "AGXUnity/Shapes/HeightField" )]
  public sealed class HeightField : Shape
  {
    /// <summary>
    /// Returns the native height field object if created.
    /// </summary>
    public agxCollide.HeightField Native { get { return m_shape as agxCollide.HeightField; } }

    /// <summary>
    /// Debug rendering scale and debug rendering in general not supported.
    /// </summary>
    public override UnityEngine.Vector3 GetScale()
    {
      return new Vector3( 1, 1, 1 );
    }

    /// <summary>
    /// Shape offset, rotates native height field from z up to y up, flips x and z (?) and
    /// moves to center of the terrain (Unity Terrain has origin "lower corner").
    /// </summary>
    /// <returns>Shape transform to be used between native geometry and shape.</returns>
    public override agx.AffineMatrix4x4 GetNativeGeometryOffset()
    {
      return HeightFieldUtils.CalculateNativeOffset( transform, GetTerrainData() );
    }

    /// <summary>
    /// Overriding synchronization of native transform since the UnityEngine.Terrain
    /// by default is static and doesn't support rotation.
    /// 
    /// IF we want to synchronize we have to ignore rotation.
    /// </summary>
    protected override void SyncNativeTransform()
    {
    }

    /// <summary>
    /// Overriding synchronization of UnityEngine.Terrain transform since the UnityEngine.Terrain
    /// (and most often agxCollide.HeightField) is static by default.
    /// 
    /// IF we want to synchronize we have to ignore rotation.
    /// </summary>
    protected override void SyncUnityTransform()
    {
    }

    /// <summary>
    /// Finds and returns the Unity Terrain object. Searches on this
    /// component level and in all parents.
    /// </summary>
    /// <returns>Unity Terrain object, if found.</returns>
    private UnityEngine.Terrain GetTerrain()
    {
      return Find.FirstParentWithComponent<Terrain>( transform );
    }

    /// <summary>
    /// Finds Unity Terrain data given current setup.
    /// </summary>
    /// <returns>Unity TerrainData object, if found.</returns>
    private UnityEngine.TerrainData GetTerrainData()
    {
      Terrain terrain = GetTerrain();
      return terrain != null ? terrain.terrainData : null;
    }

    /// <returns>Width of the height field.</returns>
    private float GetWidth()
    {
      TerrainData data = GetTerrainData();
      return data != null ? data.size.x : 0.0f;
    }

    /// <returns>Global height reference.</returns>
    private float GetHeight()
    {
      TerrainData data = GetTerrainData();
      return data != null ? data.size.z : 0.0f;
    }

    /// <summary>
    /// Creates native height field object given current Unity Terrain
    /// object - if present (in component level or in parents).
    /// </summary>
    /// <returns>Native height field shape object.</returns>
    protected override agxCollide.Shape CreateNative()
    {
      var terrainData = GetTerrainData();
      if ( terrainData == null )
        return null;

      var heights = HeightFieldUtils.FindHeights( terrainData );
      var hf = new agxCollide.HeightField( (uint)heights.ResolutionX,
                                           (uint)heights.ResolutionY,
                                           GetWidth(),
                                           GetHeight(),
                                           heights.Heights,
                                           false,
                                           150.0 );
      return hf;
    }
  }
}
