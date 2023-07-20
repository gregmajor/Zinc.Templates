using System.Data;
using RedLine.Domain;

namespace RedLine.Data.TypeHandlers
{
    /// <summary>
    /// A custom <see cref="Dapper.SqlMapper.ITypeHandler"/> for <see cref="Money"/>.
    /// This is just a pass-through for Dapper. The actual type mapping is handled in Npgsql.
    /// </summary>
    internal class MoneyTypeHandler : Dapper.SqlMapper.TypeHandler<Domain.Money>
    {
        /// <inheritdoc />
        public override void SetValue(IDbDataParameter parameter, Domain.Money value)
        {
            parameter.DbType = DbType.Object;
            parameter.Value = value;
        }

        /// <inheritdoc />
        public override Domain.Money Parse(object value)
        {
            return (Domain.Money)value;
        }
    }
}
