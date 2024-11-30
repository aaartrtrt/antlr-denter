grammar SimpleCalc;

tokens { INDENT, DEDENT }
@lexer::header {
  using AntlrDenter;
}
@lexer::members {
  private DenterHelper denter;
    
  public override IToken NextToken()
  {
      if (denter == null)
      {
          denter = DenterHelper.Builder()
              .Nl(NL)
              .Indent(SimpleCalcParser.INDENT)
              .Dedent(SimpleCalcParser.DEDENT)
              .PullToken(base.NextToken);
      }

      return denter.NextToken();
  }
}

expr: OP INDENT expr expr DEDENT # Operation
    | INT NL                     # IntLiteral
    ;

NL: ('\r'? '\n' ' '*); // note the ' '*
WS: [ \t]+ -> skip;
LINE_COMMENT: '//' ~[\r\n]* -> skip;

INT: [0-9]+;
OP: [a-zA-Z]+;
