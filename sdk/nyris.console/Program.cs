﻿using System;
using System.Diagnostics;
using System.IO;
using Io.Nyris.Sdk;
using Io.Nyris.Sdk.Utils;

namespace nyris.console
{
    static class MainClass
    {
        public static void Main(string[] args)
        {
            var nyris = Nyris.CreateInstance("", Platform.Other, true);

            var image = File.ReadAllBytes(@"");

            /*nyris.ImageMatchingAPi
                .CategoryPrediction(opt =>
                {
                    opt.Enabled = true;
                    opt.Limit = 5;
                })
                .Limit(5)
                .Match(image)
                .Subscribe(x => Debug.WriteLine(x),
                    throwable => Debug.WriteLine(throwable.Message));*/

            nyris.ObjectProposalApi
                .ExtractObjects(image)
                .Subscribe(x => Debug.WriteLine(x), 
                    throwable => Debug.WriteLine(throwable.Message));
            Console.ReadKey();
        }
    }
}