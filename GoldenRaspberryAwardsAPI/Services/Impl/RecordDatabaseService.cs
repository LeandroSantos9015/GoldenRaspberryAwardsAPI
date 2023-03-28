using GoldenRaspberryAwardsAPI.Models;
using GoldenRaspberryAwardsAPI.Repository;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.OpenApi.Validations;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using static System.Net.WebRequestMethods;

namespace GoldenRaspberryAwardsAPI.Services.Impl
{
    public class RecordDatabaseService : IRecordDatabaseService
    {
        private readonly IFilmeRepository _filmeRepository;
        public RecordDatabaseService(IFilmeRepository filmeRepository)
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
    }
}
