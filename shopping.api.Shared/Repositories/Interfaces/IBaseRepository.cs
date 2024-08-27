using System.Data;

namespace shopping.api.Shared.Repositories
{
    public interface IBaseRepository
    {
        IDbConnection GetDbConnection();
    }
}