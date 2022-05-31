using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace TimeTurned.Tests
{
    public class TestSnapshotScrapbook
    {
        private static readonly TransformDataSnapshot[] SNAPS =
        {
            new TransformDataSnapshot(0.0f, new Vector3(0.0f, 0.0f, 0.0f), 
                Quaternion.identity, Vector3.one),

            new TransformDataSnapshot(1.0f, new Vector3(1.0f, 1.0f, 1.0f),
                Quaternion.identity, Vector3.one),

            new TransformDataSnapshot(2.0f, new Vector3(-3.0f, 0.0f, 1.0f),
                Quaternion.identity, Vector3.one),

            new TransformDataSnapshot(3.0f, new Vector3(-8.0f, -4.0f, 5.0f),
                Quaternion.identity, Vector3.one),

            new TransformDataSnapshot(4.0f, new Vector3(0.0f, -7.0f, -2.0f),
                Quaternion.identity, Vector3.one),

            new TransformDataSnapshot(5.0f, new Vector3(5.0f, 3.0f, 10.0f),
                Quaternion.identity, Vector3.one),
        };

        [Test]
        public void TestGetSnapshotTransformDataSnapshotPositions()
        {
            SnapshotScrapbook<TransformDataSnapshot, TransformData> scrapbook = 
                new SnapshotScrapbook<TransformDataSnapshot, TransformData>(SNAPS);

            // 0.0f, new Vector3(0.0f, 0.0f, 0.0f)
            TestSingleSnapshotPos(scrapbook, 0.0f, new Vector3(0.0f, 0.0f, 0.0f));
            // 1.0f, new Vector3(1.0f, 1.0f, 1.0f)
            TestSingleSnapshotPos(scrapbook, 1.0f, new Vector3(1.0f, 1.0f, 1.0f));
            // 2.0f, new Vector3(-3.0f, 0.0f, 1.0f)
            TestSingleSnapshotPos(scrapbook, 2.0f, new Vector3(-3.0f, 0.0f, 1.0f));
            // 3.0f, new Vector3(-8.0f, -4.0f, 5.0f)
            TestSingleSnapshotPos(scrapbook, 3.0f, new Vector3(-8.0f, -4.0f, 5.0f));
            // 4.0f, new Vector3(0.0f, -7.0f, -2.0f)
            TestSingleSnapshotPos(scrapbook, 4.0f, new Vector3(0.0f, -7.0f, -2.0f));
            // 5.0f, new Vector3(5.0f, 3.0f, 10.0f)
            TestSingleSnapshotPos(scrapbook, 5.0f, new Vector3(5.0f, 3.0f, 10.0f));
        }


        private void TestSingleSnapshotPos(
            SnapshotScrapbook<TransformDataSnapshot, TransformData> scrapbook,
            float time, Vector3 expectedPos)
        {
            TransformDataSnapshot snap = scrapbook.GetSnapshot(time);
            Assert.AreEqual(snap.time, time);
            ExtraAsserts.AreClose(snap.data.position, expectedPos);
        }
    }
}
