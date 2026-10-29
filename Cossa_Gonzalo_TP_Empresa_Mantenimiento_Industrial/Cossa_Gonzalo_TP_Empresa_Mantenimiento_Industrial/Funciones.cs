using Cossa_Gonzalo_TP_Empresa_Mantenimiento_Industrial.Clases;
using Cossa_Gonzalo_TP_Empresa_Mantenimiento_Industrial.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cossa_Gonzalo_TP_Empresa_Mantenimiento_Industrial
{
    public class Funciones
    {
        public void mostrarMenu(Empresa empresa)
        {
            Console.Clear();
            Console.WriteLine("========================================================================");
            Console.WriteLine($"   EMPRESA: {empresa.Nombre} (CUIT: {empresa.Cuit})");
            Console.WriteLine("========================================================================");
            Console.WriteLine("1. Registrar técnico");
            Console.WriteLine("2. Registrar equipo");
            Console.WriteLine("3. Programar orden de servicio");
            Console.WriteLine("4. Iniciar una orden");
            Console.WriteLine("5. Cancelar una orden");
            Console.WriteLine("6. Realizar cierre parcial de la jornada");
            Console.WriteLine("7. Consultar órdenes y reportes");
            Console.WriteLine("8. Eliminar técnico o equipo");
            Console.WriteLine("9. Salir");
            Console.Write("\nSeleccione una opción (1-9): ");
        }

        public void registrarTecnico(Empresa empresa)
        {
            Console.Clear();
            Console.WriteLine("--- REGISTRAR TÉCNICO ---");

            Console.Write("\nIngrese el nombre del técnico: ");
            string nombre = Console.ReadLine()!;

            Console.Write("Ingrese el legajo (número): ");
            int legajo = Convert.ToInt32(Console.ReadLine());

            Console.Write("Ingrese el DNI: ");
            string dni = Console.ReadLine()!;

            Console.Write("Ingrese el sueldo básico mensual: ");
            decimal sueldo = Convert.ToDecimal(Console.ReadLine());

            Console.Write("Ingrese la fecha de nacimiento (DD-MM-YYYY): ");
            DateTime fechaNacimiento = DateTime.ParseExact(Console.ReadLine()!, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            Tecnico nuevoTecnico = new Tecnico(nombre, legajo, dni, sueldo, fechaNacimiento);
            empresa.RegistrarTecnico(nuevoTecnico);

            Console.WriteLine("\n¡Técnico registrado con éxito! Pulse cualquier tecla para continuar...");
            Console.ReadKey();
        }

        public void registrarEquipo(Empresa empresa)
        {
            Console.Clear();
            Console.WriteLine("--- REGISTRAR EQUIPO ---");

            Console.Write("\nIngrese el código identificador del equipo: ");
            string codigo = Console.ReadLine()!;

            Console.Write("Ingrese el costo de utilización por hora: ");
            decimal costoHora = Convert.ToDecimal(Console.ReadLine());

            Console.Write("¿Se encuentra operativo? (s/n): ");
            string respuesta = Console.ReadLine()!;
            bool operativo = (respuesta.ToLower() == "s");

            Equipo nuevoEquipo = new Equipo(codigo, costoHora, operativo);
            empresa.RegistrarEquipo(nuevoEquipo);

            Console.WriteLine("\n¡Equipo registrado con éxito! Pulse cualquier tecla para continuar...");
            Console.ReadKey();
        }

        public void programarOrden(Empresa empresa)
        {
            Console.Clear();
            Console.WriteLine("--- PROGRAMAR ORDEN DE SERVICIO ---");

            Console.Write("\nIngrese el ID de la orden: ");
            int id = Convert.ToInt32(Console.ReadLine());

            Console.Write("Ingrese el cliente: ");
            string cliente = Console.ReadLine()!;

            Console.Write("Ingrese la descripción del trabajo: ");
            string descripcion = Console.ReadLine()!;

            Console.Write("Ingrese la fecha (DD-MM-YYYY): ");
            DateTime fecha = DateTime.ParseExact(Console.ReadLine()!, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            Console.Write("Ingrese el horario de inicio (HH:MM): ");
            TimeSpan horarioInicio = TimeSpan.Parse(Console.ReadLine()!);

            Console.Write("Ingrese las horas estimadas de duración: ");
            double horasEstimadas = Convert.ToDouble(Console.ReadLine());

            Console.Write("Ingrese el precio base: ");
            decimal precioBase = Convert.ToDecimal(Console.ReadLine());

            // Selección de equipo
            Console.WriteLine("\nEquipos disponibles:");
            foreach (Equipo e in empresa.Equipos)
            {
                Console.WriteLine($"Código: {e.CodigoIdentificador} | Operativo: {(e.Operativo ? "Operativo" : "Fuera de Servicio")}");
            }
            Console.Write("Ingrese el código del equipo a asignar: ");
            string codEquipo = Console.ReadLine()!;
            Equipo equipoAsignado = empresa.Equipos.Find(e => e.CodigoIdentificador == codEquipo)!;

            if (equipoAsignado == null)
            {
                Console.WriteLine("\n[Error]: El equipo no existe.");
                Console.ReadKey();
                return;
            }

            // Selección de técnicos
            List<Tecnico> tecnicosSeleccionados = new List<Tecnico>();
            char continuar = 's';
            while (continuar == 's')
            {
                Console.WriteLine("\nTécnicos disponibles:");
                foreach (Tecnico t in empresa.Tecnicos)
                {
                    Console.WriteLine($"Legajo: {t.Legajo} | Nombre: {t.Nombre}");
                }
                Console.Write("Ingrese el legajo del técnico a agregar: ");
                int leg = Convert.ToInt32(Console.ReadLine());
                Tecnico tec = empresa.Tecnicos.Find(t => t.Legajo == leg)!;

                if (tec != null)
                {
                    tecnicosSeleccionados.Add(tec);
                }

                Console.Write("¿Desea agregar otro técnico? (s/n): ");
                continuar = Convert.ToChar(Console.ReadLine()!.ToLower());
            }

            // Tipo de mantenimiento
            Console.Write("\n¿Es mantenimiento Preventivo (1) o Correctivo (2)?: ");
            string tipo = Console.ReadLine()!;

            OrdenServicio nuevaOrden;
            if (tipo == "1")
            {
                nuevaOrden = new MantenimientoPreventivo(id, cliente, descripcion, fecha, horarioInicio, horasEstimadas, equipoAsignado, precioBase);
            }
            else
            {
                Console.Write("Ingrese la urgencia (Baja / Media / Alta): ");
                string urgStr = Console.ReadLine()!.Trim().ToLower();

                Urgencia urgencia = urgStr switch
                {
                    "baja" => Urgencia.Baja,
                    "media" => Urgencia.Media,
                    "alta" => Urgencia.Alta,
                    _ => throw new ArgumentException("Urgencia inválida. Debe ser Baja, Media o Alta.")
                };

                nuevaOrden = new MantenimientoCorrectivo(id, cliente, descripcion, fecha, horarioInicio, horasEstimadas, equipoAsignado, precioBase, urgencia);
            }

            empresa.ProgramarOrden(nuevaOrden, equipoAsignado, tecnicosSeleccionados);

            Console.WriteLine("\n¡Orden programada exitosamente! Pulse cualquier tecla para continuar...");
            Console.ReadKey();
        }

        public void iniciarOrden(Empresa empresa)
        {
            CambiarEstadoOrden(empresa, "--- INICIAR ORDEN DE SERVICIO ---", "\nIngrese el ID de la orden a iniciar: ", EstadoOrden.EnCurso, "\nLa orden ha pasado a estado En Curso.");
        }

        public void cancelarOrden(Empresa empresa)
        {
            CambiarEstadoOrden(empresa, "--- CANCELAR ORDEN DE SERVICIO ---", "\nIngrese el ID de la orden a cancelar: ", EstadoOrden.Cancelada, "\nLa orden ha sido cancelada.");
        }

        // Método auxiliar privado para no repetir lógica al iniciar o cancelar órdenes
        private void CambiarEstadoOrden(Empresa empresa, string titulo, string mensajeInput, EstadoOrden nuevoEstado, string mensajeExito)
        {
            Console.Clear();
            Console.WriteLine(titulo);
            Console.Write(mensajeInput);
            int id = Convert.ToInt32(Console.ReadLine());

            OrdenServicio orden = empresa.Ordenes.Find(o => o.IdOrden == id)!;
            if (orden != null)
            {
                orden.Estado = nuevoEstado;
                Console.WriteLine(mensajeExito);
            }
            else
            {
                Console.WriteLine("\n[Error]: No se encontró la orden.");
            }

            Console.Write("\nPulse cualquier tecla para continuar...");
            Console.ReadKey();
        }

        public void cierreParcial(Empresa empresa)
        {
            Console.Clear();
            Console.WriteLine("--- CIERRE PARCIAL DE LA JORNADA ---");
            Console.Write("\nIngrese la fecha del cierre (DD-MM-YYYY): ");
            DateTime fechaConsulta = DateTime.ParseExact(Console.ReadLine()!, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            decimal recaudacionTotal = 0;
            int countPreventivo = 0;
            int countCorrectivo = 0;

            foreach (OrdenServicio o in empresa.Ordenes)
            {
                if (o.Fecha.Date == fechaConsulta.Date && o.HorarioFin <= DateTime.Now.TimeOfDay)
                {
                    if (o.Estado != EstadoOrden.Cancelada)
                    {
                        o.Estado = EstadoOrden.Finalizada;
                        recaudacionTotal += o.CalcularImporte();

                        if (o.EsPreventivo) countPreventivo++;
                        else countCorrectivo++;
                    }
                }
            }

            Console.WriteLine($"\nRecaudación Total Parcial: ${recaudacionTotal}");
            Console.WriteLine($"Órdenes Preventivas procesadas: {countPreventivo}");
            Console.WriteLine($"Órdenes Correctivas procesadas: {countCorrectivo}");

            Console.Write("\nPulse cualquier tecla para continuar...");
            Console.ReadKey();
        }

        public void consultasYReportes(Empresa empresa)
        {
            bool volver = false;
            while (!volver)
            {
                Console.Clear();
                Console.WriteLine("==================================================");
                Console.WriteLine("              --- CONSULTAS Y REPORTES ---         ");
                Console.WriteLine("==================================================");
                Console.WriteLine("1. Listado de técnicos y sus órdenes asignadas");
                Console.WriteLine("2. Listado de equipos y su estado");
                Console.WriteLine("3. Órdenes por fecha");
                Console.WriteLine("4. Órdenes por estado");
                Console.WriteLine("5. Órdenes asociadas a un técnico");
                Console.WriteLine("6. Órdenes asociadas a un equipo");
                Console.WriteLine("7. Volver al menú principal");
                Console.Write("\nSeleccione una opción de reporte (1-7): ");

                string subOpt = Console.ReadLine()!;

                switch (subOpt)
                {
                    case "1": ReporteTecnicosYOrdenes(empresa); break;
                    case "2": ReporteEquiposYEstado(empresa); break;
                    case "3": ReporteOrdenesPorFecha(empresa); break;
                    case "4": ReporteOrdenesPorEstado(empresa); break;
                    case "5": ReporteOrdenesPorTecnico(empresa); break;
                    case "6": ReporteOrdenesPorEquipo(empresa); break;
                    case "7": volver = true; break;
                    default:
                        Console.WriteLine("\n[Aviso]: Opción inválida.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ReporteTecnicosYOrdenes(Empresa empresa)
        {
            Console.Clear();
            Console.WriteLine("--- TÉCNICOS Y SUS ÓRDENES ASIGNADAS ---\n");
            foreach (Tecnico t in empresa.Tecnicos)
            {
                Console.WriteLine($"Técnico: {t.Nombre} (Legajo: {t.Legajo}) - Sueldo Neto: ${t.CalcularSueldoNeto()}");
                Console.WriteLine("  Órdenes asignadas:");

                bool tieneOrdenes = false;
                foreach (OrdenServicio o in empresa.Ordenes)
                {
                    if (o.TecnicosAsignados.Any(tec => tec.Legajo == t.Legajo))
                    {
                        Console.WriteLine($"    - ID: {o.IdOrden} | Cliente: {o.Cliente} | Fecha: {o.Fecha.ToShortDateString()} | Estado: {o.Estado}");
                        tieneOrdenes = true;
                    }
                }
                if (!tieneOrdenes)
                {
                    Console.WriteLine("    (Sin órdenes asignadas)");
                }
                Console.WriteLine("--------------------------------------------------");
            }
            Console.Write("\nPulse cualquier tecla para continuar...");
            Console.ReadKey();
        }

        private void ReporteEquiposYEstado(Empresa empresa)
        {
            Console.Clear();
            Console.WriteLine("--- LISTADO DE EQUIPOS Y SU ESTADO ---\n");
            foreach (Equipo e in empresa.Equipos)
            {
                string estadoTxt = e.Operativo ? "Operativo" : "Fuera de Servicio";
                Console.WriteLine($"Código: {e.CodigoIdentificador} | Costo/Hora: ${e.CostoPorHora} | Estado: {estadoTxt}");
            }
            Console.Write("\nPulse cualquier tecla para continuar...");
            Console.ReadKey();
        }

        private void ReporteOrdenesPorFecha(Empresa empresa)
        {
            Console.Clear();
            Console.WriteLine("--- ÓRDENES POR FECHA ---\n");
            Console.Write("Ingrese la fecha a consultar (DD-MM-YYYY): ");
            DateTime fechaBuscada = DateTime.ParseExact(Console.ReadLine()!, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            Console.WriteLine($"\nÓrdenes programadas para el {fechaBuscada.ToShortDateString()}:\n");
            bool encontradas = false;
            foreach (OrdenServicio o in empresa.Ordenes)
            {
                if (o.Fecha.Date == fechaBuscada.Date)
                {
                    Console.WriteLine($"[ID: {o.IdOrden}] Cliente: {o.Cliente} | Horario: {o.HorarioInicio} a {o.HorarioFin} | Estado: {o.Estado}");
                    encontradas = true;
                }
            }
            if (!encontradas)
            {
                Console.WriteLine("No se registraron órdenes para esta fecha.");
            }
            Console.Write("\nPulse cualquier tecla para continuar...");
            Console.ReadKey();
        }

        private void ReporteOrdenesPorEstado(Empresa empresa)
        {
            Console.Clear();
            Console.WriteLine("--- ÓRDENES POR ESTADO ---\n");
            Console.WriteLine("Estados disponibles: Programada, EnCurso, Finalizada, Cancelada");
            Console.Write("Ingrese el estado a filtrar: ");
            string estadoStr = Console.ReadLine()!;

            if (Enum.TryParse<EstadoOrden>(estadoStr, true, out EstadoOrden estadoBuscado))
            {
                Console.WriteLine($"\nÓrdenes con estado '{estadoBuscado}':\n");
                bool encontradas = false;
                foreach (OrdenServicio o in empresa.Ordenes)
                {
                    if (o.Estado == estadoBuscado)
                    {
                        Console.WriteLine($"[ID: {o.IdOrden}] Cliente: {o.Cliente} | Fecha: {o.Fecha.ToShortDateString()} | Equipo: {o.EquipoAsignado.CodigoIdentificador}");
                        encontradas = true;
                    }
                }
                if (!encontradas)
                {
                    Console.WriteLine("No hay órdenes con ese estado.");
                }
            }
            else
            {
                Console.WriteLine("\n[Error]: Estado ingresado inválido.");
            }

            Console.Write("\nPulse cualquier tecla para continuar...");
            Console.ReadKey();
        }

        private void ReporteOrdenesPorTecnico(Empresa empresa)
        {
            Console.Clear();
            Console.WriteLine("--- ÓRDENES ASOCIADAS A UN TÉCNICO ---\n");

            if (empresa.Tecnicos.Count == 0)
            {
                Console.WriteLine("No hay técnicos registrados en el sistema.\n");
                Console.Write("\nPulse cualquier tecla para volver...");
                Console.ReadKey();
                return;
            }
            else
            {
                Console.WriteLine("Técnicos registrados:");
                foreach (Tecnico t in empresa.Tecnicos)
                {
                    Console.WriteLine($"Legajo: {t.Legajo} | Nombre: {t.Nombre}");
                }
                Console.WriteLine();
            }

            Console.Write("Ingrese el legajo del técnico: ");
            int legajo = Convert.ToInt32(Console.ReadLine());

            Tecnico tecnico = empresa.Tecnicos.Find(t => t.Legajo == legajo)!;
            if (tecnico == null)
            {
                Console.WriteLine("\n[Error]: El técnico no existe.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\nÓrdenes asociadas al técnico {tecnico.Nombre}:\n");
            bool encontradas = false;
            foreach (OrdenServicio o in empresa.Ordenes)
            {
                if (o.TecnicosAsignados.Any(t => t.Legajo == legajo))
                {
                    Console.WriteLine($"[ID: {o.IdOrden}] Cliente: {o.Cliente} | Fecha: {o.Fecha.ToShortDateString()} | Estado: {o.Estado}");
                    encontradas = true;
                }
            }
            if (!encontradas)
            {
                Console.WriteLine("El técnico no posee órdenes asociadas.");
            }
            Console.Write("\nPulse cualquier tecla para continuar...");
            Console.ReadKey();
        }

        private void ReporteOrdenesPorEquipo(Empresa empresa)
        {
            Console.Clear();
            Console.WriteLine("--- ÓRDENES ASOCIADAS A UN EQUIPO ---\n");

            if (empresa.Equipos.Count == 0)
            {
                Console.WriteLine("No hay equipos registrados en el sistema.\n");
                Console.Write("\nPulse cualquier tecla para volver...");
                Console.ReadKey();
                return;
            }
            else
            {
                Console.WriteLine("Equipos registrados:");
                foreach (Equipo e in empresa.Equipos)
                {
                    string estadoTxt = e.Operativo ? "Operativo" : "Fuera de Servicio";
                    Console.WriteLine($"Código: {e.CodigoIdentificador} | Estado: {estadoTxt} | Costo/Hora: ${e.CostoPorHora}");
                }
                Console.WriteLine();
            }

            Console.Write("Ingrese el código del equipo: ");
            string codigo = Console.ReadLine()!;

            Equipo equipo = empresa.Equipos.Find(e => e.CodigoIdentificador.Equals(codigo, StringComparison.OrdinalIgnoreCase))!;
            if (equipo == null)
            {
                Console.WriteLine("\n[Error]: El equipo no existe.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\nÓrdenes asociadas al equipo {equipo.CodigoIdentificador}:\n");
            bool encontradas = false;
            foreach (OrdenServicio o in empresa.Ordenes)
            {
                if (o.EquipoAsignado.CodigoIdentificador.Equals(codigo, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"[ID: {o.IdOrden}] Cliente: {o.Cliente} | Fecha: {o.Fecha.ToShortDateString()} | Estado: {o.Estado}");
                    encontradas = true;
                }
            }
            if (!encontradas)
            {
                Console.WriteLine("El equipo no posee órdenes asociadas.");
            }
            Console.Write("\nPulse cualquier tecla para continuar...");
            Console.ReadKey();
        }

        public void eliminarTecnicoOEquipo(Empresa empresa)
        {
            Console.Clear();
            Console.WriteLine("--- ELIMINAR RECURSO ---");
            Console.Write("¿Desea eliminar un Técnico (1) o un Equipo (2)?: ");
            string tipo = Console.ReadLine()!;

            if (tipo == "1")
            {
                Console.Clear();
                Console.WriteLine("--- TÉCNICOS REGISTRADOS ---\n");
                if (empresa.Tecnicos.Count == 0)
                {
                    Console.WriteLine("No hay técnicos registrados en el sistema.");
                    Console.Write("\nPulse cualquier tecla para volver...");
                    Console.ReadKey();
                    return;
                }
                else
                {
                    foreach (Tecnico t in empresa.Tecnicos)
                    {
                        Console.WriteLine($"Legajo: {t.Legajo} | Nombre: {t.Nombre} | DNI: {t.Dni}");
                    }
                }

                Console.Write("\nIngrese el legajo del técnico a eliminar: ");
                int legajo = Convert.ToInt32(Console.ReadLine());

                empresa.EliminarTecnicoOEquipo(legajo, null!);
                Console.WriteLine("\n¡Técnico eliminado con éxito!");
            }
            else if (tipo == "2")
            {
                Console.Clear();
                Console.WriteLine("--- EQUIPOS REGISTRADOS ---\n");
                if (empresa.Equipos.Count == 0)
                {
                    Console.WriteLine("No hay equipos registrados en el sistema.");
                    Console.Write("\nPulse cualquier tecla para volver...");
                    Console.ReadKey();
                    return;
                }
                else
                {
                    foreach (Equipo e in empresa.Equipos)
                    {
                        string estadoTxt = e.Operativo ? "Operativo" : "Fuera de Servicio";
                        Console.WriteLine($"Código: {e.CodigoIdentificador} | Estado: {estadoTxt} | Costo/Hora: ${e.CostoPorHora}");
                    }
                }

                Console.Write("\nIngrese el código del equipo a eliminar: ");
                string codigo = Console.ReadLine()!;

                empresa.EliminarTecnicoOEquipo(null, codigo);
                Console.WriteLine("\n¡Equipo eliminado con éxito!");
            }
            else
            {
                Console.WriteLine("\n[Error]: Opción inválida.");
            }

            Console.Write("\nPulse cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}