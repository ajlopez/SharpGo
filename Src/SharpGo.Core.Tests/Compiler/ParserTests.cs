namespace SharpGo.Core.Tests.Compiler
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SharpGo.Core.Ast;
    using SharpGo.Core.Compiler;

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
    }
}
