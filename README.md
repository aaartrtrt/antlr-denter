# ANTLR-Denter: Python-like indentation tokens for ANTLR4 C# Hosts

[![.NET](https://github.com/aaartrtrt/antlr-denter/actions/workflows/dotnet.yml/badge.svg)](https://github.com/aaartrtrt/antlr-denter/actions/workflows/dotnet.yml)

This project adds `INDENT` and `DEDENT` tokens for autogenerated [ANTLR4](https://github.com/antlr/antlr4) parsers for Python-like scopes. This defines a `DenterHelper` that can be added to an ANTLR4 grammar.

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
2. Single-line statements in your grammar should end in newlines. For example, an assignment expression might be `identifier '=' expression NL`.
3. Blocks are bookended by INDENT and DEDENT, without mentioning extra newlines: `block: INDENT statement+ DEDENT`.
   - You should _not_ include a newline before the INDENT
   - An `if` would be something like `if expression ':' block`. (Note the lack of `NL` after the `:`.)

In the example above, `universe` and `dolly` represent simple expressions, and you can imagine that the grammar would contain something like `statement: expression NL | helloBlock;`.

### Handling and asserting indentation

The `DenterHelper` processor asserts correct indentation on DEDENT. Take the following example:

```
someStatement()
if foo():
      if bar():
          fooAndBar()
    bogusLine()
```

`bogusLine()` does not dedent to the indentation of any valid scope - lacking indentation to qualify as part of the `if foo():`'s scope and too indented to share a scope with `someStatement()`. In Python this is expressed as an `IndentationError`.

The `DenterHelper` processor handles this by inserting two tokens: a `DEDENT` followed immediately by an `INDENT` (the total sequence here would actually be two `DEDENT`s followed by an `INDENT`, since `bogusLine()` is twice-dedented from `fooAndBar()`). The rationale is that the line has dedented to its parent, and then indented.

As a consequence, the `DenterHelper` processor will also assert correct indentation for all lines where an `INDENT` is not expected. Take the following example in a Python-like grammar of two method calls:

```
someStatement()
  bogusLine()
```

This would be illegal due to no `INDENT`s being expected after `someStatement()`.

## Usage

In an ANTLR grammar definition `MyGrammar.g4`, use the following.

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
            .Indent(MyGrammarParser.INDENT)
            .Dedent(MyGrammarParser.DEDENT)
            .PullToken(base.NextToken);
    }

    return denter.NextToken();
}
}

NL: ('\r'? '\n' ' '*); #For tabs just switch out ' '* with '\t'*
```

Note the injected code is dedented with respect to the `@lexer::members` block. This is so that it has the proper formatting in the resulting C# Lexer file.

## Acknowledgements

Many thanks to [yshavit](https://github.com/yshavit) for developing the original [ANTLR-Denter](https://github.com/yshavit/antlr-denter), and [tejacques](https://github.com/tejacques) for their [Deque implementation](https://github.com/tejacques/Deque).

## Related Work

* The original [ANTLR-Denter](https://github.com/yshavit/antlr-denter).
* [ANTLR4](https://github.com/antlr/antlr4), the language toolkit.
* kaby76's [Antlr4BuildTasks](https://github.com/kaby76/Antlr4BuildTasks), which has been used for further reading on C# and .NET with ANTLR.