using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace SimpleCalc
{
    public class SimpleCalcRunner : SimpleCalcBaseVisitor<int>
    {
        public override int VisitOperation(SimpleCalcParser.OperationContext ctx)
        {
            int operand0 = Visit(ctx.expr(0));
            int operand1 = Visit(ctx.expr(1));
            switch (ctx.OP().GetText().ToLower())
            {
                case "add": return operand0 + operand1;
                case "multiply": return operand0 * operand1;
                case "sub": return operand0 - operand1;
                default: throw new UnknownOperandException(ctx.OP().GetText());
            }
        }

        public override int VisitIntLiteral(SimpleCalcParser.IntLiteralContext ctx)
        {
            return int.Parse(ctx.INT().GetText());
        }
        public class UnknownOperandException : Exception
        {
            public UnknownOperandException(String operand) :
                base("Unknown operand " + operand)
            { }
        }
    }
}