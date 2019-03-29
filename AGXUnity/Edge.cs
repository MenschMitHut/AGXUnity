using System;
using UnityEngine;

namespace AGXUnity
{
  [Serializable]
  public class Edge
  {
    [SerializeField]
    private IFrame m_start = new IFrame( null );

    public IFrame Start
    {
      get { return m_start; }
      set { m_start = value; }
    }

    [SerializeField]
    private IFrame m_end = new IFrame( null );

    public IFrame End
    {
      get { return m_end; }
      set { m_end = value; }
    }
  }
}
