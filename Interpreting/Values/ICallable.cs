using System.Collections.Generic;
namespace Lab3.Interpreting.Values {
	interface ICallable {
		object Call(IReadOnlyList<object> args);
	}
}
