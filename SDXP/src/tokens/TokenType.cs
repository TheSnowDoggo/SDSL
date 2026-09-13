namespace SDSL;

public enum TokenType
{
	// Brackets
	OpenParen  , // (
	CloseParen , // )
	OpenBrace  , // {
	CloseBrace , // }
	OpenSquare , // [
	CloseSquare, // ]
    
	// Operators
	Colon    , // :
	Semicolon, // ;
    
	Comma , // ,
	Dot   , // .
	Elipse, // ..
    
	Arrow, // ->
    
	Power,
    
	Minus, // u-
	Plus , // u+
	Not  , // !
    
	Multiply, // *
	Divide  , // /
	IDivide , // //
	Modulo  , // %
    
	Add     , // +
	Subtract, // -
    
	ShiftLeft  , // <<
	ShiftRight , // >>
	ShiftRightU, // >>>
    
	LessThan          , // <
	GreaterThan       , // >
	LessThanOrEqual   , // <=
	GreaterThanOrEqual, // >=
    
	Equal   , // ==
	NotEqual, // !=
    
	And, // &
	Xor, // ^
	Or , // |
    
	ConditionalAnd, // &&
	ConditionalOr , // ||
    
	Assign           , // =
	PowerAssign      , // **=
	MultiplyAssign   , // *=
	DivideAssign     , // /=
	IDivideAssign    , // //=
	ModuloAssign     , // %=
	AddAssign        , // +=
	SubtractAssign   , // -=
	ShiftLeftAssign  , // <<=
	ShiftRightAssign , // >>=
	ShiftRightUAssign, // >>>=
	AndAssign        , // &=
	XorAssign        , // ^=
	OrAssign         , // |=
	TypeAssign       , // :=
    
	// Special
	Identifier,
	Literal,
    
	// Keywords
	Class,
	Enum,
	
	Static,
	
	Func,
	New,
	Return,
	
	Var,
	Const,
	
	If,
	Else,
	Switch,
	Case,
	Default,
	
	While,
	For,
	In,
	Continue,
	Break,
}