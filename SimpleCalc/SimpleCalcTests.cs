using Antlr4.Runtime;
using NUnit.Framework;

namespace SimpleCalc
{
    public class SimpleCalcRunnerTests
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SimpleAdd()
        {
            Assert.AreEqual(5, RunCalc(
                @"
                    ADD
                        2
                        3
                "));
        }

        [Test]
        public void SimpleSub()
        {
            Assert.AreEqual(4, RunCalc(
                @"
                    SUB
                        8
                        4
                "));
        }

        [Test]
        public void SimpleMultiply()
        {
            Assert.AreEqual(10, RunCalc(
                @"
                    MULTIPLY
                        2
                        5
                "));
        }

        [Test]
        public void ExpressionWithMultipleOperations()
        {
            Assert.AreEqual(14, RunCalc(
                @"
                    ADD
                        2
                        MULTIPLY
                            3
                            4
                "));
        }

        [Test]
        public void IndentedExpressionWithSubtraction()
        {
            Assert.AreEqual(7, RunCalc(
                @"
                    ADD
                        SUB
                            10
                            MULTIPLY
                                2
                                2
                        1
                "));
        }

        private int RunCalc(string input)
        {
            // Parse the input.
            AntlrInputStream inputStream = new AntlrInputStream(input);
            SimpleCalcLexer lexer = new SimpleCalcLexer(inputStream);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            SimpleCalcParser parser = new SimpleCalcParser(tokens);
            var context = parser.expr();

            // Evaluate the parsed tree.
            SimpleCalcRunner runner = new SimpleCalcRunner();
            return runner.Visit(context);
        }
    }
}