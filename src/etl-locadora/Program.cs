using System;
using System.Data.SqlClient;

namespace etl_locadora
{
    class Program
    {
        public static string connectionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=msdb;Integrated Security=SSPI;MultipleActiveResultSets=true";

        static void Main(string[] args)
        {
            Console.WriteLine("------------- Realização de Carga -------------");
            Console.WriteLine("1 - DM_Socio");
            Console.WriteLine("2 - DM_Artista");
            var option = Convert.ToInt32(Console.ReadLine());
            switch (option)
            {
                case 1:
                    LoadDM_Socio();
                    break;
                case 2:
                    LoadDM_Artista();
                    break;
                default:
                    Console.WriteLine("Opção inválida!");
                    break;
            }
        }

        private static void LoadDM_Socio()
        {
            var select = "Select * from Locadora.socios soc join Locadora.tipos_socios tip on soc.cod_tps = tip.cod_tps;";

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(select, connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var insert = string.Format(@"Insert into LocadoraDW.DM_Socio(NOM_SOC, TIPO_SOCIO) 
                                       VALUES('{0}', '{1}');", reader["nom_soc"], reader["dsc_tps"]);

                        var insertCommand = new SqlCommand(insert, connection);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void LoadDM_Artista()
        {
            var select = "Select * from Locadora.artistas;";

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(select, connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var tpo = (reader["tpo_art"]) switch
                        {
                            'B' => "Banda",
                            'D' => "Dupla",
                            'I' => "Artista Individual",
                            _ => "Banda",
                        };

                        var nac = (reader["nac_bras"]) switch
                        {
                            'V' => "Nacional",
                            'F' => "Estrangeiro",
                            _ => "Nacional",
                        };

                        var insert = string.Format(@"Insert into LocadoraDW.DM_Artista(TPO_ART, NAC_BRAS, NOM_ART) 
                                       VALUES('{0}', '{1}', '{2}');", tpo, nac, reader["nom_art"]);

                        var insertCommand = new SqlCommand(insert, connection);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
