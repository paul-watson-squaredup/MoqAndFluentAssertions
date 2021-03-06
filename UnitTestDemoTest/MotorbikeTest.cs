﻿namespace UnitTestDemoTest
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using FluentAssertions;
    using Moq;
    using UnitTestDemoLibrary;
    using Xunit;

    public class MotorbikeTest
    {
        private readonly Mock<IAuthentication> mockAuthentication = new();
        private readonly Mock<IEngine> mockEngine = new();
        private readonly Mock<IDashboard> mockDashboard = new();
        private readonly Key key = new Key { Id = 123 };
        private readonly Motorbike motorbike;

        public MotorbikeTest()
        {
            this.motorbike = new Motorbike(
                this.mockAuthentication.Object,
                this.mockEngine.Object,
                this.mockDashboard.Object);
        }

        [Fact]
        public void GivenBlackColorWhenPaintThenColorBlack()
        {
            this.motorbike.Paint(Color.Aqua);
            this.motorbike.Color.Should().Be(Color.Brown, "If I've painted the motorbike, it should now be this color");
        }

        [Fact]
        public void GivenUnauthorizedKeyWhenStartEngineThenExceptionBubbledUp()
        {
            this.mockAuthentication.Setup(a => a.AuthorizeKey(It.IsAny<Key>())).Throws<UnauthorizedAccessException>();
            Action startEngine = () => this.motorbike.StartEngine(this.key);
            startEngine.Should().Throw<ArgumentException>("Incorrect key throws exception")
                .WithMessage("Given key failed to start engine");
        }

        [Fact]
        public void GivenNoEngineProblemsAndCorrectKeyWhenStartEngineThenStartEngineCalled()
        {
            this.motorbike.StartEngine(this.key);
            this.mockEngine.Verify(e => e.Start(It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public void GivenEngineProblemsAndCorrectKeyWhenStartEngineThenInvalidOperationExceptionThrown()
        {
            var engineDiagnostics = new List<EngineDiagnostic>
                { new () {Description = "Low oil", Severity = Severity.Critical}};
            this.mockEngine.Setup(e => e.GetDiagnostics()).Returns(engineDiagnostics);
            Action startEngine = () => this.motorbike.StartEngine(this.key);
            startEngine.Should().Throw<InvalidOperationException>("Engine problems throws exception")
                .WithMessage("Can't start engine due to bad diagnostic report");
        }

        [Fact]
        public void GivenNoEngineProblemsAndCorrectKeyWhenStartEngineThenEngineParametersPassed()
        {
            var engineParameter = new object();
            this.motorbike.StartEngine(this.key, engineParameter);
            this.mockEngine.Verify(e => e.Start(engineParameter));
        }

        [Fact]
        public void GivenEngineDiagnosticsWhenShowDiagnosticsOnDashboardThenEngineDiagnosticsPassedToDashboard()
        {
            var expected = new List<string> {"Low oil", "Cold air intake"};
            var diagnostics = new List<EngineDiagnostic>
            {
                new () { Description = "Low oil", Severity = Severity.Critical },
                new () { Description = "Cold air intake", Severity = Severity.Warning },
            };
            var actual = new List<string>();
            this.mockEngine.Setup(e => e.GetDiagnostics()).Returns(diagnostics);
            this.mockDashboard.Setup(d => d.AppendMessage(It.IsAny<string>()))
                .Callback<string>(message => actual.Add(message));
            this.motorbike.ShowDiagnosticsOnDashboard();

            actual.Should().Equal(expected);
        }

        [Fact]
        public void GivenMotorbikeInstanceWhenGetFrontTyreDetailsThenTyreIsMetzelerRoadTec01()
        {
            var expected = new Tyre() { Brand = "Metzeler", Model = "Roadtec 01", Year = 2016 };
            this.motorbike.GetFrontTyreDetails().Should().Be(expected);
        }
    }
}