using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace etl_bolsafamilia
{
    public class Scraper
    {
        private HttpClient Client { get; set; }
        private string ConnectionString { get; set; }

        public Scraper()
        {
            Client = new HttpClient();
            ConnectionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=msdb;Integrated Security=SSPI;MultipleActiveResultSets=true";
        }

        public void Execute()
        {
            using (var package = new ExcelPackage(new FileInfo(@"D:\Code\Projetos\lab-banco\src\etl-bolsafamilia\Municipios_IBGE.xlsx")))
            {
                var firstSheet = package.Workbook.Worksheets["TCU"];
                var rows = firstSheet.Dimension.Rows - 5;

                for (int i = 5; i < rows; i++)
                {
                    var codIbge = firstSheet.Cells[$"B{i}"].Text + firstSheet.Cells[$"C{i}"].Text;
                    for (int j = 1; j <= 6; j++)
                    {
                        try
                        {
                            var resp = Request("2019", $"0{j}", codIbge).Result;
                            InsertDados(resp.FirstOrDefault());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }

                        decimal percent = (i / rows) * 100;
                        Console.WriteLine($"Percent: {percent}%");
                    }
                }
            }
        }

        private async Task<List<DadosDTO>> Request(string ano, string mes, string codIbge)
        {
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            var url = $"http://www.transparencia.gov.br/api-de-dados/bolsa-familia-por-municipio/?mesAno={ano}{mes}&codigoIbge={codIbge}&pagina=1";

            var streamTask = Client.GetStreamAsync(url);

            return await JsonSerializer.DeserializeAsync<List<DadosDTO>>(await streamTask);
        }

        private void InsertDados(DadosDTO dados)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var insert = string.Format(@$"Insert into BolsaFamilia.Dados(ID, DATA_REFERENCIA, VALOR, QTD_BENEFICIADOS, NOME_IBGE, SIGLA, NOME) 
                                       VALUES({dados.id}, '{dados.dataReferencia}', {dados.valor.ToString().Replace(",", ".")}, {dados.quantidadeBeneficiados}, 
                                       '{dados.municipio.nomeIBGE.Replace("'", "")}', '{dados.municipio.uf.sigla}', '{dados.municipio.uf.nome.Replace("'", "")}');");

                var insertCommand = new SqlCommand(insert, connection);
                connection.Open();
                insertCommand.ExecuteNonQuery();
            }
        }
    }

    [DataContract]
    public class DadosDTO
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string dataReferencia { get; set; }

        [DataMember]
        public decimal valor { get; set; }

        [DataMember]
        public int quantidadeBeneficiados { get; set; }

        [DataMember]
        public MunicipioDTO municipio { get; set; }
    }

    [DataContract]
    public class MunicipioDTO
    {
        [DataMember]
        public string nomeIBGE { get; set; }

        [DataMember]
        public UfDTO uf { get; set; }
    }

    [DataContract]
    public class UfDTO
    {
        [DataMember]
        public string sigla { get; set; }

        [DataMember]
        public string nome { get; set; }
    }
}
