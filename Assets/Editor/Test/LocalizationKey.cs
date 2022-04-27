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
        [TestCase("{value}", "{value}", "hogehoge", "hogehoge")]
        [TestCase("{{value}:ordinal}", "{value}", "1", "1st")]
        [TestCase("{{value}:ordinal}", "{value}", "2", "2nd")]
        [TestCase("{{value}:ordinal}", "{value}", "3", "3rd")]
        [TestCase("{{value}:ordinal}", "{value}", "4", "4th")]
        [TestCase("{{value}:ordinal}", "{value}", "11", "11th")]
        [TestCase("{{value}:ordinal}", "{value}", "12", "12th")]
        [TestCase("{{value}:ordinal}", "{value}", "13", "13th")]
        [TestCase("{{value}:ordinal}", "{value}", "21", "21st")]
        [TestCase("{{value}:ordinal}", "{value}", "22", "22nd")]
        [TestCase("{{value}:ordinal}", "{value}", "23", "23rd")]
        [TestCase("fuga {{value}:ordinal} piyo", "{value}", "23", "fuga 23rd piyo")]
        [TestCase("{{value}:plural:an apple|_ apples}", "{value}", "1", "an apple")]
        [TestCase("{{value}:plural:an apple|_ apples}", "{value}", "461", "461 apples")]
        [TestCase("hoge {{value}:plural:an apple|_ apples} fuga", "{value}", "461", "hoge 461 apples fuga")]
        public void SmartReplace(string original, string key, string replacer, string expected)
        {
            var text = LocalizationKey.CreateRaw(original)
                .SmartReplace(key, LocalizationKey.CreateRaw(replacer));
            Assert.AreEqual(expected, text.Localize());
        }
    }
}

