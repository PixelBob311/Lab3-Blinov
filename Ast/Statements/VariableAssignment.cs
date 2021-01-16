namespace Lab3.Ast.Statements {
	sealed class VariableAssignment : IStatement {
		public readonly string VariableName;
		public readonly IExpression Expr;
		public string FormattedString => $"{VariableName} = {Expr.FormattedString};\n";
		public VariableAssignment(string variableName, IExpression expr) {
			VariableName = variableName;
			Expr = expr;
		}
		public void Accept(IStatementVisitor visitor) => visitor.VisitVariableAssignment(this);
		public T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitVariableAssignment(this);
	}
}
