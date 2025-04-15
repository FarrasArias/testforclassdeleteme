using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ExamplePlayModeTest
{
    //[UnityTest]: For writing Play Mode tests that can span multiple frames.
    //PlayMode vs.EditMode Tests: UTF distinguishes between tests that run in the Editor environment
    //(Edit Mode) and tests that run in a simulated game environment (Play Mode).

    [UnityEngine.TestTools.UnityTest]      // fully qualified
    public IEnumerator Always_Passes_After_One_Frame()
    {
        yield return null;
        Assert.IsTrue(true);
    }
}
