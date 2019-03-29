using System;
using UnityEngine;
using UnityEditor;
using AGXUnity;
using AGXUnityEditor.Utils;

using GUI = AGXUnityEditor.Utils.GUI;

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

    public override void OnPreTargetMembersGUI( GUISkin skin )
    {
      //if ( GUI.Foldout( EditorData.Instance.GetData( DeformableTerrain,
      //                                               "Shovels",
      //                                               entry => entry.Bool = true ),
      //                  GUI.MakeLabel( "Shovels", true ),
      //                  skin ) ) {
      //  DeformableTerrainShovel shovelToRemove = null;
      //  foreach ( var shovel in DeformableTerrain.Shovels ) {
      //    using ( new GUI.Indent( 12 ) ) {
      //      if ( GUI.Foldout( EditorData.Instance.GetData( shovel, "ShovelEntry", entry => entry.Bool = false ),
      //                        GUI.MakeLabel( shovel.name ),
      //                        skin ) ) {
      //        using ( new GUI.ColorBlock( Color.Lerp( UnityEngine.GUI.color, Color.red, 0.1f ) ) ) {
      //          var removeShovel = GUILayout.Button( GUI.MakeLabel( GUI.Symbols.ListEraseElement.ToString(),
      //                                                              16,
      //                                                             false,
      //                                                             "Remove this shovel" ),
      //                                              skin.button,
      //                                              GUILayout.Width( 20 ),
      //                                              GUILayout.Height( 16 ) );
      //          if ( removeShovel )
      //            shovelToRemove = shovel;
      //        }
      //      }
      //    }
      //  }
      //}
    }
  }
}
