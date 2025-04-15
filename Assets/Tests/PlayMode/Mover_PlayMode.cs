using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class Mover_PlayMode
{
    [UnityTest]
    public IEnumerator ObjectMovesForwardAfterOneSecond()
    {
        // 1. Set‑up: an empty scene with one cube + Mover
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var mover = go.AddComponent<Mover>();
        Vector3 start = go.transform.position;

        // 2. Act: simulate one second of game time
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            mover.Move(Vector3.forward);
            elapsed += Time.deltaTime;
            yield return null;            // wait one frame
        }

        // 3. Assert
        Assert.Greater(go.transform.position.z, start.z,
            "Cube should have moved forward in Z after one second");
    }
}
