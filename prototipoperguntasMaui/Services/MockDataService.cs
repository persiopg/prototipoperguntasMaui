using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace prototipoperguntasMaui.Services
{
    public class MockDataService : IDataService
    {
        public async Task<RootData> GetDataAsync()
        {
            // Simulate async IO
            await Task.Delay(10);
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            return JsonSerializer.Deserialize<RootData>(SeedData.JsonContent, options);
        }
    }

    public class RootData
    {
        public AppMetadataDto app_metadata { get; set; }
        public ScoringRulesDto scoring_rules { get; set; }
        public List<QuestionDto> questions_catalog { get; set; }
        public List<ProductDto> products_catalog { get; set; }
        public List<CampaignDto> campaigns_catalog { get; set; }
        public List<StoreDto> stores_mock { get; set; }
    }

    public class AppMetadataDto
    {
        public string cycle_id { get; set; }
        public string description { get; set; }
        public string version { get; set; }
        public string last_update { get; set; }
    }

    public class ScoringRulesDto
    {
        public Dictionary<string, int> weights { get; set; }
        public List<TargetRuleDto> targets_by_channel { get; set; }
    }

    public class TargetRuleDto
    {
        public string pillar { get; set; }
        public int target { get; set; }
        public string channel { get; set; }
        public string obs { get; set; }
    }

    public class QuestionDto
    {
        public string id { get; set; }
        public string sub_pillar { get; set; }
        public string brand { get; set; }
        public string text { get; set; }
        public string guidance { get; set; }
        public List<string> options { get; set; }
        public Dictionary<string, double> points_config { get; set; }
    }

    public class ProductDto
    {
        public string ean { get; set; }
        public string name { get; set; }
        public string brand { get; set; }
        public ProductRulesDto rules { get; set; }
        public string tag { get; set; }
        public List<string> channels { get; set; }
    }

    public class ProductRulesDto
    {
        public bool presence { get; set; }
        public bool price { get; set; }
    }

    public class CampaignDto
    {
        public string id { get; set; }
        public string type { get; set; }
        public string channel { get; set; }
        public string task { get; set; }
        public string title { get; set; }
        public DateRangeDto dates { get; set; }
    }

    public class DateRangeDto
    {
        public string start { get; set; }
        public string end { get; set; }
    }

    public class StoreDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string channel { get; set; }
        public string address { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }
}
