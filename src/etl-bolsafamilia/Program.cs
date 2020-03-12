using System;

namespace etl_bolsafamilia
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("------------- ETL Bolsa Família -------------");
            Console.WriteLine("1 - Extração");
            Console.WriteLine("2 - Carga");

            int option = int.Parse(Console.ReadLine());

            switch (option)
            {
                case 1:
                    Scraper scraper = new Scraper();
                    scraper.Execute();
                    break;
                case 2:
                    Loader loader = new Loader();
                    loader.LoadDM_Cidade();
                    loader.LoadDM_Tempo();
                    loader.Load();
                    break;
            }
        }
    }
}
