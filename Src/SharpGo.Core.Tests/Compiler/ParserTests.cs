namespace SharpGo.Core.Tests.Compiler
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SharpGo.Core.Ast;
    using SharpGo.Core.Compiler;
    using SharpGo.Core.Language;

    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void ParseIntegerConstant()
        {
            Parser parser = new Parser("123");

            var node = parser.ParseExpressionNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(ConstantNode));
            Assert.AreEqual(123, ((ConstantNode)node).Value);

            Assert.IsNull(parser.ParseExpressionNode());
        }

        [TestMethod]
        public void ParseRealConstant()
        {
            Parser parser = new Parser("123.45");

            var node = parser.ParseExpressionNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(ConstantNode));
            Assert.AreEqual(123.45, ((ConstantNode)node).Value);

            Assert.IsNull(parser.ParseExpressionNode());
        }

        [TestMethod]
        public void ParseStringConstant()
        {
            Parser parser = new Parser("\"foo\"");

            var node = parser.ParseExpressionNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(ConstantNode));
            Assert.AreEqual("foo", ((ConstantNode)node).Value);

            Assert.IsNull(parser.ParseExpressionNode());
        }

        [TestMethod]
        public void ParseName()
        {
            Parser parser = new Parser("foo");

            var node = parser.ParseExpressionNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(NameNode));
            Assert.AreEqual("foo", ((NameNode)node).Name);

            Assert.IsNull(parser.ParseExpressionNode());
        }

        [TestMethod]
        public void ParseQualifiedName()
        {
            Parser parser = new Parser("math.foo");

            var node = parser.ParseExpressionNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(QualifiedNameNode));
            Assert.AreEqual("math", ((QualifiedNameNode)node).PackageName);
            Assert.AreEqual("foo", ((QualifiedNameNode)node).Name);

            Assert.IsNull(parser.ParseExpressionNode());
        }

        [TestMethod]
        public void ParseAddTwoIntegers()
        {
            Parser parser = new Parser("1+2");

            var node = parser.ParseExpressionNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(BinaryNode));

            var bnode = (BinaryNode)node;

            Assert.AreEqual(BinaryOperator.Add, bnode.Operator);
            Assert.IsNotNull(bnode.LeftNode);
            Assert.IsInstanceOfType(bnode.LeftNode, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)bnode.LeftNode).Value);
            Assert.IsNotNull(bnode.RightNode);
            Assert.IsInstanceOfType(bnode.RightNode, typeof(ConstantNode));
            Assert.AreEqual(2, ((ConstantNode)bnode.RightNode).Value);

            Assert.IsNull(parser.ParseExpressionNode());
        }

        [TestMethod]
        public void ParseSubtractTwoIntegers()
        {
            Parser parser = new Parser("1-2");

            var node = parser.ParseExpressionNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(BinaryNode));

            var bnode = (BinaryNode)node;

            Assert.AreEqual(BinaryOperator.Substract, bnode.Operator);
            Assert.IsNotNull(bnode.LeftNode);
            Assert.IsInstanceOfType(bnode.LeftNode, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)bnode.LeftNode).Value);
            Assert.IsNotNull(bnode.RightNode);
            Assert.IsInstanceOfType(bnode.RightNode, typeof(ConstantNode));
            Assert.AreEqual(2, ((ConstantNode)bnode.RightNode).Value);

            Assert.IsNull(parser.ParseExpressionNode());
        }

        [TestMethod]
        public void ParseArithmeticBinaryOperations()
        {
            ParseBinaryOperation("5+6", BinaryOperator.Add, 5, 6);
            ParseBinaryOperation("7-8", BinaryOperator.Substract, 7, 8);
            ParseBinaryOperation("1*2", BinaryOperator.Multiply, 1, 2);
            ParseBinaryOperation("3/4", BinaryOperator.Divide, 3, 4);
        }

        [TestMethod]
        public void ParseArithmeticBinaryOperationsWithSamePrecedence()
        {
            ParseBinaryOperation("5+6+7", BinaryOperator.Add, 5, 6, BinaryOperator.Add, 7);
            ParseBinaryOperation("7-8+1", BinaryOperator.Substract, 7, 8, BinaryOperator.Add, 1);
            ParseBinaryOperation("1*2/3", BinaryOperator.Multiply, 1, 2, BinaryOperator.Divide, 3);
            ParseBinaryOperation("3/4*5", BinaryOperator.Divide, 3, 4, BinaryOperator.Multiply, 5);
        }

        [TestMethod]
        public void ParseLogicBinaryOperationsWithSamePrecedence()
        {
            ParseBinaryOperation("true || false || true", BinaryOperator.Or, true, false, BinaryOperator.Or, true);
            ParseBinaryOperation("true && false && true", BinaryOperator.And, true, false, BinaryOperator.And, true);
        }

        [TestMethod]
        public void ParseLogicBinaryOperationsWithDifferentPrecedence()
        {
            ParseBinaryOperation("true || false && true", BinaryOperator.Or, true, BinaryOperator.And, false, true);
            ParseBinaryOperation("true && false || true", BinaryOperator.And, true, false, BinaryOperator.Or, true);
        }

        [TestMethod]
        public void ParseArithmeticBinaryOperationsWithDifferentPrecedence()
        {
            ParseBinaryOperation("5+6*7", BinaryOperator.Add, 5, BinaryOperator.Multiply, 6, 7);
            ParseBinaryOperation("7-8/1", BinaryOperator.Substract, 7, BinaryOperator.Divide, 8, 1);
        }

        [TestMethod]
        public void ParseComparisonOperations()
        {
            ParseBinaryOperation("5==6", BinaryOperator.Equal, 5, 6);
            ParseBinaryOperation("7!=8", BinaryOperator.NotEqual, 7, 8);
            ParseBinaryOperation("1<2", BinaryOperator.Less, 1, 2);
            ParseBinaryOperation("3>4", BinaryOperator.Greater, 3, 4);
            ParseBinaryOperation("1<=2", BinaryOperator.LessEqual, 1, 2);
            ParseBinaryOperation("3>=4", BinaryOperator.GreaterEqual, 3, 4);
        }

        [TestMethod]
        public void ParseExpressionStatement()
        {
            Parser parser = new Parser("1-2");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(ExpressionStatementNode));

            var exprnode = (ExpressionStatementNode)node;

            Assert.IsNotNull(exprnode.ExpressionNode);
            Assert.IsInstanceOfType(exprnode.ExpressionNode, typeof(BinaryNode));

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseAssigmentStatement()
        {
            Parser parser = new Parser("a = 1");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(AssignmentNode));

            var assignnode = (AssignmentNode)node;

            Assert.IsNotNull(assignnode.ExpressionNode);
            Assert.IsInstanceOfType(assignnode.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)assignnode.ExpressionNode).Value);

            Assert.IsNotNull(assignnode.TargetNode);
            Assert.IsInstanceOfType(assignnode.TargetNode, typeof(NameNode));
            Assert.AreEqual("a", ((NameNode)assignnode.TargetNode).Name);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseTwoAssigmentStatement()
        {
            Parser parser = new Parser("a = 1\r\nb=2");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(AssignmentNode));

            var assignnode = (AssignmentNode)node;

            Assert.IsNotNull(assignnode.ExpressionNode);
            Assert.IsInstanceOfType(assignnode.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)assignnode.ExpressionNode).Value);

            Assert.IsNotNull(assignnode.TargetNode);
            Assert.IsInstanceOfType(assignnode.TargetNode, typeof(NameNode));
            Assert.AreEqual("a", ((NameNode)assignnode.TargetNode).Name);

            node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(AssignmentNode));

            assignnode = (AssignmentNode)node;

            Assert.IsNotNull(assignnode.ExpressionNode);
            Assert.IsInstanceOfType(assignnode.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(2, ((ConstantNode)assignnode.ExpressionNode).Value);

            Assert.IsNotNull(assignnode.TargetNode);
            Assert.IsInstanceOfType(assignnode.TargetNode, typeof(NameNode));
            Assert.AreEqual("b", ((NameNode)assignnode.TargetNode).Name);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseSendStatement()
        {
            Parser parser = new Parser("a <- 1");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(SendNode));

            var sendnode = (SendNode)node;

            Assert.IsNotNull(sendnode.ExpressionNode);
            Assert.IsInstanceOfType(sendnode.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)sendnode.ExpressionNode).Value);

            Assert.IsNotNull(sendnode.TargetNode);
            Assert.IsInstanceOfType(sendnode.TargetNode, typeof(NameNode));
            Assert.AreEqual("a", ((NameNode)sendnode.TargetNode).Name);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseDeferStatement()
        {
            Parser parser = new Parser("defer a");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(DeferNode));

            var defernode = (DeferNode)node;

            Assert.IsNotNull(defernode.ExpressionNode);
            Assert.IsInstanceOfType(defernode.ExpressionNode, typeof(NameNode));
            Assert.AreEqual("a", ((NameNode)defernode.ExpressionNode).Name);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseBlockStatement()
        {
            Parser parser = new Parser("{ a = 1 }");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(BlockNode));

            var blocknode = (BlockNode)node;

            Assert.IsNotNull(blocknode.Statements);
            Assert.AreEqual(1, blocknode.Statements.Count);
            Assert.IsInstanceOfType(blocknode.Statements[0], typeof(AssignmentNode));

            var assignmentnode = (AssignmentNode)blocknode.Statements[0];

            Assert.AreEqual(1, ((ConstantNode)assignmentnode.ExpressionNode).Value);

            Assert.IsNotNull(assignmentnode.TargetNode);
            Assert.IsInstanceOfType(assignmentnode.TargetNode, typeof(NameNode));
            Assert.AreEqual("a", ((NameNode)assignmentnode.TargetNode).Name);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseBlockWithTwoStatements()
        {
            Parser parser = new Parser("{ a = 1\r\n b = 2 }");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(BlockNode));

            var blocknode = (BlockNode)node;

            Assert.IsNotNull(blocknode.Statements);
            Assert.AreEqual(2, blocknode.Statements.Count);
            Assert.IsInstanceOfType(blocknode.Statements[0], typeof(AssignmentNode));
            Assert.IsInstanceOfType(blocknode.Statements[1], typeof(AssignmentNode));

            var assignmentnode = (AssignmentNode)blocknode.Statements[0];

            Assert.AreEqual(1, ((ConstantNode)assignmentnode.ExpressionNode).Value);

            Assert.IsNotNull(assignmentnode.TargetNode);
            Assert.IsInstanceOfType(assignmentnode.TargetNode, typeof(NameNode));
            Assert.AreEqual("a", ((NameNode)assignmentnode.TargetNode).Name);

            assignmentnode = (AssignmentNode)blocknode.Statements[1];

            Assert.AreEqual(2, ((ConstantNode)assignmentnode.ExpressionNode).Value);

            Assert.IsNotNull(assignmentnode.TargetNode);
            Assert.IsInstanceOfType(assignmentnode.TargetNode, typeof(NameNode));
            Assert.AreEqual("b", ((NameNode)assignmentnode.TargetNode).Name);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseConstants()
        {
            var values = new object[] { true, false, null };
            var text = "true false nil";

            Parser parser = new Parser(text);

            foreach (var value in values)
            {
                var node = parser.ParseExpressionNode();
                Assert.IsNotNull(node);
                Assert.IsInstanceOfType(node, typeof(ConstantNode));
                Assert.AreEqual(value, ((ConstantNode)node).Value);
            }

            Assert.IsNull(parser.ParseExpressionNode());
        }

        [TestMethod]
        public void ParseNilConstantWithType()
        {
            var values = new object[] { true, false, null };
            var text = "nil";

            Parser parser = new Parser(text);

            var node = parser.ParseExpressionNode();
            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(ConstantNode));
            Assert.AreEqual(null, ((ConstantNode)node).Value);
            Assert.AreSame(TypeInfo.Nil, ((ConstantNode)node).TypeInfo);

            Assert.IsNull(parser.ParseExpressionNode());
        }

        [TestMethod]
        public void ParseVarDeclarationWithInt32Type()
        {
            Parser parser = new Parser("var foo int32");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.AreEqual(TypeInfo.Int32, ((VarNode)node).TypeInfo);
            Assert.IsNull(((VarNode)node).ExpressionNode);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseVarDeclarationWithReal64Type()
        {
            Parser parser = new Parser("var foo real64");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.AreEqual(TypeInfo.Real64, ((VarNode)node).TypeInfo);
            Assert.IsNull(((VarNode)node).ExpressionNode);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseVarDeclarationWithBooleanType()
        {
            Parser parser = new Parser("var foo bool");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.AreEqual(TypeInfo.Bool, ((VarNode)node).TypeInfo);
            Assert.IsNull(((VarNode)node).ExpressionNode);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseVarDeclarationWithStringType()
        {
            Parser parser = new Parser("var foo string");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.AreEqual(TypeInfo.String, ((VarNode)node).TypeInfo);
            Assert.IsNull(((VarNode)node).ExpressionNode);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseVarDeclarationWithNilType()
        {
            Parser parser = new Parser("var foo nil");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.AreEqual(TypeInfo.Nil, ((VarNode)node).TypeInfo);
            Assert.IsNull(((VarNode)node).ExpressionNode);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseVarDeclarationWithSliceType()
        {
            Parser parser = new Parser("var foo []string");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.IsInstanceOfType(((VarNode)node).TypeInfo, typeof(SliceTypeInfo));
            Assert.AreEqual(TypeInfo.String, ((SliceTypeInfo)((VarNode)node).TypeInfo).TypeInfo);
            Assert.IsNull(((VarNode)node).ExpressionNode);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseVarDeclarationWithArrayType()
        {
            Parser parser = new Parser("var foo [10]string");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.IsInstanceOfType(((VarNode)node).TypeInfo, typeof(ArrayTypeInfo));
            Assert.AreEqual(TypeInfo.String, ((ArrayTypeInfo)((VarNode)node).TypeInfo).TypeInfo);
            Assert.IsNull(((VarNode)node).ExpressionNode);
            Assert.IsInstanceOfType(((ArrayTypeInfo)((VarNode)node).TypeInfo).LengthExpression, typeof(ConstantNode));
            Assert.AreEqual(10, ((ConstantNode)((ArrayTypeInfo)((VarNode)node).TypeInfo).LengthExpression).Value);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseVarDeclarationWithConstantExpression()
        {
            Parser parser = new Parser("var foo = 1");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.AreEqual(TypeInfo.Int32, ((VarNode)node).TypeInfo);
            Assert.IsNotNull(((VarNode)node).ExpressionNode);

            var expr = ((VarNode)node).ExpressionNode;

            Assert.IsInstanceOfType(expr, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)expr).Value);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseShortVarDeclarationWithConstantExpression()
        {
            Parser parser = new Parser("foo := 1");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.AreEqual(TypeInfo.Int32, ((VarNode)node).TypeInfo);
            Assert.IsNotNull(((VarNode)node).ExpressionNode);

            var expr = ((VarNode)node).ExpressionNode;

            Assert.IsInstanceOfType(expr, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)expr).Value);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseSimpleReturnWithExpression()
        {
            Parser parser = new Parser("return 2");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(ReturnNode));

            var rnode = (ReturnNode)node;

            Assert.IsNotNull(rnode.Expression);
            Assert.IsInstanceOfType(rnode.Expression, typeof(ConstantNode));
            Assert.AreEqual(2, ((ConstantNode)rnode.Expression).Value);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseSimpleReturnWithoutExpression()
        {
            Parser parser = new Parser("return");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(ReturnNode));

            var rnode = (ReturnNode)node;

            Assert.IsNull(rnode.Expression);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseSimpleReturnWithoutExpressionAndWithSemicolon()
        {
            Parser parser = new Parser("return;");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(ReturnNode));

            var rnode = (ReturnNode)node;

            Assert.IsNull(rnode.Expression);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseBreakWithoutLabel()
        {
            Parser parser = new Parser("break");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(BreakNode));

            var bnode = (BreakNode)node;

            Assert.IsNull(bnode.Label);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseBreakWithoutLabelAndWithSemicolon()
        {
            Parser parser = new Parser("break;");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(BreakNode));

            var bnode = (BreakNode)node;

            Assert.IsNull(bnode.Label);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseBreakWithLabel()
        {
            Parser parser = new Parser("break Foo");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(BreakNode));

            var bnode = (BreakNode)node;

            Assert.AreEqual("Foo", bnode.Label);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseContinueWithoutLabel()
        {
            Parser parser = new Parser("continue");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(ContinueNode));

            var bnode = (ContinueNode)node;

            Assert.IsNull(bnode.Label);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseContinueWithoutLabelAndWithSemicolon()
        {
            Parser parser = new Parser("continue;");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(ContinueNode));

            var bnode = (ContinueNode)node;

            Assert.IsNull(bnode.Label);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseContinueWithLabel()
        {
            Parser parser = new Parser("continue Foo");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(ContinueNode));

            var bnode = (ContinueNode)node;

            Assert.AreEqual("Foo", bnode.Label);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseGoto()
        {
            Parser parser = new Parser("goto Error");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(GotoNode));

            var bnode = (GotoNode)node;

            Assert.AreEqual("Error", bnode.Label);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseGotoWithSemicolon()
        {
            Parser parser = new Parser("goto Error;");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(GotoNode));

            var gotonode = (GotoNode)node;

            Assert.AreEqual("Error", gotonode.Label);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParsePackage()
        {
            Parser parser = new Parser("package math");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(PackageNode));

            var pnode = (PackageNode)node;

            Assert.AreEqual("math", pnode.Name);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseImport()
        {
            Parser parser = new Parser("import \"fmt\"");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(ImportNode));

            var impnode = (ImportNode)node;

            Assert.IsNotNull(impnode.ExpressionNode);
            Assert.IsInstanceOfType(impnode.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual("fmt", ((ConstantNode)impnode.ExpressionNode).Value);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseGo()
        {
            Parser parser = new Parser("go 1");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(GoNode));

            var gonode = (GoNode)node;

            Assert.IsNotNull(gonode.ExpressionNode);
            Assert.IsInstanceOfType(gonode.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)gonode.ExpressionNode).Value);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseGoWithSemicolon()
        {
            Parser parser = new Parser("go 2;");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(GoNode));

            var gonode = (GoNode)node;

            Assert.IsNotNull(gonode.ExpressionNode);
            Assert.IsInstanceOfType(gonode.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(2, ((ConstantNode)gonode.ExpressionNode).Value);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseSimpleIf()
        {
            Parser parser = new Parser("if x == 1 { y = 2 }");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(IfNode));

            var ifnode = (IfNode)node;

            Assert.IsNotNull(ifnode.Expression);
            Assert.IsInstanceOfType(ifnode.Expression, typeof(BinaryNode));

            var expr = (BinaryNode)ifnode.Expression;

            Assert.IsNotNull(expr.LeftNode);
            Assert.IsNotNull(expr.RightNode);
            Assert.AreEqual(expr.Operator, BinaryOperator.Equal);

            Assert.IsInstanceOfType(expr.LeftNode, typeof(NameNode));
            Assert.AreEqual("x", ((NameNode)expr.LeftNode).Name);
            Assert.IsInstanceOfType(expr.RightNode, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)expr.RightNode).Value);

            Assert.IsNotNull(ifnode.BlockNode);

            var block = ifnode.BlockNode;

            Assert.AreEqual(1, block.Statements.Count);

            var stmt = block.Statements[0];

            Assert.IsInstanceOfType(stmt, typeof(AssignmentNode));

            var assign = (AssignmentNode)stmt;

            Assert.IsInstanceOfType(assign.TargetNode, typeof(NameNode));
            Assert.AreEqual("y", ((NameNode)assign.TargetNode).Name);
            Assert.IsInstanceOfType(assign.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(2, ((ConstantNode)assign.ExpressionNode).Value);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseSimpleFor()
        {
            Parser parser = new Parser("for x < 1 { x = 1 }");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(ForNode));

            var fornode = (ForNode)node;

            Assert.IsNotNull(fornode.Expression);
            Assert.IsInstanceOfType(fornode.Expression, typeof(BinaryNode));

            var expr = (BinaryNode)fornode.Expression;

            Assert.IsNotNull(expr.LeftNode);
            Assert.IsNotNull(expr.RightNode);
            Assert.AreEqual(expr.Operator, BinaryOperator.Less);

            Assert.IsInstanceOfType(expr.LeftNode, typeof(NameNode));
            Assert.AreEqual("x", ((NameNode)expr.LeftNode).Name);
            Assert.IsInstanceOfType(expr.RightNode, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)expr.RightNode).Value);

            Assert.IsNotNull(fornode.BlockNode);

            var block = fornode.BlockNode;

            Assert.AreEqual(1, block.Statements.Count);

            var stmt = block.Statements[0];

            Assert.IsInstanceOfType(stmt, typeof(AssignmentNode));

            var assign = (AssignmentNode)stmt;

            Assert.IsInstanceOfType(assign.TargetNode, typeof(NameNode));
            Assert.AreEqual("x", ((NameNode)assign.TargetNode).Name);
            Assert.IsInstanceOfType(assign.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)assign.ExpressionNode).Value);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseFuncWithoutParametersAndWithEmptyBody()
        {
            Parser parser = new Parser("func foo() { }");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(FuncNode));

            var fnode = (FuncNode)node;

            Assert.AreEqual("foo", fnode.Name);
            Assert.IsNotNull(fnode.Parameters);
            Assert.AreEqual(0, fnode.Parameters.Count);
            Assert.IsNotNull(fnode.BodyNode);
            Assert.IsInstanceOfType(fnode.BodyNode, typeof(BlockNode));

            var bnode = (BlockNode)fnode.BodyNode;

            Assert.IsNotNull(bnode.Statements);
            Assert.AreEqual(0, bnode.Statements.Count);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseFuncWithoutParametersAndWithAssignmentInBody()
        {
            Parser parser = new Parser("func foo() { a = 1 }");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(FuncNode));

            var fnode = (FuncNode)node;

            Assert.AreEqual("foo", fnode.Name);
            Assert.IsNotNull(fnode.BodyNode);
            Assert.IsInstanceOfType(fnode.BodyNode, typeof(BlockNode));

            var bnode = (BlockNode)fnode.BodyNode;

            Assert.IsNotNull(bnode.Statements);
            Assert.AreEqual(1, bnode.Statements.Count);

            var stmt = bnode.Statements[0];

            Assert.IsInstanceOfType(stmt, typeof(AssignmentNode));

            var astmt = (AssignmentNode)stmt;

            Assert.IsInstanceOfType(astmt.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)astmt.ExpressionNode).Value);

            Assert.IsInstanceOfType(astmt.TargetNode, typeof(NameNode));
            Assert.AreEqual("a", ((NameNode)astmt.TargetNode).Name);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseFuncWithoutParametersAndWithAssignmentInBodyUsingNewLines()
        {
            Parser parser = new Parser("func foo() { \r\n a = 1 \r\n}");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(FuncNode));

            var fnode = (FuncNode)node;

            Assert.AreEqual("foo", fnode.Name);
            Assert.IsNotNull(fnode.BodyNode);
            Assert.IsInstanceOfType(fnode.BodyNode, typeof(BlockNode));

            var bnode = (BlockNode)fnode.BodyNode;

            Assert.IsNotNull(bnode.Statements);
            Assert.AreEqual(1, bnode.Statements.Count);

            var stmt = bnode.Statements[0];

            Assert.IsInstanceOfType(stmt, typeof(AssignmentNode));

            var astmt = (AssignmentNode)stmt;

            Assert.IsInstanceOfType(astmt.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)astmt.ExpressionNode).Value);

            Assert.IsInstanceOfType(astmt.TargetNode, typeof(NameNode));
            Assert.AreEqual("a", ((NameNode)astmt.TargetNode).Name);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void RaiseIfNoBlockInFunc()
        {
            Parser parser = new Parser("func foo() x = 1");

            try
            {
                parser.ParseStatementNode();
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ParserException));
                Assert.AreEqual("Expected '{'", ex.Message);
            }
        }

        [TestMethod]
        public void ParseCall()
        {
            Parser parser = new Parser("foo()");

            var node = parser.ParseExpressionNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(CallNode));

            var cnode = (CallNode)node;

            Assert.AreEqual("foo", cnode.Name);
            Assert.IsNotNull(cnode.Arguments);
            Assert.AreEqual(0, cnode.Arguments.Count);

            Assert.IsNull(parser.ParseExpressionNode());
        }

        [TestMethod]
        public void ParseCallWithArgument()
        {
            Parser parser = new Parser("foo(1)");

            var node = parser.ParseExpressionNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(CallNode));

            var cnode = (CallNode)node;

            Assert.AreEqual("foo", cnode.Name);
            Assert.IsNotNull(cnode.Arguments);
            Assert.AreEqual(1, cnode.Arguments.Count);
            Assert.IsInstanceOfType(cnode.Arguments[0], typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)cnode.Arguments[0]).Value);

            Assert.IsNull(parser.ParseExpressionNode());
        }

        [TestMethod]
        public void ParseCallWithThreeArguments()
        {
            Parser parser = new Parser("foo(1,a,3)");

            var node = parser.ParseExpressionNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(CallNode));

            var cnode = (CallNode)node;

            Assert.AreEqual("foo", cnode.Name);
            Assert.IsNotNull(cnode.Arguments);
            Assert.AreEqual(3, cnode.Arguments.Count);
            Assert.IsInstanceOfType(cnode.Arguments[0], typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)cnode.Arguments[0]).Value);
            Assert.IsInstanceOfType(cnode.Arguments[1], typeof(NameNode));
            Assert.AreEqual("a", ((NameNode)cnode.Arguments[1]).Name);
            Assert.IsInstanceOfType(cnode.Arguments[2], typeof(ConstantNode));
            Assert.AreEqual(3, ((ConstantNode)cnode.Arguments[2]).Value);

            Assert.IsNull(parser.ParseExpressionNode());
        }

        [TestMethod]
        public void ParseIndexed()
        {
            Parser parser = new Parser("foo[1]");

            var node = parser.ParseExpressionNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(IndexedNode));

            var inode = (IndexedNode)node;

            Assert.AreEqual("foo", inode.Name);
            Assert.IsNotNull(inode.Index);
            Assert.IsInstanceOfType(inode.Index, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)inode.Index).Value);

            Assert.IsNull(parser.ParseExpressionNode());
        }

        [TestMethod]
        public void ParseSimpleSlice()
        {
            Parser parser = new Parser("foo[1:2]");

            var node = parser.ParseExpressionNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(SliceNode));

            var snode = (SliceNode)node;

            Assert.AreEqual("foo", snode.Name);
            Assert.IsNotNull(snode.Low);
            Assert.IsInstanceOfType(snode.Low, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)snode.Low).Value);
            Assert.IsNotNull(snode.High);
            Assert.IsInstanceOfType(snode.High, typeof(ConstantNode));
            Assert.AreEqual(2, ((ConstantNode)snode.High).Value);

            Assert.IsNull(parser.ParseExpressionNode());
        }

        [TestMethod]
        public void ParseSimpleSliceWithMissingHigh()
        {
            Parser parser = new Parser("foo[1:]");

            var node = parser.ParseExpressionNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(SliceNode));

            var snode = (SliceNode)node;

            Assert.AreEqual("foo", snode.Name);
            Assert.IsNotNull(snode.Low);
            Assert.IsInstanceOfType(snode.Low, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)snode.Low).Value);
            Assert.IsNull(snode.High);

            Assert.IsNull(parser.ParseExpressionNode());
        }

        [TestMethod]
        public void ParseSimpleSliceWithMissingLow()
        {
            Parser parser = new Parser("foo[:2]");

            var node = parser.ParseExpressionNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(SliceNode));

            var snode = (SliceNode)node;

            Assert.AreEqual("foo", snode.Name);
            Assert.IsNull(snode.Low);
            Assert.IsNotNull(snode.High);
            Assert.IsInstanceOfType(snode.High, typeof(ConstantNode));
            Assert.AreEqual(2, ((ConstantNode)snode.High).Value);

            Assert.IsNull(parser.ParseExpressionNode());
        }

        private static void ParseBinaryOperation(string text, BinaryOperator oper, int leftvalue, int rightvalue)
        {
            Parser parser = new Parser(text);

            var result = parser.ParseExpressionNode();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BinaryNode));

            var bnode = (BinaryNode)result;

            Assert.AreEqual(oper, bnode.Operator);
            Assert.IsNotNull(bnode.LeftNode);
            Assert.IsInstanceOfType(bnode.LeftNode, typeof(ConstantNode));
            Assert.AreEqual(leftvalue, ((ConstantNode)bnode.LeftNode).Value);
            Assert.IsNotNull(bnode.RightNode);
            Assert.IsInstanceOfType(bnode.RightNode, typeof(ConstantNode));
            Assert.AreEqual(rightvalue, ((ConstantNode)bnode.RightNode).Value);

            Assert.IsNull(parser.ParseExpressionNode());
        }

        private static void ParseBinaryOperation(string text, BinaryOperator oper, int leftvalue, int middlevalue, BinaryOperator oper2, int rightvalue)
        {
            Parser parser = new Parser(text);

            var result = parser.ParseExpressionNode();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BinaryNode));

            var bnode = (BinaryNode)result;

            Assert.AreEqual(oper2, bnode.Operator);
            Assert.IsNotNull(bnode.LeftNode);
            Assert.IsInstanceOfType(bnode.LeftNode, typeof(BinaryNode));

            var bnode2 = (BinaryNode)bnode.LeftNode;

            Assert.AreEqual(oper, bnode2.Operator);
            Assert.IsNotNull(bnode2.LeftNode);
            Assert.IsInstanceOfType(bnode2.LeftNode, typeof(ConstantNode));
            Assert.AreEqual(leftvalue, ((ConstantNode)bnode2.LeftNode).Value);
            Assert.IsNotNull(bnode2.RightNode);
            Assert.IsInstanceOfType(bnode2.RightNode, typeof(ConstantNode));
            Assert.AreEqual(middlevalue, ((ConstantNode)bnode2.RightNode).Value);

            Assert.IsNotNull(bnode.RightNode);
            Assert.IsInstanceOfType(bnode.RightNode, typeof(ConstantNode));
            Assert.AreEqual(rightvalue, ((ConstantNode)bnode.RightNode).Value);

            Assert.IsNull(parser.ParseExpressionNode());
        }

        private static void ParseBinaryOperation(string text, BinaryOperator oper, bool leftvalue, bool middlevalue, BinaryOperator oper2, bool rightvalue)
        {
            Parser parser = new Parser(text);

            var result = parser.ParseExpressionNode();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BinaryNode));

            var bnode = (BinaryNode)result;

            Assert.AreEqual(oper2, bnode.Operator);
            Assert.IsNotNull(bnode.LeftNode);
            Assert.IsInstanceOfType(bnode.LeftNode, typeof(BinaryNode));

            var bnode2 = (BinaryNode)bnode.LeftNode;

            Assert.AreEqual(oper, bnode2.Operator);
            Assert.IsNotNull(bnode2.LeftNode);
            Assert.IsInstanceOfType(bnode2.LeftNode, typeof(ConstantNode));
            Assert.AreEqual(leftvalue, ((ConstantNode)bnode2.LeftNode).Value);
            Assert.IsNotNull(bnode2.RightNode);
            Assert.IsInstanceOfType(bnode2.RightNode, typeof(ConstantNode));
            Assert.AreEqual(middlevalue, ((ConstantNode)bnode2.RightNode).Value);

            Assert.IsNotNull(bnode.RightNode);
            Assert.IsInstanceOfType(bnode.RightNode, typeof(ConstantNode));
            Assert.AreEqual(rightvalue, ((ConstantNode)bnode.RightNode).Value);

            Assert.IsNull(parser.ParseExpressionNode());
        }

        private static void ParseBinaryOperation(string text, BinaryOperator oper, int leftvalue, BinaryOperator oper2, int middlevalue, int rightvalue)
        {
            Parser parser = new Parser(text);

            var result = parser.ParseExpressionNode();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BinaryNode));

            var bnode = (BinaryNode)result;

            Assert.AreEqual(oper, bnode.Operator);

            Assert.IsNotNull(bnode.LeftNode);
            Assert.IsInstanceOfType(bnode.LeftNode, typeof(ConstantNode));
            Assert.AreEqual(leftvalue, ((ConstantNode)bnode.LeftNode).Value);
            
            Assert.IsNotNull(bnode.RightNode);
            Assert.IsInstanceOfType(bnode.RightNode, typeof(BinaryNode));

            var bnode2 = (BinaryNode)bnode.RightNode;

            Assert.AreEqual(oper2, bnode2.Operator);
            Assert.IsNotNull(bnode2.LeftNode);
            Assert.IsInstanceOfType(bnode2.LeftNode, typeof(ConstantNode));
            Assert.AreEqual(middlevalue, ((ConstantNode)bnode2.LeftNode).Value);
            Assert.IsNotNull(bnode2.RightNode);
            Assert.IsInstanceOfType(bnode2.RightNode, typeof(ConstantNode));
            Assert.AreEqual(rightvalue, ((ConstantNode)bnode2.RightNode).Value);

            Assert.IsNull(parser.ParseExpressionNode());
        }

        private static void ParseBinaryOperation(string text, BinaryOperator oper, bool leftvalue, BinaryOperator oper2, bool middlevalue, bool rightvalue)
        {
            Parser parser = new Parser(text);

            var result = parser.ParseExpressionNode();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BinaryNode));

            var bnode = (BinaryNode)result;

            Assert.AreEqual(oper, bnode.Operator);

            Assert.IsNotNull(bnode.LeftNode);
            Assert.IsInstanceOfType(bnode.LeftNode, typeof(ConstantNode));
            Assert.AreEqual(leftvalue, ((ConstantNode)bnode.LeftNode).Value);

            Assert.IsNotNull(bnode.RightNode);
            Assert.IsInstanceOfType(bnode.RightNode, typeof(BinaryNode));

            var bnode2 = (BinaryNode)bnode.RightNode;

            Assert.AreEqual(oper2, bnode2.Operator);
            Assert.IsNotNull(bnode2.LeftNode);
            Assert.IsInstanceOfType(bnode2.LeftNode, typeof(ConstantNode));
            Assert.AreEqual(middlevalue, ((ConstantNode)bnode2.LeftNode).Value);
            Assert.IsNotNull(bnode2.RightNode);
            Assert.IsInstanceOfType(bnode2.RightNode, typeof(ConstantNode));
            Assert.AreEqual(rightvalue, ((ConstantNode)bnode2.RightNode).Value);

            Assert.IsNull(parser.ParseExpressionNode());
        }
    }
}
