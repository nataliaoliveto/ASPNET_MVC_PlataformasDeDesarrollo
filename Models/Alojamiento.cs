using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Models
{
    public class Alojamiento
    {
        public int id { get; set; }
        public int aCodigo { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "La ciudad debe tener 2 caracteres como mínimo")]
        public String aCiudad { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El barrio debe tener 2 caracteres como mínimo")]
        public String aBarrio { get; set; }
        [Required]
        public int aEstrellas { get; set; }
        [Required]
        public int aCantPersonas { get; set; }
        public bool aTV { get; set; }
        public String Tipo { get; set; }
        public double cPrecioxDia { get; set; }
        public int cHabitaciones { get; set; }
        public int cbanios { get; set; }
        public double hPrecioxPersona { get; set; }

        public Alojamiento() { }

        public Alojamiento(int aCodigo, String aCiudad, String aBarrio, int aEstrellas, int aCantPersonas, bool aTV,
            String Tipo, double cPrecioxDia, int cHabitaciones, int cbanios, double hPrecioxPersona)
        {
            this.aCodigo = aCodigo;
            this.aCiudad = aCiudad;
            this.aBarrio = aBarrio;
            this.aEstrellas = aEstrellas;
            this.aCantPersonas = aCantPersonas;
            this.aTV = aTV;
            this.Tipo = Tipo;
            this.cPrecioxDia = cPrecioxDia;
            this.cHabitaciones = cHabitaciones;
            this.cbanios = cbanios;
            this.hPrecioxPersona = hPrecioxPersona;
        }

        public override String ToString()
        {
            return aCodigo + "," + aCiudad + "," + aBarrio + "," + aEstrellas + "," + aCantPersonas + "," + aTV + ","
                + Tipo + "," + cPrecioxDia + "," + cHabitaciones + "," + cbanios + "," + hPrecioxPersona;
        }
    }
}
