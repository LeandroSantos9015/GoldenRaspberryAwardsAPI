using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GoldenRaspberryAwardsAPI.Models
{
    public class ModelFilmes
    {
        [Key]
        public int Id { get; set; }


        [JsonPropertyName("year")]
        public int Year { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("studios")]
        public string Studios { get; set; }

        [JsonPropertyName("producers")] 
        public string Producers { get; set; }
        
        [JsonPropertyName("winner")]
        public string Winner { get; set; }

        [JsonPropertyName("isWinner")]
        public bool IsWinner => "yes".Equals(Winner);

    }

}
