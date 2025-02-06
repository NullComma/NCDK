using System;

namespace EnigmaCore
{
    public static class Disposables {
        public static IDisposable Empty { get; } = new EmptyDisposable();

        class EmptyDisposable : IDisposable {
            public void Dispose() { }
        }
    }
}