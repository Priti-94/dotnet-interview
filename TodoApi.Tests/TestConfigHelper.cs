using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace TodoApi.Tests
{
    public static class TestConfigHelper
    {
        public static IConfiguration GetInMemoryConfig()
        {
            var dict = new Dictionary<string, string>
        {
            { "ConnectionStrings:TodoDb", "Data Source=:memory:" }
        };
            return new ConfigurationBuilder()
                .AddInMemoryCollection(dict)
                .Build();
        }

    }
}
