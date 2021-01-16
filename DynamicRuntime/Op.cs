using System;
namespace DynamicRuntime {
	public static class Op {
		public static object Add(object a, object b) {
			return To<int>(a) + To<int>(b);
		}
		public static object Sub(object a, object b) {
			return To<int>(a) - To<int>(b);
		}
		public static object Mul(object a, object b) {
			return To<int>(a) * To<int>(b);
		}
		public static object Div(object a, object b) {
			return To<int>(a) / To<int>(b);
		}
		public static object Rem(object a, object b) {
			return To<int>(a) % To<int>(b);
		}
		public static object Eq(object a, object b) {
			if (a == null) {
				return b == null;
			}
			if (b == null) {
				return a == null;
			}
			if (a.GetType() != b.GetType()) {
				return false;
			}
			if (a is int) {
				return (int)a == (int)b;
			}
			if (a is bool) {
				return (bool)a == (bool)b;
			}
			if (a is IReferenceEquatable) {
				return a == b;
			}
			throw MakeError($"Неподдерживаемые типы операндов {a} {b}");
		}
		public static object Lt(object a, object b) {
			if (a == null && b == null) {
				return false;
			}
			if (a is bool && b is bool) {
				return !(bool)a && (bool)b;
			}
			if (a is int && b is int) {
				return (int)a < (int)b;
			}
			throw MakeError($"Неподдерживаемые типы операндов {a} {b}");
		}
		public static bool ToBool(object value) {
			return To<bool>(value);
		}
		public static object Call(object value, params object[] arguments) {
			if (!(value is ICallable function)) {
				throw MakeError($"Вызвали не функцию, а {value}");
			}
			return function.Call(arguments);
		}
		static T To<T>(object value) {
			if (!(value is T)) {
				throw MakeError($"Ожидали {typeof(T)}, получили {value}");
			}
			return (T)value;
		}
		static Exception MakeError(string message) {
			return new Exception(message);
		}
	}
}
