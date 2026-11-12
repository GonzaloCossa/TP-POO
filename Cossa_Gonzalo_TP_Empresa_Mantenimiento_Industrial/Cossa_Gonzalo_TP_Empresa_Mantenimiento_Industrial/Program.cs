using Cossa_Gonzalo_TP_Empresa_Mantenimiento_Industrial.Clases;
using Cossa_Gonzalo_TP_Empresa_Mantenimiento_Industrial.Excepciones;

namespace Cossa_Gonzalo_TP_Empresa_Mantenimiento_Industrial
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Empresa empresa = new Empresa("Mantenimiento Industrial S.A.", "30-98765432-1");
            empresa.Tecnicos = GestorArchivos.CargarTecnicos();
            empresa.Equipos = GestorArchivos.CargarEquipos();
            empresa.Ordenes = GestorArchivos.CargarOrdenes(empresa.Equipos, empresa.Tecnicos);
            bool salir = false;

            while (!salir)
            {
                try
                {
                    Funciones.mostrarMenu(empresa);
                    string opcion = Console.ReadLine()!;

                    switch (opcion)
                    {
                        case "1":
                            Funciones.registrarTecnico(empresa);
                            break;
                        case "2":
                            Funciones.registrarEquipo(empresa);
                            break;
                        case "3":
                            Funciones.programarOrden(empresa);
                            break;
                        case "4":
                            Funciones.iniciarOrden(empresa);
                            break;
                        case "5":
                            Funciones.cancelarOrden(empresa);
                            break;
                        case "6":
                            Funciones.cierreParcial(empresa);
                            break;
                        case "7":
                            Funciones.cierreDelDia(empresa);
                            break;
                        case "8":
                            Funciones.consultasYReportes(empresa);
                            break;
                        case "9":
                            Funciones.eliminarTecnicoOEquipo(empresa);
                            break;
                        case "10":
                            GestorArchivos.GuardarTecnicos(empresa.Tecnicos);
                            GestorArchivos.GuardarEquipos(empresa.Equipos);
                            GestorArchivos.GuardarOrdenes(empresa.Ordenes);
                            salir = true;
                            Console.WriteLine("\nSaliendo del sistema...");
                            break;
                        default:
                            Console.WriteLine("\nOpción inválida.");
                            Console.ReadKey();
                            break;
                    }
                }
                catch (ExcepcionPersonalizada ex)
                {
                    GestorExcepciones.ManejarNegocio(ex);
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    GestorExcepciones.ManejarSistema(ex);
                    Console.ReadKey();
                }
            }
        }
    }
}
