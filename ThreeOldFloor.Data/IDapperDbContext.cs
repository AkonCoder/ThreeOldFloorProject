using System;
using System.Data;

namespace ThreeOldFloor.Data
{
    public interface IDapperDbContext : IDisposable
    {
        IDbConnection Connection { get; }
    }
}
