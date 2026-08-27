namespace SDSL;

public enum TokenType
{
    // Brackets
    OpenParen,   // (
    CloseParen,  // )
    OpenBrace,   // {
    CloseBrace,  // }
    OpenSquare,  // [
    CloseSquare, // ]
    
    // Operators
    Scope,     // ::
    Colon,     // :
    Semicolon, // ;
    
    Comma, // ,
    Dot,   // .
    
    Arrow, // ->
    
    UnaryMinus, // -
    Not,        // !
    
    Multiply, // *
    Divide,   // /
    Modulo,   // %
    Add,      // +
    Subtract, // -
    
    LessThan,           // <
    GreaterThan,        // >
    LessThanOrEqual,    // <=
    GreaterThanOrEqual, // >=
    
    Equals,    // ==
    NotEquals, // !=
    
    And, // &
    Xor, // ^
    Or,  // |
    
    ConditionalAnd, // &&
    ConditionalOr,  // ||
    
    Assign,         // =
    MultiplyAssign, // *=
    DivideAssign,   // /=
    ModuloAssign,   // %=
    AddAssign,      // +=
    SubtractAssign, // -=
    AndAssign,      // &=
    XorAssign,      // ^=
    OrAssign,       // |=
    
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
}