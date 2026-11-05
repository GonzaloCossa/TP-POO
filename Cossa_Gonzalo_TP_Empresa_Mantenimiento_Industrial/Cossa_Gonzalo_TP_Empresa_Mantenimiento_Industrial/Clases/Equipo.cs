using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cossa_Gonzalo_TP_Empresa_Mantenimiento_Industrial.Clases
{
    public class Equipo
    {
        private string codigoIdentificador;
        private decimal costoPorHora;
        private bool operativo;

        public string CodigoIdentificador { get => codigoIdentificador; set => codigoIdentificador = value; }
        public decimal CostoPorHora { get => costoPorHora; set => costoPorHora = value; }
        public bool Operativo { get => operativo; set => operativo = value; }

        public Equipo(string codigoIdentificador, decimal costoPorHora, bool operativo)
        {
            CodigoIdentificador = codigoIdentificador;
            CostoPorHora = costoPorHora;
            Operativo = operativo;
        }
    }
}