using System;
using UnityEngine;

namespace AGXUnity.Utils
{
  public static class HeightFieldUtils
  {
    public struct NativeHeights
    {
      public int ResolutionX;
      public int ResolutionY;
      public agx.RealVector Heights;
    }

    public static NativeHeights FindHeights( TerrainData data )
    {
      var result = new NativeHeights()
      {
        ResolutionX = data.heightmapWidth,
        ResolutionY = data.heightmapHeight,
        Heights     = new agx.RealVector( data.heightmapWidth * data.heightmapHeight )
      };
      Vector3 scale = data.heightmapScale;
      float[,] heights = data.GetHeights( 0, 0, result.ResolutionX, result.ResolutionY );

      for ( int x = result.ResolutionX - 1; x >= 0; --x )
        for ( int y = result.ResolutionY - 1; y >= 0; --y )
          result.Heights.Add( heights[ x, y ] * scale.y );

      return result;
    }

    public static agx.AffineMatrix4x4 CalculateNativeOffset( Transform transform, TerrainData data )
    {
      if ( transform == null || data == null )
        return new agx.AffineMatrix4x4();

      return agx.AffineMatrix4x4.rotate( agx.Vec3.Z_AXIS(),
                                         agx.Vec3.Y_AXIS() ) *
             agx.AffineMatrix4x4.translate( transform.position.ToHandedVec3() +
                                            new Vector3( 0.5f * data.size.x,
                                                         0,
                                                         0.5f * data.size.z ).ToHandedVec3() );
    }
  }
}
