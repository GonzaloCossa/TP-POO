using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cossa_Gonzalo_TP_Empresa_Mantenimiento_Industrial.Excepciones
{
    public static class GestorExcepciones
    {
        public static void ManejarNegocio(ExcepcionPersonalizada ex)
        {
            Console.WriteLine("\n==================================================");
            Console.WriteLine($"[REGLA DE NEGOCIO INCUMPLIDA]: {ex.Message}");
            Console.WriteLine("==================================================");
        }

        public static void ManejarSistema(Exception ex)
        {
            Console.WriteLine("\n==================================================");
            Console.WriteLine($"[ERROR DEL SISTEMA]: {ex.Message}");
            Console.WriteLine("==================================================");
        }
    }
}
