using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Final.Models
{
    public class Usuario
    {
        public int id { get; set; }
        [Required]
        [Range(1000000, 99999999,
        ErrorMessage = "DNI debe tener 7 u 8 dígitos")]
        public int DNI { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener 2 caracteres como mínimo")]
        public String nombre { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(100, ErrorMessage = "El correo es inválido")]
        public String mail { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(500, MinimumLength = 8, ErrorMessage = "La contraseña debe tener 8 caracteres como mínimo")]
        public String password { get; set; }
        public bool esADMIN { get; set; }
        public bool bloqueado { get; set; }

        public Usuario() { }

        public Usuario(int DNI, String nombre, String mail, String password, bool esADMIN)
        {
            this.DNI = DNI;
            this.nombre = nombre;
            this.mail = mail;
            this.password = password;
            this.esADMIN = esADMIN;
            this.bloqueado = false;
        }

        public override String ToString()
        {
            return DNI + "," + nombre + "," + mail + "," + password + "," + esADMIN + "," + bloqueado;
        }
    }
}
