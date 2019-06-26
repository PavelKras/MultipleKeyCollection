using System;
using System.Collections.Generic;
using DoubleKeyCollection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoubleKeyCollectionTest
{
    [TestClass]
    public class DoubleKeyCollectionTest
    {
        [TestMethod]
        public void DoubleKeyCollectionTest1()
        {
            var col = new DoubleKeyCollection<UserType, UserType, string>();
            var I = 100000;
            var J = 10;


            for (int i = 0; i < I; i++)
            {
                for (int j = 0; j < J; j++)
                {
                    col.Add(new UserType(i), new UserType(j), $"{i}_{j}");
                }
            }

            var r = new Random();
            var i1 = r.Next(I);
            var j1 = r.Next(J);
            Assert.IsTrue(col.Count == I * J);
            Assert.IsTrue(col.Contains($"{i1}_{j1}"));
            var col90000 = col.GetById(new UserType(i1));
            Assert.IsTrue(col90000.Count == J);
            Assert.IsTrue(col90000.Contains($"{i1}_{j1}"));
            Assert.ThrowsException<ArgumentException>(() => col.Add(new UserType(i1), new UserType(j1), ""));
            Assert.IsFalse(col.Contains(""));
            Assert.IsTrue(col.Remove(new UserType(i1), new UserType(j1)));
            Assert.IsTrue(col.Count == I * J - 1);
            col.Add(new UserType(i1), new UserType(j1), "");
            Assert.IsTrue(col.Count == I * J);
            Assert.IsTrue(col.Contains(""));

            var count = 0;
            foreach (var p in col)
            {
                if (count == i1 * J + j1)
                {
                    Assert.IsTrue(p.Key.Item1.Index == i1);
                    Assert.IsTrue(p.Key.Item2.Index == j1);
                    Assert.IsTrue(p.Value == "");
                }
                count++;
            }
            Assert.IsTrue(count == col.Count);

        }
    }

    internal class UserType
    {
        public int Index;

        public UserType(int i) => Index = i;

        public override bool Equals(object obj)
        {
            var o = obj as UserType;
            if (o == null)
                return false;
            return Index == o.Index;
        }

        public override int GetHashCode() => Index;

        public override string ToString() => Index.ToString();
    }
}
