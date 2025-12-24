using ParserGenerator.Grammar;
using ParserGenerator.Lexer;
using ParserGenerator.Parser.State;
using ParserGenerator.Positions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ParserGenerator.Parser;

public sealed class Parser<TAstNode>(
    ParserState<TAstNode> state,
    Parser<TAstNode>.ParserOptions options)
    where TAstNode : IWithPosition<TAstNode>
{
    private const char Sep = '\u0001';

    private readonly int _k = state.K;

    public record ReturnFromSymbol(string Name, int ChildrenCount, Production<TAstNode> Production)
        : Symbol($"ReturnFrom_{Name}_Symbol", SymbolType.Terminal)
    {
        public override string ToString() => $"Return from {Name}({ChildrenCount}, {Production})";

        public int ChildrenCount { get; } = ChildrenCount;
        public Production<TAstNode> Production { get; } = Production;
    }

    private readonly Dictionary<Symbol, Dictionary<string, Production<TAstNode>>> _parsingTable = state.ParseTable;
    private readonly Symbol _startSymbol = state.StartSymbol;
    private readonly Symbol _eofSymbol = state.EndOfInput;


    public TAstNode Parse(IEnumerable<Token> inputTokens)
    {
        using var reader = new StreamingTokenReader(inputTokens, _eofSymbol, _k);

        var stack = new Stack<Symbol>();
        var valueStack = new Stack<TAstNode>();

        stack.Push(_eofSymbol);
        stack.Push(_startSymbol);

        var lookahead = reader.Peek(_k);

        while (stack.Count > 0)
        {
            var top = stack.Peek();

            if (top == _eofSymbol && lookahead.Length > 0 && lookahead[0] == _eofSymbol)
            {
                if (stack.Count == 1 && valueStack.Count == 1)
                {
                    stack.Pop();
                    break;
                }
            }

            if (top is ReturnFromSymbol ret)
            {
                stack.Pop();

                var production = ret.Production;

                var reducibleValues = new List<TAstNode>();
                for (var i = 0; i < ret.ChildrenCount; i++)
                {
                    if (valueStack.Count > 0)
                        reducibleValues.Insert(0, valueStack.Pop());
                }

                var reduceAction = production.ReduceAction ?? (x => x[0]);
                var reducedValue = reduceAction([..reducibleValues]);
                reducedValue.WithPosition(reducibleValues);

                valueStack.Push(reducedValue);
                continue;
            }

            if (top.Type is SymbolType.Terminal or SymbolType.EndOfInput)
            {
                if (lookahead.Length > 0 && top == lookahead[0])
                {
                    stack.Pop();

                    var currentToken = reader.Consume();
                    var terminalNode = options.TokenToNode(currentToken);
                    valueStack.Push(terminalNode);

                    lookahead = reader.Peek(_k);
                }
                else
                {
                    var got = lookahead.Length > 0 ? lookahead[0].Name : "<empty>";
                    var pos = reader.CurrentPosition;
                    throw new Exception(
                        $"Syntax error at {pos}: expected '{top.Name}', got '{got}'");
                }
            }
            else if (top.Type == SymbolType.NonTerminal)
            {
                var key = LookaheadKey(lookahead);

                if (_parsingTable.TryGetValue(top, out var row) &&
                    row.TryGetValue(key, out var production))
                {
                    stack.Pop();

                    var returnFromSymbol = new ReturnFromSymbol(
                        top.Name,
                        production.Right.Count,
                        production);
                    stack.Push(returnFromSymbol);

                    for (var i = production.Right.Count - 1; i >= 0; i--)
                        stack.Push(production.Right[i]);
                }
                else
                {
                    var expected = _parsingTable.TryGetValue(top, out var expRow)
                        ? string.Join(", ", expRow.Keys.Select(kk => $"'{kk}'"))
                        : "no productions";
                    var got = key.Length > 0 ? key : "<empty>";
                    var pos = reader.CurrentPosition;
                    throw new Exception(
                        $"Syntax error at {pos}: unexpected lookahead '{got}' for {top.Name}. Expected: {expected}");
                }
            }
        }

        if (!reader.IsExhausted)
        {
            var remainingToken = reader.Consume();
            if (remainingToken.Symbol != _eofSymbol)
            {
                throw new Exception(
                    $"Unexpected tokens after end of input at {remainingToken.Position}: '{remainingToken.Symbol.Name}'");
            }
        }

        if (valueStack.Count != 1)
        {
            throw new Exception(
                $"Parser did not produce a single AST root node. Stack count: {valueStack.Count}");
        }

        return valueStack.Pop();
    }

    private static string LookaheadKey(ReadOnlySpan<Symbol> lookahead)
        => string.Join(Sep, lookahead.ToArray().Select(s => s.Name));

    public class ParserOptions(Func<Token, TAstNode> tokenToNode)
    {
        public TextWriter? Out { get; set; }

        public Func<Token, TAstNode> TokenToNode { get; } = tokenToNode;
    }

    private sealed class StreamingTokenReader : IDisposable
    {
        private readonly IEnumerator<Token> _enumerator;
        private readonly Symbol _eofSymbol;
        private readonly int _k;
        private readonly Queue<Token> _buffer = new();

        private readonly Symbol[] _lookaheadBuffer;

        private bool _streamEnded;
        private Token? _lastConsumed;

        public StreamingTokenReader(IEnumerable<Token> tokens, Symbol eofSymbol, int k)
        {
            if (k < 1)
                throw new ArgumentOutOfRangeException(nameof(k));

            _enumerator = tokens.GetEnumerator();
            _eofSymbol = eofSymbol;
            _k = k;
            _lookaheadBuffer = new Symbol[k];
        }

        public Position CurrentPosition =>
            _lastConsumed?.Position ?? default;

        public bool IsExhausted =>
            _streamEnded && _buffer.Count == 0;

        
        public ReadOnlySpan<Symbol> Peek(int count)
        {
            if (count > _k)
            {
                throw new ArgumentOutOfRangeException(nameof(count),
                    $"Peek count ({count}) exceeds reader window size ({_k})");
            }

            EnsureBuffered(count);

            var i = 0;
            foreach (var t in _buffer)
            {
                if (i >= count)
                {
                    break;
                }
                _lookaheadBuffer[i++] = t.Symbol;
            }

            while (i < count)
            {
                _lookaheadBuffer[i++] = _eofSymbol;
            }

            return _lookaheadBuffer.AsSpan(0, count);
        }

        public Token Consume()
        {
            EnsureBuffered(1);

            if (_buffer.Count == 0)
            {
                var eof = new Token(_eofSymbol, null!, default);
                _lastConsumed = eof;
                return eof;
            }

            var token = _buffer.Dequeue();
            _lastConsumed = token;
            return token;
        }

        private void EnsureBuffered(int needed)
        {
            while (_buffer.Count < needed && !_streamEnded)
            {
                if (_enumerator.MoveNext())
                {
                    _buffer.Enqueue(_enumerator.Current);
                }
                else
                {
                    _streamEnded = true;
                    break;
                }
            }
        }

        public void Dispose()
        {
            _enumerator.Dispose();
        }
    }
}
