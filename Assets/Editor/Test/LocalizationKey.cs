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

        [Test]
        public void Fixed()
        {
            Assert.True(LocalizationKey.CreateRaw("hoge").Fixed);
            Assert.False(new LocalizationKey("hoge").Fixed);
            Assert.False(new LocalizationKey(0).Fixed);
            Assert.True(LocalizationKey.CreateRaw("hoge").Replace(("hoge", "piyo")).Fixed);
            Assert.False(LocalizationKey.CreateRaw("hoge").Replace(("hoge", new LocalizationKey("k"))).Fixed);
            Assert.True(LocalizationKey.CreateRaw("hoge").Replace(("hoge", LocalizationKey.CreateRaw("piyo"))).Fixed);
        }

        [Test]
        public void Concat()
        {
            var left = LocalizationKey.CreateRaw("hoge");
            var right = LocalizationKey.CreateRaw("fuga");
            var hogefuga = left.Concat(right);
            Assert.AreEqual("hogefuga", hogefuga.Localize());
        }

        [Test]
        public void Join()
        {
            var key = LocalizationKey.Join(
                LocalizationKey.CreateRaw(", "),
                LocalizationKey.CreateRaw("hoge"),
                LocalizationKey.CreateRaw("fuga"),
                LocalizationKey.CreateRaw("piyo"));
            Assert.AreEqual("hoge, fuga, piyo", key.Localize());
        }

        [Test]
        public void Select()
        {
            var hoge = LocalizationKey.CreateRaw("hoge");
            var hogepiyo = hoge.Select(_ => _ + "piyo");
            Assert.AreEqual("hogepiyo", hogepiyo.Localize());
            var fuga = hoge.Select((_lang, _) => _.Replace("hoge", "fuga"));
            Assert.AreEqual("fuga", fuga.Localize());
        }
    }
}

