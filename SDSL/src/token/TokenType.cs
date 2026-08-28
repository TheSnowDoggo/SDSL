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
    
    Comma,  // ,
    Dot,    // .
    Elipse, // ..
    
    Question, // ?
    
    Arrow, // ->
    
    Power,
    
    Minus, // -
    Not,   // !
    
    Multiply, // *
    Divide,   // /
    IDivide,  // //
    Modulo,   // %
    Add,      // +
    Subtract, // -
    
    LessThan,           // <
    GreaterThan,        // >
    LessThanOrEqual,    // <=
    GreaterThanOrEqual, // >=
    
    Equal,    // ==
    NotEqual, // !=
    
    And, // &
    Xor, // ^
    Or,  // |
    
    ConditionalAnd, // &&
    ConditionalOr,  // ||
    
    Assign,         // =
    PowerAssign,    // **=
    MultiplyAssign, // *=
    DivideAssign,   // /=
    IDivideAssign,  // //=
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
    If,
    Else,
    While,
    Continue,
    Break,
    For,
    In,
}