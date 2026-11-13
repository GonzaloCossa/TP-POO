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
    public static class Funciones
    {
        /// <summary>
        /// Muestra el menú principal de opciones del programa.
        /// </summary>
        /// <param name="empresa">Recibe una empresa por parametro para mostrar en su nombre y cuit.</param>
        public static void mostrarMenu(Empresa empresa)
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
            Console.WriteLine("7. Realizar cierre total de la jornada");
            Console.WriteLine("8. Consultar órdenes y reportes");
            Console.WriteLine("9. Eliminar técnico o equipo");
            Console.WriteLine("10. Salir");
            Console.Write("\nSeleccione una opción (1-10): ");
        }

        /// <summary>
        /// Solicita por consola los datos necesarios y registra un nuevo técnico en el sistema.
        /// </summary>
        /// <param name="empresa">Recibe por parametro la empresa donde se registrará el tecnico.</param>
        public static void registrarTecnico(Empresa empresa)
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

        /// <summary>
        /// Solicita por consola los parámetros y da de alta un nuevo equipo.
        /// </summary>
        /// <param name="empresa">Recibe por parametro la empresa donde se registrará el equipo.</param>
        public static void registrarEquipo(Empresa empresa)
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

        /// <summary>
        /// Se encarga de pedirle al usuario la información para 
        /// la programación de una nueva orden de servicio (preventiva o correctiva),
        /// calculando automáticamente su ID y validando la selección de tecnicos y equipo.
        /// </summary>
        /// <param name="empresa">Recibe la empresa a la que se le asignará la orden creada.</param>
        public static void programarOrden(Empresa empresa)
        {
            Console.Clear();
            Console.WriteLine("--- PROGRAMAR ORDEN DE SERVICIO ---");

            int id = empresa.Ordenes.Any() ? empresa.Ordenes.Max(o => o.IdOrden) + 1 : 1;

            Console.Write("\nIngrese el cliente: ");
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

            Console.WriteLine("\nEquipos disponibles:\n");
            foreach (Equipo e in empresa.Equipos)
            {
                Console.WriteLine($"Código: {e.CodigoIdentificador} | Operativo: {(e.Operativo ? "Operativo" : "Fuera de Servicio")}");
            }
            Console.Write("\nIngrese el código del equipo a asignar: ");
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
                Console.WriteLine("\nTécnicos disponibles:\n");
                foreach (Tecnico t in empresa.Tecnicos)
                {
                    Console.WriteLine($"Legajo: {t.Legajo} | Nombre: {t.Nombre}");
                }

                Console.Write("\nIngrese el legajo del técnico a agregar: ");
                int leg = Convert.ToInt32(Console.ReadLine());
                Tecnico tec = empresa.Tecnicos.Find(t => t.Legajo == leg)!;

                if (tec != null)
                {
                    tecnicosSeleccionados.Add(tec);
                }

                Console.Write("\n¿Desea agregar otro técnico? (s/n): ");
                continuar = Convert.ToChar(Console.ReadLine()!.ToLower());
            }

            string tipo;
            while (true)
            {
                Console.Write("\n¿Es mantenimiento Preventivo (1) o Correctivo (2)?: ");
                tipo = Console.ReadLine()!.Trim();

                if (tipo == "1" || tipo == "2")
                {
                    break;
                }

                Console.WriteLine("\n[ERROR] Opción inválida. Debe ingresar 1 para Preventivo o 2 para Correctivo. Intente nuevamente.");
            }

            OrdenServicio nuevaOrden;
            if (tipo == "1")
            {
                nuevaOrden = new MantenimientoPreventivo(id, cliente, descripcion, fecha, horarioInicio, horasEstimadas, equipoAsignado, precioBase);
            }
            else
            {
                Console.Write("\nIngrese la urgencia (Baja / Media / Alta): ");
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

        /// <summary>
        /// Cambia el estado de una orden a En Curso llamado a la funcion CambiarEstadoOrden
        /// y pasando los parametros requeridos, en este caso para iniciar la orden.
        /// </summary>
        /// <param name="empresa">Recibe la empresa en cuestion para buscar la orden solicitada.</param>
        public static void iniciarOrden(Empresa empresa)
        {
            CambiarEstadoOrden(empresa, "--- INICIAR ORDEN DE SERVICIO ---", "\nIngrese el ID de la orden a iniciar: ", EstadoOrden.EnCurso, "\nLa orden ha pasado a estado En Curso.");
        }

        /// <summary>
        /// Cambia el estado de una orden a Cancelada llamado a la funcion CambiarEstadoOrden
        /// y pasando los parametros requeridos, en este caso para cancelar la orden.
        /// </summary>
        /// <param name="empresa">Recibe la empresa en cuestion para buscar la orden solicitada.</param>
        public static void cancelarOrden(Empresa empresa)
        {
            CambiarEstadoOrden(empresa, "--- CANCELAR ORDEN DE SERVICIO ---", "\nIngrese el ID de la orden a cancelar: ", EstadoOrden.Cancelada, "\nLa orden ha sido cancelada.");
        }

        /// <summary>
        /// Su función basicamente es cambiar el estado de una orden por un nuevo estado
        /// dependiendo si es para Iniciar o Cancelar una orden muestra un listado diferente
        /// y ,mensajes de error para el usuario en caso de ser necesario.
        /// </summary>
        /// <param name="empresa">Empresa de la cual comparar las ordenes</param>
        /// <param name="titulo">Titulo a ser mostrado en el listado</param>
        /// <param name="mensajeInput">Mensaje que se le va a mostrar al usurio para pedir ingreso</param>
        /// <param name="nuevoEstado">Estado pasado por parametro para diferenciar si es para Iniciar o Cancelar</param>
        /// <param name="mensajeExito">Mensaje en caso de exito para el usuario</param>
        private static void CambiarEstadoOrden(Empresa empresa, string titulo, string mensajeInput, EstadoOrden nuevoEstado, string mensajeExito)
        {
            Console.Clear();
            Console.WriteLine(titulo);

            if (empresa.Ordenes.Count == 0)
            {
                Console.WriteLine("\nNo hay órdenes registradas en el sistema.");
                Console.Write("\nPulse cualquier tecla para volver...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nListado de órdenes disponibles:\n");
            bool hayOrdenesValidas = false;

            foreach (OrdenServicio o in empresa.Ordenes)
            {
                if (nuevoEstado == EstadoOrden.EnCurso && o.Estado == EstadoOrden.Programada)
                {
                    string estado = o.Estado.ToString().Replace("EnCurso", "En curso");
                    Console.WriteLine($"[ID: {o.IdOrden}] Cliente: {o.Cliente} | Fecha: {o.Fecha.ToShortDateString()} | Estado: {estado}");
                    hayOrdenesValidas = true;
                }
                else if (nuevoEstado == EstadoOrden.Cancelada && (o.Estado == EstadoOrden.Programada || o.Estado == EstadoOrden.EnCurso))
                {
                    string estado = o.Estado.ToString().Replace("EnCurso", "En curso");
                    Console.WriteLine($"[ID: {o.IdOrden}] Cliente: {o.Cliente} | Fecha: {o.Fecha.ToShortDateString()} | Estado: {estado}");
                    hayOrdenesValidas = true;
                }
            }

            if (!hayOrdenesValidas)
            {
                Console.WriteLine("--------------------------------------------------------------------------------");
                Console.WriteLine("\n[Aviso]: No hay órdenes disponibles para realizar esta acción.");
                Console.Write("\nPulse cualquier tecla para volver...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("--------------------------------------------------------------------------------");

            Console.Write(mensajeInput);
            int id = Convert.ToInt32(Console.ReadLine());

            OrdenServicio orden = empresa.Ordenes.Find(o => o.IdOrden == id)!;
            if (orden != null)
            {
                if (nuevoEstado == EstadoOrden.EnCurso)
                {
                    if (orden.Estado != EstadoOrden.Programada)
                    {
                        Console.WriteLine($"\n[Error]: No se puede iniciar la orden porque su estado actual es '{orden.Estado}'. Solo se pueden iniciar órdenes en estado 'Programada'.");
                        Console.Write("\nPulse cualquier tecla para continuar...");
                        Console.ReadKey();
                        return;
                    }
                }
                else if (nuevoEstado == EstadoOrden.Cancelada)
                {
                    if (orden.Estado == EstadoOrden.Finalizada)
                    {
                        Console.WriteLine("\n[Error]: No se puede cancelar una orden que ya se encuentra Finalizada.");
                        Console.Write("\nPulse cualquier tecla para continuar...");
                        Console.ReadKey();
                        return;
                    }
                    if (orden.Estado == EstadoOrden.Cancelada)
                    {
                        Console.WriteLine("\n[Aviso]: Esta orden ya estaba cancelada previamente.");
                        Console.Write("\nPulse cualquier tecla para continuar...");
                        Console.ReadKey();
                        return;
                    }
                }

                orden.Estado = nuevoEstado;
                Console.WriteLine(mensajeExito);
            }
            else
            {
                Console.WriteLine("\n[Error]: No se encontró una orden con el ID ingresado.");
            }

            Console.Write("\nPulse cualquier tecla para continuar...");
            Console.ReadKey();
        }

        /// <summary>
        /// Realiza el cierre parcial provisorio de una fecha consultada, mostrando un resumen estadístico.
        /// </summary>
        /// <param name="empresa">Empresa a la cual realizar el cierre.</param>
        public static void cierreParcial(Empresa empresa)
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
                DateTime fechaHoraFin = o.Fecha.Date.Add(o.HorarioFin);

                if (o.Fecha.Date == fechaConsulta.Date && fechaHoraFin <= DateTime.Now)
                {
                    if (o.Estado != EstadoOrden.Cancelada)
                    {
                        o.Estado = EstadoOrden.Finalizada;
                        recaudacionTotal += o.CalcularImporte();

                        if (o.GenerarLineaCsv().StartsWith("PREVENTIVO;"))
                            countPreventivo++;
                        else
                            countCorrectivo++;
                    }
                }
            }

            Console.WriteLine($"\nRecaudación Total Parcial: ${recaudacionTotal}");
            Console.WriteLine($"Órdenes Preventivas procesadas: {countPreventivo}");
            Console.WriteLine($"Órdenes Correctivas procesadas: {countCorrectivo}");

            Console.Write("\nPulse cualquier tecla para continuar...");
            Console.ReadKey();
        }

        /// <summary>
        /// Ejecuta el cierre total definitivo de la jornada para una fecha específica, 
        /// emitiendo un reporte detallado de las ordenes.
        /// </summary>
        /// <param name="empresa">Empresa a la cual realizar el cierre.</param>
        public static void cierreDelDia(Empresa empresa)
        {
            Console.Clear();
            Console.WriteLine("========================================================================");
            Console.WriteLine("                      --- CIERRE TOTAL DEL DÍA ---                      ");
            Console.WriteLine("========================================================================");
            Console.Write("\nIngrese la fecha del cierre total (DD-MM-YYYY): ");
            DateTime fechaConsulta = DateTime.ParseExact(Console.ReadLine()!, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            decimal recaudacionTotal = 0;
            int contadorProcesadas = 0;

            Console.WriteLine($"\nDetalle de órdenes procesadas para la fecha {fechaConsulta.ToShortDateString()}:\n");
            Console.WriteLine($"{"ID",-6} | {"Cliente",-25} | {"Tipo",-15} | {"Importe",12}");
            Console.WriteLine("------------------------------------------------------------------------------");

            foreach (OrdenServicio o in empresa.Ordenes)
            {
                DateTime fechaHoraFin = o.Fecha.Date.Add(o.HorarioFin);

                // Evaluamos que sea de la fecha, que ya haya finalizado su horario y que NO esté cancelada.
                if (o.Fecha.Date == fechaConsulta.Date && fechaHoraFin <= DateTime.Now && o.Estado != EstadoOrden.Cancelada)
                {
                    // Forzamos el estado a Finalizada por si había alguna pendiente
                    o.Estado = EstadoOrden.Finalizada;

                    decimal importeOrden = o.CalcularImporte();
                    recaudacionTotal += importeOrden;
                    contadorProcesadas++;

                    string tipoMantenimiento = o.GenerarLineaCsv().StartsWith("PREVENTIVO;") ? "Preventivo" : "Correctivo";

                    Console.WriteLine($"{o.IdOrden,-6} | {o.Cliente,-25} | {tipoMantenimiento,-15} | ${importeOrden,11:F2}");
                }
            }

            Console.WriteLine("------------------------------------------------------------------------------");

            if (contadorProcesadas == 0)
            {
                Console.WriteLine("No se registraron órdenes válidas para procesar en esta fecha.");
            }
            else
            {
                Console.WriteLine($"\nTotal de órdenes procesadas: {contadorProcesadas}");
                Console.WriteLine($"RECAUDACIÓN TOTAL DEL DÍA: ${recaudacionTotal:F2}");
            }

            Console.Write("\nPulse cualquier tecla para continuar...");
            Console.ReadKey();
        }

        /// <summary>
        /// Despliega el submenú de consultas y reportes operativos y financieros del sistema.
        /// </summary>
        /// <param name="empresa">Empresa a la cual se le realizan las consultas y reportes.</param>
        public static void consultasYReportes(Empresa empresa)
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
                Console.WriteLine("7. Reporte económico y de sueldos");
                Console.WriteLine("8. Volver al menú principal");
                Console.Write("\nSeleccione una opción de reporte (1-8): ");

                string subOpt = Console.ReadLine()!;

                switch (subOpt)
                {
                    case "1": 
                        ReporteTecnicosYOrdenes(empresa); 
                        break;
                    case "2": 
                        ReporteEquiposYEstado(empresa); 
                        break;
                    case "3": 
                        ReporteOrdenesPorFecha(empresa); 
                        break;
                    case "4": 
                        ReporteOrdenesPorEstado(empresa); 
                        break;
                    case "5": 
                        ReporteOrdenesPorTecnico(empresa); 
                        break;
                    case "6": 
                        ReporteOrdenesPorEquipo(empresa);
                        break;
                    case "7":
                        ReporteEcoSueldos(empresa);
                        break;
                    case "8":
                        volver = true; 
                        break;
                    default:
                        Console.WriteLine("\n[Aviso]: Opción inválida.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        /// <summary>
        /// Muestra el listado de todos los técnicos registrados junto con las órdenes asignadas a cada uno.
        /// <param name="empresa">Empresa a la cual se le realizan las consultas y reportes.</param>
        /// </summary>
        private static void ReporteTecnicosYOrdenes(Empresa empresa)
        {
            Console.Clear();
            Console.WriteLine("--- TÉCNICOS Y SUS ÓRDENES ASIGNADAS ---\n");
            foreach (Tecnico t in empresa.Tecnicos)
            {
                Console.WriteLine($"Técnico: {t.Nombre} (Legajo: {t.Legajo}) - Sueldo Neto: ${t.CalcularSueldoNeto():F2}");
                Console.WriteLine("\n  Órdenes asignadas:\n");

                bool tieneOrdenes = false;
                foreach (OrdenServicio o in empresa.Ordenes)
                {
                    if (o.TecnicosAsignados.Any(tec => tec.Legajo == t.Legajo))
                    {
                        Console.WriteLine($"    - ID: {o.IdOrden} | Cliente: {o.Cliente} | Fecha: {o.Fecha.ToShortDateString()} | Estado: {o.Estado.ToString().Replace("EnCurso", "En curso")}");
                        tieneOrdenes = true;
                    }
                }

                if (!tieneOrdenes)
                {
                    Console.WriteLine("    (Sin órdenes asignadas)");
                }

                Console.WriteLine("\n--------------------------------------------------------------------------------");
            }

            Console.Write("\nPulse cualquier tecla para continuar...");
            Console.ReadKey();
        }

        /// <summary>
        /// Muestra el inventario completo de equipos y su estado de operatividad actual.
        /// <param name="empresa">Empresa a la cual se le realizan las consultas y reportes.</param>
        /// </summary>
        private static void ReporteEquiposYEstado(Empresa empresa)
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

        /// <summary>
        /// Filtra y muestra por pantalla las órdenes programadas para una fecha específica ingresada por el usuario.
        /// <param name="empresa">Empresa a la cual se le realizan las consultas y reportes.</param>
        /// </summary>
        private static void ReporteOrdenesPorFecha(Empresa empresa)
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
                    Console.WriteLine($"[ID: {o.IdOrden}] Cliente: {o.Cliente} | Horario: {o.HorarioInicio} a {o.HorarioFin} | Estado: {o.Estado.ToString().Replace("EnCurso", "En curso")}");
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

        /// <summary>
        /// Filtra y muestra por pantalla las órdenes que coincidan con un estado determinado.
        /// <param name="empresa">Empresa a la cual se le realizan las consultas y reportes.</param>
        /// </summary>
        private static void ReporteOrdenesPorEstado(Empresa empresa)
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

        /// <summary>
        /// Muestra las órdenes asociadas a un legajo de técnico específico ingresado por teclado.
        /// <param name="empresa">Empresa a la cual se le realizan las consultas y reportes.</param>
        /// </summary>
        private static void ReporteOrdenesPorTecnico(Empresa empresa)
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
                    Console.WriteLine($"[ID: {o.IdOrden}] Cliente: {o.Cliente} | Fecha: {o.Fecha.ToShortDateString()} | Estado: {o.Estado.ToString().Replace("EnCurso", "En curso")}");
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

        /// <summary>
        /// Muestra las órdenes asociadas a un código de equipo específico ingresado por teclado.
        /// <param name="empresa">Empresa a la cual se le realizan las consultas y reportes.</param>
        /// </summary>
        private static void ReporteOrdenesPorEquipo(Empresa empresa)
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
                    Console.WriteLine($"[ID: {o.IdOrden}] Cliente: {o.Cliente} | Fecha: {o.Fecha.ToShortDateString()} | Estado: {o.Estado.ToString().Replace("EnCurso", "En curso")}");
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

        /// <summary>
        /// Emite un reporte económico detallando la nómina de sueldos de los técnicos y el total facturable global de las órdenes finalizadas.
        /// <param name="empresa">Empresa a la cual se le realizan las consultas y reportes.</param>
        /// </summary>
        private static void ReporteEcoSueldos(Empresa empresa)
        {
            Console.Clear();
            Console.WriteLine("======================================================================================================");
            Console.WriteLine("                            --- TÉCNICOS, ÓRDENES Y RESUMEN ECONÓMICO ---                             ");
            Console.WriteLine("======================================================================================================");

            decimal totalSueldosNomina = 0;
            decimal totalFacturableOrdenes = 0;

            foreach (Tecnico t in empresa.Tecnicos)
            {
                decimal sueldoNetoTecnico = t.CalcularSueldoNeto();
                totalSueldosNomina += sueldoNetoTecnico;

                Console.WriteLine($"\nTécnico: {t.Nombre} (Legajo: {t.Legajo}) - Sueldo Neto: ${sueldoNetoTecnico:F2}");
                Console.WriteLine("  Órdenes asignadas:");

                bool tieneOrdenes = false;
                foreach (OrdenServicio o in empresa.Ordenes)
                {
                    //Uso .Any para comparar justamente cualquier Tecnico dentro de la lista de la orden que tenga el legajo
                    if (o.TecnicosAsignados.Any(tec => tec.Legajo == t.Legajo))
                    {
                        decimal importeOrden = o.CalcularImporte();

                        Console.WriteLine($"    - ID: {o.IdOrden} | Cliente: {o.Cliente} | Fecha: {o.Fecha.ToShortDateString()} | Estado: {o.Estado.ToString().Replace("EnCurso", "En curso")} | Importe: ${importeOrden:F2}");
                        tieneOrdenes = true;
                    }
                }
                if (!tieneOrdenes)
                {
                    Console.WriteLine("    (Sin órdenes asignadas)");
                }
                Console.WriteLine("-----------------------------------------------------------------------------------------------------");
            }

            foreach (OrdenServicio o in empresa.Ordenes)
            {
                if (o.Estado == EstadoOrden.Finalizada)
                {
                    totalFacturableOrdenes += o.CalcularImporte();
                }
            }

            Console.WriteLine("\n======================================================================================================");
            Console.WriteLine($" TOTAL A DESEMBOLSAR EN SUELDOS (NÓMINA): ${totalSueldosNomina:F2}");
            Console.WriteLine($" TOTAL FACTURABLE DE ÓRDENES PROCESADAS:  ${totalFacturableOrdenes:F2}");
            Console.WriteLine("======================================================================================================");

            Console.Write("\nPulse cualquier tecla para continuar...");
            Console.ReadKey();
        }

        /// <summary>
        /// Gestiona la interfaz por consola para eliminar un técnico o un equipo del sistema de forma segura.
        /// <param name="empresa">Empresa a la cual se le eliminará ya sea un tecnico o un equipo.</param>
        /// </summary>
        public static void eliminarTecnicoOEquipo(Empresa empresa)
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