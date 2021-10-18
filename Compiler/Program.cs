namespace Compiler
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var compiler = new CompilerC();
            compiler.Compile();
            compiler.WriteCompileFile();
        }
    }
}