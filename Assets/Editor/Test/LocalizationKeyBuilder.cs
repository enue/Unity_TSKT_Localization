using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using R3;

namespace TSKT.Tests
{
    public class LocalizationKeyBuilderTest
    {
        [Test]
        public void CompareStringBuilder()
        {
            var builder = new LocalizationKeyBuilder();
            builder.AppendLine();
            builder.Append(LocalizationKey.CreateRaw("hoge"));
            builder.AppendLine(LocalizationKey.CreateRaw("fuga"));

            var stringBuilder = new System.Text.StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.Append("hoge");
            stringBuilder.AppendLine("fuga");

            Assert.AreEqual(stringBuilder.ToString(), builder.ToLocalizationKey().Localize());
        }

        [Test]
        public void Combine()
        {
            var builder = new LocalizationKeyBuilder();
            builder.Append(LocalizationKey.CreateRaw("a"));
            var source = new ReactiveProperty<string>("b");
            builder.Append(new LocalizationKey(source));
            builder.Append(LocalizationKey.CreateRaw("c"));

            Assert.AreEqual("abc", builder.ToString());
            source.Value = "z";
            Assert.AreEqual("azc", builder.ToString());
        }
    }
}

