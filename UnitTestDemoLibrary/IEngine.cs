using System.Collections.Generic;

namespace UnitTestDemoLibrary
{
    public interface IEngine
    {
        void Start(object engineParameters);

        void Stop();

        IEnumerable<EngineDiagnostic> GetDiagnostics();
    }
}