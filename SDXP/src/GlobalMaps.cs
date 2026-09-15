using System.Collections.Frozen;

namespace SDSL;

public static class GlobalMaps
{
	public const int MaxPrecedence = 13;
	
	public static readonly FrozenDictionary<string, TokenType> KeywordMap = new Dictionary<string, TokenType>()
	{
		{ "class"    , TokenType.Class     },
		{ "enum"     , TokenType.Enum      },
		
		{ "static"   , TokenType.Static    },
		
		{ "func"     , TokenType.Func      },
		{ "new"      , TokenType.New       },
		{ "return"   , TokenType.Return    },
		{ "base"     , TokenType.Base      },
		
		{ "var"      , TokenType.Var       },
		{ "const"    , TokenType.Const     },
		
		{ "if"       , TokenType.If        },
		{ "else"     , TokenType.Else      },
		{ "switch"   , TokenType.Switch    },
		{ "default"  , TokenType.Default   },
		
		{ "while"    , TokenType.While     },
		{ "for"      , TokenType.For       },
		{ "in"       , TokenType.In        },
		{ "break"    , TokenType.Break     },
		{ "continue" , TokenType.Continue  },
		
		// Alternate conditional
		{ "not", TokenType.Not            },
		{ "and", TokenType.ConditionalAnd },
		{ "or" , TokenType.ConditionalOr  },
	}.ToFrozenDictionary();
	
	public static readonly FrozenDictionary<string, Variant> LiteralMap = new Dictionary<string, Variant>()
    {
        { "true" , true        },
        { "false", false       },
        { "nil"  , Variant.Nil },
    }.ToFrozenDictionary();

    public static readonly FrozenDictionary<TokenType, int> PrecedenceMap = new Dictionary<TokenType, int>()
    {
        { TokenType.Dot    , MaxPrecedence },
        { TokenType.Minus             , 12 },
        { TokenType.Plus              , 12 },
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
        { TokenType.Add     , TokenType.Plus  },
    }.ToFrozenDictionary();

    public static readonly FrozenSet<TokenType> RightAssociativeSet = new HashSet<TokenType>()
    {
        TokenType.Minus,
        TokenType.Plus,
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
}