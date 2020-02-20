using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AdministrarPeliculaActorAPI.Entity
{
 //Esta es mi clase que hace referencia a la estructura que existe en la base de datos de la tabla Actor
    public class Actor
    {
        public int ActorId { get; set; }
        public string NombreCompleto { get; set; }
        public DateTime FechaNacimiento { get; set; }
        [MaxLength(1)]
        public string Sexo { get; set; }
        public string Foto { get; set; }
        public List<Pelicula> Peliculas { get; set; }
    }

}

