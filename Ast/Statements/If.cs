namespace Lab3.Ast.Statements {
	sealed class If : IStatement {
		public readonly IExpression Condition;
		public readonly Block Body;
		public string FormattedString => $"if ({Condition.FormattedString}) {Body.FormattedString}";
		public If(IExpression condition, Block body) {
			Condition = condition;
			Body = body;
		}
		public void Accept(IStatementVisitor visitor) => visitor.VisitIf(this);
		public T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitIf(this);
	}
}
