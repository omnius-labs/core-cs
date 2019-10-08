using System;
using System.Text;

namespace Omnix.Serialization.RocketPack.DefinitionCompiler
{
    internal sealed class CodeBuilder
    {
        private readonly StringBuilder _sb = new StringBuilder();
        private int _indentDepth = 0;
        private bool _wroteIndent = false;

        public CodeBuilder()
        {

        }

        private bool TryWriteIndent()
        {
            if (_wroteIndent)
            {
                return false;
            }

            _wroteIndent = true;

            for (int i = 0; i < _indentDepth; i++)
            {
                _sb.Append("    ");
            }

            return true;
        }

        public void WriteLine()
        {
            _sb.AppendLine();
            _wroteIndent = false;
        }

        public void WriteLine(string value)
        {
            foreach (var line in value.Split(new string[] { "\r\n", "\r", "\n" }, options: StringSplitOptions.None))
            {
                this.TryWriteIndent();
                _sb.AppendLine(line);
                _wroteIndent = false;
            }
        }

        public IDisposable Indent()
        {
            this.PushIndent();
            return new IndentCookie(this);
        }

        private struct IndentCookie : IDisposable
        {
            private CodeBuilder _CodeBuilder;

            public IndentCookie(CodeBuilder CodeBuilder)
            {
                _CodeBuilder = CodeBuilder;
            }

            public void Dispose()
            {
                _CodeBuilder.PopIndent();
            }
        }

        public void PushIndent()
        {
            _indentDepth++;
        }

        public void PopIndent()
        {
            _indentDepth--;
        }

        public override string ToString()
        {
            return _sb.ToString().Replace("\r\n", "\n");
        }
    }
}
