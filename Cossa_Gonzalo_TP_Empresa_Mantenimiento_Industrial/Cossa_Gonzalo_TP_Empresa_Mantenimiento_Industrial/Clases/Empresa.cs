using Cossa_Gonzalo_TP_Empresa_Mantenimiento_Industrial.Enums;
using Cossa_Gonzalo_TP_Empresa_Mantenimiento_Industrial.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cossa_Gonzalo_TP_Empresa_Mantenimiento_Industrial.Clases
{
    public class Empresa
    {
        private string nombre;
        private string cuit;
        private List<Tecnico> tecnicos;
        private List<Equipo> equipos;
        private List<OrdenServicio> ordenes;

        public string Nombre { get => nombre; set => nombre = value; }
        public string Cuit { get => cuit; set => cuit = value; }
        public List<Tecnico> Tecnicos { get => tecnicos; set => tecnicos = value; }
        public List<Equipo> Equipos { get => equipos; set => equipos = value; }
        public List<OrdenServicio> Ordenes { get => ordenes; set => ordenes = value; }

        public Empresa(string nombre, string cuit)
        {
            this.Nombre = nombre;
            this.Cuit = cuit;
            this.Tecnicos = new List<Tecnico>();
            this.Equipos = new List<Equipo>();
            this.Ordenes = new List<OrdenServicio>();
        }

        public void RegistrarTecnico(Tecnico tecnico)
        {
            foreach (Tecnico t in Tecnicos)
            {
                if (t.Legajo == tecnico.Legajo)
                    throw new ExcepcionPersonalizada($"Ya existe un técnico registrado con el legajo {tecnico.Legajo}.");
                if (t.Dni == tecnico.Dni)
                    throw new ExcepcionPersonalizada($"Ya existe un técnico registrado con el DNI {tecnico.Dni}.");
            }
            Tecnicos.Add(tecnico);
        }

        public void RegistrarEquipo(Equipo equipo)
        {
            foreach (Equipo e in Equipos)
            {
                if (e.CodigoIdentificador.Equals(equipo.CodigoIdentificador, StringComparison.OrdinalIgnoreCase))
                    throw new ExcepcionPersonalizada($"Ya existe un equipo registrado con el código {equipo.CodigoIdentificador}.");
            }
            Equipos.Add(equipo);
        }

        public void ProgramarOrden(OrdenServicio nuevaOrden, Equipo equipo, List<Tecnico> tecnicosSeleccionados)
        {
            // 1. Validar que el equipo se encuentre operativo
            if (!equipo.Operativo)
            {
                throw new EquipoNoOperativoException($"El equipo {equipo.CodigoIdentificador} no se encuentra operativo y no puede ser asignado.");
            }

            // 2. Validar que el equipo no esté ocupado en el mismo horario
            foreach (OrdenServicio orden in Ordenes)
            {
                if (orden.Estado != EstadoOrden.Cancelada && orden.EquipoAsignado.CodigoIdentificador == equipo.CodigoIdentificador)
                {
                    if (orden.SeSuperponeCon(nuevaOrden.Fecha, nuevaOrden.HorarioInicio, nuevaOrden.HorasEstimadas))
                    {
                        throw new EquipoOcupadoException($"El equipo {equipo.CodigoIdentificador} ya se encuentra ocupado en otra orden durante ese intervalo horario.");
                    }
                }
            }

            // 3. Validar que los técnicos no estén ocupados en el mismo horario
            foreach (Tecnico tecnico in tecnicosSeleccionados)
            {
                foreach (OrdenServicio orden in Ordenes)
                {
                    if (orden.Estado != EstadoOrden.Cancelada)
                    {
                        foreach (Tecnico tAsignado in orden.TecnicosAsignados)
                        {
                            if (tAsignado.Legajo == tecnico.Legajo)
                            {
                                if (orden.SeSuperponeCon(nuevaOrden.Fecha, nuevaOrden.HorarioInicio, nuevaOrden.HorasEstimadas))
                                {
                                    throw new TecnicoOcupadoException($"El técnico {tecnico.Nombre} (Legajo: {tecnico.Legajo}) ya tiene otra orden asignada que se superpone en ese horario.");
                                }
                            }
                        }
                    }
                }
            }

            // Si pasa todas las validaciones, asignamos los técnicos a la orden y la guardamos
            foreach (Tecnico t in tecnicosSeleccionados)
            {
                nuevaOrden.TecnicosAsignados.Add(t);
            }

            Ordenes.Add(nuevaOrden);
        }

        public void EliminarTecnicoOEquipo(int? legajoTecnico, string codigoEquipo)
        {
            if (legajoTecnico.HasValue)
            {
                // Validamos si el técnico participa en alguna orden con estado Programada o En Curso
                foreach (OrdenServicio orden in Ordenes)
                {
                    if (orden.Estado == EstadoOrden.Programada || orden.Estado == EstadoOrden.EnCurso)
                    {
                        foreach (Tecnico t in orden.TecnicosAsignados)
                        {
                            if (t.Legajo == legajoTecnico.Value)
                            {
                                throw new EliminacionAsignadaException("No se puede eliminar el técnico porque se encuentra asignado a órdenes activas o en curso.");
                            }
                        }
                    }
                }

                Tecnico tecnicoAEliminar = Tecnicos.Find(t => t.Legajo == legajoTecnico.Value)!;
                if (tecnicoAEliminar != null)
                {
                    Tecnicos.Remove(tecnicoAEliminar);
                }
                else
                {
                    throw new ExcepcionPersonalizada("No se encontró el técnico a eliminar.");
                }
            }
            else if (!string.IsNullOrEmpty(codigoEquipo))
            {
                foreach (OrdenServicio orden in Ordenes)
                {
                    if ((orden.Estado == EstadoOrden.Programada || orden.Estado == EstadoOrden.EnCurso) &&
                        orden.EquipoAsignado.CodigoIdentificador.Equals(codigoEquipo, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new EliminacionAsignadaException("No se puede eliminar el equipo porque está asignado a órdenes activas o en curso.");
                    }
                }

                Equipo equipoAEliminar = Equipos.Find(e => e.CodigoIdentificador.Equals(codigoEquipo, StringComparison.OrdinalIgnoreCase))!;
                if (equipoAEliminar != null)
                {
                    Equipos.Remove(equipoAEliminar);
                }
                else
                {
                    throw new ExcepcionPersonalizada("No se encontró el equipo a eliminar.");
                }
            }
        }
    }
}