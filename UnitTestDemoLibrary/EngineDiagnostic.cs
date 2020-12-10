namespace UnitTestDemoLibrary
{
    public record EngineDiagnostic
    {
        public Severity Severity { get; init; }

        public string Description { get; init; }
    }
}