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
            Funciones funciones = new Funciones();
            bool salir = false;

            while (!salir)
            {
                try
                {
                    funciones.mostrarMenu(empresa);
                    string opcion = Console.ReadLine()!;

                    switch (opcion)
                    {
                        case "1":
                            funciones.registrarTecnico(empresa);
                            break;
                        case "2":
                            funciones.registrarEquipo(empresa);
                            break;
                        case "3":
                            funciones.programarOrden(empresa);
                            break;
                        case "4":
                            funciones.iniciarOrden(empresa);
                            break;
                        case "5":
                            funciones.cancelarOrden(empresa);
                            break;
                        case "6":
                            funciones.cierreParcial(empresa);
                            break;
                        case "7":
                            funciones.consultasYReportes(empresa);
                            break;
                        case "8":
                            funciones.eliminarTecnicoOEquipo(empresa);
                            break;
                        case "9":
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
