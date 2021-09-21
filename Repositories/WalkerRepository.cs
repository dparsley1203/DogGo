using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public class WalkerRepository : IWalkerRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public WalkerRepository(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<Walker> GetAllWalkers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT w.Id, w.Name, w.ImageUrl, w.NeighborhoodId, n.Name as NeighborhoodName
                        FROM Walker w
                        LEFT JOIN Neighborhood n ON n.Id = w.NeighborhoodId
                    ";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walker> walkers = new List<Walker>();
                    while (reader.Read())
                    {
                        Walker walker = new Walker
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("NeighborhoodId")))
                        {
                            walker.Neighborhood = new Neighborhood
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("NeighborhoodName")),
                            };
                        }

                        walkers.Add(walker);
                    }

                    reader.Close();

                    return walkers;
                }
            }
        }

        public Walker GetWalkerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT w.Id, w.Name, w.ImageUrl, w.NeighborhoodId, n.Name as NeighborhoodName
                        FROM Walker w
                        LEFT JOIN Neighborhood n ON n.Id = w.NeighborhoodId
                        WHERE w.Id = @id
                    ";

                    cmd.Parameters.AddWithValue("@id", id);
                    Walker walker = null;

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {

                         walker = new Walker()
                         {
                             Id = reader.GetInt32(reader.GetOrdinal("Id")),
                             Name = reader.GetString(reader.GetOrdinal("Name")),
                             ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl")),
                             NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                         };
                        

                        if (!reader.IsDBNull(reader.GetOrdinal("NeighborhoodId")))
                        {
                            walker.Neighborhood = new Neighborhood
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("NeighborhoodName")),
                            };

                        }

                        reader.Close();
                        return walker;
                    }
                    else
                    {
                        reader.Close();
                        return walker;
                    }

                }
            }
        }
    }
}
