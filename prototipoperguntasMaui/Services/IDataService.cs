using System.Collections.Generic;
using System.Threading.Tasks;

namespace prototipoperguntasMaui.Services
{
    public interface IDataService
    {
        Task<RootData> GetDataAsync();
    }
}
