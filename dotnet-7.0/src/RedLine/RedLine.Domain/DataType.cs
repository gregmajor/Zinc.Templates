using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using RedLine.Domain.Services;

namespace RedLine.Domain;

/// <summary>
/// A structure representing data type.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1303:Const field names should begin with upper-case letter", Scope = "type", Justification = "I can't capitalize everything.")]
[JsonConverter(typeof(DataTypeConverter))]
public readonly struct DataType : IEquatable<DataType>
{
    /// <summary>
    /// 'boolean' data type.
    /// </summary>
    public static readonly DataType Boolean = new(boolean);

    /// <summary>
    /// 'date' data type.
    /// </summary>
    public static readonly DataType Date = new(date);

    /// <summary>
    /// 'decimal' data type.
    /// </summary>
    public static readonly DataType Decimal = new(@decimal);

    /// <summary>
    /// 'integer' data type.
    /// </summary>
    public static readonly DataType Integer = new(integer);

    /// <summary>
    /// 'string' data type.
    /// </summary>
    public static readonly DataType String = new(@string);

    // const for switch statements/expressions
    private const string boolean = "boolean";
    private const string date = "date";
    private const string @decimal = "decimal";
    private const string integer = "integer";
    private const string @string = "string";

    private readonly string typeName;

    private DataType(string value)
    {
        if (new[]
            {
                boolean,
                date,
                @decimal,
                integer,
                @string,
            }
            .All(i => i != value))
        {
            throw new ArgumentException($"{value} is not a valid type.", nameof(value));
        }

        this.typeName = value;
    }

    /// <summary>
    /// Implicit operator for string conversion.
    /// </summary>
    /// <param name="dataType">The 'JsonType' being operated on.</param>
    public static implicit operator string(DataType dataType) => dataType.typeName;

    /// <summary>
    /// Implicit operator for string conversion.
    /// </summary>
    /// <param name="value">The value to parse.</param>
    public static implicit operator DataType(string value) => Parse(value);

    /// <summary>
    /// Converts the string representation of the JsonType to the enumeration struct.
    /// </summary>
    /// <param name="value">The value to parse.</param>
    /// <returns>The matching JsonType.</returns>
    /// <exception cref="ArgumentException">The passed value doesn't match a defined JsonType.</exception>
    public static DataType Parse(string value)
    {
        if (TryParse(value, out var dataType))
        {
            return dataType;
        }

        throw new ArgumentException($"{value} is outside the range of values for JsonType.");
    }

    /// <summary>
    /// Converts the string representation of the JsonType to the enumeration struct.
    /// </summary>
    /// <param name="value">The value to parse.</param>
    /// <param name="dataType">The parsed DataType.</param>
    /// <returns><c>true</c>if value was converted successfully; otherwise, <c>false</c>.</returns>
    public static bool TryParse(string value, out DataType dataType)
    {
        var match = new DataType?[]
            {
                Boolean,
                Date,
                Decimal,
                Integer,
                String,
            }
            .FirstOrDefault(i => i.Value.typeName == value);

        if (match != null)
        {
            dataType = match.Value;
            return true;
        }

        dataType = default;
        return false;
    }

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is DataType dataType && Equals(dataType);

    /// <summary>
    /// Indicates whether this instance the specified <see cref="DataType"/> are equal.
    /// </summary>
    /// <param name="other">The 'JsonType' to compare with the current instance.</param>
    /// <returns>[true], if `jsonType` and this object represent the same value.</returns>
    public bool Equals(DataType other) => typeName == other.typeName;

    /// <inheritdoc />
    public override int GetHashCode() => typeName.GetHashCode();

    /// <inheritdoc />
    public override string ToString() => typeName;

    /// <summary>
    /// Indicates if the type is a primitive type, versus a complex type.
    /// </summary>
    /// <returns>[true], if this object represents a primitive type.</returns>
    public bool IsValidType() => typeName != null;

    /// <summary>
    /// Indicates if the type can represent an CLR type.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>[true], if the JSON type and the CLR type are compatible.</returns>
    public bool CanRepresentValue(object value)
    {
        if (value == null)
        {
            return true;
        }

        return this.typeName switch
        {
            boolean => value is bool,
            date => value is DateTimeOffset or DateOnly,
            @decimal => value is decimal or float or double or long or int or short or byte,
            integer => value is long or int or short or byte,
            @string => value is string,
            _ => false,
        };
    }

    /// <summary>
    /// Serialize a value to a string.
    /// </summary>
    /// <param name="value">The value to serialize.</param>
    /// <returns>The serialized representation of the value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if value can not be serialized by this DataType.</exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1301:\"switch\" statements should have at least 3 \"case\" clauses", Justification = "No, tha")]
    public string SerializeValue(object value)
    {
        if (!CanRepresentValue(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), $"Unable to serialize type {value.GetType().Name}.");
        }

        if (value == null)
        {
            return null;
        }

        return this.typeName switch
        {
            boolean => value switch
            {
                bool v => v ? "true" : "false",
                _ => throw new ArgumentOutOfRangeException(nameof(value), $"Unable to serialize type {value.GetType().Name}."),
            },
            date => value switch
            {
                DateTimeOffset v => v.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture),
                DateOnly v => v.ToString("o", CultureInfo.InvariantCulture),
                _ => throw new ArgumentOutOfRangeException(nameof(value), $"Unable to serialize type {value.GetType().Name}."),
            },
            @decimal => value switch
            {
                decimal v => v.ToString(CultureInfo.InvariantCulture),
                float v => v.ToString(CultureInfo.InvariantCulture),
                double v => v.ToString(CultureInfo.InvariantCulture),
                long v => v.ToString(),
                int v => v.ToString(),
                short v => v.ToString(),
                byte v => v.ToString(),
                _ => throw new ArgumentOutOfRangeException(nameof(value), $"Unable to serialize type {value.GetType().Name}."),
            },
            integer => value switch
            {
                long v => v.ToString(),
                int v => v.ToString(),
                short v => v.ToString(),
                byte v => v.ToString(),
                _ => throw new ArgumentOutOfRangeException(nameof(value), $"Unable to serialize type {value.GetType().Name}."),
            },
            @string => value switch
            {
                string v => v,
                _ => throw new ArgumentOutOfRangeException(nameof(value), $"Unable to serialize type {value.GetType().Name}."),
            },
            _ => throw new InvalidOperationException($"{typeName} is not a valid type."),
        };
    }

    /// <summary>
    /// Serialize a string to a value.
    /// </summary>
    /// <param name="value">The string to deserialize.</param>
    /// <returns>The deserialized value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if string can not be deserialized by this DataType.</exception>
    public object DeserializeValue(string value)
    {
        if (value == null)
        {
            return null;
        }

        switch (this.typeName)
        {
            case boolean:
                switch (value.ToLower(CultureInfo.InvariantCulture))
                {
                    case "true":
                        return true;
                    case "false":
                        return false;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), $"Unable to deserialize {value} to type {value.GetType().Name}.");
                }

            case date:
            {
                if (DateTimeOffset.TryParseExact(value, "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var result))
                {
                    return result;
                }

                // For some reason DateTimeStyles.RoundtripKind doesn't match here.
                if (DateOnly.TryParseExact(value, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result2))
                {
                    return result2;
                }

                throw new ArgumentOutOfRangeException(nameof(value), $"Unable to deserialize {value} to type {value.GetType().Name}.");
            }

            case @decimal:
            {
                if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                {
                    return result;
                }

                throw new ArgumentOutOfRangeException(nameof(value), $"Unable to deserialize {value} to type {value.GetType().Name}.");
            }

            case integer:
            {
                if (long.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                {
                    return result;
                }

                throw new ArgumentOutOfRangeException(nameof(value), $"Unable to deserialize {value} to type {value.GetType().Name}.");
            }

            case @string:
                return value;

            default:
                throw new ArgumentOutOfRangeException(nameof(value), $"Unable to deserialize {value} to type {value.GetType().Name}.");
        }
    }
}
