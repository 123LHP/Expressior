using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ProtoCore.DSASM;
using ProtoTestFx.TD;
using ProtoTest;

namespace ProtoFFITests
{
    [TestFixture]
    class ArgumentMarshalingTests  : ProtoTestBase
    {
        [Test]
        public void TestReturnIList()
        {
            string code = @"
            
            thisTest.RunScriptSource(code);
            var methods = thisTest.GetMethods("DummyCollection", "ReturnIList");
            //IList is marshaled as arbitrary rank var array
            Assert.AreEqual((int)ProtoCore.PrimitiveType.Var, methods[0].ReturnType.Value.UID);
            Assert.AreEqual(Constants.kArbitraryRank, methods[0].ReturnType.Value.rank);
            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.Integer, args[0].UID);
            Assert.AreEqual(1, args[0].rank); //Expecting it tobe marshaled as 1D array

            thisTest.Verify("b", new int[] { 1, 2, 3, 4, 5 });
        }

        [Test]
        public void TestReturnIEnumerableOfInt()
        {
            string code = @"

            thisTest.RunScriptSource(code);
            var methods = thisTest.GetMethods("DummyCollection", "ReturnIEnumerableOfInt");
            //IEnumerable<int> ==> int[]
            Assert.AreEqual((int)ProtoCore.PrimitiveType.Integer, methods[0].ReturnType.Value.UID);
            Assert.AreEqual(1, methods[0].ReturnType.Value.rank);

            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.Integer, args[0].UID);
            Assert.AreEqual(1, args[0].rank); //Expecting it tobe marshaled as 1D array

            thisTest.Verify("b", new int[] { 1, 2, 3, 4, 5 });
        }

        [Test]
        public void TestReturnIEnumerablOfIList()
        {
            string code = @"

            thisTest.RunScriptSource(code);
            var methods = thisTest.GetMethods("DummyCollection", "ReturnIEnumerablOfIList");
            //IEnumerable<IList> ==> var[]..[]
            Assert.AreEqual((int)ProtoCore.PrimitiveType.Var, methods[0].ReturnType.Value.UID);
            Assert.AreEqual(Constants.kArbitraryRank, methods[0].ReturnType.Value.rank);

            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.Var, args[0].UID);
            Assert.AreEqual(Constants.kArbitraryRank, args[0].rank); //Expecting it tobe marshaled as arbitrary dimension array

            thisTest.Verify("b", new List<object> { new int[] { 1, 2, 3, 4, 5 }, new int[] { 6, 7, 8, 9, 10 } });
        }

        [Test]
        public void TestAcceptIEnumerablOfIList()
        {
            string code = @"

            thisTest.RunScriptSource(code);
            var methods = thisTest.GetMethods("DummyCollection", "AcceptIEnumerablOfIList");
            //IEnumerable<IList> ==> var[]..[]
            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.Var, args[0].UID);
            Assert.AreEqual(Constants.kArbitraryRank, args[0].rank); //Expecting it tobe marshaled as arbitrary dimension array

            thisTest.Verify("b", new List<object> { new int[] { 1, 2, 3, 4, 5 }, new int[] { 6, 7, 8, 9, 10 } });
        }

        [Test]
        public void TestReturnIListOfIListInt()
        {
            string code = @"

            thisTest.RunScriptSource(code);
            var methods = thisTest.GetMethods("DummyCollection", "ReturnIListOfIListInt");
            //IEnumerable<IList<int>> ==> int[][]
            Assert.AreEqual((int)ProtoCore.PrimitiveType.Integer, methods[0].ReturnType.Value.UID);
            Assert.AreEqual(2, methods[0].ReturnType.Value.rank);

            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.Integer, args[0].UID);
            Assert.AreEqual(2, args[0].rank); //Expecting it tobe marshaled as 2D array

            thisTest.Verify("b", new List<object> { new int[] { 1, 2, 3, 4, 5 }, new int[] { 6, 7, 8, 9, 10 } });
        }

        [Test]
        public void TestAcceptIEnumerablOfIListInt()
        {
            string code = @"

            thisTest.RunScriptSource(code);
            var methods = thisTest.GetMethods("DummyCollection", "AcceptIEnumerablOfIListInt");
            //IEnumerable<IList<int>> ==> int[][]
            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.Integer, args[0].UID);
            Assert.AreEqual(2, args[0].rank); //Expecting it tobe marshaled as 2D array

            thisTest.Verify("b", new List<object> { new int[] { 1, 2, 3, 4, 5 }, new int[] { 6, 7, 8, 9, 10 } });
        }

        [Test]
        public void TestReturnListOfList()
        {
            string code = @"

            thisTest.RunScriptSource(code);
            var methods = thisTest.GetMethods("DummyCollection", "ReturnListOfList");
            //List<List<int>> ==> int[][]
            Assert.AreEqual((int)ProtoCore.PrimitiveType.Integer, methods[0].ReturnType.Value.UID);
            Assert.AreEqual(2, methods[0].ReturnType.Value.rank);

            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.Integer, args[0].UID);
            Assert.AreEqual(2, args[0].rank); //Expecting it tobe marshaled as 2D array

            thisTest.Verify("b", new List<object> { new int[] { 1, 2, 3, 4, 5 }, new int[] { 6, 7, 8, 9, 10 } });
        }

        [Test]
        public void TestAcceptListOfList()
        {
            string code = @"

            thisTest.RunScriptSource(code);
            var methods = thisTest.GetMethods("DummyCollection", "AcceptListOfList");
            //List<List<int>> ==> int[][]
            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.Integer, args[0].UID);
            Assert.AreEqual(2, args[0].rank); //Expecting it tobe marshaled as 2D array

            thisTest.Verify("b", new List<object> { new int[] { 1, 2, 3, 4, 5 }, new int[] { 6, 7, 8, 9, 10 } });
        }

        [Test]
        public void TestReturn3DList()
        {
            string code = @"

            thisTest.RunScriptSource(code);
            var methods = thisTest.GetMethods("DummyCollection", "Return3DList");
            //List<List<List<int>>> ==> int[][][]
            Assert.AreEqual((int)ProtoCore.PrimitiveType.Integer, methods[0].ReturnType.Value.UID);
            Assert.AreEqual(3, methods[0].ReturnType.Value.rank); //Expecting it tobe marshaled as 3D array

            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.Integer, args[0].UID);
            Assert.AreEqual(3, args[0].rank); //Expecting it tobe marshaled as 3D array

            thisTest.Verify("b", new List<object> { new List<object> { new int[] { 1, 2, 3, 4, 5 } } });
        }

        [Test]
        public void TestAccept3DList()
        {
            string code = @"

            thisTest.RunScriptSource(code);
            var methods = thisTest.GetMethods("DummyCollection", "Accept3DList");
            //List<List<List<int>>> ==> int[][][]
            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.Integer, args[0].UID);
            Assert.AreEqual(3, args[0].rank); //Expecting it tobe marshaled as 3D array

            thisTest.Verify("b", new List<object> { new List<object> { new int[] { 1, 2, 3, 4, 5 } } });
        }

        [Test]
        public void TestReturnListOf5Points()
        {
            string code = @"

            thisTest.RunScriptSource(code);
            var methods = thisTest.GetMethods("DummyCollection", "ReturnListOf5Points");
            //Verify DummyPoint class is marshaled
            Assert.IsTrue(ProtoCore.DSASM.Constants.kInvalidIndex != thisTest.GetClassIndex("DummyPoint"));

            //IList<DummyPoint> ==> DummyPoint[]
            Assert.AreEqual("FFITarget.DummyPoint", methods[0].ReturnType.Value.Name);
            Assert.AreEqual(1, methods[0].ReturnType.Value.rank);

            var args = methods[0].GetArgumentTypes();
            Assert.IsTrue(args == null || args.Count == 0);
            
            thisTest.Verify("c", 5);
        }

        [Test]
        public void TestAcceptListOf5PointsReturnAsObject()
        {
            string code = @"

            thisTest.RunScriptSource(code);
            var methods = thisTest.GetMethods("DummyCollection", "AcceptListOf5PointsReturnAsObject");
            //Verify DummyPoint class is marshaled
            Assert.IsTrue(ProtoCore.DSASM.Constants.kInvalidIndex != thisTest.GetClassIndex("DummyPoint"));

            //object is marshaled as arbitrary rank var array
            Assert.AreEqual((int)ProtoCore.PrimitiveType.Var, methods[0].ReturnType.Value.UID);
            Assert.AreEqual(Constants.kArbitraryRank, methods[0].ReturnType.Value.rank);

            //IEnumerable<DummyPoint> ==> DummyPoint[]
            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual("FFITarget.DummyPoint", args[0].Name);
            Assert.AreEqual(1, args[0].rank); //Expecting it tobe marshaled as 3D array

            thisTest.Verify("c", 5);
        }

        [Test]
        public void TestAcceptObjectAsVar()
        {
            string code = @"

            thisTest.RunScriptSource(code);
            var methods = thisTest.GetMethods("DummyCollection", "AcceptObjectAsVar");
            //Return object ==> var[]..[]
            Assert.AreEqual((int)ProtoCore.PrimitiveType.Var, methods[0].ReturnType.Value.UID);
            Assert.AreEqual(Constants.kArbitraryRank, methods[0].ReturnType.Value.rank);

            //Arg object ==> var
            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.Var, args[0].UID);
            Assert.AreEqual(0, args[0].rank); //Expecting it tobe marshaled as singleton

            thisTest.Verify("c", 5);
            //Replication will cause it to return a collection
            thisTest.Verify("b", FFITarget.DummyCollection.ReturnListOf5Points());
        }

        [Test]
        public void TestObjectAsArbitraryDimensionArrayImport()
        {
            string code = @"

            thisTest.RunScriptSource(code);
            var methods = thisTest.GetMethods("DummyCollection", "ObjectAsArbitraryDimensionArrayImport");

            //[ArbitraryDimensionArrayImport] object ==> var[]..[]
            var args = methods[0].GetArgumentTypes();
            Assert.AreEqual((int)ProtoCore.PrimitiveType.Var, args[0].UID);
            Assert.AreEqual(Constants.kArbitraryRank, args[0].rank); //Expecting it tobe marshaled as 3D array

            thisTest.Verify("c", 5);
            thisTest.Verify("b", new List<object> { FFITarget.DummyCollection.ReturnListOf5Points(), new int[] { 1, 2, 3, 4, 5 } });
        }

        [Test]
        public void TestReturnIDictionary()
        {
            string code = @"

            thisTest.RunScriptSource(code);
            var methods = thisTest.GetMethods("DummyCollection", "ReturnIDictionary");
            
            //IDictionary is marshaled as arbitrary rank var array
            Assert.AreEqual((int)ProtoCore.PrimitiveType.Var, methods[0].ReturnType.Value.UID);
            Assert.AreEqual(Constants.kArbitraryRank, methods[0].ReturnType.Value.rank);

            thisTest.Verify("b", new List<object> { new string[] { "A", "B", "C", "D" }, new int[] { 1, 2, 3, 4 } });
        }

        [Test]
        public void TestReturnDictionaryAsObject()
        {
            string code = @"

            thisTest.RunScriptSource(code);
            var methods = thisTest.GetMethods("DummyCollection", "ReturnDictionaryAsObject");
            
            //object is marshaled as arbitrary rank var array
            Assert.AreEqual((int)ProtoCore.PrimitiveType.Var, methods[0].ReturnType.Value.UID);
            Assert.AreEqual(Constants.kArbitraryRank, methods[0].ReturnType.Value.rank);

            thisTest.Verify("b", new List<object> { new string[] { "A", "B", "C", "D" }, new int[] { 1, 2, 3, 4 } });
        }
    }
}