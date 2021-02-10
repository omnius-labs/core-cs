using System;
using System.Text;

namespace Omnius.Core.RocketPack.DefinitionCompiler
{
    internal sealed class CodeWriter
    {
        private readonly StringBuilder _sb = new StringBuilder();
        private int _indentDepth;
        private bool _wroteIndent;

        public CodeWriter()
        {
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

        private bool TryWriteIndent()
        {
            if (_wroteIndent) return false;

            _wroteIndent = true;

            for (int i = 0; i < _indentDepth; i++)
            {
                _sb.Append("    ");
            }

            return true;
        }

        private struct IndentCookie : IDisposable
        {
            private readonly CodeWriter _codeBuilder;

            public IndentCookie(CodeWriter codeBuilder)
            {
                _codeBuilder = codeBuilder;
            }

            public void Dispose()
            {
                _codeBuilder.PopIndent();
            }
        }
    }
}
