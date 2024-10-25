using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Monkeymoto.NativeGenericDelegates.Tests
{
    [TestClass]
    public sealed unsafe partial class Tests
    {
        [TestMethod]
        public void Test_INativeAction_0_FromAction()
        {
            _ = INativeAction.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_0_FromFunctionPointer()
        {
            _ = INativeAction.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction0));
        }

        [TestMethod]
        public void Test_INativeAction_1_int_FromAction()
        {
            _ = INativeAction<int>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_1_int_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<int>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_1_int_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<int>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_1_int_FromFunctionPointer()
        {
            _ = INativeAction<int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction1));
        }

        [TestMethod]
        public void Test_INativeAction_1_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction1));
        }

        [TestMethod]
        public void Test_INativeAction_1_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction1));
        }

        [TestMethod]
        public void Test_INativeAction_1_string_FromAction()
        {
            _ = INativeAction<string>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_1_string_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<string>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_1_string_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<string>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_1_string_FromFunctionPointer()
        {
            _ = INativeAction<string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction1));
        }

        [TestMethod]
        public void Test_INativeAction_1_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction1));
        }

        [TestMethod]
        public void Test_INativeAction_1_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction1));
        }

        [TestMethod]
        public void Test_INativeAction_2_int_FromAction()
        {
            _ = INativeAction<int, int>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_2_int_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<int, int>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_2_int_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<int, int>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_2_int_FromFunctionPointer()
        {
            _ = INativeAction<int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction2));
        }

        [TestMethod]
        public void Test_INativeAction_2_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction2));
        }

        [TestMethod]
        public void Test_INativeAction_2_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction2));
        }

        [TestMethod]
        public void Test_INativeAction_2_string_FromAction()
        {
            _ = INativeAction<string, string>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_2_string_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<string, string>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_2_string_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<string, string>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_2_string_FromFunctionPointer()
        {
            _ = INativeAction<string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction2));
        }

        [TestMethod]
        public void Test_INativeAction_2_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction2));
        }

        [TestMethod]
        public void Test_INativeAction_2_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction2));
        }

        [TestMethod]
        public void Test_INativeAction_3_int_FromAction()
        {
            _ = INativeAction<int, int, int>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_3_int_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_3_int_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_3_int_FromFunctionPointer()
        {
            _ = INativeAction<int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction3));
        }

        [TestMethod]
        public void Test_INativeAction_3_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction3));
        }

        [TestMethod]
        public void Test_INativeAction_3_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction3));
        }

        [TestMethod]
        public void Test_INativeAction_3_string_FromAction()
        {
            _ = INativeAction<string, string, string>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_3_string_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_3_string_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_3_string_FromFunctionPointer()
        {
            _ = INativeAction<string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction3));
        }

        [TestMethod]
        public void Test_INativeAction_3_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction3));
        }

        [TestMethod]
        public void Test_INativeAction_3_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction3));
        }

        [TestMethod]
        public void Test_INativeAction_4_int_FromAction()
        {
            _ = INativeAction<int, int, int, int>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_4_int_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_4_int_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_4_int_FromFunctionPointer()
        {
            _ = INativeAction<int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction4));
        }

        [TestMethod]
        public void Test_INativeAction_4_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction4));
        }

        [TestMethod]
        public void Test_INativeAction_4_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction4));
        }

        [TestMethod]
        public void Test_INativeAction_4_string_FromAction()
        {
            _ = INativeAction<string, string, string, string>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_4_string_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_4_string_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_4_string_FromFunctionPointer()
        {
            _ = INativeAction<string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction4));
        }

        [TestMethod]
        public void Test_INativeAction_4_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction4));
        }

        [TestMethod]
        public void Test_INativeAction_4_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction4));
        }

        [TestMethod]
        public void Test_INativeAction_5_int_FromAction()
        {
            _ = INativeAction<int, int, int, int, int>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_5_int_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int, int>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_5_int_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int, int>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_5_int_FromFunctionPointer()
        {
            _ = INativeAction<int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction5));
        }

        [TestMethod]
        public void Test_INativeAction_5_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction5));
        }

        [TestMethod]
        public void Test_INativeAction_5_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction5));
        }

        [TestMethod]
        public void Test_INativeAction_5_string_FromAction()
        {
            _ = INativeAction<string, string, string, string, string>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_5_string_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string, string>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_5_string_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string, string>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_5_string_FromFunctionPointer()
        {
            _ = INativeAction<string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction5));
        }

        [TestMethod]
        public void Test_INativeAction_5_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction5));
        }

        [TestMethod]
        public void Test_INativeAction_5_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction5));
        }

        [TestMethod]
        public void Test_INativeAction_6_int_FromAction()
        {
            _ = INativeAction<int, int, int, int, int, int>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_6_int_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_6_int_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_6_int_FromFunctionPointer()
        {
            _ = INativeAction<int, int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction6));
        }

        [TestMethod]
        public void Test_INativeAction_6_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction6));
        }

        [TestMethod]
        public void Test_INativeAction_6_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction6));
        }

        [TestMethod]
        public void Test_INativeAction_6_string_FromAction()
        {
            _ = INativeAction<string, string, string, string, string, string>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_6_string_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_6_string_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_6_string_FromFunctionPointer()
        {
            _ = INativeAction<string, string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction6));
        }

        [TestMethod]
        public void Test_INativeAction_6_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction6));
        }

        [TestMethod]
        public void Test_INativeAction_6_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction6));
        }

        [TestMethod]
        public void Test_INativeAction_7_int_FromAction()
        {
            _ = INativeAction<int, int, int, int, int, int, int>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_7_int_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_7_int_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_7_int_FromFunctionPointer()
        {
            _ = INativeAction<int, int, int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction7));
        }

        [TestMethod]
        public void Test_INativeAction_7_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction7));
        }

        [TestMethod]
        public void Test_INativeAction_7_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction7));
        }

        [TestMethod]
        public void Test_INativeAction_7_string_FromAction()
        {
            _ = INativeAction<string, string, string, string, string, string, string>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_7_string_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_7_string_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_7_string_FromFunctionPointer()
        {
            _ = INativeAction<string, string, string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction7));
        }

        [TestMethod]
        public void Test_INativeAction_7_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction7));
        }

        [TestMethod]
        public void Test_INativeAction_7_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction7));
        }

        [TestMethod]
        public void Test_INativeAction_8_int_FromAction()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_8_int_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_8_int_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_8_int_FromFunctionPointer()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction8));
        }

        [TestMethod]
        public void Test_INativeAction_8_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction8));
        }

        [TestMethod]
        public void Test_INativeAction_8_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction8));
        }

        [TestMethod]
        public void Test_INativeAction_8_string_FromAction()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_8_string_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_8_string_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_8_string_FromFunctionPointer()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction8));
        }

        [TestMethod]
        public void Test_INativeAction_8_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction8));
        }

        [TestMethod]
        public void Test_INativeAction_8_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction8));
        }

        [TestMethod]
        public void Test_INativeAction_9_int_FromAction()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_9_int_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_9_int_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_9_int_FromFunctionPointer()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction9));
        }

        [TestMethod]
        public void Test_INativeAction_9_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction9));
        }

        [TestMethod]
        public void Test_INativeAction_9_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction9));
        }

        [TestMethod]
        public void Test_INativeAction_9_string_FromAction()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_9_string_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_9_string_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_9_string_FromFunctionPointer()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction9));
        }

        [TestMethod]
        public void Test_INativeAction_9_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction9));
        }

        [TestMethod]
        public void Test_INativeAction_9_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction9));
        }

        [TestMethod]
        public void Test_INativeAction_10_int_FromAction()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_10_int_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_10_int_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_10_int_FromFunctionPointer()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction10));
        }

        [TestMethod]
        public void Test_INativeAction_10_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction10));
        }

        [TestMethod]
        public void Test_INativeAction_10_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction10));
        }

        [TestMethod]
        public void Test_INativeAction_10_string_FromAction()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_10_string_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_10_string_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_10_string_FromFunctionPointer()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction10));
        }

        [TestMethod]
        public void Test_INativeAction_10_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction10));
        }

        [TestMethod]
        public void Test_INativeAction_10_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction10));
        }

        [TestMethod]
        public void Test_INativeAction_11_int_FromAction()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_11_int_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_11_int_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_11_int_FromFunctionPointer()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction11));
        }

        [TestMethod]
        public void Test_INativeAction_11_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction11));
        }

        [TestMethod]
        public void Test_INativeAction_11_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction11));
        }

        [TestMethod]
        public void Test_INativeAction_11_string_FromAction()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_11_string_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_11_string_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_11_string_FromFunctionPointer()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction11));
        }

        [TestMethod]
        public void Test_INativeAction_11_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction11));
        }

        [TestMethod]
        public void Test_INativeAction_11_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction11));
        }

        [TestMethod]
        public void Test_INativeAction_12_int_FromAction()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_12_int_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_12_int_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_12_int_FromFunctionPointer()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction12));
        }

        [TestMethod]
        public void Test_INativeAction_12_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction12));
        }

        [TestMethod]
        public void Test_INativeAction_12_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction12));
        }

        [TestMethod]
        public void Test_INativeAction_12_string_FromAction()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_12_string_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_12_string_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_12_string_FromFunctionPointer()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction12));
        }

        [TestMethod]
        public void Test_INativeAction_12_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction12));
        }

        [TestMethod]
        public void Test_INativeAction_12_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction12));
        }

        [TestMethod]
        public void Test_INativeAction_13_int_FromAction()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int, int>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_13_int_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int, int>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_13_int_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int, int>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_13_int_FromFunctionPointer()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction13));
        }

        [TestMethod]
        public void Test_INativeAction_13_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction13));
        }

        [TestMethod]
        public void Test_INativeAction_13_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction13));
        }

        [TestMethod]
        public void Test_INativeAction_13_string_FromAction()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string, string>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_13_string_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string, string>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_13_string_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string, string>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_13_string_FromFunctionPointer()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction13));
        }

        [TestMethod]
        public void Test_INativeAction_13_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction13));
        }

        [TestMethod]
        public void Test_INativeAction_13_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction13));
        }

        [TestMethod]
        public void Test_INativeAction_14_int_FromAction()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_14_int_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_14_int_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_14_int_FromFunctionPointer()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction14));
        }

        [TestMethod]
        public void Test_INativeAction_14_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction14));
        }

        [TestMethod]
        public void Test_INativeAction_14_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction14));
        }

        [TestMethod]
        public void Test_INativeAction_14_string_FromAction()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_14_string_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_14_string_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_14_string_FromFunctionPointer()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction14));
        }

        [TestMethod]
        public void Test_INativeAction_14_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction14));
        }

        [TestMethod]
        public void Test_INativeAction_14_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction14));
        }

        [TestMethod]
        public void Test_INativeAction_15_int_FromAction()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_15_int_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_15_int_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_15_int_FromFunctionPointer()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction15));
        }

        [TestMethod]
        public void Test_INativeAction_15_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction15));
        }

        [TestMethod]
        public void Test_INativeAction_15_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction15));
        }

        [TestMethod]
        public void Test_INativeAction_15_string_FromAction()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_15_string_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_15_string_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_15_string_FromFunctionPointer()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction15));
        }

        [TestMethod]
        public void Test_INativeAction_15_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction15));
        }

        [TestMethod]
        public void Test_INativeAction_15_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction15));
        }

        [TestMethod]
        public void Test_INativeAction_16_int_FromAction()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_16_int_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_16_int_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_16_int_FromFunctionPointer()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction16));
        }

        [TestMethod]
        public void Test_INativeAction_16_int_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction16));
        }

        [TestMethod]
        public void Test_INativeAction_16_int_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction16));
        }

        [TestMethod]
        public void Test_INativeAction_16_string_FromAction()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromAction(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_16_string_FromAction_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromAction<DefaultMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_16_string_FromAction_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromAction<CdeclMarshaller>(Stubs.Stub_Action);
        }

        [TestMethod]
        public void Test_INativeAction_16_string_FromFunctionPointer()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction16));
        }

        [TestMethod]
        public void Test_INativeAction_16_string_FromFunctionPointer_DefaultMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<DefaultMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction16));
        }

        [TestMethod]
        public void Test_INativeAction_16_string_FromFunctionPointer_CdeclMarshaller()
        {
            _ = INativeAction<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string, string>.FromFunctionPointer<CdeclMarshaller>(Stubs.ToPointer(&Stubs.Stub_UnmanagedAction16));
        }
    }
}
