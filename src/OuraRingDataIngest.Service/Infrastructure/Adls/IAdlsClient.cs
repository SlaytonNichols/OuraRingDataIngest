using System.Threading.Tasks;

namespace OuraRingDataIngest.Service.Infrastructure.Adls;
public interface IAdlsClient
{
    Task WriteJsonToAdls(string json);
}