using AdministrarPeliculaActorAPI.Entity;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AdministrarPeliculaActorAPI.Services
{
    //Este servicio realiza todas las funcionalidades de datos hacia la base de datos.
    public class ActorService
    {
        //Este es el string de coneccion de la base de datos
        private string connectionString = "Server=NESHGOGO;Database=AdministrarPeliculaYActor;Trusted_Connection=True;MultipleActiveResultSets=true;";

        //Este metodo devuelve una lista de todos los actores registrados en la base de datos.
        public List<Actor> GetActores()
        {
           //Este es el query.
            string query = @"select * from Actor";
            //Coneccion a la base de datos
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    List<Actor> peliculas = new List<Actor>();
                    connection.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                   //Cargo los datos a la lista de actores.
                    while (reader.Read())
                    {
                        //Creo un objeto de tipo actor  y luego lo cargo a la lista que fue definida mas arriba
                        var actor = new Actor
                        {
                            ActorId = Convert.ToInt32(reader["ActorId"]),
                            NombreCompleto = Convert.ToString(reader["NombreCompleto"]),
                            FechaNacimiento = Convert.ToDateTime(reader["FechaNacimiento"]),
                            Sexo = Convert.ToString(reader["Sexo"]),
                            Foto = Convert.ToString(reader["Foto"])

                        };
                        //aqui obtenego las peliculas que pertenecen al actor
                        actor.Peliculas = GetPeliculaByActor(actor.ActorId);

                        peliculas.Add(actor);
                    }

                    return peliculas;


                }
            }
        }
        //Aqui obtengo un autor por su id
        public Actor GetActorByID(int id)
        {
            //query 
            string query = @"select * from Actor where ActorId = @id";

            //basicamente realizo la misma operacion que en el proceso anterior con la diferencia de que este devuelve un objeto en vez de una lista de objetos.
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    connection.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    Actor actor = new Actor();
                    while (reader.Read())
                    {
                        actor = new Actor
                        {
                            ActorId = Convert.ToInt32(reader["ActorId"]),
                            NombreCompleto = Convert.ToString(reader["NombreCompleto"]),
                            FechaNacimiento = Convert.ToDateTime(reader["FechaNacimiento"]),
                            Sexo = Convert.ToString(reader["Sexo"]),
                            Foto = Convert.ToString(reader["Foto"])

                        };
                        //aqui obtengo las peliculas que pertenecen a el actor.
                        actor.Peliculas = GetPeliculaByActor(actor.ActorId);
                    }
                    return actor;
                }
            }
        }
        //Este metodo inserta un autor en la base de datos.
        public bool CreateActor(Actor actor)
        {
            //query.
            string query = @"insert into Actor ( NombreCompleto, FechaNacimiento,Sexo, Foto) values 
                                (@NombreCompleto,@FechaNacimiento,@Sexo,@Foto)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    //agrego los parametros para evitar sql injection.
                    cmd.Parameters.Add("@NombreCompleto", System.Data.SqlDbType.VarChar, 50).Value = actor.NombreCompleto;
                    cmd.Parameters.Add("@Sexo", System.Data.SqlDbType.Char).Value = actor.Sexo;
                    cmd.Parameters.Add("@FechaNacimiento", System.Data.SqlDbType.Date).Value = actor.FechaNacimiento;
                    cmd.Parameters.Add("@Foto", System.Data.SqlDbType.VarChar, 250).Value = actor.Foto;

                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();

                        return true;
                    }
                    catch (Exception)
                    {                        
                        return false;
                    }
                }
            }
        }
        //Este metodo actualiza un actor de la base de datos.
        public bool UpdateActor(Actor actor)
        {

            string query = @"update Pelicula set
                                    NombreCompleto = @NombreCompleto,
                                    Sexo = @Sexo,
                                    FechaNacimiento = @FechaNacimiento,
                                    Foto=@Foto
                                    where ActorId = @ActorId 
                                ";

            //Se realiza la operacion similar a la anterior
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {

                    cmd.Parameters.Add("@NombreCompleto", System.Data.SqlDbType.VarChar, 50).Value = actor.NombreCompleto;
                    cmd.Parameters.Add("@Sexo", System.Data.SqlDbType.Char).Value = actor.Sexo;
                    cmd.Parameters.Add("@FechaNacimiento", System.Data.SqlDbType.Date).Value = actor.FechaNacimiento;
                    cmd.Parameters.Add("@Foto", System.Data.SqlDbType.VarChar, 250).Value = actor.Foto;
                    cmd.Parameters.Add("@ActorId", System.Data.SqlDbType.Int).Value = actor.ActorId;
                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
        }
        // Elimino un actor de la base de datos
        public bool DeleteActor(int id)
        {
            // este es el query
            string query = @"delete Actor where ActorId = @id";

            //El proceso es similar a los anteriores.
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
        }

        //Este metodo obtiene las peliculas relacionadas a un actor especifico
        private List<Pelicula> GetPeliculaByActor(int id)
        {
            //query para realizar la busqueda
            string query = @"select p.PeliculaId,p.Titulo, p.FechaEstreno, p.Genero, p.Foto  from Actor as a join PeliculaYActor as pa on a.ActorId = pa.ActorId
                                join Pelicula as p on p.PeliculaId = pa.PeliculaId
                                where a.ActorId = @id";

            //El proceso aqui es similar al realizado en procesos anteririores
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    List<Pelicula> peliculas = new List<Pelicula>();
                    connection.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var pelicula = new Pelicula
                        {
                            PeliculaId = Convert.ToInt32(reader["PeliculaId"]),
                            Titulo = Convert.ToString(reader["Titulo"]),
                            Genero = Convert.ToString(reader["Genero"]),
                            FechaEstreno = Convert.ToDateTime(reader["FechaEstreno"]),
                            Foto = Convert.ToString(reader["Foto"])
                        };

                        peliculas.Add(pelicula);
                    }

                    return peliculas;


                }
            }
        }
    }
}
