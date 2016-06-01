﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using CreviceApp.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Config.Tests
{
    [TestClass()]
    public class IfStrokeElementTests
    {
        [TestMethod()]
        public void doTest()
        {
            var root = new DSL.Root();
            var appElement = root.@when(() => true);
            var onElement = appElement.@on(new Def.RightButton());
            var ifElement = onElement.@if(new Def.MoveDown(), new Def.MoveRight());
            Assert.AreEqual(root.whenElements[0].onElements[0].ifStrokeElements[0].doElements.Count, 0);
            ifElement.@do(() => { });
            Assert.AreEqual(root.whenElements[0].onElements[0].ifStrokeElements[0].doElements.Count, 1);
        }
    }
}