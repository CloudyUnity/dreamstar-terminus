using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

public static class CompileTest
{
    [MenuItem("Tools/CompileTest")]
    public static void CheckCompile()
    {
        Debug.Log(DateTime.Now);
        CompilationPipeline.RequestScriptCompilation();
        Debug.Log(DateTime.Now);
    }
}
