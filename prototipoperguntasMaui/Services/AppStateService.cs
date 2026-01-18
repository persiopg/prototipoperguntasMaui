using System.Text.Json;
using System.Threading.Tasks;

namespace prototipoperguntasMaui.Services
{
    public class AppStateService
    {
        private readonly IDataService _dataService;
        
        public event Action OnChange;

        public bool IsLoggedIn { get; private set; } = false;

        public RootData RawData { get; private set; }
        public MetaData Meta { get; set; } = new();
        public List<Store> Stores { get; set; } = new();
        public List<VisitLog> ExecutionLog { get; set; } = new();
        public Dictionary<int, TempExecution> PausedExecutions { get; set; } = new();
        
        // Simplified state management for the current execution
        public int? CurrentStoreId { get; set; }
        public TempExecution TempExecution { get; set; } = new();

        public AppStateService(IDataService dataService)
        {
            _dataService = dataService;
            // Initialize data asynchronously
             Task.Run(InitializeDataAsync);
        }

        private async Task InitializeDataAsync()
        {
            var data = await _dataService.GetDataAsync();
            if (data == null) return;
            
            RawData = data;

            Meta = new MetaData
            {
                Cycle = new CycleInfo 
                { 
                    Id = data.app_metadata.cycle_id, 
                    Desc = data.app_metadata.description, 
                    Target = "Equipe de Trade" // Default
                },
                User = new UserInfo { Uuid = "colab_rep_007", Name = "James Bond da Silva", Role = "Promotor", Manager = "M. S. Manager" },
                Device = new DeviceInfo { Model = "Samsung Galaxy S23", Os = "Android 14", AppVersion = data.app_metadata.version },
                BatteryStart = 95
            };

            Stores = new List<Store>();

            foreach (var s in data.stores_mock)
            {
                var store = new Store
                {
                    Id = s.id,
                    Name = s.name,
                    Channel = s.channel,
                    Address = s.address,
                    Cluster = "PADRAO", // Default as removed from source
                    Zone = "GERAL",    // Default as removed from source
                    Status = "PENDENTE",
                    Lat = s.lat,
                    Lng = s.lng,
                    Data = new StoreData()
                };

                // 1. Positioning
                var questions = data.questions_catalog.Where(q => q.points_config != null).ToList();
                foreach (var q in questions)
                {
                    string channelKey = s.channel.ToLower();
                    if (!q.points_config.ContainsKey(channelKey)) continue; // Skip if no config for channel
                    
                    int points = (int)Math.Round(q.points_config[channelKey]);
                    if (points == 0 && !q.points_config.Values.Any(v => v > 0)) continue; // Skip if 0 points? Or maybe keep it? Let's keep if defined.

                    store.Data.Positioning.Add(new PositioningQuestion
                    {
                        Id = q.id,
                        Text = q.text,
                        Points = points,
                        Context = q.sub_pillar, 
                        Brand = q.brand,
                        Guidance = q.guidance,
                        ObsNote = "",
                        Options = q.options ?? new List<string>()
                    });
                }

                // 2. Presence & Pricing
                var filteredProducts = data.products_catalog.Where(p => p.channels.Contains(s.channel)).ToList();
                
                // Helper to get target points for presence
                // Logic based on scoring_rules not fully implemented here for brevity, simple logic:
                // If rule exists for channel/pillar, use it.
                
                // Group by Tag (LANCAMENTO vs BASELINE/Null)
                var groupedByTag = filteredProducts.GroupBy(p => p.tag);
                
                foreach (var group in groupedByTag)
                {
                    // Find target points
                    int pointsPerItem = 10;
                    string pillarName = "PRESENCA";
                    var rule = data.scoring_rules?.targets_by_channel?
                        .FirstOrDefault(r => r.pillar == pillarName && (r.channel == s.channel || r.channel == "ALL") && (group.Key == "LANCAMENTO" ? r.obs.Contains("Lançamento") : r.obs.Contains("Baseline")));
                    
                    if (rule != null && group.Any())
                    {
                        pointsPerItem = Math.Max(1, rule.target / group.Count());
                    }
                    
                    foreach (var p in group)
                    {
                        bool isMandatory = p.rules?.presence == true;
                        
                        store.Data.Presence.Add(new PresenceSku
                        {
                            Ean = p.ean,
                            Name = p.name,
                            Points = pointsPerItem, 
                            Mandatory = isMandatory,
                            Brand = p.brand,
                            Category = "Geral", 
                            Tag = p.tag ?? "BASELINE",
                            RuleDesc = isMandatory ? "Item Obrigatório" : "Mix Sugerido"
                        });

                        if (p.rules?.price == true)
                        {
                            store.Data.Pricing.Add(new PricingSku
                            {
                                Ean = p.ean,
                                Name = p.name,
                                Min = 0, 
                                    Max = 0, 
                                    Suggested = 0, 
                                    Points = 10,
                                    Brand = p.brand
                            });
                        }
                    }
                }

                // 3. Campaigns
                var storeCampaigns = data.campaigns_catalog.Where(c => c.channel == s.channel || c.channel == "TODOS").ToList();
                foreach (var c in storeCampaigns)
                {
                    store.Data.Extras.Add(new ExtraCampaign
                    {
                        Id = c.id,
                        Title = c.title ?? c.task,
                        Task = c.task,
                        Start = c.dates.start,
                        End = c.dates.end
                    });
                }

                Stores.Add(store);
            }
            
            NotifyStateChanged();
        }
        
        private void NotifyStateChanged() => OnChange?.Invoke();

        public Store GetCurrentStore() => Stores.FirstOrDefault(s => s.Id == CurrentStoreId);

        public void StartStore(int storeId)
        {
            // Mantido por compatibilidade: prepara o contexto de check-in
            PrepareCheckin(storeId);
        }

        public void PrepareCheckin(int storeId)
        {
            CurrentStoreId = storeId;
            NotifyStateChanged();
        }

        public void CompleteCheckin(int storeId, string photoBase64)
        {
            var store = Stores.FirstOrDefault(s => s.Id == storeId);
            if (store == null) return;

            CurrentStoreId = storeId;

            if (PausedExecutions.TryGetValue(storeId, out var existingExecution))
            {
                TempExecution = existingExecution;
            }
            else
            {
                TempExecution = new TempExecution();
            }

            if (!TempExecution.CheckinTime.HasValue)
            {
                TempExecution.ArrivalTimestamp = DateTime.Now.AddMinutes(-5); // Mock
                TempExecution.CheckinTime = DateTime.Now;
                TempExecution.CheckinCoords = new Coords { Lat = store.Lat, Lng = store.Lng };
            }

            TempExecution.CheckinPhotoBase64 = photoBase64;
            EnsureExecutionDefaults(store, TempExecution);

            NotifyStateChanged();
        }

        public void ResumeVisit(int storeId)
        {
            var store = Stores.FirstOrDefault(s => s.Id == storeId);
            if (store == null) return;

            CurrentStoreId = storeId;

            if (PausedExecutions.TryGetValue(storeId, out var existingExecution))
            {
                TempExecution = existingExecution;
            }

            EnsureExecutionDefaults(store, TempExecution);
            NotifyStateChanged();
        }

        public void PauseVisit()
        {
            var store = GetCurrentStore();
            if (store == null) return;

            store.Status = "EM_ANDAMENTO";
            PausedExecutions[store.Id] = TempExecution;
            CurrentStoreId = null;
            TempExecution = new TempExecution();
            NotifyStateChanged();
        }

        public bool Login(string email, string password)
        {
            // Hardcoded credentials for prototype
            var normalizedEmail = (email ?? string.Empty).Trim().ToLowerInvariant();
            var normalizedPassword = (password ?? string.Empty).Trim();

            if (normalizedEmail == "persio.godoy@novasingular.com.br" && normalizedPassword == "1q2w3E#")
            {
                IsLoggedIn = true;
                if (Meta.User != null)
                {
                    Meta.User.Name = "Persio Godoy";
                    Meta.User.Uuid = "colab_rep_persio"; 
                }
                NotifyStateChanged();
                return true;
            }
            return false;
        }

        public void Logout()
        {
            IsLoggedIn = false;
            NotifyStateChanged();
        }

        public void FinishVisit()
        {
            var store = GetCurrentStore();
            if (store == null) return;

            // Calculate Score
            int pAchieved = 0;
            
            // Positioning Score
            foreach (var q in store.Data.Positioning)
            {
                if (TempExecution.Answers.ContainsKey(q.Id))
                {
                    var ans = TempExecution.Answers[q.Id];
                    bool givesPoints = false;
                    
                    if (ans is bool b) givesPoints = b;
                    else if (ans != null) givesPoints = true; // Non-null answer (string option) implies success/found
                    
                    if (givesPoints) pAchieved += q.Points;
                }
            }

            var auditData = store.Data.Presence.Select(p =>
            {
                var status = TempExecution.Audit.ContainsKey(p.Ean) ? TempExecution.Audit[p.Ean] : "ABSENT";
                if (status == "PRESENT") pAchieved += p.Points;
                return new
                {
                    sku_ean = p.Ean,
                    product_name = p.Name,
                    points_config = new { points_possible = p.Points },
                    audit_result = new { status_found = status }
                };
            }).ToList();

            var posData = store.Data.Positioning.Select(q => new
            {
                question_id = q.Id,
                user_response = new
                {
                    answer_value = TempExecution.Answers.ContainsKey(q.Id) ? TempExecution.Answers[q.Id] : null,
                    justification_text = TempExecution.Justifications.ContainsKey(q.Id) ? TempExecution.Justifications[q.Id] : null
                }
            }).ToList();

             var priceData = store.Data.Pricing.Select(p => new
             {
                 sku_ean = p.Ean,
                 collection_data = new { price_collected = TempExecution.Prices.ContainsKey(p.Ean) ? TempExecution.Prices[p.Ean] : 0 }
             }).ToList();

            var visitLog = new VisitLog
            {
                VisitUuid = "exec_" + DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                StoreFullContext = new { id = store.Id, name = store.Name },
                Timestamps = new { checkin = TempExecution.CheckinTime, checkout = DateTime.Now },
                ScorecardConsolidated = new { final_score_achieved = pAchieved },
                ExecutionModulesDetailed = new
                {
                    module_positioning = new { answers_rich_data = posData },
                    module_presence = new { audit_rich_data = auditData },
                    module_pricing = new { price_check_rich_data = priceData }
                }
            };

            ExecutionLog.Add(visitLog);
            store.Status = "REALIZADO";
            PausedExecutions.Remove(store.Id);
            CurrentStoreId = null;
            TempExecution = new TempExecution();

            var logJson = JsonSerializer.Serialize(visitLog, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine(logJson);
            NotifyStateChanged();
        }

        public void SyncAllVisits()
        {
            if (ExecutionLog.Count == 0) return;

            var payload = JsonSerializer.Serialize(ExecutionLog, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine(payload);

            foreach (var store in Stores)
            {
                if (store.Status == "REALIZADO")
                {
                    store.Status = "SYNCED";
                }
            }

            NotifyStateChanged();
        }

        private void EnsureExecutionDefaults(Store store, TempExecution execution)
        {
            foreach (var sku in store.Data.Presence)
            {
                if (!execution.Audit.ContainsKey(sku.Ean))
                {
                    execution.Audit[sku.Ean] = "ABSENT";
                }
            }

            foreach (var extra in store.Data.Extras)
            {
                if (!execution.Campaigns.ContainsKey(extra.Id))
                {
                    execution.Campaigns[extra.Id] = false;
                }
            }
        }
    }

    // Models
    public class MetaData { public CycleInfo Cycle { get; set; } public UserInfo User { get; set; } public DeviceInfo Device { get; set; } public int BatteryStart { get; set; } }
    public class CycleInfo { public string Id { get; set; } public string Desc { get; set; } public string Target { get; set; } }
    public class UserInfo { public string Uuid { get; set; } public string Name { get; set; } public string Role { get; set; } public string Manager { get; set; } }
    public class DeviceInfo { public string Model { get; set; } public string Os { get; set; } public string AppVersion { get; set; } }
    
    public class Store 
    { 
        public int Id { get; set; } 
        public string Name { get; set; } 
        public string Channel { get; set; } 
        public string Address { get; set; } 
        public string Cluster { get; set; } 
        public string Zone { get; set; } 
        public string Status { get; set; } 
        public double Lat { get; set; } 
        public double Lng { get; set; } 
        public StoreData Data { get; set; } 
    }
    public class StoreData 
    { 
        public List<PositioningQuestion> Positioning { get; set; } = new(); 
        public List<PresenceSku> Presence { get; set; } = new(); 
        public List<PricingSku> Pricing { get; set; } = new(); 
        public List<ExtraCampaign> Extras { get; set; } = new();
    }
    
    public class PositioningQuestion 
    { 
        public string Id { get; set; } 
        public string Text { get; set; } 
        public int Points { get; set; } 
        public string Context { get; set; } 
        public string Brand { get; set; } 
        public string Guidance { get; set; } 
        public string ObsNote { get; set; } 
        public List<string> Options { get; set; } // New property for multiple choice options
    }
    public class PresenceSku 
    { 
        public string Ean { get; set; } 
        public string Name { get; set; } 
        public int Points { get; set; } 
        public bool Mandatory { get; set; } 
        public string Brand { get; set; } 
        public string Category { get; set; } 
        public string Tag { get; set; } 
        public string RuleDesc { get; set; } 
    }
    public class PricingSku 
    { 
        public string Ean { get; set; } 
        public string Name { get; set; } 
        public decimal Min { get; set; } 
        public decimal Max { get; set; } 
        public decimal Suggested { get; set; } 
        public int Points { get; set; } 
        public string Brand { get; set; } 
    }
    public class ExtraCampaign { public string Id { get; set; } public string Title { get; set; } public string Task { get; set; } public string Start { get; set; } public string End { get; set; } }

    public class TempExecution 
    { 
        public DateTime? ArrivalTimestamp { get; set; } 
        public DateTime? CheckinTime { get; set; } 
        public string CheckinPhotoBase64 { get; set; }
        public Dictionary<string, string> EvidencePhotos { get; set; } = new();
        public Dictionary<string, object> Answers { get; set; } = new(); 
        public Dictionary<string, string> Justifications { get; set; } = new(); 
        public Dictionary<string, string> Audit { get; set; } = new(); 
        public Dictionary<string, decimal> Prices { get; set; } = new(); 
        public Dictionary<string, bool> Campaigns { get; set; } = new(); 
        public Coords CheckinCoords { get; set; } 
    }
    public class Coords { public double Lat { get; set; } public double Lng { get; set; } }
    public class VisitLog 
    { 
        public string VisitUuid { get; set; } 
        public object StoreFullContext { get; set; } 
        public object Timestamps { get; set; } 
        public object ScorecardConsolidated { get; set; } 
        public object ExecutionModulesDetailed { get; set; } 
    }
}
