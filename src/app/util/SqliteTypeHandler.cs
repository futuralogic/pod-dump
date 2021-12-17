using Dapper;
using System.Data;

namespace futura.Util;

abstract class SqliteTypeHandler<T> : SqlMapper.TypeHandler<T>
{
	// Parameters are converted by Microsoft.Data.Sqlite
	public override void SetValue(IDbDataParameter parameter, T value)
			=> parameter.Value = value;
}