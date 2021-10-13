using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Models
{
    public class Reserva
    {
        public int id { get; set; }
        public DateTime fDesde { get; set; }
        public DateTime fHasta { get; set; }
        public Alojamiento propiedad { get; set; }
        public Usuario persona { get; set; }
        public float precio { get; set; }
        public int cantPersonas { get; set; }
        public int propiedadId { get; set; }
        public int personaId { get; set; }

        public Reserva() { }

        public Reserva(DateTime fDesde, DateTime fHasta, Alojamiento propiedad, Usuario persona, float precio, int cantPersonas)
        {
            this.fDesde = fDesde;
            this.fHasta = fHasta;
            this.propiedad = propiedad;
            this.persona = persona;
            this.precio = precio;
            this.cantPersonas = cantPersonas;
        }

        public override String ToString()
        {
            return id + "," + fDesde.ToString("yyyy/MM/dd") + "," + fHasta.ToString("yyyy/MM/dd") + "," + propiedad.aCodigo + "," + persona.DNI;
        }
    }
}
