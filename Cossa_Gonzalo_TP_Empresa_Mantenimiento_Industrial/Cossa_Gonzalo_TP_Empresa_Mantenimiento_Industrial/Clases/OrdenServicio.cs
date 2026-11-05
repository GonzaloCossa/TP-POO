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
        protected int idOrden;
        protected string cliente;
        protected string descripcion;
        protected DateTime fecha;
        protected TimeSpan horarioInicio;
        protected double horasEstimadas;
        protected Equipo equipoAsignado;
        protected List<Tecnico> tecnicosAsignados;
        protected decimal precioBase;

        public int IdOrden { get; set; }
        public string Cliente { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HorarioInicio { get; set; }
        public double HorasEstimadas { get; set; }
        public Equipo EquipoAsignado { get; set; }
        public List<Tecnico> TecnicosAsignados { get; set; }
        public EstadoOrden Estado { get; set; }
        public decimal PrecioBase { get; set; }
        public abstract bool EsPreventivo { get; }

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

        public TimeSpan HorarioFin => HorarioInicio.Add(TimeSpan.FromHours(HorasEstimadas));

        public bool SeSuperponeCon(DateTime otraFecha, TimeSpan otroInicio, double otrasHoras)
        {
            if (Fecha.Date != otraFecha.Date) return false;

            TimeSpan otroFin = otroInicio.Add(TimeSpan.FromHours(otrasHoras));
            return HorarioInicio < otroFin && otroInicio < HorarioFin;
        }

        public abstract decimal CalcularImporte();

        // Metodo abstracto para que cada hijo cree el toString que se guardará en el CSV.
        public abstract string GenerarLineaCsv();
    }
}