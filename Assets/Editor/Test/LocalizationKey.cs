using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;

namespace TSKT.Tests
{
    public class LocalizationKeyTest
    {
        [Test]
        public void Replace()
        {
            var hoge = LocalizationKey.CreateRaw("hogehoge");
            var fuga = hoge.Replace(("hoge", "fuga"));
            var piyo = fuga.Replace(("fuga", "piyo"));

            Assert.AreEqual("fugafuga", fuga.Localize());
            Assert.AreEqual("piyopiyo", piyo.Localize());
        }
    }
}

