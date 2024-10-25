using System.Runtime.InteropServices;

namespace Monkeymoto.NativeGenericDelegates.Tests
{
    public sealed unsafe partial class Tests
    {
        internal readonly struct DefaultMarshaller : IMarshaller<DefaultMarshaller> { }

        internal readonly struct CdeclMarshaller : IMarshaller<CdeclMarshaller>
        {
            static CallingConvention? IMarshaller<CdeclMarshaller>.CallingConvention => CallingConvention.Cdecl;
        }
    }
}
