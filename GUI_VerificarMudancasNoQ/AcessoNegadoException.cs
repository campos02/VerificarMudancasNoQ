using System;
using System.Collections.Generic;
using System.Text;

namespace GUI_VerificarMudancasNoQ
{
    public class AcessoNegadoException : Exception
    {
        public AcessoNegadoException()
        {
        }

        public AcessoNegadoException(string message) : base(message)
        {
        }

        public AcessoNegadoException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
