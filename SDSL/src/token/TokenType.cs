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
    Scope    , // ::
    Colon    , // :
    Semicolon, // ;
    
    Comma , // ,
    Dot   , // .
    Elipse, // ..
    
    Question, // ?
    
    Arrow, // ->
    
    Power,
    
    Minus, // -
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
    Namespace,
    Using,
    Class,
    Func,
    New,
    Static,
    Var,
    Const,
    Return,
    If,
    Else,
    While,
    Continue,
    Break,
    For,
    In,
    Constepxr,
    Switch,
    Case,
    Default,
    Enum,
}