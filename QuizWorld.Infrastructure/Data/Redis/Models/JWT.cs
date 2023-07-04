using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis.OM.Modeling;


namespace QuizWorld.Infrastructure.Data.Redis.Models
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] {"JWT"})]
    public class JWT
    {
        [RedisField]
        [Indexed]
        public string? Id { get; set; }
        public string? Token { get; set; }
    }
}
