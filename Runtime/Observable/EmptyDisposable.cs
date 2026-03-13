using System;

namespace HUtil.Runtime.Observable
{
    public class EmptyDisposable : IDisposable
    {
        public static EmptyDisposable Instance = new EmptyDisposable();

        public void Dispose()
        {
            return;
        }
    }
}