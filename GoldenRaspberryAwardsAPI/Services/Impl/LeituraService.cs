using GoldenRaspberryAwardsAPI.Models;
using GoldenRaspberryAwardsAPI.Repository;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.OpenApi.Validations;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Net.WebRequestMethods;

namespace GoldenRaspberryAwardsAPI.Services.Impl
{
    public class LeituraService : ILeituraService
    {
        private readonly IFilmeRepository _filmeRepository;
        public LeituraService(IFilmeRepository filmeRepository)
        {
            _filmeRepository = filmeRepository;
        }

        public bool CarregarDadosNoBanco(string path)
        {
            if (string.IsNullOrEmpty(path))
                path = Environment.CurrentDirectory.ToString() + "/movielist.csv";

            var listaDeFilmes = CsvToListObjectGeneric<ModelFilmes>(path);

            _filmeRepository.Inserir(listaDeFilmes);

            return true;
        }

        public string QuemObtevePremioMaisRapido()
        {
            var valor = QuemObteveDoisPremiosMaisRapido();

             return valor.Interval == 1 ? JsonConvert.SerializeObject(valor, Formatting.Indented) : null;
        }

        public string QuemLevouMaisTempoConsecutivos()
        {
            var valor = QuemLevouMaisTempoPraGanharPremicConsecutivo();

            return valor.Interval > 0 ? JsonConvert.SerializeObject(valor, Formatting.Indented) : null;
        }


        public string MenorIntervaloPrimeiroEultimo()
        {
            var listaDoBanco = _filmeRepository.Listar();

            var listaAgrupada = listaDoBanco.GroupBy(x => x.Producers).ToList();

            List<object> lista = new List<object>();

            IList<ModelIntervalo> modelo = new List<ModelIntervalo>();

            var maior = new ModelIntervalo();

            var menor = new ModelIntervalo();

            menor.PreviousWin = 9999;

            foreach (var item in listaAgrupada)
            {

                modelo = PrimeiroEultimoAvencerComMenorIntervaloDeTempo(item.ToList());

                var menorEncontrado = modelo?.OrderBy(x => x.PreviousWin).FirstOrDefault();
                var maiorEncontrado = modelo?.OrderBy(x => x.PreviousWin).FirstOrDefault();

                if (maiorEncontrado != null || menorEncontrado != null)
                {
                    "".ToString();
                }


                if (menorEncontrado != null && menor.PreviousWin > menorEncontrado.PreviousWin)
                    menor = menorEncontrado;

                if (maiorEncontrado != null && maior.FollowingWin < maiorEncontrado.FollowingWin)
                    maior = maiorEncontrado;
            }

            var retorno = new
            {
                min = new List<ModelIntervalo>() { menor, maior }
            };

            return JsonConvert.SerializeObject(retorno, Formatting.Indented);
        }

        private ModelIntervalo QuemLevouMaisTempoPraGanharPremicConsecutivo()
        {
            var listaDoBanco = _filmeRepository.Listar();

            var listaAgrupada = listaDoBanco.GroupBy(x => x.Producers).ToList();

            List<object> lista = new List<object>();

            var maiorIntervalo = new ModelIntervalo();

            foreach (var item in listaAgrupada)
            {
                var auxMaiorIntervalo = ComparacaoMaiorIntervaloPremioConsecutivo(item.ToList());

                if (auxMaiorIntervalo != null && auxMaiorIntervalo.Interval > maiorIntervalo.Interval)
                    maiorIntervalo = auxMaiorIntervalo;

            }

            return maiorIntervalo;

        }

        private ModelIntervalo QuemObteveDoisPremiosMaisRapido()
        {
            var listaDoBanco = _filmeRepository.Listar();

            var listaAgrupada = listaDoBanco.GroupBy(x => x.Producers).ToList();

            var consecutivoMaisRapido = new ModelIntervalo();

            consecutivoMaisRapido.PreviousWin = 999999;

            foreach (var item in listaAgrupada)
            {
                var auxConsecutivoMaisRapido = VencedorConsecutivoMaisRapido(item.ToList());

                if (auxConsecutivoMaisRapido != null && auxConsecutivoMaisRapido.PreviousWin < consecutivoMaisRapido.PreviousWin)
                    consecutivoMaisRapido = auxConsecutivoMaisRapido;

            }

            return consecutivoMaisRapido;
        }

        private ModelIntervalo VencedorConsecutivoMaisRapido(IList<ModelFilmes> filmes)
        {
            if (filmes.Count() < 2)
                return null;

            var filmesOrdenadosPorAno = filmes.OrderBy(x => x.Year).ToList();

            ModelFilmes modelFilmes = null;
            IList<ModelIntervalo> intervalos = new List<ModelIntervalo>();

            for (int i = 0; i < filmesOrdenadosPorAno.Count(); i++)
            {
                var anoAtual = filmes[i].Year;

                var anoSeguinte = filmes.Count > i + 1 ?
                    filmes[i + 1].Year : 0;

                if (anoAtual + 1 == anoSeguinte)
                {
                    intervalos.Add(new ModelIntervalo
                    {
                        FollowingWin = anoSeguinte,
                        PreviousWin = anoAtual,
                        Interval = anoSeguinte - anoAtual,
                        Producer = filmes.FirstOrDefault().Producers
                    });
                }
            }

            var retorno = intervalos.Count > 0 ? intervalos.OrderBy(x => x.PreviousWin).FirstOrDefault() : null;

            return retorno;
        }

        /// <summary>
        /// Obter o produtor com maior intervalo entre dois prêmios consecutivos, e o que
        /// obteve dois prêmios mais rápido, seguindo a especificação de formato definida na
        /// página 2;
        /// obs do programador: Conforme os dados colhidos no CSV, nenhum deles entrou na regra abaixo.
        /// O meu entendimento foi que o produtor com maior intervalo entre dois premios consecutivos seria
        /// ele ter ganho por exemplo a primeira vez em 1990 e 1991 e depois 2010 e 2011, então o intervalo é 2010 - 1991 = 19
        /// no caso não levei em consideração o fato de ser vencedor pois com os dados recebidos estava ficando muito vago
        /// </summary>
        /// <param name="filmes"></param>
        /// <returns></returns>
        private ModelIntervalo ComparacaoMaiorIntervaloPremioConsecutivo(IList<ModelFilmes> filmes)
        {
            // pra ser 2 intervalos consecutivos então tem que ter pelo menos 4 premios
            if (filmes.Count() <= 4)
                return null;

            var filmesOrdenadosPorAno = filmes.OrderBy(x => x.Year).ToList();

            int anoInicial = 0;
            int anoFinal = 0;

            for (int i = 0; i < filmesOrdenadosPorAno.Count(); i++)
            {
                var anoAtual = filmes[i].Year;

                var anoSeguinte = filmes.Count > i + 1 ?
                    filmes[i + 1].Year : 0;

                var consecutivos = anoAtual + 1 == anoSeguinte;

                if (consecutivos && anoInicial == 0)
                    anoInicial = anoSeguinte;

                if (consecutivos && anoInicial > 0)
                    anoFinal = anoAtual;
            }

            if (anoFinal > 0 && anoInicial > 0)
                return new ModelIntervalo
                {
                    FollowingWin = anoFinal,
                    PreviousWin = anoInicial,
                    Interval = anoFinal - anoInicial,
                    Producer = filmes.FirstOrDefault().Producers
                };

            return null;
        }

        private IList<T> CsvToListObjectGeneric<T>(string path)
        {
            var csv = new List<string[]>();
            var lines = System.IO.File.ReadAllLines(path);

            foreach (string line in lines)
                csv.Add(line.Split(','));

            if (!csv.Any())
                return null;

            var properties = lines[0].Split(',')[0].Split(";");

            var listObjResult = new List<Dictionary<string, string>>();

            for (int i = 1; i < lines.Length; i++)
            {
                var objResult = new Dictionary<string, string>();

                for (int j = 0; j < properties.Length; j++)
                    objResult.Add(properties[j], lines[i].Split(";")[j]);

                listObjResult.Add(objResult);
            }

            string serializa = JsonConvert.SerializeObject(listObjResult);
            var value = JsonConvert.DeserializeObject<T[]>(serializa);

            return value;
        }

        private IList<ModelIntervalo> PrimeiroEultimoAvencerComMenorIntervaloDeTempo(IList<ModelFilmes> filmes)
        {
            if (filmes.Count() < 2)
                return null;

            var filmesOrdenadosPorAno = filmes.OrderBy(x => x.Year).ToList();

            var intervalos = new List<ModelIntervalo>();

            for (int i = 0; i < filmesOrdenadosPorAno.Count(); i++)
            {
                var anoAtual = filmes[i].Year;

                var anoSeguinte = filmes.Count > i + 1 ?
                    filmes[i + 1].Year : 0;

                if (anoAtual + 1 == anoSeguinte)
                {
                    intervalos.Add(new ModelIntervalo
                    {
                        FollowingWin = anoSeguinte,
                        PreviousWin = anoAtual,
                        Interval = anoSeguinte - anoAtual,
                        Producer = filmes.FirstOrDefault().Producers
                    });
                    break;
                }
            }

            //var filmesOrdenadosPorAnoDecresc = filmes.OrderByDescending(x => x.Year).ToList();
            //var ultimoIntervalo = new ModelIntervalo();

            //for (int i = 0; i < filmesOrdenadosPorAnoDecresc.Count(); i++)
            //{
            //    var anoAtual = filmes[i].Year;

            //    var anoSeguinte = filmes.Count > i + 1 ?
            //        filmes[i + 1].Year : 0;

            //    if (anoAtual + 1 == anoSeguinte)
            //    {
            //        intervalos.Add(new ModelIntervalo
            //        {
            //            FollowingWin = anoSeguinte,
            //            PreviousWin = anoAtual,
            //            Interval = anoSeguinte - anoAtual,
            //            Producer = filmes.FirstOrDefault().Producers
            //        });
            //        break;
            //    }
            //}

            return intervalos;
        }

        private IList<ModelIntervalo> PrimeiroEultimoAvnecerComMaiorIntervaloDeTempo()
        {
            var listaDoBanco = _filmeRepository.Listar();

            var vencedores = listaDoBanco.Where(x => x.IsWinner).ToList().OrderBy(x => x.Year).ToList();

            //var listaDeNomes = "";

            List<string> listaDeNomes = new List<string>();

            foreach (var item in vencedores)
            {
                item.Producers = item.Producers.Replace(" and ", ",");
                listaDeNomes.AddRange(item.Producers.Split(","));
            }

            listaDeNomes = listaDeNomes.Distinct().OrderBy(x => x).ToList();

            listaDeNomes.Remove("");

            IList<ModelIntervalo> modelo = new List<ModelIntervalo>();

            var maior = new ModelIntervalo();

            var menor = new ModelIntervalo();


            foreach (var item in vencedores)
            {
                var listagem = new List<ModelFilmes>();

                for (int i = 0; i < listaDeNomes.Count(); i++)
                {
                    if (item.Producers.Contains(listaDeNomes[i].Trim()))
                        listagem.Add(item);
                }

                if (listagem.Count > 1)
                    "".ToString();

                modelo = PrimeiroEultimoAvencerComMenorIntervaloDeTempo(listagem);

                var menorEncontrado = modelo?.OrderBy(x => x.PreviousWin).FirstOrDefault();
                var maiorEncontrado = modelo?.OrderBy(x => x.PreviousWin).FirstOrDefault();

                if (maiorEncontrado != null || menorEncontrado != null)
                {
                    "".ToString();
                }


                if (menorEncontrado != null && menor.PreviousWin > menorEncontrado.PreviousWin)
                    menor = menorEncontrado;

                if (maiorEncontrado != null && maior.FollowingWin < maiorEncontrado.FollowingWin)
                    maior = maiorEncontrado;


            }



            return null;

        }


    }
}
