using System.Collections.Generic;
namespace DynamicRuntime {
	interface ICallable {
		object Call(IReadOnlyList<object> args);
	}
}
