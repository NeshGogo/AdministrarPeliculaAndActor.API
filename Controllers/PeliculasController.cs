using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AdministrarPeliculaActorAPI.Entity;
using AdministrarPeliculaActorAPI.Services;
using System.Data;

namespace AdministrarPeliculaActorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeliculasController : ControllerBase
    {
        //Instancia del servicio que contiene todo el acceso a los datos.
        private PeliculaService peliculaService = new PeliculaService();

        /*Este es un controlador que contiene las acciones habituales en una API*/

        // GET: api/Peliculas
        [HttpGet]
        public ActionResult<List<Pelicula>> Get()
        {
            var peliculas = peliculaService.GetPeliculas();

            return peliculas;
       
        }

        // GET: api/Peliculas/5
        [HttpGet("{id}", Name = "GetPelicula")]
        public ActionResult<Pelicula> Get(int id)
        {
            Pelicula pelicula = peliculaService.GetPeliculasByID(id);

            if (pelicula!= null)
            {
                return pelicula;
            }
            return NotFound();
            
        }

        // POST: api/Peliculas
        [HttpPost]
        public ActionResult Post([FromBody] Pelicula pelicula)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (peliculaService.CreatePeliculas(pelicula))
            {
                var lastId = peliculaService.GetPeliculas().Last().PeliculaId;
                pelicula.PeliculaId = lastId;
                //Esta funcio es utilizada para que cuando se inserte la pelicula devuelva la pelicula insertada..
                return new CreatedAtRouteResult("GetPelicula", new { id = lastId }, pelicula);
            }
            else
            {
                return BadRequest();
            }           
        }

        // PUT: api/Peliculas/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Pelicula pelicula)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if(pelicula.PeliculaId == id)
            {
                if (peliculaService.UpdatePeliculas(pelicula))
                {
                    pelicula.PeliculaId = id;
                    //Esta funcio es utilizada para que cuando se actualize la pelicula devuelva la pelicula insertada..
                    return new CreatedAtRouteResult("GetPelicula", new { id = id }, pelicula);
                }
                return BadRequest();

            }            
            else
            {
                return BadRequest();
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            bool delete = peliculaService.DeletePeliculas(id);

            if (delete)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}
