using AdministrarPeliculaActorAPI.Entity;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AdministrarPeliculaActorAPI.Services
{
    public class PeliculaService
    {
        //string de coneccion a la base de datos
        private string connectionString = "Server=NESHGOGO;Database=AdministrarPeliculaYActor;Trusted_Connection=True;MultipleActiveResultSets=true;";
      
        //Este metodo trae todas las peliculas de la base de datos.
        public  List<Pelicula>  GetPeliculas()
        {
            //query
            string query = @"select * from Pelicula";
            //Esto es similar a procesos anterirores
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using(SqlCommand cmd = new SqlCommand(query, connection))
                {
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
                        //Busca los actores que pertenecen a cada pelicula
                        pelicula.Actorer = GetActorByPelicula(pelicula.PeliculaId);

                        peliculas.Add(pelicula);
                    }

                    return peliculas;


                }
            }
        }
        //obtiene las peliculas por ID
        public Pelicula GetPeliculasByID(int id)
        {
            //query
            string query = @"select * from Pelicula where PeliculaId = @id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.Add(new SqlParameter("@id", id));
               
                    connection.Open();
                    /*DataTable dt = new DataTable();*/
                    SqlDataReader reader = cmd.ExecuteReader();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    Pelicula pelicula = new Pelicula();
                    while (reader.Read())
                    {
                   
                        pelicula = new Pelicula
                        {
                            PeliculaId = Convert.ToInt32(reader["PeliculaId"]),
                            Titulo = Convert.ToString(reader["Titulo"]),
                            Genero = Convert.ToString(reader["Genero"]),
                            FechaEstreno = Convert.ToDateTime(reader["FechaEstreno"]),
                            Foto = Convert.ToString(reader["Foto"])

                        };
                        
                    }
                    //Busca los actores que pertenecen a esta pelicula
                    pelicula.Actorer = GetActorByPelicula(pelicula.PeliculaId);
                    return pelicula;
                }
            }
        }
        //Inserta una pelicula en la base de datos.
        public bool CreatePeliculas(Pelicula pelicula)
        {
            //query
            string query = @"insert into Pelicula ( Titulo, Genero, FechaEstreno, Foto) values 
                                ( @Titulo,@Genero,@FechaEstreno,@Foto)";

            //similar a procesos anteriores
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    
                    cmd.Parameters.Add("@Titulo", System.Data.SqlDbType.VarChar,50).Value = pelicula.Titulo;
                    cmd.Parameters.Add("@Genero", System.Data.SqlDbType.VarChar, 50).Value = pelicula.Genero;
                    cmd.Parameters.Add("@FechaEstreno", System.Data.SqlDbType.Date, 50).Value = pelicula.FechaEstreno;
                    cmd.Parameters.Add("@Foto", System.Data.SqlDbType.VarChar, 250).Value = pelicula.Foto;
        
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
        //Actualiza una pelicula en la base de datos.
        public bool UpdatePeliculas( Pelicula pelicula)
        {
            string query = @"update Pelicula set
                                    Genero = @Genero,
                                    Titulo = @Titulo,
                                    FechaEstreno = @FechaEstreno,
                                    Foto=@Foto
                                    where PeliculaId = @PeliculaId 
                                ";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.Add("@Titulo", System.Data.SqlDbType.VarChar, 50).Value = pelicula.Titulo;
                    cmd.Parameters.Add("@Genero", System.Data.SqlDbType.VarChar, 50).Value = pelicula.Genero;
                    cmd.Parameters.Add("@FechaEstreno", System.Data.SqlDbType.Date, 50).Value = pelicula.FechaEstreno;
                    cmd.Parameters.Add("@Foto", System.Data.SqlDbType.VarChar, 250).Value = pelicula.Foto;
                    cmd.Parameters.Add("@PeliculaId", System.Data.SqlDbType.Int).Value = pelicula.PeliculaId;
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

        //Elimina una pelicula especifica de la base de datos.
        public bool DeletePeliculas(int id)
        {
            string query = @"delete Pelicula where PeliculaId = @id";

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
         //Obtiene una lista de los autores que pertenecen a una pelicula en especifico
        private List<Actor> GetActorByPelicula(int id)
        {
            //query
            string query = @"select a.ActorId,a.NombreCompleto, a.FechaNacimiento, a.Sexo, a.Foto  from Actor as a join PeliculaYActor as pa on a.ActorId = pa.ActorId
                                join Pelicula as p on p.PeliculaId = pa.PeliculaId
                                where p.PeliculaId = @id";

            //similar a procesos anteriores
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    List<Actor> actores = new List<Actor>();
                    connection.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    while (reader.Read())
                    {
                        var actor = new Actor
                        {
                            ActorId = Convert.ToInt32(reader["ActorId"]),
                            NombreCompleto = Convert.ToString(reader["NombreCompleto"]),
                            FechaNacimiento = Convert.ToDateTime(reader["FechaNacimiento"]),
                            Sexo = Convert.ToString(reader["Sexo"]),
                            Foto = Convert.ToString(reader["Foto"])

                        };

                        actores.Add(actor);
                    }

                    return actores;


                }
            }
        }

    }
}
