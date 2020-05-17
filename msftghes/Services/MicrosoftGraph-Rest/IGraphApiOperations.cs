using System.Threading.Tasks;

namespace msftghes.Services.GraphOperations
{
    public interface IGraphApiOperations
    {
        Task<dynamic> GetUserInformation(string accessToken);
        Task<string> GetPhotoAsBase64Async(string accessToken);
    }
}