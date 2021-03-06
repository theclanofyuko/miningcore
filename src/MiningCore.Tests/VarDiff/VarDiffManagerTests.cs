﻿using System.Collections.Generic;
using MiningCore.Configuration;
using MiningCore.Crypto.Hashing.Algorithms;
using MiningCore.Extensions;
using MiningCore.Mining;
using MiningCore.VarDiff;
using NLog;
using Xunit;

namespace MiningCore.Tests.VarDiff
{
    public class VarDiffManagerTests : TestBase
    {
        private static readonly VarDiffConfig config = new VarDiffConfig
        {
            RetargetTime = 90,
            MinDiff = 1000,
            MaxDiff = 10000,
            TargetTime = 15,
            VariancePercent = 30,
            MaxDelta = 1000,
        };

        private static readonly ILogger logger = LogManager.CreateNullLogger();

        [Fact]
        public void VarDiff_Should_Honor_MaxDelta_When_Adjusting_Up()
        {
            var vdm = new VarDiffManager(config);
            var ctx = new WorkerContextBase {Difficulty = 7500, VarDiff = new VarDiffContext() };

            var shares = new List<long> { 2, 3, 4 };
            var newDiff = vdm.Update(ctx, shares, string.Empty, logger);
            Assert.NotNull(newDiff);
            Assert.True(newDiff.Value.EqualsDigitPrecision3(8500));
        }

        [Fact]
        public void VarDiff_Should_Honor_MaxDelta_When_Adjusting_Down()
        {
            var vdm = new VarDiffManager(config);
            var ctx = new WorkerContextBase { Difficulty = 7500, VarDiff = new VarDiffContext() };

            var shares = new List<long> { 2000000000, 3000000000, 4000000000 };
            var newDiff = vdm.Update(ctx, shares, string.Empty, logger);
            Assert.NotNull(newDiff);
            Assert.True(newDiff.Value.EqualsDigitPrecision3(6500));
        }

        [Fact]
        public void VarDiff_Should_Honor_MaxDiff_When_Adjusting_Up()
        {
            var vdm = new VarDiffManager(config);
            var ctx = new WorkerContextBase { Difficulty = 9500, VarDiff = new VarDiffContext() };

            var shares = new List<long> { 2, 3, 4 };
            var newDiff = vdm.Update(ctx, shares, string.Empty, logger);
            Assert.NotNull(newDiff);
            Assert.True(newDiff.Value.EqualsDigitPrecision3(10000));
        }

        [Fact]
        public void VarDiff_Should_Honor_MinDiff_When_Adjusting_Down()
        {
            var vdm = new VarDiffManager(config);
            var ctx = new WorkerContextBase { Difficulty = 1500, VarDiff = new VarDiffContext() };

            var shares = new List<long> { 2000000000, 3000000000, 4000000000 };
            var newDiff = vdm.Update(ctx, shares, string.Empty, logger);
            Assert.NotNull(newDiff);
            Assert.True(newDiff.Value.EqualsDigitPrecision3(1000));
        }
    }
}
