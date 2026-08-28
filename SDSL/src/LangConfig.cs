using System.Collections.Frozen;

namespace SDSL;

public class LangConfig
{
    public const string Global = "global";
    
    public static readonly FrozenDictionary<string, TokenType> KeywordMap = new Dictionary<string, TokenType>()
    {
        { "namespace", TokenType.Namespace },
        { "using"    , TokenType.Using     },
        { "class"    , TokenType.Class     },
        { "func"     , TokenType.Func      },
        { "new"      , TokenType.New       },
        { "static"   , TokenType.Static    },
        { "var"      , TokenType.Var       },
        { "const"    , TokenType.Const     },
        { "return"   , TokenType.Return    },
        { "if"       , TokenType.If        },
        { "else"     , TokenType.Else      },
        { "while"    , TokenType.While     },
        { "break"    , TokenType.Break     },
        { "continue" , TokenType.Continue  },
    }.ToFrozenDictionary();

    public static readonly FrozenDictionary<string, SealValue> LiteralMap = new Dictionary<string, SealValue>()
    {
        { "true" , new SealValue(true)  },
        { "false", new SealValue(false) },
        { "nil"  , SealValue.Nil        },
    }.ToFrozenDictionary();

    public const int MaxPrecedence = 12;
    
    public static readonly FrozenDictionary<TokenType, int> PrecedenceMap = new Dictionary<TokenType, int>()
    {
        { TokenType.Dot, MaxPrecedence },
        { TokenType.Minus, 11 },
        { TokenType.Not  , 11 },
        { TokenType.Power, 10 },
        { TokenType.Multiply, 9 },
        { TokenType.Divide  , 9 },
        { TokenType.IDivide , 9 },
        { TokenType.Modulo  , 9 },
        { TokenType.Add     , 8 },
        { TokenType.Subtract, 8 },
        { TokenType.LessThan          , 7 },
        { TokenType.GreaterThan       , 7 },
        { TokenType.LessThanOrEqual   , 7 },
        { TokenType.GreaterThanOrEqual, 7 },
        { TokenType.Equal   , 6 },
        { TokenType.NotEqual, 6 },
        { TokenType.And, 5 },
        { TokenType.Xor, 4 },
        { TokenType.Or , 3 },
        { TokenType.ConditionalAnd, 2 },
        { TokenType.ConditionalOr , 1 },
        { TokenType.Assign        , 0 },
        { TokenType.MultiplyAssign, 0 },
        { TokenType.DivideAssign  , 0 },
        { TokenType.IDivideAssign , 0 },
        { TokenType.ModuloAssign  , 0 },
        { TokenType.AddAssign     , 0 },
        { TokenType.SubtractAssign, 0 },
        { TokenType.AndAssign     , 0 },
        { TokenType.XorAssign     , 0 },
        { TokenType.OrAssign      , 0 },
    }.ToFrozenDictionary();
    
    public static readonly FrozenDictionary<TokenType, TokenType> UnaryMap = new Dictionary<TokenType, TokenType>()
    {
        { TokenType.Subtract, TokenType.Minus },
    }.ToFrozenDictionary();

    public static readonly FrozenSet<TokenType> RightAssociativeSet = new HashSet<TokenType>()
    {
        TokenType.Minus,
        TokenType.Not,
        TokenType.Assign,
    }.ToFrozenSet();

    public static readonly FrozenDictionary<SealClass, TypeCatagory> TypeCatagoryMap = new Dictionary<SealClass, TypeCatagory>()
    {
        { SealClass.Nil     , TypeCatagory.Nil      },
        { SealClass.Bool    , TypeCatagory.Bool     },
        { SealClass.Number  , TypeCatagory.Number   },
        { SealClass.String  , TypeCatagory.String   },
        { SealClass.Function, TypeCatagory.Function },
    }.ToFrozenDictionary();
}