using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cossa_Gonzalo_TP_Empresa_Mantenimiento_Industrial.Excepciones
{
    public class EliminacionAsignadaException : ExcepcionPersonalizada
    {
        public EliminacionAsignadaException(string mensaje) : base(mensaje) { }
    }
}
