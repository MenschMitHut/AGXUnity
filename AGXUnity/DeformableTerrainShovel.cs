using UnityEngine;

namespace AGXUnity
{
  [AddComponentMenu( "AGXUnity/Deformable Terrain Shovel" )]
  [DisallowMultipleComponent]
  [RequireComponent( typeof( RigidBody ) )]
  public class DeformableTerrainShovel : ScriptComponent
  {
    public agxTerrain.Shovel Native { get; private set; } = null;

    [SerializeField]
    private Edge m_cuttingEdge = new Edge();

    public Edge CuttingEdge
    {
      get { return m_cuttingEdge; }
      set { m_cuttingEdge = value; }
    }

    [SerializeField]
    private Edge m_topEdge = new Edge();

    public Edge TopEdge
    {
      get { return m_topEdge; }
      set { m_topEdge = value; }
    }
  }
}
