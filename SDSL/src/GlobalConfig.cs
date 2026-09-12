using System.Collections.Frozen;
using SDSL.Functions;

namespace SDSL;

public static class GlobalConfig
{
    public const string Global = "global";
    
    public const int MaxPrecedence = 13;
    
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
        { "for"      , TokenType.For       },
        { "in"       , TokenType.In        },
        { "constexpr", TokenType.Constepxr },
        { "switch"   , TokenType.Switch    },
        { "default"  , TokenType.Default   },
        { "enum"     , TokenType.Enum      },
        // Alternate conditional
        { "not", TokenType.Not            },
        { "and", TokenType.ConditionalAnd },
        { "or" , TokenType.ConditionalOr  },
    }.ToFrozenDictionary();

    public static readonly FrozenDictionary<string, SealValue> LiteralMap = new Dictionary<string, SealValue>()
    {
        { "true" , new SealValue(true)  },
        { "false", new SealValue(false) },
        { "nil"  , SealValue.Nil        },
    }.ToFrozenDictionary();

    public static readonly FrozenDictionary<TokenType, int> PrecedenceMap = new Dictionary<TokenType, int>()
    {
        { TokenType.Dot    , MaxPrecedence },
        { TokenType.Minus             , 12 },
        { TokenType.Not               , 12 },
        
        { TokenType.Power             , 11 },
        
        { TokenType.Multiply          , 10 },
        { TokenType.Divide            , 10 },
        { TokenType.IDivide           , 10 },
        { TokenType.Modulo            , 10 },
        
        { TokenType.Add               , 9  },
        { TokenType.Subtract          , 9  },
        
        { TokenType.ShiftLeft         , 8  },
        { TokenType.ShiftRight        , 8  },
        { TokenType.ShiftRightU       , 8  },
        
        { TokenType.LessThan          , 7  },
        { TokenType.GreaterThan       , 7  },
        { TokenType.LessThanOrEqual   , 7  },
        { TokenType.GreaterThanOrEqual, 7  },
        
        { TokenType.Equal             , 6  },
        { TokenType.NotEqual          , 6  },
        
        { TokenType.And               , 5  },
        { TokenType.Xor               , 4  },
        { TokenType.Or                , 3  },
        
        { TokenType.ConditionalAnd    , 2  },
        { TokenType.ConditionalOr     , 1  },
        
        { TokenType.Assign            , 0  },
        
        { TokenType.PowerAssign       , 0  },
        
        { TokenType.MultiplyAssign    , 0  },
        { TokenType.DivideAssign      , 0  },
        { TokenType.IDivideAssign     , 0  },
        { TokenType.ModuloAssign      , 0  },
        
        { TokenType.AddAssign         , 0  },
        { TokenType.SubtractAssign    , 0  },
        
        { TokenType.ShiftLeftAssign   , 0  },
        { TokenType.ShiftRightAssign  , 0  },
        { TokenType.ShiftRightUAssign , 0  },
        
        { TokenType.AndAssign         , 0  },
        { TokenType.XorAssign         , 0  },
        { TokenType.OrAssign          , 0  },
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
        
        TokenType.PowerAssign,
        
        TokenType.MultiplyAssign,
        TokenType.DivideAssign,
        TokenType.IDivideAssign,
        TokenType.ModuloAssign,
        
        TokenType.AddAssign,
        TokenType.SubtractAssign,
        
        TokenType.ShiftLeftAssign,
        TokenType.ShiftRightAssign,
        TokenType.ShiftRightUAssign,
        
        TokenType.AndAssign,
        TokenType.XorAssign,
        TokenType.OrAssign,
    }.ToFrozenSet();

    public static readonly FrozenDictionary<Type, SealValueType> TypeMaps = new Dictionary<Type, SealValueType>()
    {
        { typeof(bool)    , SealValueType.Bool     },
        { typeof(double)  , SealValueType.Number   },
        { typeof(DateTime), SealValueType.DateTime },
        { typeof(TimeSpan), SealValueType.TimeSpan },
        { typeof(string)  , SealValueType.String   },
        { typeof(Function), SealValueType.Function },
    }.ToFrozenDictionary();
}