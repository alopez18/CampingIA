using System.Data;

namespace CampingAI.Infra.Configuration.Factories.Interfaces {
    public interface ISqlConnectionFactory {
        IDbConnection CreateConnection();
    }
}
