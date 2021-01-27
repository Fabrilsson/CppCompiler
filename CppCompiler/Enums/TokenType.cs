namespace CppCompiler.Enums
{
    enum TokenType
    {
        Undefined,

        // Commands
        IfCommand,
        ElseCommand,
        ForCommand,
        WhileCommand,
        DoCommand,
        SwitchCommand,
        CaseCommand,
        DefaultCommand,
        BreakCommand,
        ContinueCommand,
        ReturnCommand,
        
        // Boolean Operators
        AndOperator,
        OrOperator,
        NotOperator,

        // Comparsion Operators
        EqualToOperator,
        NotEqualToOperator,
        LessThanOperator,
        GreaterThanOperator,
        LessThanOrEqualToOperator,
        GreaterThanOrEqualToOperator,

        // Arithmetic Operators
        AdditionOperator,
        SubtractionOperator,
        MultiplicationOperator,
        DivisionOperator,
        ModuleOperator,
        AssignmentOperator,
        PowOperator,

        // Compound Operators
        IncrementOperator,
        DecrementOperator,
        CompoundAdditionOperator,
        CompoundSubtractionOperator,
        CompoundMultiplicationOperator,
        CompoundDivisionOperator,
        CompoundModuleOperator,

        // Identifier
        Identifier,

        // Syntax Symbols
        Colon,
        Semicolon,
        Comma,
        LeftBracers,
        RightBracers,
        LeftParenthesis,
        RightParenthesis,
        LeftBrackets,
        RightBrackets,

        // Types
        IntType,
        FloatType,
        LongType,
        ShortType,
        CharType,
        UnsignedType,
        VoidType,
        StructType,

        // Constants
        BooleanConstant,
        IntegerConstant,
        LongConstant,
        FloatingPointConstant,
        CharConstant,
        StringConstant
    }
}