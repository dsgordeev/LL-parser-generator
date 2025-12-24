# LL-parser-generator
Pure C# LL(k) parser generator for .NET

### How to use
```csharp
// Program.cs

var parserStateGenerator = new ParserGenerator<AstNode>(Grammar.Productions, Grammar.Start, k: 1);
var parserState = parserStateGenerator.GenerateState();
var parser = new Parser<AstNode>(
    parserState,
    new Parser<AstNode>.ParserOptions(token => token.Value switch
    {
        double num => new AstNode.Num(num),
        long num => new AstNode.Num(num),
        string => new AstNode.Num(-1),
        null => new AstNode.Num(-2),
        _ => throw new ArgumentOutOfRangeException($"Unexpected token {token}")
    }));

var ast = parser.Parse(new LanguageLexer().Reset("1 + 2 * 3").Tokenize());

// ast ~ ([1] + ([2] * [3]))

public static class Grammar
{
    public static Symbol NUM = new(nameof(NUM), SymbolType.Terminal);
    public static Symbol PLUS = new("+", SymbolType.Terminal);
    public static Symbol MUL = new("*", SymbolType.Terminal);
    public static Symbol LP = new("(", SymbolType.Terminal);
    public static Symbol RP = new(")", SymbolType.Terminal);

    public static Symbol Program = new(nameof(Program), SymbolType.NonTerminal);
    public static Symbol Expr = new(nameof(Expr), SymbolType.NonTerminal);
    public static Symbol Term = new(nameof(Term), SymbolType.NonTerminal);
    public static Symbol TermTail = new(nameof(TermTail), SymbolType.NonTerminal);
    public static Symbol Factor = new(nameof(Factor), SymbolType.NonTerminal);
    public static Symbol FactorTail = new(nameof(FactorTail), SymbolType.NonTerminal);

    public static Production<AstNode>[] Productions =
    [
        new(Program, [Expr]),

        new(Expr, [Term, TermTail],
            vs => vs[1]
                .AsList()
                .Chunk(2)
                .Aggregate(
                    vs[0],
                    (acc, pair) => new AstNode.Binary(
                        (pair[0] as AstNode.Id
                         ?? throw new InvalidOperationException("op expected")).ToString()!,
                        acc,
                        pair[1]))),
        new(TermTail, [PLUS, Term, TermTail],
            vs => new AstNode.List([..vs[2].AsList(), new AstNode.Id("+"), vs[1]])),
        new(TermTail, [], _ => new AstNode.List([])),

        new(Term, [Factor, FactorTail],
            vs => vs[1]
                .AsList()
                .Chunk(2)
                .Aggregate(
                    vs[0],
                    (acc, pair) => new AstNode.Binary(
                        (pair[0] as AstNode.Id
                         ?? throw new InvalidOperationException("op expected")).ToString()!,
                        acc,
                        pair[1]))),
        new(FactorTail, [MUL, Factor, FactorTail],
            vs => new AstNode.List([..vs[2].AsList(), new AstNode.Id("*"), vs[1]])),
        new(FactorTail, [], _ => new AstNode.List([])),

        new(Factor, [LP, Expr, RP], vs => vs[1]),
        new(Factor, [NUM])
    ];

    public static Symbol Start => Program;
}

public abstract class AstNode : IWithPosition<AstNode>
{
    public Position Position { get; private set; }

    public AstNode WithPosition(IList<AstNode> list) { /* Position = f(list) */ return this; }

    public virtual List<AstNode> AsList() => [this];

    public class Id(string Value) : AstNode;

    public class Num(object Value) : AstNode;

    public class Binary(string Op, AstNode Left, AstNode Right) : AstNode;

    public class List(AstNode[] items) : AstNode
    {
        public override List<AstNode> AsList() => [.. items];
    }
}
```