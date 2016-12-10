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
        public void ParseQualifiedNameAsDotNode()
        {
            Parser parser = new Parser("math.foo");

            var node = parser.ParseExpressionNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(DotNode));

            var dnode = (DotNode)node;

            Assert.IsNotNull(dnode.ExpressionNode);
            Assert.IsInstanceOfType(dnode.ExpressionNode, typeof(NameNode));
            Assert.AreEqual("math", ((NameNode)dnode.ExpressionNode).Name);
            Assert.AreEqual("foo", dnode.Name);

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
            ParseBinaryOperation("5%2", BinaryOperator.Mod, 5, 2);
            ParseBinaryOperation("5<<2", BinaryOperator.LeftShift, 5, 2);
            ParseBinaryOperation("5>>2", BinaryOperator.RightShift, 5, 2);
            ParseBinaryOperation("5&2", BinaryOperator.BitwiseAnd, 5, 2);
            ParseBinaryOperation("5|2", BinaryOperator.BitwiseOr, 5, 2);
            ParseBinaryOperation("5^2", BinaryOperator.BitwiseXor, 5, 2);
            ParseBinaryOperation("5&^2", BinaryOperator.BitwiseAndNot, 5, 2);
        }

        [TestMethod]
        public void ParseArithmeticBinaryOperationsInParens()
        {
            ParseBinaryOperation("(5+6)", BinaryOperator.Add, 5, 6);
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

            Assert.AreEqual(AssignmentOperator.Set, assignnode.Operator);
            Assert.IsNotNull(assignnode.ExpressionNode);
            Assert.IsInstanceOfType(assignnode.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)assignnode.ExpressionNode).Value);

            Assert.IsNotNull(assignnode.TargetNode);
            Assert.IsInstanceOfType(assignnode.TargetNode, typeof(NameNode));
            Assert.AreEqual("a", ((NameNode)assignnode.TargetNode).Name);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseAssigmentStatementWithAddOperator()
        {
            Parser parser = new Parser("a += 1");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(AssignmentNode));

            var assignnode = (AssignmentNode)node;

            Assert.AreEqual(AssignmentOperator.Add, assignnode.Operator);
            Assert.IsNotNull(assignnode.ExpressionNode);
            Assert.IsInstanceOfType(assignnode.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)assignnode.ExpressionNode).Value);

            Assert.IsNotNull(assignnode.TargetNode);
            Assert.IsInstanceOfType(assignnode.TargetNode, typeof(NameNode));
            Assert.AreEqual("a", ((NameNode)assignnode.TargetNode).Name);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseAssigmentStatementWithSubtractOperator()
        {
            Parser parser = new Parser("a -= 1");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(AssignmentNode));

            var assignnode = (AssignmentNode)node;

            Assert.AreEqual(AssignmentOperator.Subtract, assignnode.Operator);
            Assert.IsNotNull(assignnode.ExpressionNode);
            Assert.IsInstanceOfType(assignnode.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)assignnode.ExpressionNode).Value);

            Assert.IsNotNull(assignnode.TargetNode);
            Assert.IsInstanceOfType(assignnode.TargetNode, typeof(NameNode));
            Assert.AreEqual("a", ((NameNode)assignnode.TargetNode).Name);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseAssigmentStatementWithMultiplyOperator()
        {
            Parser parser = new Parser("a *= 2");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(AssignmentNode));

            var assignnode = (AssignmentNode)node;

            Assert.AreEqual(AssignmentOperator.Multiply, assignnode.Operator);
            Assert.IsNotNull(assignnode.ExpressionNode);
            Assert.IsInstanceOfType(assignnode.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(2, ((ConstantNode)assignnode.ExpressionNode).Value);

            Assert.IsNotNull(assignnode.TargetNode);
            Assert.IsInstanceOfType(assignnode.TargetNode, typeof(NameNode));
            Assert.AreEqual("a", ((NameNode)assignnode.TargetNode).Name);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseAssigmentStatementWithDivideOperator()
        {
            Parser parser = new Parser("a /= 2");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(AssignmentNode));

            var assignnode = (AssignmentNode)node;

            Assert.AreEqual(AssignmentOperator.Divide, assignnode.Operator);
            Assert.IsNotNull(assignnode.ExpressionNode);
            Assert.IsInstanceOfType(assignnode.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(2, ((ConstantNode)assignnode.ExpressionNode).Value);

            Assert.IsNotNull(assignnode.TargetNode);
            Assert.IsInstanceOfType(assignnode.TargetNode, typeof(NameNode));
            Assert.AreEqual("a", ((NameNode)assignnode.TargetNode).Name);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseAssigmentStatementWithModulusOperator()
        {
            Parser parser = new Parser("a %= 2");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(AssignmentNode));

            var assignnode = (AssignmentNode)node;

            Assert.AreEqual(AssignmentOperator.Modulus, assignnode.Operator);
            Assert.IsNotNull(assignnode.ExpressionNode);
            Assert.IsInstanceOfType(assignnode.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(2, ((ConstantNode)assignnode.ExpressionNode).Value);

            Assert.IsNotNull(assignnode.TargetNode);
            Assert.IsInstanceOfType(assignnode.TargetNode, typeof(NameNode));
            Assert.AreEqual("a", ((NameNode)assignnode.TargetNode).Name);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseAssigmentStatementWithLeftShiftOperator()
        {
            Parser parser = new Parser("a <<= 2");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(AssignmentNode));

            var assignnode = (AssignmentNode)node;

            Assert.AreEqual(AssignmentOperator.LeftShift, assignnode.Operator);
            Assert.IsNotNull(assignnode.ExpressionNode);
            Assert.IsInstanceOfType(assignnode.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(2, ((ConstantNode)assignnode.ExpressionNode).Value);

            Assert.IsNotNull(assignnode.TargetNode);
            Assert.IsInstanceOfType(assignnode.TargetNode, typeof(NameNode));
            Assert.AreEqual("a", ((NameNode)assignnode.TargetNode).Name);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseAssigmentStatementWithRightShiftOperator()
        {
            Parser parser = new Parser("a >>= 2");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(AssignmentNode));

            var assignnode = (AssignmentNode)node;

            Assert.AreEqual(AssignmentOperator.RightShift, assignnode.Operator);
            Assert.IsNotNull(assignnode.ExpressionNode);
            Assert.IsInstanceOfType(assignnode.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(2, ((ConstantNode)assignnode.ExpressionNode).Value);

            Assert.IsNotNull(assignnode.TargetNode);
            Assert.IsInstanceOfType(assignnode.TargetNode, typeof(NameNode));
            Assert.AreEqual("a", ((NameNode)assignnode.TargetNode).Name);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseAssigmentStatementWithBitOrOperator()
        {
            Parser parser = new Parser("a |= 2");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(AssignmentNode));

            var assignnode = (AssignmentNode)node;

            Assert.AreEqual(AssignmentOperator.BitOr, assignnode.Operator);
            Assert.IsNotNull(assignnode.ExpressionNode);
            Assert.IsInstanceOfType(assignnode.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(2, ((ConstantNode)assignnode.ExpressionNode).Value);

            Assert.IsNotNull(assignnode.TargetNode);
            Assert.IsInstanceOfType(assignnode.TargetNode, typeof(NameNode));
            Assert.AreEqual("a", ((NameNode)assignnode.TargetNode).Name);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseAssigmentStatementWithBitAndOperator()
        {
            Parser parser = new Parser("a &= 2");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(AssignmentNode));

            var assignnode = (AssignmentNode)node;

            Assert.AreEqual(AssignmentOperator.BitAnd, assignnode.Operator);
            Assert.IsNotNull(assignnode.ExpressionNode);
            Assert.IsInstanceOfType(assignnode.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(2, ((ConstantNode)assignnode.ExpressionNode).Value);

            Assert.IsNotNull(assignnode.TargetNode);
            Assert.IsInstanceOfType(assignnode.TargetNode, typeof(NameNode));
            Assert.AreEqual("a", ((NameNode)assignnode.TargetNode).Name);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseAssigmentStatementWithBitXorOperator()
        {
            Parser parser = new Parser("a ^= 2");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(AssignmentNode));

            var assignnode = (AssignmentNode)node;

            Assert.AreEqual(AssignmentOperator.BitXor, assignnode.Operator);
            Assert.IsNotNull(assignnode.ExpressionNode);
            Assert.IsInstanceOfType(assignnode.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(2, ((ConstantNode)assignnode.ExpressionNode).Value);

            Assert.IsNotNull(assignnode.TargetNode);
            Assert.IsInstanceOfType(assignnode.TargetNode, typeof(NameNode));
            Assert.AreEqual("a", ((NameNode)assignnode.TargetNode).Name);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseAssigmentStatementWithBitClearOperator()
        {
            Parser parser = new Parser("a &^= 2");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(AssignmentNode));

            var assignnode = (AssignmentNode)node;

            Assert.AreEqual(AssignmentOperator.BitClear, assignnode.Operator);
            Assert.IsNotNull(assignnode.ExpressionNode);
            Assert.IsInstanceOfType(assignnode.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(2, ((ConstantNode)assignnode.ExpressionNode).Value);

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
        public void ParseFallThroughStatement()
        {
            Parser parser = new Parser("fallthrough");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(FallthroughNode));

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseEmptyStatement()
        {
            Parser parser = new Parser("\r\n");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(EmptyNode));

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseEmptyStatementWithSemicolon()
        {
            Parser parser = new Parser(";");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(EmptyNode));

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
        public void ParseVarDeclarationWithInt16Type()
        {
            Parser parser = new Parser("var foo int16");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.AreEqual(TypeInfo.Int16, ((VarNode)node).TypeInfo);
            Assert.IsNull(((VarNode)node).ExpressionNode);

            Assert.IsNull(parser.ParseStatementNode());
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
        public void ParseVarDeclarationWithRuneType()
        {
            Parser parser = new Parser("var foo rune");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.AreEqual(TypeInfo.Int32, ((VarNode)node).TypeInfo);
            Assert.IsNull(((VarNode)node).ExpressionNode);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseVarDeclarationWithIntType()
        {
            Parser parser = new Parser("var foo int");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.AreEqual(TypeInfo.Int, ((VarNode)node).TypeInfo);
            Assert.IsNull(((VarNode)node).ExpressionNode);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseVarDeclarationWithUIntType()
        {
            Parser parser = new Parser("var foo uint");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.AreEqual(TypeInfo.UInt, ((VarNode)node).TypeInfo);
            Assert.IsNull(((VarNode)node).ExpressionNode);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseVarDeclarationWithInt64Type()
        {
            Parser parser = new Parser("var foo int64");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.AreEqual(TypeInfo.Int64, ((VarNode)node).TypeInfo);
            Assert.IsNull(((VarNode)node).ExpressionNode);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseVarDeclarationWithFloat32Type()
        {
            Parser parser = new Parser("var foo float32");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.AreEqual(TypeInfo.Float32, ((VarNode)node).TypeInfo);
            Assert.IsNull(((VarNode)node).ExpressionNode);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseVarDeclarationWithFloat64Type()
        {
            Parser parser = new Parser("var foo float64");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.AreEqual(TypeInfo.Float64, ((VarNode)node).TypeInfo);
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
        public void ParseVarDeclarationWithDualChannelType()
        {
            Parser parser = new Parser("var foo chan int");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.IsInstanceOfType(((VarNode)node).TypeInfo, typeof(ChannelTypeInfo));
            Assert.AreEqual(TypeInfo.Int, ((ChannelTypeInfo)((VarNode)node).TypeInfo).ReceiveTypeInfo);
            Assert.AreEqual(TypeInfo.Int, ((ChannelTypeInfo)((VarNode)node).TypeInfo).SendTypeInfo);
            Assert.IsNull(((VarNode)node).ExpressionNode);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseVarDeclarationWithSendChannelType()
        {
            Parser parser = new Parser("var foo chan<- int");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.IsInstanceOfType(((VarNode)node).TypeInfo, typeof(ChannelTypeInfo));
            Assert.IsNull(((ChannelTypeInfo)((VarNode)node).TypeInfo).ReceiveTypeInfo);
            Assert.AreEqual(TypeInfo.Int, ((ChannelTypeInfo)((VarNode)node).TypeInfo).SendTypeInfo);
            Assert.IsNull(((VarNode)node).ExpressionNode);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseVarDeclarationWithReceiveChannelType()
        {
            Parser parser = new Parser("var foo <-chan int");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.IsInstanceOfType(((VarNode)node).TypeInfo, typeof(ChannelTypeInfo));
            Assert.IsNull(((ChannelTypeInfo)((VarNode)node).TypeInfo).SendTypeInfo);
            Assert.AreEqual(TypeInfo.Int, ((ChannelTypeInfo)((VarNode)node).TypeInfo).ReceiveTypeInfo);
            Assert.IsNull(((VarNode)node).ExpressionNode);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseVarDeclarationWithMapType()
        {
            Parser parser = new Parser("var foo map[string]int");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.IsInstanceOfType(((VarNode)node).TypeInfo, typeof(MapTypeInfo));
            Assert.AreEqual(TypeInfo.String, ((MapTypeInfo)((VarNode)node).TypeInfo).KeyTypeInfo);
            Assert.AreEqual(TypeInfo.Int, ((MapTypeInfo)((VarNode)node).TypeInfo).ElementTypeInfo);
            Assert.IsNull(((VarNode)node).ExpressionNode);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseVarDeclarationWithPointerType()
        {
            Parser parser = new Parser("var foo *string");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.IsInstanceOfType(((VarNode)node).TypeInfo, typeof(PointerTypeInfo));
            Assert.AreEqual(TypeInfo.String, ((PointerTypeInfo)((VarNode)node).TypeInfo).TypeInfo);
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
        public void RaiseIfNoTypeInfoInArrayVar()
        {
            Parser parser = new Parser("var foo [10]");

            try
            {
                parser.ParseStatementNode();
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ParserException));
                Assert.AreEqual("Expected type info", ex.Message);
            }
        }

        [TestMethod]
        public void ParseVarDeclarationWithComplex64Type()
        {
            Parser parser = new Parser("var foo complex64");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.AreEqual(TypeInfo.Complex64, ((VarNode)node).TypeInfo);
            Assert.IsNull(((VarNode)node).ExpressionNode);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseVarDeclarationWithComplex128Type()
        {
            Parser parser = new Parser("var foo complex128");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.AreEqual(TypeInfo.Complex128, ((VarNode)node).TypeInfo);
            Assert.IsNull(((VarNode)node).ExpressionNode);

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
        public void ParseVarDeclarationWithBinaryExpression()
        {
            Parser parser = new Parser("var foo = 1 + 2");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.AreEqual(TypeInfo.Int32, ((VarNode)node).TypeInfo);
            Assert.IsNotNull(((VarNode)node).ExpressionNode);

            var expr = ((VarNode)node).ExpressionNode;

            Assert.IsInstanceOfType(expr, typeof(BinaryNode));

            var lnode = ((BinaryNode)expr).LeftNode;
            var rnode = ((BinaryNode)expr).RightNode;

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseConsDeclarationWithConstantExpression()
        {
            Parser parser = new Parser("const foo = 1");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(ConstNode));
            Assert.AreEqual("foo", ((ConstNode)node).Name);
            Assert.AreEqual(TypeInfo.Int32, ((ConstNode)node).TypeInfo);
            Assert.IsNotNull(((ConstNode)node).ExpressionNode);

            var expr = ((ConstNode)node).ExpressionNode;

            Assert.IsInstanceOfType(expr, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)expr).Value);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseConsDeclarationWithBinaryExpression()
        {
            Parser parser = new Parser("const foo = 1 + 2");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(ConstNode));
            Assert.AreEqual("foo", ((ConstNode)node).Name);
            Assert.AreEqual(TypeInfo.Int32, ((ConstNode)node).TypeInfo);
            Assert.IsNotNull(((ConstNode)node).ExpressionNode);

            var expr = ((ConstNode)node).ExpressionNode;

            Assert.IsInstanceOfType(expr, typeof(BinaryNode));

            var lnode = ((BinaryNode)expr).LeftNode;
            var rnode = ((BinaryNode)expr).RightNode;

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseConversionExpression()
        {
            Parser parser = new Parser("string(1)");

            var node = parser.ParseExpressionNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(ConversionNode));
            Assert.AreEqual(TypeInfo.String, ((ConversionNode)node).TypeInfo);
            Assert.IsNotNull(((ConversionNode)node).ExpressionNode);

            var expr = ((ConversionNode)node).ExpressionNode;

            Assert.IsInstanceOfType(expr, typeof(ConstantNode));

            Assert.AreEqual(1, ((ConstantNode)expr).Value);
        }

        [TestMethod]
        public void ParseConversionToChannelExpression()
        {
            Parser parser = new Parser("(<-chan int)(x)");

            var node = parser.ParseExpressionNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(ConversionNode));
            Assert.IsInstanceOfType(((ConversionNode)node).TypeInfo, typeof(ChannelTypeInfo));

            var ti = (ChannelTypeInfo)((ConversionNode)node).TypeInfo;
            Assert.IsNull(ti.SendTypeInfo);
            Assert.AreEqual(TypeInfo.Int, ti.ReceiveTypeInfo);

            var expr = ((ConversionNode)node).ExpressionNode;

            Assert.IsInstanceOfType(expr, typeof(NameNode));

            Assert.AreEqual("x", ((NameNode)expr).Name);
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
        public void ParseShortVarDeclarationWithTypeInfoAndConstantExpression()
        {
            Parser parser = new Parser("foo := int 1");

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
        public void ParseShortVarDeclarationWithSliceTypeInfoAndListExpression()
        {
            Parser parser = new Parser("foo := []int { 1, 2 }");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
            Assert.IsInstanceOfType(((VarNode)node).TypeInfo, typeof(SliceTypeInfo));
            Assert.IsNotNull(((VarNode)node).ExpressionNode);

            var expr = ((VarNode)node).ExpressionNode;

            Assert.IsInstanceOfType(expr, typeof(ExpressionBlockNode));

            var bexpr = (ExpressionBlockNode)expr;

            Assert.IsNotNull(bexpr.Expressions);
            Assert.AreEqual(2, bexpr.Expressions.Count);

            Assert.IsNotNull(bexpr.Expressions[0]);
            Assert.IsInstanceOfType(bexpr.Expressions[0], typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)bexpr.Expressions[0]).Value);

            Assert.IsNotNull(bexpr.Expressions[1]);
            Assert.IsInstanceOfType(bexpr.Expressions[1], typeof(ConstantNode));
            Assert.AreEqual(2, ((ConstantNode)bexpr.Expressions[1]).Value);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseAliasTypeDeclaration()
        {
            Parser parser = new Parser("type Day int");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(AliasTypeNode));
            Assert.AreEqual("Day", ((AliasTypeNode)node).Name);
            Assert.AreEqual(TypeInfo.Int32, ((AliasTypeNode)node).TypeInfo);

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
        public void ParseLabel()
        {
            Parser parser = new Parser("Process: go 1");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(LabelNode));

            var lnode = (LabelNode)node;

            Assert.IsNotNull(lnode.StatementNode);
            Assert.IsInstanceOfType(lnode.StatementNode, typeof(GoNode));

            var gonode = (GoNode)lnode.StatementNode;

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

            Assert.IsNull(ifnode.Statement);
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

            Assert.IsNotNull(ifnode.ThenCommand);

            var block = ifnode.ThenCommand;

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
        public void ParseIfWithSimpleStatement()
        {
            Parser parser = new Parser("if x := 1; x == 1 { y = 2 }");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(IfNode));

            var ifnode = (IfNode)node;

            Assert.IsNotNull(ifnode.Statement);
            Assert.IsInstanceOfType(ifnode.Statement, typeof(VarNode));
            Assert.IsNotNull(ifnode.Expression);
            Assert.IsInstanceOfType(ifnode.Expression, typeof(BinaryNode));

            var stmt = (VarNode)ifnode.Statement;

            Assert.AreEqual("x", stmt.Name);
            Assert.IsNotNull(stmt.ExpressionNode);
            Assert.IsInstanceOfType(stmt.ExpressionNode, typeof(ConstantNode));

            Assert.AreEqual(1, ((ConstantNode)stmt.ExpressionNode).Value);

            var expr = (BinaryNode)ifnode.Expression;

            Assert.IsNotNull(expr.LeftNode);
            Assert.IsNotNull(expr.RightNode);
            Assert.AreEqual(expr.Operator, BinaryOperator.Equal);

            Assert.IsInstanceOfType(expr.LeftNode, typeof(NameNode));
            Assert.AreEqual("x", ((NameNode)expr.LeftNode).Name);
            Assert.IsInstanceOfType(expr.RightNode, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)expr.RightNode).Value);

            Assert.IsNotNull(ifnode.ThenCommand);

            var block = ifnode.ThenCommand;

            Assert.AreEqual(1, block.Statements.Count);

            var st = block.Statements[0];

            Assert.IsInstanceOfType(st, typeof(AssignmentNode));

            var assign = (AssignmentNode)st;

            Assert.IsInstanceOfType(assign.TargetNode, typeof(NameNode));
            Assert.AreEqual("y", ((NameNode)assign.TargetNode).Name);
            Assert.IsInstanceOfType(assign.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(2, ((ConstantNode)assign.ExpressionNode).Value);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseIfWithElse()
        {
            Parser parser = new Parser("if x := 1; x == 1 { y = 2; } else { y = 3; }");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(IfNode));

            var ifnode = (IfNode)node;

            Assert.IsNotNull(ifnode.Statement);
            Assert.IsInstanceOfType(ifnode.Statement, typeof(VarNode));
            Assert.IsNotNull(ifnode.Expression);
            Assert.IsInstanceOfType(ifnode.Expression, typeof(BinaryNode));

            var stmt = (VarNode)ifnode.Statement;

            Assert.AreEqual("x", stmt.Name);
            Assert.IsNotNull(stmt.ExpressionNode);
            Assert.IsInstanceOfType(stmt.ExpressionNode, typeof(ConstantNode));

            Assert.AreEqual(1, ((ConstantNode)stmt.ExpressionNode).Value);

            var expr = (BinaryNode)ifnode.Expression;

            Assert.IsNotNull(expr.LeftNode);
            Assert.IsNotNull(expr.RightNode);
            Assert.AreEqual(expr.Operator, BinaryOperator.Equal);

            Assert.IsInstanceOfType(expr.LeftNode, typeof(NameNode));
            Assert.AreEqual("x", ((NameNode)expr.LeftNode).Name);
            Assert.IsInstanceOfType(expr.RightNode, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)expr.RightNode).Value);

            Assert.IsNotNull(ifnode.ThenCommand);

            var block = ifnode.ThenCommand;

            Assert.AreEqual(1, block.Statements.Count);

            var st = block.Statements[0];

            Assert.IsInstanceOfType(st, typeof(AssignmentNode));

            var assign = (AssignmentNode)st;

            Assert.IsInstanceOfType(assign.TargetNode, typeof(NameNode));
            Assert.AreEqual("y", ((NameNode)assign.TargetNode).Name);
            Assert.IsInstanceOfType(assign.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(2, ((ConstantNode)assign.ExpressionNode).Value);

            Assert.IsNotNull(ifnode.ElseCommand);

            block = ifnode.ElseCommand;

            Assert.AreEqual(1, block.Statements.Count);

            st = block.Statements[0];

            Assert.IsInstanceOfType(st, typeof(AssignmentNode));

            assign = (AssignmentNode)st;

            Assert.IsInstanceOfType(assign.TargetNode, typeof(NameNode));
            Assert.AreEqual("y", ((NameNode)assign.TargetNode).Name);
            Assert.IsInstanceOfType(assign.ExpressionNode, typeof(ConstantNode));
            Assert.AreEqual(3, ((ConstantNode)assign.ExpressionNode).Value);

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

            Assert.IsNull(fornode.InitStatement);
            Assert.IsNull(fornode.PostStatement);

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
        public void ParseForWithInitAndPostStatements()
        {
            Parser parser = new Parser("for x := 1; x < 10; x = x + 1 { y = 1 }");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(ForNode));

            var fornode = (ForNode)node;

            Assert.IsNotNull(fornode.InitStatement);
            Assert.IsInstanceOfType(fornode.InitStatement, typeof(VarNode));
            Assert.IsNotNull(fornode.PostStatement);
            Assert.IsInstanceOfType(fornode.PostStatement, typeof(AssignmentNode));

            Assert.IsNotNull(fornode.Expression);
            Assert.IsInstanceOfType(fornode.Expression, typeof(BinaryNode));

            var expr = (BinaryNode)fornode.Expression;

            Assert.IsNotNull(expr.LeftNode);
            Assert.IsNotNull(expr.RightNode);
            Assert.AreEqual(expr.Operator, BinaryOperator.Less);

            Assert.IsInstanceOfType(expr.LeftNode, typeof(NameNode));
            Assert.AreEqual("x", ((NameNode)expr.LeftNode).Name);
            Assert.IsInstanceOfType(expr.RightNode, typeof(ConstantNode));
            Assert.AreEqual(10, ((ConstantNode)expr.RightNode).Value);

            Assert.IsNotNull(fornode.BlockNode);

            var block = fornode.BlockNode;

            Assert.AreEqual(1, block.Statements.Count);

            var stmt = block.Statements[0];

            Assert.IsInstanceOfType(stmt, typeof(AssignmentNode));

            var assign = (AssignmentNode)stmt;

            Assert.IsInstanceOfType(assign.TargetNode, typeof(NameNode));
            Assert.AreEqual("y", ((NameNode)assign.TargetNode).Name);
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

            Assert.IsNotNull(cnode.ExpressionNode);
            Assert.IsInstanceOfType(cnode.ExpressionNode, typeof(NameNode));
            Assert.AreEqual("foo", ((NameNode)cnode.ExpressionNode).Name);
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

            Assert.IsNotNull(cnode.ExpressionNode);
            Assert.IsInstanceOfType(cnode.ExpressionNode, typeof(NameNode));
            Assert.AreEqual("foo", ((NameNode)cnode.ExpressionNode).Name);
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

            Assert.IsNotNull(cnode.ExpressionNode);
            Assert.IsInstanceOfType(cnode.ExpressionNode, typeof(NameNode));
            Assert.AreEqual("foo", ((NameNode)cnode.ExpressionNode).Name);
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

        [TestMethod]
        public void ParseEmptyStruct()
        {
            Parser parser = new Parser("struct {}");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(StructNode));

            var snode = (StructNode)node;

            Assert.IsNotNull(snode.Members);
            Assert.AreEqual(0, snode.Members.Count);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseStructWithOneIntegerMember()
        {
            Parser parser = new Parser("struct {\n a int\n}");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(StructNode));

            var snode = (StructNode)node;

            Assert.IsNotNull(snode.Members);
            Assert.AreEqual(1, snode.Members.Count);

            var mnode = snode.Members[0];

            Assert.AreEqual("a", mnode.Name);
            Assert.AreSame(TypeInfo.Int, mnode.TypeInfo);

            Assert.IsNull(parser.ParseStatementNode());
        }

        [TestMethod]
        public void ParseStructWithOneAnonymousMember()
        {
            Parser parser = new Parser("struct {\n int\n}");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(StructNode));

            var snode = (StructNode)node;

            Assert.IsNotNull(snode.Members);
            Assert.AreEqual(1, snode.Members.Count);

            var mnode = snode.Members[0];

            Assert.IsNull(mnode.Name);
            Assert.AreSame(TypeInfo.Int, mnode.TypeInfo);

            Assert.IsNull(parser.ParseStatementNode());
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
