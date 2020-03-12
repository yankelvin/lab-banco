using System;
using System.Data.SqlClient;

namespace etl_bolsafamilia
{
    public class Loader
    {
        private string ConnectionString { get; set; }

        public Loader()
        {
            ConnectionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=msdb;Integrated Security=SSPI;MultipleActiveResultSets=true";
        }

        public void Load()
        {
            var select = "Select * from BolsaFamilia.Dados;";

            using (var connection = new SqlConnection(ConnectionString))
            {
                var command = new SqlCommand(select, connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var data = Convert.ToDateTime(reader["DATA_REFERENCIA"]);

                        var insert = @$"
                            DECLARE @ID_CIDADE INT,
                                    @ID_TEMPO INT;

                            Select @ID_CIDADE = ID_CIDADE from BolsaFamiliaDW.DM_Cidade where NOM_CIDADE = '{reader["NOME_IBGE"]}';

                            Select @ID_TEMPO = ID_TEMPO from BolsaFamiliaDW.DM_Tempo where NUM_MES = {data.Month} and NUM_ANO = {data.Year};

                            Insert into BolsaFamiliaDW.FT_Registros (ID_CIDADE, ID_TEMPO, VLR_GASTO, QTD_BENEFICIADOS)
                            VALUES(@ID_CIDADE, @ID_TEMPO, {reader["VALOR"]}, {reader["QTD_BENEFICIADOS"]});
                        ";

                        var insertCommand = new SqlCommand(insert, connection);
                        insertCommand.ExecuteNonQuery();
                        Console.WriteLine($"Inserindo - Cidade: {reader["NOME_IBGE"]} | Valor Gasto: {reader["VALOR"]} | Qtd. Beneficiados: {reader["QTD_BENEFICIADOS"]}");
                    }
                }
            }
        }

        public void LoadDM_Cidade()
        {
            var select = "Select distinct NOME_IBGE, NOME, SIGLA from BolsaFamilia.Dados;";

            using (var connection = new SqlConnection(ConnectionString))
            {
                var command = new SqlCommand(select, connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var insert = @$"Insert into BolsaFamiliaDW.DM_Cidade (NOM_CIDADE, NOM_UF, SGL_UF)
                                       VALUES('{reader["NOME_IBGE"]}', '{reader["NOME"]}', '{reader["SIGLA"]}');";

                        var insertCommand = new SqlCommand(insert, connection);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        public void LoadDM_Tempo()
        {
            var select = "Select distinct DATA_REFERENCIA from BolsaFamilia.Dados;";

            using (var connection = new SqlConnection(ConnectionString))
            {
                var command = new SqlCommand(select, connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var data = Convert.ToDateTime(reader["DATA_REFERENCIA"]);

                        var insert = @$"Insert into BolsaFamiliaDW.DM_Tempo (NUM_MES, NUM_ANO)
                                        VALUES({data.Month}, {data.Year});";

                        var insertCommand = new SqlCommand(insert, connection);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
