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
            Console.WriteLine("3 - DM_Titulo");
            Console.WriteLine("4 - DM_Gravadora");
            Console.WriteLine("5 - DM_Tempo");

            var option = Convert.ToInt32(Console.ReadLine());
            switch (option)
            {
                case 1:
                    LoadDM_Socio();
                    break;
                case 2:
                    LoadDM_Artista();
                    break;
                case 3:
                    LoadDM_Titulo();
                    break;
                case 4:
                    LoadDM_Gravadora();
                    break;
                case 5:
                    LoadDM_Tempo();
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

        private static void LoadDM_Titulo()
        {
            var select = "Select * from Locadora.titulos;";
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(select, connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var tpo = (reader["tpo_tit"]) switch
                        {
                            'C' => "CD",
                            'D' => "DVD",
                            _ => "CD",
                        };

                        var cla = (reader["cla_tit"]) switch
                        {
                            'L' => "Lançamento",
                            'N' => "Normal",
                            'P' => "Promocional",
                            _ => "Normal",
                        };

                        var insert = string.Format(@"Insert into LocadoraDW.DM_Titulo(TPO_TITULO, CLA_TITULO, DSC_TITULO) 
                                       VALUES('{0}', '{1}', '{2}');", tpo, cla, reader["dsc_tit"].ToString().Replace("'", ""));

                        var insertCommand = new SqlCommand(insert, connection);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void LoadDM_Gravadora()
        {
            var select = "Select * from Locadora.gravadoras;";

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(select, connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var nac = (reader["nac_bras"]) switch
                        {
                            'V' => "Nacional",
                            'F' => "Estrangeiro",
                            _ => "Nacional",
                        };

                        var insert = string.Format(@"Insert into LocadoraDW.DM_Gravadora(UF_GRAV, NAC_BRAS, NOM_GRAV) 
                                       VALUES('{0}', '{1}', '{2}');", reader["uf_grav"], nac, reader["nom_grav"]);

                        var insertCommand = new SqlCommand(insert, connection);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void LoadDM_Tempo()
        {
            var select = "Select * from Locadora.locacoes;";

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(select, connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var data = (DateTimeOffset)reader["dat_pgto"];
                        var nu_anomes = Convert.ToInt32(data.Year.ToString() + data.Month.ToString());
                        var sg_mes = data.ToString("MMM");
                        var nm_mesano = $"{sg_mes}-{data.Year}";
                        var nm_mes = data.ToString("MMMM");
                        var turno = data.Hour < 12 ? "Manhã" : (data.Hour < 18 ? "Tarde" : "Noite");

                        var insert = string.Format(@$"Insert into LocadoraDW.DM_Tempo(NU_ANO, NU_MES, NU_ANOMES, SG_MES, NM_MESANO, NM_MES, NU_DIA, DT_TEMPO, NU_HORA, TURNO) 
                                       VALUES({data.Year}, {data.Month}, {nu_anomes}, '{sg_mes}', '{nm_mesano}', '{nm_mes}', {data.Day}, {data}, {data.Hour}, '{turno}');");

                        var insertCommand = new SqlCommand(insert, connection);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
