using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

namespace Test
{
    public class GraphTest
    {
        [Test]
        public void NodeConnectTest()
        {
            Node a = new Node();
            Node b = new Node();

            a.ConnectTo(b);

            foreach(var e in a.OutEdges)
            {
                Assert.AreEqual(b, e.node);
            }
        }

        [Test]
        public void NodeNoDuplicationTest()
        {
            Node a = new Node();
            Node b = new Node();
            a.ConnectTo(b);
            a.ConnectTo(b);

            int count = 0;

            foreach(var e in a.OutEdges)
            {
                count++;
            }
            Assert.AreEqual(count, 1);
        }
    }
}
