using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cossa_Gonzalo_TP_Empresa_Mantenimiento_Industrial.Excepciones
{
    public class EquipoOcupadoException : ExcepcionPersonalizada
    {
        public EquipoOcupadoException(string mensaje) : base(mensaje) { }
    }
}
