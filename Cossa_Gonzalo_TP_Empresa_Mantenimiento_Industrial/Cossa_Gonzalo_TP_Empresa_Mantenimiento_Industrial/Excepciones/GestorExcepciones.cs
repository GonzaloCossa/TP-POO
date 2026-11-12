using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cossa_Gonzalo_TP_Empresa_Mantenimiento_Industrial.Excepciones
{
    public static class GestorExcepciones
    {
        // Con la clase ExcepcionPersonalizada nos encargamos de separar justamente las nuevas excepciones creadas para el proyecto,
        // pudiendo mostrarlas desde el mismo punto.
        public static void ManejarNegocio(ExcepcionPersonalizada ex)
        {
            Console.WriteLine("\n=================================================================================================================");
            Console.WriteLine($"[REGLA DE NEGOCIO INCUMPLIDA]: {ex.Message}");
            Console.WriteLine("=================================================================================================================");
        }

        // En caso de haber una excepción generica o con un mensaje personalizado, por este punto las mostramos.
        public static void ManejarSistema(Exception ex)
        {
            Console.WriteLine("\n=================================================================================================================");
            Console.WriteLine($"[ERROR DEL SISTEMA]: {ex.Message}");
            Console.WriteLine("=================================================================================================================");
        }
    }
}
