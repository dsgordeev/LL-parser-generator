using System.Collections.Generic;
using ParserGenerator.Grammar;

namespace ParserGenerator.Parser.State;

public sealed record ParserState<TAstNode>
{
    public Symbol StartSymbol { get; set; } = null!;

    public Symbol EndOfInput { get; set; } = null!;

    public Dictionary<Symbol, Dictionary<string, Production<TAstNode>>> ParseTable { get; set; } = null!;
    
    public int K { get; set; }
}