using Cossa_Gonzalo_TP_Empresa_Mantenimiento_Industrial.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cossa_Gonzalo_TP_Empresa_Mantenimiento_Industrial.Clases
{
    public static class GestorArchivos
    {
        private static readonly string carpetaRaiz = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;

        private static readonly string carpetaArchivos = Path.Combine(carpetaRaiz, "Archivos");

        private static string rutaTecnicos => Path.Combine(carpetaArchivos, "tecnicos.csv");
        private static string rutaEquipos => Path.Combine(carpetaArchivos, "equipos.csv");
        private static string rutaOrdenes => Path.Combine(carpetaArchivos, "ordenes.csv");

        static GestorArchivos()
        {
            if (!Directory.Exists(carpetaArchivos))
            {
                Directory.CreateDirectory(carpetaArchivos);
            }
        }

        // ==========================================
        // PERSISTENCIA DE TÉCNICOS
        // ==========================================
        public static void GuardarTecnicos(List<Tecnico> tecnicos)
        {
            using (StreamWriter sw = new StreamWriter(rutaTecnicos))
            {
                foreach (Tecnico t in tecnicos)
                {
                    // Formato: Nombre;Legajo;Dni;SueldoBasico;FechaNacimiento
                    string sueldoStr = t.SueldoBasico.ToString(CultureInfo.InvariantCulture);
                    string fechaStr = t.FechaNacimiento.ToString("dd-MM-yyyy");
                    sw.WriteLine($"{t.Nombre};{t.Legajo};{t.Dni};{sueldoStr};{fechaStr}");
                }
            }
        }

        public static List<Tecnico> CargarTecnicos()
        {
            List<Tecnico> tecnicos = new List<Tecnico>();
            if (!File.Exists(rutaTecnicos)) return tecnicos;

            using (StreamReader sr = new StreamReader(rutaTecnicos))
            {
                string linea;
                while ((linea = sr.ReadLine()!) != null)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;
                    string[] partes = linea.Split(';');

                    string nombre = partes[0];
                    int legajo = Convert.ToInt32(partes[1]);
                    string dni = partes[2];
                    decimal sueldo = Convert.ToDecimal(partes[3], CultureInfo.InvariantCulture);
                    DateTime fechaNac = DateTime.ParseExact(partes[4], "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    tecnicos.Add(new Tecnico(nombre, legajo, dni, sueldo, fechaNac));
                }
            }
            return tecnicos;
        }

        // ==========================================
        // PERSISTENCIA DE EQUIPOS
        // ==========================================
        public static void GuardarEquipos(List<Equipo> equipos)
        {
            using (StreamWriter sw = new StreamWriter(rutaEquipos))
            {
                foreach (Equipo e in equipos)
                {
                    // Formato: CodigoIdentificador;CostoPorHora;Operativo
                    string costoStr = e.CostoPorHora.ToString(CultureInfo.InvariantCulture);
                    sw.WriteLine($"{e.CodigoIdentificador};{costoStr};{e.Operativo}");
                }
            }
        }

        public static List<Equipo> CargarEquipos()
        {
            List<Equipo> equipos = new List<Equipo>();
            if (!File.Exists(rutaEquipos)) return equipos;

            using (StreamReader sr = new StreamReader(rutaEquipos))
            {
                string linea;
                while ((linea = sr.ReadLine()!) != null)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;
                    string[] partes = linea.Split(';');

                    string codigo = partes[0];
                    decimal costo = Convert.ToDecimal(partes[1], CultureInfo.InvariantCulture);
                    bool operativo = Convert.ToBoolean(partes[2]);

                    equipos.Add(new Equipo(codigo, costo, operativo));
                }
            }
            return equipos;
        }

        // ==========================================
        // PERSISTENCIA DE ÓRDENES DE SERVICIO
        // ==========================================
        public static void GuardarOrdenes(List<OrdenServicio> ordenes)
        {
            using (StreamWriter sw = new StreamWriter(rutaOrdenes))
            {
                foreach (OrdenServicio o in ordenes)
                {
                    sw.WriteLine(o.GenerarLineaCsv());
                }
            }
        }

        public static List<OrdenServicio> CargarOrdenes(List<Equipo> equiposDisponibles, List<Tecnico> tecnicosDisponibles)
        {
            List<OrdenServicio> ordenes = new List<OrdenServicio>();
            if (!File.Exists(rutaOrdenes)) return ordenes;

            using (StreamReader sr = new StreamReader(rutaOrdenes))
            {
                string linea;
                while ((linea = sr.ReadLine()!) != null)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;
                    string[] partes = linea.Split(';');

                    string tipo = partes[0];
                    int id = Convert.ToInt32(partes[1]);
                    string cliente = partes[2];
                    string descripcion = partes[3];
                    DateTime fecha = Convert.ToDateTime(partes[4]);
                    TimeSpan horarioInicio = TimeSpan.Parse(partes[5]);
                    double horasEstimadas = Convert.ToDouble(partes[6], CultureInfo.InvariantCulture);
                    decimal precioBase = Convert.ToDecimal(partes[7], CultureInfo.InvariantCulture);

                    EstadoOrden estado = Enum.Parse<EstadoOrden>(partes[8], true);

                    string codigoEquipo = partes[9];
                    string urgenciaStr = partes[10];
                    string legajosStr = partes[11];

                    // Buscar el equipo en la lista de disponibles
                    Equipo equipoAsignado = equiposDisponibles.Find(e => e.CodigoIdentificador.Equals(codigoEquipo, StringComparison.OrdinalIgnoreCase))!;

                    // Creo una orden y luego dependiendo el dato obtenido del CSV defino si era una orden Preventiva o Correctiva.
                    OrdenServicio orden;

                    if (tipo == "PREVENTIVO")
                    {
                        orden = new MantenimientoPreventivo(id, cliente, descripcion, fecha, horarioInicio, horasEstimadas, equipoAsignado, precioBase);
                    }
                    else
                    {
                        Urgencia urgencia = Enum.Parse<Urgencia>(urgenciaStr, true);
                        orden = new MantenimientoCorrectivo(id, cliente, descripcion, fecha, horarioInicio, horasEstimadas, equipoAsignado, precioBase, urgencia);
                    }

                    // Asignamos el estado recuperado
                    orden.Estado = estado;

                    // Reconstruir la lista de técnicos asignados
                    if (!string.IsNullOrEmpty(legajosStr))
                    {
                        string[] legajosArray = legajosStr.Split(',');
                        foreach (string legStr in legajosArray)
                        {
                            if (int.TryParse(legStr, out int legajo))
                            {
                                Tecnico tec = tecnicosDisponibles.Find(t => t.Legajo == legajo)!;
                                if (tec != null)
                                {
                                    orden.TecnicosAsignados.Add(tec);
                                }
                            }
                        }
                    }

                    ordenes.Add(orden);
                }
            }
            return ordenes;
        }
    }
}