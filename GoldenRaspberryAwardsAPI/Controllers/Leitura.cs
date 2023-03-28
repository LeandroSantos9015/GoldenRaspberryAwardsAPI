using GoldenRaspberryAwardsAPI.Services;
using Microsoft.AspNetCore.Mvc;


namespace GoldenRaspberryAwardsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Leitura : ControllerBase
    {

        private readonly ILogger<Leitura> _logger;
        private readonly ILeituraService _leitura;

        public Leitura(ILogger<Leitura> logger, ILeituraService leitura)
        {
            _leitura = leitura;
            _logger = logger;
           
        }

        [HttpPost]
        [Route("receive")]
        public IActionResult LerDados()
        {


            return Ok();
        }

        [HttpPost]
        [Route("carregarArquivoCsvNoBD")]
        public IActionResult SalvarCsvNoBancoDeDados()
        {

            var retorno = _leitura.CarregarDadosNoBanco("");

            if (retorno != null)
                return Ok();
            else
                return BadRequest("Não foi encontrado nenhum vencedor consecutivo");
        }

        [HttpGet]
        [Route("obterQuemObteveDoisPremiosMaisRapido")]
        public IActionResult ObterQuemObteveDoisPremiosMaisRapido()
        {
            var retorno = _leitura.QuemObtevePremioMaisRapido();

            if (retorno != null)
                return Ok(retorno);
            else
                return BadRequest("Não foi encontrado nenhum vencedor consecutivo");
        }


        [HttpGet]
        [Route("obterMaiorIntervaloEntrePremiosConsecutivos")]
        public IActionResult ObterMaiorIntervaloEntrePremiosConsecutivos()
        {
            var retorno = _leitura.QuemLevouMaisTempoConsecutivos();

            if (retorno != null)
                return Ok(retorno);
            else
                return BadRequest("Não foi encontrado nenhum produtor que venceu em dois momentos consecutivamente");
        }

        [HttpGet]
        [Route("obterPrimeiroEultimoAvencerEmMenorIntervalo")]
        public IActionResult Maior()
        {
            var retorno = _leitura.MenorIntervaloPrimeiroEultimo();

            if (retorno != null)
                return Ok(retorno);
            else
                return BadRequest("Não foi encontrado nenhum produtor que venceu em dois momentos consecutivamente");
        }
        
    }
}
