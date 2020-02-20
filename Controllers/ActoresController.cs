using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdministrarPeliculaActorAPI.Entity;
using AdministrarPeliculaActorAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdministrarPeliculaActorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActoresController : ControllerBase
    {
        //Instancia del servicio que accede a los datos.
        private ActorService actorService = new ActorService();

       /*Estos son las Acciones habituales en una API. Cada Accion devuelve codigo xml, la configuracion fue realizada en el startup*/
        // GET: api/Actores
        [HttpGet]
        public ActionResult<List<Actor>> Get()
        {
            var actores = actorService.GetActores();
            return actores;
        }

        // GET: api/Actores/5
        [HttpGet("{id}", Name = "GetActores")]
        public ActionResult<Actor> Get(int id)
        {
            Actor actor = actorService.GetActorByID(id);
            if (actor!=null)
            {
                return actor;
            }
            return NotFound();
            
        }

        // POST: api/Actores
        [HttpPost]
        public ActionResult Post([FromBody] Actor actor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (actorService.CreateActor(actor))
            {
                var lastId = actorService.GetActores().Last().ActorId;
                actor.ActorId = lastId;
                return new CreatedAtRouteResult("GetActores", new { id = lastId }, actor);
            }
            else
            {
                return BadRequest();
            }
        }

        // PUT: api/Actores/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Actor actor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //aqui valido que el usuario no utilice ID diferentes entre el cuerpo y el parametro de la URL
            if (actor.ActorId == id)
            {
                if (actorService.UpdateActor(actor))
                {
                    actor.ActorId = id;
                    return new CreatedAtRouteResult("GetActores", new { id = id }, actor);
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
            bool delete = actorService.DeleteActor(id);

            if (delete)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}
