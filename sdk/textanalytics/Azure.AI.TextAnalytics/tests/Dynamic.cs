// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using NUnit.Framework;
using Azure.Core;

namespace Azure.AI.TextAnalytics.Dynamic
{
    public class Dynamic
    {
        [Test]
        public void Test1()
        {
            dynamic d = DynamicJson.Object();
            d.Foo = "Bar";
            d.Buzz = 42;
            Console.WriteLine (d);
        }
    }
}