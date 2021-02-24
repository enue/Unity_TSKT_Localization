using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;

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
    }
}

