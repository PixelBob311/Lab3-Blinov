using DynamicRuntime.Functions;
namespace DynamicRuntime {
	public static class BuiltinVariables {
		public static object @true = true;
		public static object @false = false;
		public static object @null = null;
		public static object dump = new DumpFunction();
		public static object trace = new TraceFunction();
	}
}
