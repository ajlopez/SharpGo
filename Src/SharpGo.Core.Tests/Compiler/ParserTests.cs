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
        public void ParseVarDeclaration()
        {
            Parser parser = new Parser("var foo");

            var node = parser.ParseStatementNode();

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(node, typeof(VarNode));
            Assert.AreEqual("foo", ((VarNode)node).Name);
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
            Assert.IsNotNull(((VarNode)node).ExpressionNode);

            var expr = ((VarNode)node).ExpressionNode;

            Assert.IsInstanceOfType(expr, typeof(ConstantNode));
            Assert.AreEqual(1, ((ConstantNode)expr).Value);

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
