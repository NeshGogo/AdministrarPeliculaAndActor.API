using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdministrarPeliculaActorAPI.Entity
{
    //Esta es mi clase que hace referencia a la estructura que existe en la base de datos de la tabla Pelicula
    public class Pelicula
    {
        public int PeliculaId { get; set; }
        public string Titulo { get; set; }
        public string Genero { get; set; }
        public DateTime FechaEstreno { get; set; }
        public String Foto { get; set; }
        public List<Actor> Actorer { get; set; }
    }
}
