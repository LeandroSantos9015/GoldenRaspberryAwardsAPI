using GoldenRaspberryAwardsAPI.Models;
using Microsoft.VisualBasic;

namespace GoldenRaspberryAwardsAPI.Repository
{
    public interface IFilmeRepository
    {

        void Inserir(ModelFilmes filme);

        void Inserir(IList<ModelFilmes> filme);

        IList<ModelFilmes> Listar();

    }
}
