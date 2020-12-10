namespace UnitTestDemoLibrary
{
    using System;
    using System.Drawing;
    using System.Linq;

    public class Motorbike
    {
        private readonly IAuthentication _authentication;
        private readonly IEngine _engine;
        private readonly IDashboard _dashboard;

        public Motorbike(IAuthentication authentication, IEngine engine, IDashboard dashboard)
        {
            this._authentication = authentication;
            this._engine = engine;
            this._dashboard = dashboard;
        }

        public Color Color { get; private set; }

        public void Paint(Color color)
        {
            this.Color = color;
        }

        public void StartEngine(Key key, object engineParameters = null)
        {
            try
            {
                this._authentication.AuthorizeKey(key);
            }
            catch (UnauthorizedAccessException e)
            {
                throw new ArgumentException("Given key failed to start engine", e);
            }

            if (this._engine.GetDiagnostics().Any(d => d.Severity == Severity.Critical))
            {
                throw new InvalidOperationException("Can't start engine due to bad diagnostic report");
            }

            this._engine.Start(engineParameters);
        }

        public void ShowDiagnosticsOnDashboard()
        {
            var diagnosticMessages = this._engine.GetDiagnostics();

            foreach (var diagnosticMessage in diagnosticMessages)
            {
                this._dashboard.AppendMessage(diagnosticMessage.Description);
            }
        }

        public Tyre GetFrontTyreDetails()
        {
            return new () { Brand = "Metzeler", Model = "Roadtec 01", Year = 2016 };
        }
    }
}