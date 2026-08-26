namespace SDSL;

public enum TokenType
{
    // Brackets
    OpenParen,  // (
    CloseParen, // )
    OpenBrace,  // {
    CloseBrace, // }
    
    // Operators
    Scope,     // ::
    Colon,     // :
    Semicolon, // ;
    
    Comma, // ,
    Dot,   // .
    
    Arrow, // ->
    
    UnaryMinus, // -
    
    Multiply, // *
    Divide,   // /
    Modulo,   // %
    Add,      // +
    Subtract, // -
    
    Assign, // =
    
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
}