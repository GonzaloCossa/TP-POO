using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cossa_Gonzalo_TP_Empresa_Mantenimiento_Industrial.Clases
{
    public class Tecnico
    {
        public string Nombre { get; set; }
        public int Legajo { get; set; }
        public string Dni { get; set; }
        public decimal SueldoBasico { get; set; }
        public DateTime FechaNacimiento { get; set; }

        public Tecnico(string nombre, int legajo, string dni, decimal sueldoBasico, DateTime fechaNacimiento)
        {
            Nombre = nombre;
            Legajo = legajo;
            Dni = dni;
            SueldoBasico = sueldoBasico;
            FechaNacimiento = fechaNacimiento;
        }

        /// <summary>
        /// Se encarga de calcular cual es el sueldo real que tiene cada Tecnico que por el enunciado se le reduce un 17%
        /// </summary>
        /// <returns>Retorna en decimal el salario real.</returns>
        public decimal CalcularSueldoNeto()
        {
            return SueldoBasico * 0.83m;
        }
    }
}