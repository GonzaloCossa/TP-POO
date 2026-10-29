using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cossa_Gonzalo_TP_Empresa_Mantenimiento_Industrial.Clases
{
    public class Equipo
    {

        public string CodigoIdentificador { get; set; }
        public decimal CostoPorHora { get; set; }
        public bool Operativo { get; set; }

        public Equipo(string codigoIdentificador, decimal costoPorHora, bool operativo)
        {
            CodigoIdentificador = codigoIdentificador;
            CostoPorHora = costoPorHora;
            Operativo = operativo;
        }
    }
}