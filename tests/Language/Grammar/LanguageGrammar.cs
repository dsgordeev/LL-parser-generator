using System;
using System.Collections.Generic;
using System.Linq;
using Language.Ast;
using Language.Ast.Nodes;
using ParserGenerator.Grammar;

namespace Language.Grammar;

public static class LanguageGrammar
{
    public static readonly Symbol ID = new(nameof(ID), SymbolType.Terminal);
    public static readonly Symbol NUM = new(nameof(NUM), SymbolType.Terminal);
    public static readonly Symbol TRUE = new("true", SymbolType.Terminal);
    public static readonly Symbol FALSE = new("false", SymbolType.Terminal);
    public static readonly Symbol PLUS = new("+", SymbolType.Terminal);
    public static readonly Symbol MINUS = new("-", SymbolType.Terminal);
    public static readonly Symbol MUL = new("*", SymbolType.Terminal);
    public static readonly Symbol DIV = new("/", SymbolType.Terminal);
    public static readonly Symbol POWER = new("**", SymbolType.Terminal);
    public static readonly Symbol MOD = new("%", SymbolType.Terminal);
    public static readonly Symbol EQ = new("==", SymbolType.Terminal);
    public static readonly Symbol NEQ = new("!=", SymbolType.Terminal);
    public static readonly Symbol LT = new("<", SymbolType.Terminal);
    public static readonly Symbol GT = new(">", SymbolType.Terminal);
    public static readonly Symbol LTE = new("<=", SymbolType.Terminal);
    public static readonly Symbol GTE = new(">=", SymbolType.Terminal);
    public static readonly Symbol AND = new("&", SymbolType.Terminal);
    public static readonly Symbol OR = new("|", SymbolType.Terminal);
    public static readonly Symbol XOR = new("^", SymbolType.Terminal);
    public static readonly Symbol CONCAT = new("||", SymbolType.Terminal);
    public static readonly Symbol NOT = new("!", SymbolType.Terminal);
    public static readonly Symbol IF = new(nameof(IF), SymbolType.Terminal);
    public static readonly Symbol THEN = new(nameof(THEN), SymbolType.Terminal);
    public static readonly Symbol ELSE = new(nameof(ELSE), SymbolType.Terminal);
    public static readonly Symbol ELSEIF = new(nameof(ELSEIF), SymbolType.Terminal);
    public static readonly Symbol END = new(nameof(END), SymbolType.Terminal);
    public static readonly Symbol RETURNS = new(nameof(RETURNS), SymbolType.Terminal);
    public static readonly Symbol LPAREN = new("(", SymbolType.Terminal);
    public static readonly Symbol RPAREN = new(")", SymbolType.Terminal);
    public static readonly Symbol LBRACE = new("{", SymbolType.Terminal);
    public static readonly Symbol RBRACE = new("}", SymbolType.Terminal);
    public static readonly Symbol LBRACKET = new("[", SymbolType.Terminal);
    public static readonly Symbol RBRACKET = new("]", SymbolType.Terminal);
    public static readonly Symbol COMMA = new(",", SymbolType.Terminal);
    public static readonly Symbol SEMI = new(";", SymbolType.Terminal);
    public static readonly Symbol COLON = new(":", SymbolType.Terminal);
    public static readonly Symbol DOT = new(".", SymbolType.Terminal);
    public static readonly Symbol FUNCTION = new(nameof(FUNCTION), SymbolType.Terminal);
    public static readonly Symbol INTEGER = new(nameof(INTEGER), SymbolType.Terminal);
    public static readonly Symbol REAL = new(nameof(REAL), SymbolType.Terminal);
    public static readonly Symbol DOUBLE = new(nameof(DOUBLE), SymbolType.Terminal);
    public static readonly Symbol STRING = new(nameof(STRING), SymbolType.Terminal);
    public static readonly Symbol CHAR = new(nameof(CHAR), SymbolType.Terminal);
    public static readonly Symbol BOOLEAN = new(nameof(BOOLEAN), SymbolType.Terminal);

    public static readonly Dictionary<string, Symbol> KeywordSymbols = new()
    {
        ["function"] = FUNCTION,
        ["if"] = IF,
        ["then"] = THEN,
        ["else"] = ELSE,
        ["elseif"] = ELSEIF,
        ["end"] = END,
        ["returns"] = RETURNS,
        ["true"] = TRUE,
        ["false"] = FALSE,
        ["integer"] = INTEGER,
        ["real"] = REAL,
        ["double"] = DOUBLE,
        ["string"] = STRING,
        ["char"] = CHAR,
        ["boolean"] = BOOLEAN
    };

    public static readonly Symbol Program = new(nameof(Program), SymbolType.NonTerminal);
    public static readonly Symbol FunctionList = new(nameof(FunctionList), SymbolType.NonTerminal);
    public static readonly Symbol FunctionListTail = new(nameof(FunctionListTail), SymbolType.NonTerminal);
    public static readonly Symbol Function = new(nameof(Function), SymbolType.NonTerminal);

    public static readonly Symbol ParamGroupList = new(nameof(ParamGroupList), SymbolType.NonTerminal);
    public static readonly Symbol ParamGroupListTail = new(nameof(ParamGroupListTail), SymbolType.NonTerminal);
    public static readonly Symbol ParamGroup = new(nameof(ParamGroup), SymbolType.NonTerminal);
    public static readonly Symbol ParamNames = new(nameof(ParamNames), SymbolType.NonTerminal);
    public static readonly Symbol ParamNamesTail = new(nameof(ParamNamesTail), SymbolType.NonTerminal);

    public static readonly Symbol TypeList = new(nameof(TypeList), SymbolType.NonTerminal);
    public static readonly Symbol TypeListTail = new(nameof(TypeListTail), SymbolType.NonTerminal);
    public static readonly Symbol TypeValue = new(nameof(TypeValue), SymbolType.NonTerminal);
    public static readonly Symbol PrimitiveType = new(nameof(PrimitiveType), SymbolType.NonTerminal);
    public static readonly Symbol FunctionType = new(nameof(FunctionType), SymbolType.NonTerminal);

    public static readonly Symbol ExprList = new(nameof(ExprList), SymbolType.NonTerminal);
    public static readonly Symbol ExprListTail = new(nameof(ExprListTail), SymbolType.NonTerminal);

    public static readonly Symbol Expr = new(nameof(Expr), SymbolType.NonTerminal);

    public static readonly Symbol Expr1 = new(nameof(Expr1), SymbolType.NonTerminal);
    public static readonly Symbol Expr2 = new(nameof(Expr2), SymbolType.NonTerminal);
    public static readonly Symbol Expr3 = new(nameof(Expr3), SymbolType.NonTerminal);
    public static readonly Symbol Expr4 = new(nameof(Expr4), SymbolType.NonTerminal);
    public static readonly Symbol Expr5 = new(nameof(Expr5), SymbolType.NonTerminal);
    public static readonly Symbol Expr6 = new(nameof(Expr6), SymbolType.NonTerminal);
    public static readonly Symbol Expr7 = new(nameof(Expr7), SymbolType.NonTerminal);
    public static readonly Symbol Expr8 = new(nameof(Expr8), SymbolType.NonTerminal);
    public static readonly Symbol Expr9 = new(nameof(Expr9), SymbolType.NonTerminal); // right-assoc **
    public static readonly Symbol Tail9 = new(nameof(Tail9), SymbolType.NonTerminal); // right-assoc **
    public static readonly Symbol Expr10 = new(nameof(Expr10), SymbolType.NonTerminal); // unary
    public static readonly Symbol Expr11 = new(nameof(Expr11), SymbolType.NonTerminal); // postfix
    public static readonly Symbol Expr12 = new(nameof(Expr12), SymbolType.NonTerminal); // atom

    public static readonly Symbol Tail1 = new(nameof(Tail1), SymbolType.NonTerminal);
    public static readonly Symbol Tail2 = new(nameof(Tail2), SymbolType.NonTerminal);
    public static readonly Symbol Tail3 = new(nameof(Tail3), SymbolType.NonTerminal);
    public static readonly Symbol Tail4 = new(nameof(Tail4), SymbolType.NonTerminal);
    public static readonly Symbol Tail5 = new(nameof(Tail5), SymbolType.NonTerminal);
    public static readonly Symbol Tail6 = new(nameof(Tail6), SymbolType.NonTerminal);
    public static readonly Symbol Tail7 = new(nameof(Tail7), SymbolType.NonTerminal);
    public static readonly Symbol Tail8 = new(nameof(Tail8), SymbolType.NonTerminal);

    public static readonly Symbol PostfixOp = new(nameof(PostfixOp), SymbolType.NonTerminal);
    public static readonly Symbol PostfixTail = new(nameof(PostfixTail), SymbolType.NonTerminal);

    public static readonly Symbol IfExpr = new(nameof(IfExpr), SymbolType.NonTerminal);
    public static readonly Symbol ElseIfClauses = new(nameof(ElseIfClauses), SymbolType.NonTerminal);
    public static readonly Symbol ElseIfClause = new(nameof(ElseIfClause), SymbolType.NonTerminal);

    public static readonly Symbol ArgsTail = new(nameof(ArgsTail), SymbolType.NonTerminal);
    public static readonly Symbol ArgsListOpt = new(nameof(ArgsListOpt), SymbolType.NonTerminal);
    public static readonly Symbol ArgsTailRest = new(nameof(ArgsTailRest), SymbolType.NonTerminal);
    public static readonly Symbol ArgsHead = new(nameof(ArgsHead), SymbolType.NonTerminal);
    public static readonly Symbol ArgsSeq = new(nameof(ArgsSeq), SymbolType.NonTerminal);

    public static Symbol Start => Program;

    private static AstNodeExpression FoldChain(AstNodeExpression lhs, AstNodeExpression chain)
    {
        var items = chain.AsList();
        var cur = lhs;
        for (var i = 0; i + 1 < items.Count; i += 2)
        {
            cur = new BinaryOpNode(
                (items[i] as IdentifierNode
                 ?? throw new InvalidOperationException("op expected")).Name,
                cur,
                items[i + 1]);
        }
        return cur;
    }


    public static Production<AstNodeExpression>[] Productions =>
    [
        new(Program, [FunctionList], vs => new ModuleNode(null, null!, null!, vs[0])),

        new(FunctionList, [Function, FunctionListTail],
            vs => new ExprListNode([..vs[0].AsList(), ..vs[1].AsList()])),
        new(FunctionListTail, [], _ => new ExprListNode([])),
        new(FunctionListTail, [Function, FunctionListTail],
            vs => new ExprListNode([..vs[0].AsList(), ..vs[1].AsList()])),

        new(Function,
            [FUNCTION, ID, LPAREN, ParamGroupList, RETURNS, TypeList,
             RPAREN, ExprList, END, FUNCTION],
            vs => new FunctionNode(vs[1], vs[3], vs[5], vs[7])),

        new(ParamGroupList, [ParamGroup, ParamGroupListTail],
            vs => new ExprListNode([..vs[0].AsList(), ..vs[1].AsList()])),
        new(ParamGroupListTail, [], _ => new ExprListNode([])),
        new(ParamGroupListTail, [SEMI, ParamGroup, ParamGroupListTail],
            vs => new ExprListNode([..vs[1].AsList(), ..vs[2].AsList()])),

        new(ParamGroup, [ParamNames, COLON, TypeValue],
            vs => new ParamGroupNode(vs[0], vs[2])),

        new(ParamNames, [ID, ParamNamesTail],
            vs => new ExprListNode([..vs[0].AsList(), ..vs[1].AsList()])),
        new(ParamNamesTail, [], _ => new ExprListNode([])),
        new(ParamNamesTail, [COMMA, ID, ParamNamesTail],
            vs => new ExprListNode([..vs[1].AsList(), ..vs[2].AsList()])),

        new(TypeList, [TypeValue, TypeListTail],
            vs => new ExprListNode([..vs[0].AsList(), ..vs[1].AsList()])),
        new(TypeListTail, [], _ => new ExprListNode([])),
        new(TypeListTail, [COMMA, TypeValue, TypeListTail],
            vs => new ExprListNode([..vs[1].AsList(), ..vs[2].AsList()])),

        new(TypeValue, [PrimitiveType]),
        new(TypeValue, [FunctionType]),
        new(TypeValue, [ID]),

        new(PrimitiveType, [INTEGER], vs => new TypeNode(vs[0])),
        new(PrimitiveType, [REAL],    vs => new TypeNode(vs[0])),
        new(PrimitiveType, [DOUBLE],  vs => new TypeNode(vs[0])),
        new(PrimitiveType, [CHAR],    vs => new TypeNode(vs[0])),
        new(PrimitiveType, [BOOLEAN], vs => new TypeNode(vs[0])),
        new(PrimitiveType, [STRING],  vs => new TypeNode(vs[0])),

        new(FunctionType, [FUNCTION, LBRACKET, RETURNS, TypeList, RBRACKET],
            vs => new TypeNode.FunctionTypeNode(null, null, vs[3])),
        new(FunctionType, [FUNCTION, LBRACKET, TypeList, RETURNS, TypeList, RBRACKET],
            vs => new TypeNode.FunctionTypeNode(null, vs[2], vs[4])),

        new(ExprList, [Expr, ExprListTail],
            vs => new ExprListNode([..vs[0].AsList(), ..vs[1].AsList()])),
        new(ExprListTail, [], _ => new ExprListNode([])),
        new(ExprListTail, [COMMA, Expr, ExprListTail],
            vs => new ExprListNode([..vs[1].AsList(), ..vs[2].AsList()])),

        new(Expr, [IfExpr]),
        new(Expr, [Expr1]),

        new(Expr1, [Expr2, Tail1],
            vs => FoldChain(vs[0], vs[1])),
        new(Tail1, [], _ => new ExprListNode([])),
        new(Tail1, [CONCAT, Expr2, Tail1],
            vs => new ExprListNode([.. vs[2].AsList(), new IdentifierNode("||"), vs[1]])),

        new(Expr2, [Expr3, Tail2],
            vs => FoldChain(vs[0], vs[1])),
        new(Tail2, [], _ => new ExprListNode([])),
        new(Tail2, [OR, Expr3, Tail2],
            vs => new ExprListNode([.. vs[2].AsList(), new IdentifierNode("|"), vs[1]])),

        new(Expr3, [Expr4, Tail3],
            vs => FoldChain(vs[0], vs[1])),
        new(Tail3, [], _ => new ExprListNode([])),
        new(Tail3, [XOR, Expr4, Tail3],
            vs => new ExprListNode([.. vs[2].AsList(), new IdentifierNode("^"), vs[1]])),

        new(Expr4, [Expr5, Tail4],
            vs => FoldChain(vs[0], vs[1])),
        new(Tail4, [], _ => new ExprListNode([])),
        new(Tail4, [AND, Expr5, Tail4],
            vs => new ExprListNode([.. vs[2].AsList(), new IdentifierNode("&"), vs[1]])),

        new(Expr5, [Expr6, Tail5],
            vs => FoldChain(vs[0], vs[1])),
        new(Tail5, [], _ => new ExprListNode([])),
        new(Tail5, [EQ, Expr6, Tail5],
            vs => new ExprListNode([.. vs[2].AsList(), new IdentifierNode("=="), vs[1]])),
        new(Tail5, [NEQ, Expr6, Tail5],
            vs => new ExprListNode([.. vs[2].AsList(), new IdentifierNode("!="), vs[1]])),

        new(Expr6, [Expr7, Tail6],
            vs => FoldChain(vs[0], vs[1])),
        new(Tail6, [], _ => new ExprListNode([])),
        new(Tail6, [LT,  Expr7, Tail6], vs => new ExprListNode([.. vs[2].AsList(), new IdentifierNode("<"), vs[1]])),
        new(Tail6, [GT,  Expr7, Tail6], vs => new ExprListNode([.. vs[2].AsList(), new IdentifierNode(">"), vs[1]])),
        new(Tail6, [LTE, Expr7, Tail6], vs => new ExprListNode([.. vs[2].AsList(), new IdentifierNode("<="), vs[1]])),
        new(Tail6, [GTE, Expr7, Tail6], vs => new ExprListNode([.. vs[2].AsList(), new IdentifierNode(">="), vs[1]])),

        new(Expr7, [Expr8, Tail7],
            vs => FoldChain(vs[0], vs[1])),
        new(Tail7, [], _ => new ExprListNode([])),
        new(Tail7, [PLUS,  Expr8, Tail7], vs => new ExprListNode([.. vs[2].AsList(), new IdentifierNode("+"), vs[1]])),
        new(Tail7, [MINUS, Expr8, Tail7], vs => new ExprListNode([.. vs[2].AsList(), new IdentifierNode("-"), vs[1]])),

        new(Expr8, [Expr9, Tail8],
            vs => FoldChain(vs[0], vs[1])),
        new(Tail8, [], _ => new ExprListNode([])),
        new(Tail8, [MUL, Expr9, Tail8], vs => new ExprListNode([.. vs[2].AsList(), new IdentifierNode("*"), vs[1]])),
        new(Tail8, [DIV, Expr9, Tail8], vs => new ExprListNode([.. vs[2].AsList(), new IdentifierNode("/"), vs[1]])),
        new(Tail8, [MOD, Expr9, Tail8], vs => new ExprListNode([.. vs[2].AsList(), new IdentifierNode("%"), vs[1]])),

        new(Expr9, [Expr10, Tail9],
            vs => vs[1].AsList().Count == 0
                ? vs[0]
                : new BinaryOpNode("**", vs[0],
                    vs[1].AsList().AsEnumerable()
                        .Reverse()
                        .Aggregate((right, left) => new BinaryOpNode("**", left, right)))),
        new(Tail9, [], _ => new ExprListNode([])),
        new(Tail9, [POWER, Expr10, Tail9],
            vs => new ExprListNode([.. vs[2].AsList(), .. vs[1].AsList()])),

        new(Expr10, [MINUS, Expr10], vs => new UnaryOpNode("-", vs[1])),
        new(Expr10, [NOT,   Expr10], vs => new UnaryOpNode("!", vs[1])),
        new(Expr10, [PLUS,  Expr10], vs => new UnaryOpNode("+", vs[1])),
        new(Expr10, [Expr11]),

        new(Expr11, [Expr12, PostfixTail],
            vs => vs[1].AsList().Aggregate(vs[0], (current, op) => new PostfixNode(current, op))),
        new(PostfixTail, [], _ => new ExprListNode([])),
        new(PostfixTail, [PostfixOp, PostfixTail],
            vs => new ExprListNode([vs[0], ..vs[1].AsList()])),

        new(PostfixOp, [LPAREN, ArgsListOpt, RPAREN],
            vs => new PostfixNode.FunctionCallNode(
                vs[1].AsList().Count == 0 ? null : vs[1])),
        new(PostfixOp, [COLON, PrimitiveType],
            vs => new PostfixNode.TypeCastNode(vs[1])),

        new(Expr12, [TRUE],  _ => new BoolNode(true)),
        new(Expr12, [FALSE], _ => new BoolNode(false)),
        new(Expr12, [STRING]),
        new(Expr12, [NUM]),
        new(Expr12, [ID]),
        new(Expr12, [LPAREN, Expr, RPAREN], vs => vs[1].WithWrap()),

        new(IfExpr, [IF, Expr, THEN, ExprList, ElseIfClauses, ELSE, ExprList, END, IF],
            vs => new IfNode(vs[1], vs[3], vs[6],
                     vs[4].AsList().Count == 0 ? null : vs[4])),

        new(ElseIfClauses, [], _ => new ExprListNode([])),
        new(ElseIfClauses, [ElseIfClause, ElseIfClauses],
            vs => new ExprListNode([..vs[0].AsList(), ..vs[1].AsList()])),

        new(ElseIfClause, [ELSEIF, Expr, THEN, ExprList],
            vs => new IfNode.ElseIfNode(vs[1], vs[3])),

        new(ArgsListOpt, [], _ => new ExprListNode([])),
        new(ArgsListOpt, [ArgsSeq], vs => vs[0]),

        new(ArgsSeq, [ArgsHead, ArgsTail],
            vs => new ExprListNode([..vs[0].AsList(), ..vs[1].AsList()])),

        new(ArgsHead, [Expr]),
        new(ArgsHead, [COMMA], _ => new ExprListNode([ExprListNode.Missed])),

        new(ArgsTail, [], _ => new ExprListNode([])),
        new(ArgsTail, [COMMA, ArgsTailRest, ArgsTail],
            vs => new ExprListNode([vs[1], ..vs[2].AsList()])),

        new(ArgsTailRest, [Expr], vs => vs[0]),
        new(ArgsTailRest, [], _ => new ExprListNode([ExprListNode.Missed]))
    ];
}