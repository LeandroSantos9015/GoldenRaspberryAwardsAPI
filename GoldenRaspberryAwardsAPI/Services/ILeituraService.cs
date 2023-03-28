namespace GoldenRaspberryAwardsAPI.Services
{
    public interface ILeituraService
    {

        bool CarregarDadosNoBanco(string path);

        string QuemObtevePremioMaisRapido();

        string QuemLevouMaisTempoConsecutivos();


        string MenorIntervaloPrimeiroEultimo();

    }
}
