namespace CppCompiler
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
        LeftChaves,
        RightChaves,
        LeftParenthesis,
        RightParenthesis,
        LeftColchete,
        RightColchete,

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