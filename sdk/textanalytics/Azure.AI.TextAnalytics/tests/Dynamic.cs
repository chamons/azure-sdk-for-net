// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using NUnit.Framework;
using Azure.Core;

namespace Azure.AI.TextAnalytics.Dynamic.Tests
{
    public class Dynamic
    {
        [Test]
        public void Test1()
        {
            var document = new string [] { @"The concierge Paulette was extremely helpful. Sadly when we arrived the elevator was broken, but with Paulette's help we barely noticed this inconvenience.
            She arranged for our baggage to be brought up to our room with no extra charge and gave us a free meal to refurbish all of the calories we lost from
            walking up the stairs :). Can't say enough good things about my experience!",
            "最近由于工作压力太大，我们决定去富酒店度假。那儿的温泉实在太舒服了，我跟我丈夫都完全恢复了工作前的青春精神！加油！"};

            dynamic d = DynamicJson.Object();
            d.id = 0;
            d.text = document[0];
            Console.WriteLine (d);

            TextAnalyticsClient client = new TextAnalyticsClient ();
        }
    }
}