using GoldenRaspberryAwardsAPI.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GoldenRaspberryAwardsAPI.Repository.Impl
{
    public class FilmeRepository : IFilmeRepository
    {
        private readonly DataContext _context;

        public FilmeRepository(DataContext context)
        {
            try
            {
                _context = context;

            }
            catch (Exception ex)
            {
                ex.ToString();

            }



        }


        public void Inserir(ModelFilmes filme)
        {
            _context.Add(filme);
            _context.SaveChanges();
        }

        public void Inserir(IList<ModelFilmes> filme)
        {
            _context.AddRange(filme);
            _context.SaveChanges();
        }

        public IList<ModelFilmes> Listar()
        {
            return _context.Filmes.ToList();
        }



        public void Insert<T>(IList<T> item, string nomeTabela)
        {
            var list = new List<T>();
            item.ToString();
        }
    }
}
