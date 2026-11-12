using Cossa_Gonzalo_TP_Empresa_Mantenimiento_Industrial.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cossa_Gonzalo_TP_Empresa_Mantenimiento_Industrial.Clases
{
    public class MantenimientoCorrectivo : OrdenServicio
    {
        public Urgencia NivelUrgencia { get; private set; }

        public MantenimientoCorrectivo(int idOrden, string cliente, string descripcion, DateTime fecha, TimeSpan horarioInicio, double horasEstimadas, Equipo equipoAsignado, decimal precioBase, Urgencia urgencia)
            : base(idOrden, cliente, descripcion, fecha, horarioInicio, horasEstimadas, equipoAsignado, precioBase)
        {
            NivelUrgencia = urgencia;
        }

        public override decimal CalcularImporte()
        {
            decimal costoEquipo = EquipoAsignado.CostoPorHora * (decimal)HorasEstimadas;
            decimal extraTecnicos = 20000m * TecnicosAsignados.Count;

            decimal adicionalUrgencia = NivelUrgencia switch
            {
                Urgencia.Baja => 20000m,
                Urgencia.Media => 50000m,
                Urgencia.Alta => 100000m,
                _ => 0m
            };

            return PrecioBase + costoEquipo + extraTecnicos + adicionalUrgencia;
        }

        public override string GenerarLineaCsv()
        {
            string precioStr = PrecioBase.ToString(CultureInfo.InvariantCulture);
            string fechaStr = Fecha.ToString("dd-MM-yyyy");
            string horasEstStr = HorasEstimadas.ToString(CultureInfo.InvariantCulture);
            string codigoEquipo = EquipoAsignado != null ? EquipoAsignado.CodigoIdentificador : "";
            string legajosStr = string.Join(",", TecnicosAsignados.Select(t => t.Legajo));

            return $"CORRECTIVO;{IdOrden};{Cliente};{Descripcion};{fechaStr};{HorarioInicio};{horasEstStr};{precioStr};{Estado};{codigoEquipo};{NivelUrgencia};{legajosStr}";
        }
    }
}