using DotHttpTest.Runner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Tests.Unit.ParserTests
{
    [TestCategory("UnitTests")]
    [TestClass]
    public class StageParserTests
    {

        [TestMethod]
        public void ParseStage_WithDurationInDays_DurationParsedCorrectly()
        {
            // Act
            var stage = StageParser.Parse("name duration:7d");

            // Assert
            Assert.AreEqual(TimeSpan.FromDays(7), stage.Duration);
        }

        [TestMethod]
        public void ParseStage_WithDurationInHours_DurationParsedCorrectly()
        {
            // Act
            var stage = StageParser.Parse("name duration:2h");

            // Assert
            Assert.AreEqual(TimeSpan.FromHours(2), stage.Duration);
        }

        [TestMethod]
        public void ParseStage_WithDurationInMinutes_DurationParsedCorrectly()
        {
            // Act
            var stage = StageParser.Parse("name duration:10m");

            // Assert
            Assert.AreEqual(TimeSpan.FromMinutes(10), stage.Duration);
        }

        [TestMethod]
        public void ParseStage_WithDurationInSeconds_DurationParsedCorrectly()
        {
            // Act
            var stage = StageParser.Parse("name duration:5s");

            // Assert
            Assert.AreEqual(TimeSpan.FromSeconds(5), stage.Duration);
        }

        [TestMethod]
        public void ParseStage_WithDurationInDaysAndMinutes_DurationParsedCorrectly()
        {
            // Act
            var stage = StageParser.Parse("name duration:7d2m");

            // Assert
            Assert.AreEqual(TimeSpan.FromDays(7)+TimeSpan.FromMinutes(2), stage.Duration);
        }
    }
}
