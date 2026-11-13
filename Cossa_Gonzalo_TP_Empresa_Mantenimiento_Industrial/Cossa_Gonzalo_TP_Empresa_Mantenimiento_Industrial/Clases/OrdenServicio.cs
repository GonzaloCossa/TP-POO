using Cossa_Gonzalo_TP_Empresa_Mantenimiento_Industrial.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cossa_Gonzalo_TP_Empresa_Mantenimiento_Industrial.Clases
{
    public abstract class OrdenServicio
    {
        public int IdOrden { get; protected set; }
        public string Cliente { get; protected set; }
        public string Descripcion { get; protected set; }
        public DateTime Fecha { get; protected set; }
        public TimeSpan HorarioInicio { get; protected set; }
        public double HorasEstimadas { get; protected set; }
        public Equipo EquipoAsignado { get; protected set; }
        public List<Tecnico> TecnicosAsignados { get; protected set; }
        public EstadoOrden Estado { get; set; }
        public decimal PrecioBase { get; protected set; }
        public TimeSpan HorarioFin => HorarioInicio.Add(TimeSpan.FromHours(HorasEstimadas));

        public OrdenServicio(int idOrden, string cliente, string descripcion, DateTime fecha, TimeSpan horarioInicio, double horasEstimadas, Equipo equipoAsignado, decimal precioBase)
        {
            IdOrden = idOrden;
            Cliente = cliente;
            Descripcion = descripcion;
            Fecha = fecha;
            HorarioInicio = horarioInicio;
            HorasEstimadas = horasEstimadas;
            EquipoAsignado = equipoAsignado;
            TecnicosAsignados = new List<Tecnico>();
            Estado = EstadoOrden.Programada;
            PrecioBase = precioBase;
        }

        /// <summary>
        /// Compara si la orden actual se superpone en fecha y horario con otro intervalo temporal indicado.
        /// </summary>
        /// <param name="otraFecha">Fecha de la otra orden a evaluar.</param>
        /// <param name="otroInicio">Horario de inicio del otro servicio.</param>
        /// <param name="otrasHoras">Duración estimada en horas del otro servicio.</param>
        /// <returns> Retorna True si se superponen las horas de la comparación, sino, retorna False.</returns>
        public bool SeSuperponeCon(DateTime otraFecha, TimeSpan otroInicio, double otrasHoras)
        {
            if (Fecha.Date != otraFecha.Date) return false;

            TimeSpan otroFin = otroInicio.Add(TimeSpan.FromHours(otrasHoras));
            return HorarioInicio < otroFin && otroInicio < HorarioFin;
        }

        // Metodo abstracto el cual cada hijo modificará para responder como calcula el importe de la orden.
        public abstract decimal CalcularImporte();

        // Metodo abstracto para que cada hijo cree el toString que se guardará en el CSV.
        public abstract string GenerarLineaCsv();
    }
}