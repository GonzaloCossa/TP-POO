using Cossa_Gonzalo_TP_Empresa_Mantenimiento_Industrial.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cossa_Gonzalo_TP_Empresa_Mantenimiento_Industrial.Clases
{
    public class MantenimientoPreventivo : OrdenServicio
    {
        public MantenimientoPreventivo(int idOrden, string cliente, string descripcion, DateTime fecha, TimeSpan horarioInicio, double horasEstimadas, Equipo equipoAsignado, decimal precioBase)
            : base(idOrden, cliente, descripcion, fecha, horarioInicio, horasEstimadas, equipoAsignado, precioBase)
        {
        }

        public override decimal CalcularImporte()
        {
            decimal costoEquipo = EquipoAsignado.CostoPorHora * (decimal)HorasEstimadas;
            decimal extraTecnicos = 15000m * TecnicosAsignados.Count;
            return PrecioBase + extraTecnicos + costoEquipo;
        }

        public override string GenerarLineaCsv()
        {
            string precioStr = PrecioBase.ToString(CultureInfo.InvariantCulture);
            string fechaStr = Fecha.ToString("dd-MM-yyyy");
            string horasEstStr = HorasEstimadas.ToString(CultureInfo.InvariantCulture);
            string codigoEquipo = EquipoAsignado != null ? EquipoAsignado.CodigoIdentificador : "";
            string legajosStr = string.Join(",", TecnicosAsignados.Select(t => t.Legajo));

            // Como un Mantenimiento Preventivo no tiene un nivel de urgencia, para poder guardalo en el csv, le agrego en el string que su urgencia es 'NA' (no asignada) para poder escribir en el archivo sin problema
            return $"PREVENTIVO;{IdOrden};{Cliente};{Descripcion};{fechaStr};{HorarioInicio};{horasEstStr};{precioStr};{Estado};{codigoEquipo};NA;{legajosStr}";
        }
    }
}