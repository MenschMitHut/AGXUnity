using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;

using Debug = UnityEngine.Debug;

namespace AGXUnityScript.Editor
{
  [InitializeOnLoad]
  public static class VerifyCheckout
  {
    static VerifyCheckout()
    {
      Debug.Log( "Well hello there!" );
    }
  }
}
