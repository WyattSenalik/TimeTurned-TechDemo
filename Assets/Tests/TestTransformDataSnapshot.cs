using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace TimeTurned.Tests
{
    public class TestTransformDataSnapshot
    {
        // A Test behaves as an ordinary method
        [Test]
        public void TestTransformDataSnapshotSimplePasses()
        {
            // Use the Assert class to test conditions
        }


        [Test]
        public void TestTransformDataSnapshotConstructor1()
        {
            float time = 12.31f;
            Vector3 pos = new Vector3(-12.56f, 34.94f, 1.59f);
            Quaternion rot = Quaternion.Euler(-48.70f, 29.41f, -24.32f);
            Vector3 size = new Vector3(30.40f, 25.49f, 26.20f);

            TransformDataSnapshot snap1 = new TransformDataSnapshot(time,
                new TransformData(pos, rot, size));

            Assert.AreEqual(snap1.time, time);
            Assert.AreEqual(snap1.data.position, pos);
            Assert.AreEqual(snap1.data.rotation, rot);
            Assert.AreEqual(snap1.data.scale, size);
        }
        [Test]
        public void TestTransformDataSnapshotConstructor2()
        {
            float time = 19.94f;
            Vector3 pos = new Vector3(-49.90f, -37.72f, 40.70f);
            Quaternion rot = Quaternion.Euler(-12.37f, -44.00f, 13.18f);
            Vector3 size = new Vector3(14.00f, 3.86f, 25.24f);

            TransformDataSnapshot snap1 = new TransformDataSnapshot(time,
                pos, rot, size);

            Assert.AreEqual(snap1.time, time);
            Assert.AreEqual(snap1.data.position, pos);
            Assert.AreEqual(snap1.data.rotation, rot);
            Assert.AreEqual(snap1.data.scale, size);
        }
        [Test]
        public void TestTransformDataSnapshotConstructorSame()
        {
            Vector3 pos1 = new Vector3(1.4f, 9.7f, -4.0f);
            Quaternion rot1 = Quaternion.Euler(30.0f, -45.0f, 82.0f);
            Vector3 size1 = new Vector3(0.1f, 9.2f, 4.3f);

            TransformDataSnapshot snap1 = new TransformDataSnapshot(4.67f,
                new TransformData(pos1, rot1, size1));
            TransformDataSnapshot snap1Unwrapped = new TransformDataSnapshot(4.67f,
                pos1, rot1, size1);

            Assert.AreEqual(snap1.time, snap1Unwrapped.time);
            Assert.AreEqual(snap1.data.position, snap1Unwrapped.data.position);
            Assert.AreEqual(snap1.data.rotation, snap1Unwrapped.data.rotation);
            Assert.AreEqual(snap1.data.scale, snap1Unwrapped.data.scale);
        }
        [Test]
        public void TestTransformDataSnapshotInterpolate()
        {
            float time1 = 12.31f;
            Vector3 pos1 = new Vector3(-12.56f, 34.94f, 1.59f);
            Quaternion rot1 = Quaternion.Euler(-48.70f, 29.41f, -24.32f);
            Vector3 size1 = new Vector3(30.40f, 25.49f, 26.20f);

            float time2 = 19.94f;
            Vector3 pos2 = new Vector3(-49.90f, -37.72f, 40.70f);
            Quaternion rot2 = Quaternion.Euler(-12.37f, -44.00f, 13.18f);
            Vector3 size2 = new Vector3(14.00f, 3.86f, 25.24f);

            TransformDataSnapshot snap1 = new TransformDataSnapshot(time1,
                pos1, rot1, size1);
            TransformDataSnapshot snap2 = new TransformDataSnapshot(time2,
                pos2, rot2, size2);

            TransformDataSnapshot interpolated = snap1.Interpolate(snap2, time1);
            Assert.AreEqual(snap1.time, interpolated.time);
            Assert.AreEqual(snap1.data.position, interpolated.data.position);
            Assert.AreEqual(snap1.data.rotation, interpolated.data.rotation);
            Assert.AreEqual(snap1.data.scale, interpolated.data.scale);
        }
    }
}