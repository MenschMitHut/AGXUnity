using System;
using System.Linq;
using System.Collections.Generic;
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

    [SerializeField]
    private List<DeformableTerrainShovel> m_shovels = new List<DeformableTerrainShovel>();

    [HideInInspector]
    public DeformableTerrainShovel[] Shovels { get{ return m_shovels.ToArray(); } }

    [SerializeField]
    private RigidBody m_shovel = null;

    public RigidBody Shovel
    {
      get { return m_shovel; }
      set { m_shovel = value; }
    }

    [HideInInspector]
    public TerrainData TerrainData
    {
      get { return Terrain?.terrainData; }
    }

    public agxTerrain.Terrain Native { get; private set; } = null;

    public bool Add( DeformableTerrainShovel shovel )
    {
      if ( shovel == null || m_shovels.Contains( shovel ) )
        return false;

      m_shovels.Add( shovel );

      return true;
    }

    public bool Remove( DeformableTerrainShovel shovel )
    {
      if ( shovel == null )
        return false;

      return m_shovels.Remove( shovel );
    }

    protected override bool Initialize()
    {
      if ( TerrainData == null ) {
        Debug.LogWarning( "Unable to find Terrain component in game object.", gameObject );
        return false;
      }

      var heights = HeightFieldUtils.FindHeights( TerrainData );
      var elementSize = TerrainData.size.x / Convert.ToSingle( heights.ResolutionX - 1 );
      Native = new agxTerrain.Terrain( (uint)heights.ResolutionX,
                                       (uint)heights.ResolutionY,
                                       elementSize,
                                       heights.Heights,
                                       false,
                                       20.0f );
      Native.setTransform( HeightFieldUtils.CalculateNativeOffset( transform, TerrainData ) );
      Native.loadLibraryMaterial( agxTerrain.Terrain.MaterialLibrary.GRAVEL_1 );

      GetSimulation().add( Native );

      if ( Shovel?.GetInitialized<RigidBody>() != null ) {
        var cuttingStart = new agx.Vec3( 0.3898726, -1.854617, 2.233677 );
        var cuttingEnd = new agx.Vec3( 0.3898726, -1.854617, -2.233677 );

        var topEdgeStart = new agx.Vec3( 0.06162071, 0.6465979, 2.233677 );
        var topEdgeEnd = new agx.Vec3( 0.06162071, 0.6465979, -2.233677 );

        m_nativeShovel = new agxTerrain.Shovel( Shovel.Native,
                                                new agx.Edge( topEdgeStart, topEdgeEnd ),
                                                new agx.Edge( cuttingStart, cuttingEnd ) );
        Native.add( m_nativeShovel );
        m_nativeShovel.setVerticalBladeSoilMergeDistance( 0.5 );
      }

      Simulation.Instance.StepCallbacks.PostStepForward += OnPostStep;

      m_initialHeights = TerrainData.GetHeights( 0, 0, TerrainData.heightmapWidth, TerrainData.heightmapHeight );

      return true;
    }

    protected override void OnDestroy()
    {
      TerrainData.SetHeights( 0, 0, m_initialHeights );

      if ( GetSimulation() != null ) {
        GetSimulation().remove( Native );
        Simulation.Instance.StepCallbacks.PostStepForward -= OnPostStep;
      }
      Native = null;
      m_nativeShovel = null;
    }

    private void OnPostStep()
    {
      if ( Native == null )
        return;

      UpdateHeights( Native.getModifiedVertices() );
    }

    private void UpdateHeights( agxTerrain.ModifiedVerticesVector modifiedVertices )
    {
      if ( modifiedVertices.Count == 0 )
        return;

      var scale = TerrainData.heightmapScale.y;
      var resX = TerrainData.heightmapWidth;
      var resY = TerrainData.heightmapHeight;
      var result = new float[,] { { 0.0f } };

      foreach ( var index in modifiedVertices ) {
        var i = (int)index.x;
        var j = (int)index.y;
        var h = (float)Native.getHeight( index );

        result[ 0, 0 ] = h / scale;

        TerrainData.SetHeightsDelayLOD( resX - i - 1, resY - j - 1, result );
      }

      Terrain.ApplyDelayedHeightmapModification();
    }

    private Mesh m_innerShapeMesh = null;
    private Mesh m_fractureShapeMesh = null;

    private Mesh CreateMesh( agxCollide.Shape shape )
    {
      if ( Shovel == null || m_nativeShovel == null )
        return null;

      var fractureMeshData = shape?.asMesh()?.getMeshData();
      var meshToLocal = Shovel.transform.worldToLocalMatrix;
      if ( fractureMeshData != null ) {
        var nativeToWorld = shape.getTransform();

        var source = new Mesh();
        source.SetVertices( ( from v
                              in fractureMeshData.getVertices()
                              select meshToLocal.MultiplyPoint3x4( nativeToWorld.preMult( v ).ToHandedVector3() ) ).ToList() );

        var triangles = new List<int>();
        var indexArray = fractureMeshData.getIndices();
        triangles.Capacity = indexArray.Count;
        for ( int i = 0; i < indexArray.Count; i += 3 ) {
          triangles.Add( Convert.ToInt32( indexArray[ i + 0 ] ) );
          triangles.Add( Convert.ToInt32( indexArray[ i + 2 ] ) );
          triangles.Add( Convert.ToInt32( indexArray[ i + 1 ] ) );
        }
        source.SetTriangles( triangles, 0, false );

        source.RecalculateBounds();
        source.RecalculateNormals();
        source.RecalculateTangents();

        return source;
      }

      return null;
    }

    private void OnDrawGizmos()
    {
      if ( m_nativeShovel == null )
        return;

      var collection = Native.getToolCollection( m_nativeShovel );

      Gizmos.color = Color.red;
      Gizmos.DrawLine( m_nativeShovel.getCuttingEdgeWorld().p1.ToHandedVector3(),
                       m_nativeShovel.getCuttingEdgeWorld().p2.ToHandedVector3() );
      Gizmos.color = Color.green;
      Gizmos.DrawLine( m_nativeShovel.getTopEdgeWorld().p1.ToHandedVector3(),
                       m_nativeShovel.getTopEdgeWorld().p2.ToHandedVector3() );
      var topEdgeCenter = 0.5f * ( m_nativeShovel.getTopEdgeWorld().p1 + m_nativeShovel.getTopEdgeWorld().p2 ).ToHandedVector3();
      Gizmos.DrawLine( topEdgeCenter,
                       topEdgeCenter + collection.getActiveZone().getForwardVectorInWorld().ToHandedVector3() );

      if ( m_fractureShapeMesh == null )
        m_fractureShapeMesh = CreateMesh( collection.getActiveZone().getFractureShape() );
      if ( m_innerShapeMesh == null )
        m_innerShapeMesh = CreateMesh( collection.getActiveZone().getInnerShape() );

      if ( m_fractureShapeMesh != null ) {
        Gizmos.color = Color.green;
        Gizmos.DrawWireMesh( m_fractureShapeMesh, Shovel.transform.position, Shovel.transform.rotation );
      }
      if ( m_innerShapeMesh != null ) {
        Gizmos.color = Color.red;
        Gizmos.DrawWireMesh( m_innerShapeMesh, Shovel.transform.position, Shovel.transform.rotation );
      }
    }

    private float[,] m_initialHeights = null;
    private agxTerrain.Shovel m_nativeShovel = null;
  }
}
