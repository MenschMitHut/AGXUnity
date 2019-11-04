﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace AGXUnity
{
  public class DeformableTerrainShovelSettings : ScriptAsset
  {
    [SerializeField]
    private int m_numberOfTeeth = 6;
    [ClampAboveZeroInInspector]
    public int NumberOfTeeth
    {
      get { return m_numberOfTeeth; }
      set
      {
        m_numberOfTeeth = value;
        Propagate( shovel => shovel.setNumberOfTeeth( Convert.ToUInt64( m_numberOfTeeth ) ) );
      }
    }

    [SerializeField]
    private float m_toothLength = 0.15f;
    public float ToothLength
    {
      get { return m_toothLength; }
      set
      {
        m_toothLength = value;
        Propagate( shovel => shovel.setToothLength( m_toothLength ) );
      }
    }

    [SerializeField]
    private RangeReal m_toothRadius = new RangeReal( 0.015f, 0.075f );
    public RangeReal ToothRadius
    {
      get { return m_toothRadius; }
      set
      {
        m_toothRadius = value;
        Propagate( shovel =>
        {
          shovel.setToothMinimumRadius( m_toothRadius.Min );
          shovel.setToothMaximumRadius( m_toothRadius.Max );
        } );
      }
    }

    [SerializeField]
    private float m_noMergeExtensionDistance = 0.5f;
    [ClampAboveZeroInInspector( true )]
    public float NoMergeExtensionDistance
    {
      get { return m_noMergeExtensionDistance; }
      set
      {
        m_noMergeExtensionDistance = value;
        Propagate( shovel => shovel.setNoMergeExtensionDistance( m_noMergeExtensionDistance ) );
      }
    }

    [SerializeField]
    private float m_minimumSubmergedContactLengthFraction = 0.5f;
    [FloatSliderInInspector( 0.0f, 1.0f )]
    public float MinimumSubmergedContactLengthFraction
    {
      get { return m_minimumSubmergedContactLengthFraction; }
      set
      {
        m_minimumSubmergedContactLengthFraction = value;
        Propagate( shovel => shovel.setMinimumSubmergedContactLengthFraction( m_minimumSubmergedContactLengthFraction ) );
      }
    }

    [SerializeField]
    private float m_verticalBladeSoilMergeDistance = 0.0f;
    [ClampAboveZeroInInspector( true )]
    public float VerticalBladeSoilMergeDistance
    {
      get { return m_verticalBladeSoilMergeDistance; }
      set
      {
        m_verticalBladeSoilMergeDistance = value;
        Propagate( shovel => shovel.setVerticalBladeSoilMergeDistance( m_verticalBladeSoilMergeDistance ) );
      }
    }

    [SerializeField]
    private float m_secondarySeparationDeadloadLimit = 0.8f;
    [ClampAboveZeroInInspector( true )]
    public float SecondarySeparationDeadloadLimit
    {
      get { return m_secondarySeparationDeadloadLimit; }
      set
      {
        m_secondarySeparationDeadloadLimit = value;
        Propagate( shovel => shovel.setSecondarySeparationDeadloadLimit( m_secondarySeparationDeadloadLimit ) );
      }
    }

    [SerializeField]
    private float m_penetrationDepthThreshold = 0.2f;
    [ClampAboveZeroInInspector( true )]
    public float PenetrationDepthThreshold
    {
      get { return m_penetrationDepthThreshold; }
      set
      {
        m_penetrationDepthThreshold = value;
        Propagate( shovel => shovel.setPenetrationDepthThreshold( m_penetrationDepthThreshold ) );
      }
    }

    [SerializeField]
    private float m_penetrationForceScaling = 1.0f;
    [ClampAboveZeroInInspector( true )]
    public float PenetrationForceScaling
    {
      get { return m_penetrationForceScaling; }
      set
      {
        m_penetrationForceScaling = value;
        Propagate( shovel => shovel.setPenetrationForceScaling( m_penetrationForceScaling ) );
      }
    }

    [SerializeField]
    private float m_maxPenetrationForce = float.PositiveInfinity;
    [ClampAboveZeroInInspector( true )]
    public float MaxPenetrationForce
    {
      get { return m_maxPenetrationForce; }
      set
      {
        m_maxPenetrationForce = value;
        Propagate( shovel => shovel.setMaxPenetrationForce( m_maxPenetrationForce ) );
      }
    }

    [SerializeField]
    private bool m_removeContacts = false;
    public bool RemoveContacts
    {
      get { return m_removeContacts; }
      set
      {
        m_removeContacts = value;
        Propagate( shovel => shovel.setAlwaysRemoveShovelContacts( m_removeContacts ) );
      }
    }

    public static DeformableTerrainShovelSettings operator+( DeformableTerrainShovelSettings self,
                                                             DeformableTerrainShovel shovel )
    {
      self.m_shovelListener += shovel;

      // Currently synchronizing all current listeners. Could
      // be solved using 'context' state in m_shovelListener
      // if this is an issue.
      Utils.PropertySynchronizer.Synchronize( self );

      return self;
    }

    public static DeformableTerrainShovelSettings operator-( DeformableTerrainShovelSettings self,
                                                             DeformableTerrainShovel shovel )
    {
      self.m_shovelListener -= shovel;
      return self;
    }

    public override void Destroy()
    {
      m_shovelListener.OnDestroy();
    }

    protected override void Construct()
    {
      m_shovelListener = new ShovelListener();
    }

    protected override bool Initialize()
    {
      return true;
    }

    private void Propagate( Action<agxTerrain.Shovel> action )
    {
      if ( action == null || m_shovelListener == null )
        return;

      m_shovelListener.Enumerate( action );
    }

    private ShovelListener m_shovelListener = null;

    private class ShovelListener
    {
      public IEnumerable<agxTerrain.Shovel> Natives
      {
        get
        {
          foreach ( var shovel in m_shovels )
            if ( shovel.Native != null )
              yield return shovel.Native;
        }
      }

      public void Enumerate( Action<agxTerrain.Shovel> action )
      {
        foreach ( var native in Natives )
          action( native );
      }

      public void OnDestroy()
      {
        m_shovels.Clear();
      }

      private List<DeformableTerrainShovel> m_shovels = new List<DeformableTerrainShovel>();
      public static ShovelListener operator+( ShovelListener self, DeformableTerrainShovel shovel )
      {
        if ( !self.m_shovels.Contains( shovel ) )
          self.m_shovels.Add( shovel );
        return self;
      }
      public static ShovelListener operator-( ShovelListener self, DeformableTerrainShovel shovel )
      {
        self.m_shovels.Remove( shovel );
        return self;
      }
    }
  }
}
