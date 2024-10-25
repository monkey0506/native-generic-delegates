using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Monkeymoto.NativeGenericDelegates.Tests
{
    public sealed unsafe partial class Tests
    {
        [TestMethod]
        public void Test_INativeFunc_0_int_FromFunc()
        {
            _ = INativeFunc<int>.FromFunc(Stubs.Stub_Func<int>);
        }

        [TestMethod]
        public void Test_INativeFunc_0_int_FromFunctionPointer()
        {
            _ = INativeFunc<int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc0));
        }

        [TestMethod]
        public void Test_INativeFunc_0_string_FromFunc()
        {
            _ = INativeFunc<string>.FromFunc(Stubs.Stub_Func<string>);
        }

        [TestMethod]
        public void Test_INativeFunc_0_string_FromFunctionPointer()
        {
            _ = INativeFunc<string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc0));
        }

        [TestMethod]
        public void Test_INativeFunc_1_int_FromFunc()
        {
            _ = INativeFunc<int, int>.FromFunc(Stubs.Stub_Func<int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_1_int_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<int, int>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_1_int_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<int, int>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_1_int_FromFunctionPointer()
        {
            _ = INativeFunc<int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc1));
        }

        [TestMethod]
        public void Test_INativeFunc_1_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc1));
        }

        [TestMethod]
        public void Test_INativeFunc_1_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc1));
        }

        [TestMethod]
        public void Test_INativeFunc_1_string_FromFunc()
        {
            _ = INativeFunc<string, string>.FromFunc(Stubs.Stub_Func<string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_1_string_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<string, string>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_1_string_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<string, string>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_1_string_FromFunctionPointer()
        {
            _ = INativeFunc<string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc1));
        }

        [TestMethod]
        public void Test_INativeFunc_1_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc1));
        }

        [TestMethod]
        public void Test_INativeFunc_1_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc1));
        }

        [TestMethod]
        public void Test_INativeFunc_2_int_FromFunc()
        {
            _ = INativeFunc<int, int, int>.FromFunc(Stubs.Stub_Func<int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_2_int_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_2_int_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_2_int_FromFunctionPointer()
        {
            _ = INativeFunc<int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc2));
        }

        [TestMethod]
        public void Test_INativeFunc_2_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc2));
        }

        [TestMethod]
        public void Test_INativeFunc_2_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc2));
        }

        [TestMethod]
        public void Test_INativeFunc_2_string_FromFunc()
        {
            _ = INativeFunc<string, string, string>.FromFunc(Stubs.Stub_Func<string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_2_string_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_2_string_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_2_string_FromFunctionPointer()
        {
            _ = INativeFunc<string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc2));
        }

        [TestMethod]
        public void Test_INativeFunc_2_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc2));
        }

        [TestMethod]
        public void Test_INativeFunc_2_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc2));
        }

        [TestMethod]
        public void Test_INativeFunc_3_int_FromFunc()
        {
            _ = INativeFunc<int, int, int, int>.FromFunc(Stubs.Stub_Func<int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_3_int_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_3_int_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_3_int_FromFunctionPointer()
        {
            _ = INativeFunc<int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc3));
        }

        [TestMethod]
        public void Test_INativeFunc_3_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc3));
        }

        [TestMethod]
        public void Test_INativeFunc_3_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc3));
        }

        [TestMethod]
        public void Test_INativeFunc_3_string_FromFunc()
        {
            _ = INativeFunc<string, string, string, string>.FromFunc(Stubs.Stub_Func<string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_3_string_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_3_string_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_3_string_FromFunctionPointer()
        {
            _ = INativeFunc<string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc3));
        }

        [TestMethod]
        public void Test_INativeFunc_3_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc3));
        }

        [TestMethod]
        public void Test_INativeFunc_3_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc3));
        }

        [TestMethod]
        public void Test_INativeFunc_4_int_FromFunc()
        {
            _ = INativeFunc<int, int, int, int, int>.FromFunc(Stubs.Stub_Func<int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_4_int_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_4_int_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_4_int_FromFunctionPointer()
        {
            _ = INativeFunc<int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_4_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_4_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_4_string_FromFunc()
        {
            _ = INativeFunc<string, string, string, string, string>.FromFunc(Stubs.Stub_Func<string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_4_string_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_4_string_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_4_string_FromFunctionPointer()
        {
            _ = INativeFunc<string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_4_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_4_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_5_int_FromFunc()
        {
            _ = INativeFunc<int, int, int, int, int, int>.FromFunc(Stubs.Stub_Func<int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_5_int_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_5_int_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_5_int_FromFunctionPointer()
        {
            _ = INativeFunc<int, int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_5_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_5_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_5_string_FromFunc()
        {
            _ = INativeFunc<string, string, string, string, string, string>.FromFunc(Stubs.Stub_Func<string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_5_string_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_5_string_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_5_string_FromFunctionPointer()
        {
            _ = INativeFunc<string, string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_5_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_5_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_6_int_FromFunc()
        {
            _ = INativeFunc<int, int, int, int, int, int, int>.FromFunc(Stubs.Stub_Func<int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_6_int_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_6_int_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_6_int_FromFunctionPointer()
        {
            _ = INativeFunc<int, int, int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_6_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_6_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_6_string_FromFunc()
        {
            _ = INativeFunc<string, string, string, string, string, string, string>.FromFunc(Stubs.Stub_Func<string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_6_string_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_6_string_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_6_string_FromFunctionPointer()
        {
            _ = INativeFunc<string, string, string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_6_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_6_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_7_int_FromFunc()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int>.FromFunc(Stubs.Stub_Func<int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_7_int_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_7_int_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_7_int_FromFunctionPointer()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_7_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_7_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_7_string_FromFunc()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string>.FromFunc(Stubs.Stub_Func<string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_7_string_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_7_string_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_7_string_FromFunctionPointer()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_7_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_7_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_8_int_FromFunc()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int>.FromFunc(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_8_int_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_8_int_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_8_int_FromFunctionPointer()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_8_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_8_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_8_string_FromFunc()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string>.FromFunc(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_8_string_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_8_string_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_8_string_FromFunctionPointer()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_8_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_8_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_9_int_FromFunc()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int>.FromFunc(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_9_int_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_9_int_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_9_int_FromFunctionPointer()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_9_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_9_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_9_string_FromFunc()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string>.FromFunc(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_9_string_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_9_string_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_9_string_FromFunctionPointer()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_9_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_9_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_10_int_FromFunc()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int>.FromFunc(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_10_int_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_10_int_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_10_int_FromFunctionPointer()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_10_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_10_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_10_string_FromFunc()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string>.FromFunc(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_10_string_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_10_string_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_10_string_FromFunctionPointer()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_10_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_10_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_11_int_FromFunc()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int>.FromFunc(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_11_int_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_11_int_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_11_int_FromFunctionPointer()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_11_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_11_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_11_string_FromFunc()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string>.FromFunc(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_11_string_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_11_string_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_11_string_FromFunctionPointer()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_11_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_11_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_12_int_FromFunc()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunc(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_12_int_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_12_int_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_12_int_FromFunctionPointer()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_12_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_12_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_12_string_FromFunc()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunc(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_12_string_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_12_string_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_12_string_FromFunctionPointer()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_12_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_12_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_13_int_FromFunc()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunc(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_13_int_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_13_int_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_13_int_FromFunctionPointer()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_13_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_13_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_13_string_FromFunc()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunc(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_13_string_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_13_string_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_13_string_FromFunctionPointer()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_13_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_13_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_14_int_FromFunc()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunc(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_14_int_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_14_int_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_14_int_FromFunctionPointer()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_14_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_14_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_14_string_FromFunc()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunc(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_14_string_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_14_string_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_14_string_FromFunctionPointer()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_14_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_14_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_15_int_FromFunc()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunc(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_15_int_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_15_int_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_15_int_FromFunctionPointer()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_15_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_15_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_15_string_FromFunc()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunc(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_15_string_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_15_string_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_15_string_FromFunctionPointer()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_15_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_15_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_16_int_FromFunc()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunc(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_16_int_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_16_int_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>);
        }

        [TestMethod]
        public void Test_INativeFunc_16_int_FromFunctionPointer()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_16_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_16_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc5));
        }

        [TestMethod]
        public void Test_INativeFunc_16_string_FromFunc()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunc(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_16_string_FromFunc_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunc<DefaultMarshaller>(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_16_string_FromFunc_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunc<CdeclMarshaller>(Stubs.Stub_Func<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>);
        }

        [TestMethod]
        public void Test_INativeFunc_16_string_FromFunctionPointer()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_16_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }

        [TestMethod]
        public void Test_INativeFunc_16_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeFunc<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedFunc4));
        }
    }
}
