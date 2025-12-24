using System;
using System.Collections.Generic;
using System.Linq;
using ParserGenerator.Grammar;
using ParserGenerator.Parser.State;
using ParserGenerator.Positions;

namespace ParserGenerator.Parser;

public sealed class ParserGenerator<TAstNode>
    where TAstNode : IWithPosition<TAstNode>
{
    private const char Sep = '\u0001';

    private readonly int _k;

    private readonly Symbol _startSymbol;
    private readonly Symbol _eofSymbol = new("\0", SymbolType.EndOfInput);
    private readonly List<Production<TAstNode>> _productions;

    public ParserGenerator(
        IEnumerable<Production<TAstNode>> productions,
        Symbol startSymbol,
        int k)
    {
        if (k < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(k), "k must be >= 1");
        }

        _startSymbol = startSymbol;
        _k = k;
        _productions = [..productions];

        foreach (var production in _productions.Where(x => x.Left == startSymbol))
        {
            if (production.Right[^1] != _eofSymbol)
            {
                production.Right.Add(_eofSymbol);
            }
        }
    }

    public ParserState<TAstNode> GenerateState()
    {
        var parsingTable = BuildParsingTable(_productions);

        return new ParserState<TAstNode>
        {
            K = _k,
            StartSymbol = _startSymbol,
            EndOfInput = _eofSymbol,
            ParseTable = parsingTable
        };
    }

    private static int CountTokens(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return 0;
        }
        return 1 + s.Count(c => c == Sep);
    }

    private static string Truncate(string s, int k)
    {
        if (string.IsNullOrEmpty(s)) return s;
        var count = 1;
        for (var i = 0; i < s.Length; i++)
        {
            if (s[i] == Sep)
            {
                count++;
                if (count > k)
                {
                    return s[..i];
                }
            }
        }
        return s;
    }

    private static string Concat(string a, string? b)
    {
        if (string.IsNullOrEmpty(a)) return b ?? "";
        if (string.IsNullOrEmpty(b)) return a;
        return a + Sep + b;
    }

    private Dictionary<Symbol, Dictionary<string, Production<TAstNode>>> BuildParsingTable(
        List<Production<TAstNode>> productions)
    {
        var firstSets = ComputeAllFirstKSets(productions, _k);
        var followSets = ComputeAllFollowKSets(productions, firstSets, _k);

        var table = new Dictionary<Symbol, Dictionary<string, Production<TAstNode>>>();

        foreach (var production in productions)
        {
            if (!table.ContainsKey(production.Left))
            {
                table[production.Left] = [];
            }

            // FIRST_k(alpha)
            var firstOfProduction = FirstKOfSequence(production.Right, firstSets, _k);

            var followOfLeft = followSets.TryGetValue(production.Left, out var f)
                ? f
                : [];

            foreach (var s in firstOfProduction)
            {
                if (CountTokens(s) >= _k)
                {
                    AddToTable(table, production, s);
                }
                else
                {
                    if (followOfLeft.Count == 0)
                    {
                        AddToTable(table, production, s);
                    }
                    else
                    {
                        foreach (var f2 in followOfLeft)
                        {
                            var combined = Concat(s, f2);
                            AddToTable(table, production, Truncate(combined, _k));
                        }
                    }
                }
            }
        }

        return table;
    }

    private void AddToTable(
        Dictionary<Symbol, Dictionary<string, Production<TAstNode>>> table,
        Production<TAstNode> production,
        string lookahead)
    {
        var row = table[production.Left];
        if (row.TryGetValue(lookahead, out var existing))
        {
            if (!ReferenceEquals(existing, production))
            {
                throw new Exception(
                    $"LL({_k}) conflict for [{production.Left.Name}, '{lookahead}']. " +
                    $"Productions: '{existing}' and '{production}'");
            }
        }
        else
        {
            row[lookahead] = production;
        }
    }


    private Dictionary<Symbol, HashSet<string>> ComputeAllFirstKSets(
        List<Production<TAstNode>> productions, int k)
    {
        var firstSets = new Dictionary<Symbol, HashSet<string>>();

        var allSymbols = new HashSet<Symbol>();
        foreach (var production in productions)
        {
            allSymbols.Add(production.Left);
            foreach (var symbol in production.Right)
                allSymbols.Add(symbol);
        }
        allSymbols.Add(_eofSymbol);

        foreach (var symbol in allSymbols)
        {
            firstSets[symbol] = [];

            if (symbol.Type is SymbolType.Terminal or SymbolType.EndOfInput)
            {
                firstSets[symbol].Add(symbol.Name);
            }
        }

        bool changed;
        do
        {
            changed = false;

            foreach (var production in productions)
            {
                var target = firstSets[production.Left];
                var seqFirst = FirstKOfSequence(production.Right, firstSets, k);

                foreach (var s in seqFirst)
                {
                    if (target.Add(s))
                    {
                        changed = true;
                    }
                }
            }
        } while (changed);

        return firstSets;
    }

    private static HashSet<string> FirstKOfSequence(
        IReadOnlyList<Symbol> sequence,
        Dictionary<Symbol, HashSet<string>> firstSets,
        int k)
    {
        var result = new HashSet<string>();

        if (k <= 0 || sequence.Count == 0)
        {
            result.Add("");
            return result;
        }

        var current = new HashSet<string> { "" };

        foreach (var symbol in sequence)
        {
            var next = new HashSet<string>();

            HashSet<string> symbolFirst;
            if (symbol.Type is SymbolType.Terminal or SymbolType.EndOfInput)
            {
                symbolFirst = [symbol.Name];
            }
            else
            {
                symbolFirst = firstSets.TryGetValue(symbol, out var fs)
                    ? fs
                    : [""];
            }

            foreach (var prefix in current)
            {
                if (CountTokens(prefix) >= k)
                {
                    next.Add(prefix);
                    continue;
                }

                foreach (var suffix in symbolFirst)
                {
                    var combined = Concat(prefix, suffix);
                    next.Add(Truncate(combined, k));
                }
            }

            current = next;

            if (current.All(s => CountTokens(s) >= k))
            {
                break;
            }
        }

        foreach (var s in current)
            result.Add(s);

        return result;
    }

    private Dictionary<Symbol, HashSet<string>> ComputeAllFollowKSets(
        List<Production<TAstNode>> productions,
        Dictionary<Symbol, HashSet<string>> firstSets,
        int k)
    {
        var followSets = new Dictionary<Symbol, HashSet<string>>();

        var nonTerminals = new HashSet<Symbol>();
        foreach (var production in productions)
        {
            nonTerminals.Add(production.Left);
            foreach (var symbol in production.Right)
            {
                if (symbol.Type == SymbolType.NonTerminal)
                {
                    nonTerminals.Add(symbol);
                }
            }
        }

        foreach (var nt in nonTerminals)
        {
            followSets[nt] = [];
        }

        if (followSets.ContainsKey(_startSymbol))
        {
            followSets[_startSymbol].Add(string.Join(Sep, Enumerable.Range(1, _k).Select(_ => _eofSymbol.Name)));
        }

        bool changed;
        do
        {
            changed = false;

            foreach (var production in productions)
            {
                for (var i = 0; i < production.Right.Count; i++)
                {
                    var symbol = production.Right[i];
                    if (symbol.Type != SymbolType.NonTerminal)
                    {
                        continue;
                    }

                    if (!followSets.ContainsKey(symbol))
                    {
                        followSets[symbol] = [];
                    }

                    var followSet = followSets[symbol];

                    var rest = production.Right.Skip(i + 1).ToList();
                    var firstOfRest = FirstKOfSequence(rest, firstSets, k);

                    foreach (var s in firstOfRest)
                    {
                        if (CountTokens(s) >= k)
                        {
                            if (followSet.Add(s))
                            {
                                changed = true;
                            }
                        }
                        else
                        {
                            if (followSets.TryGetValue(production.Left, out var leftFollow))
                            {
                                if (leftFollow.Count == 0)
                                {
                                    if (followSet.Add(s))
                                    {
                                        changed = true;
                                    }
                                }
                                else
                                {
                                    foreach (var lf in leftFollow)
                                    {
                                        var combined = Truncate(Concat(s, lf), k);
                                        if (followSet.Add(combined))
                                        {
                                            changed = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        } while (changed);

        return followSets;
    }
}
