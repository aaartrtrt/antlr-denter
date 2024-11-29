# ANTLR-Denter: Python-like indentation tokens for ANTLR4 C# Hosts
This project implements `INDENT` and `DEDENT` tokens in ANTLR v4 for Python-like scopes. This defines a `DenterHelper` that can be added to an ANTLR4 grammar.

This repository is a fork of [yshavit](https://github.com/yshavit)'s [ANTLR-Denter](https://github.com/yshavit/antlr-denter) project since the original repo is no longer maintained. This fork aims to maintain a C#-specific library using the official [Antlr4.Runtime.Standard](https://www.nuget.org/packages/Antlr4.Runtime.Standard#dependencies-body-tab) and support work with [RenDisco](https://github.com/aaartrtrt/rendisco).

## Overview

This is a plugin that is spliced into an ANTLR grammar's lexer, and allows that lexer to make use of `INDENT` and `DEDENT` to represent Python-like scope entry and termination.

## Features
### Using `INDENT` and `DEDENT` tokens in a parser

When `DenterHelper` injects `DEDENT` tokens, it will prefix any string of them with a single `NL`. A single `NL` is also inserted before the EOF token if there are no `DEDENT`s to insert (that is, if the last line of the source file is not indented). A `NL` is _not_ inserted before an `INDENT`, since indents always imply a newline before them (and thus make the newline token meaningless).

For example, given this input;

```
hello
  world
    universe
dolly
```

Would be parsed as;

```
"hello"
INDENT
  "world"
  INDENT
    "universe"
    NL
  DEDENT
DEDENT
"dolly"
NL
<eof>
```

This approach lets you define expressions, single-line statements, and block statements naturally.

1. Expressions in your parser grammar should not end in newlines. This makes compound expressions work naturally.
2. Single-line statements in your grammar _should_ end in newlines. For example, an assignment expression might be `identifier '=' expression NL`.
3. Blocks are bookended by INDENT and DEDENT, without mentioning extra newlines: `block: INDENT statement+ DEDENT`.
   - You should _not_ include a newline before the INDENT
   - An `if` would be something like `if expression ':' block`. (Note the lack of `NL` after the `:`.)

In the example above, `universe` and `dolly` represent simple expressions, and you can imagine that the grammar would contain something like `statement: expression NL | helloBlock;`.

### Handling "Half-DEDENTs"

What happens when you dedent to an indentation level that was never established?

    someStatement()
    if foo():
          if bar():
              fooAndBar()
       bogusLine()

Notice that `bogusLine()` doesn't match with any indentation level: it's more indented than `if foo()` but less than its first statement, `if bar()`.

This is invalid in Python. If you to run such a program, you'll get:

> IndentationError: unindent does not match any outer indentation level

The `DenterHelper` processor handles this by inserting two tokens: a `DEDENT` followed immediately by an `INDENT` (the total sequence here would actually be two `DEDENT`s followed by an `INDENT`, since `bogusLine()` is twice-dedented from `fooAndBar()`). The rationale is that the line has dedened to its parent, and then indented. It's consistent with the indentation tokens for something like:

    someStatement()
      bogusLine()

If your indentation scheme is anything like python's, chances are you want this to be a compilation error. The good news is that it will be, as long as your parser doesn't allow "spontaneous" indents. That is, if the example just before this paragraph fails, then so will the half-dedent example above. In both cases, the parser rules will bork on an unexpected `INDENT` token.

## Usage

In an ANTLR grammar definition, use the following.
```antlrv4
tokens { INDENT, DEDENT }

@lexer::header {
using AntlrDenter.DenterHelper;
}

@lexer::members {
private DenterHelper denter;
  
public override IToken NextToken()
{
    if (denter == null)
    {
        denter = DenterHelper.Builder()
            .Nl(NL)
            .Indent(MyCoolParser.INDENT)
            .Dedent(MyCoolParser.DEDENT)
            .PullToken(base.NextToken);
    }

    return denter.NextToken();
}
}

NL: ('\r'? '\n' ' '*); #For tabs just switch out ' '* with '\t'*
```

Note the injected code is dedented with respect to the `@lexer::members` block. This is so that it has the proper formatting in the resulting C# Lexer file.

## Acknowledgements

Many thanks to [yshavit](https://github.com/yshavit) for developing the original [ANTLR-Denter](https://github.com/yshavit/antlr-denter).

