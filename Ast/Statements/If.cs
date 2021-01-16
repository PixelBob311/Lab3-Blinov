namespace Lab3.Ast.Statements {
	sealed class If : IStatement {
		public readonly IExpression Condition;
		public readonly Block Body;
		public readonly INode ElseIf;
		public string FormattedString {
			get {
				if (ElseIf == null) {
					return $"if ({Condition.FormattedString}) {Body.FormattedString}";
				}
				else {
					return $"if ({Condition.FormattedString}) {Body.FormattedString} else {ElseIf.FormattedString}";
				}
			}
		}
		public If(IExpression condition, Block body, INode elseIf) {
			Condition = condition;
			Body = body;
			ElseIf = elseIf;
		}
		public void Accept(IStatementVisitor visitor) => visitor.VisitIf(this);
		public T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitIf(this);
	}
}
