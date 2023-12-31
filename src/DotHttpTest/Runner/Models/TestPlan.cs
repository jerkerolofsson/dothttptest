﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Runner.Models
{
    public class TestPlan
    {
        public string Name { get; set; } = "TestPlan";
        public List<TestPlanStage> Stages { get; set; } = new();

        /// <summary>
        /// Total duration of all stages
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                return TimeSpan.FromSeconds(
                    Stages.Select(x => x.Attributes.Duration.TotalSeconds)
                    .Sum());
            }
        }
    }
}
