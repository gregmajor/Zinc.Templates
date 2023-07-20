using System;
using System.Globalization;

namespace RedLine.Domain;

/// <summary>
/// A culture-aware class that represents money.
/// </summary>
/// <remarks>Note that this type does NOT convert currency between cultures.</remarks>
public readonly struct Money : IEquatable<Money>
{
    /// <summary>
    /// Gets a Money with zero amount.
    /// </summary>
    /// <returns>The zero amount in current culture.</returns>
    public static readonly Money Zero = default;

    /// <summary>
    /// The money's culture name.
    /// </summary>
    private readonly string cultureName;

    /// <summary>
    /// The culture information providing context for the money.
    /// </summary>
    private readonly CultureInfo cultureInfo;

    /// <summary>
    /// The region information providing context for the money.
    /// </summary>
    private readonly RegionInfo regionInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="Money"/> class.
    /// </summary>
    /// <param name="amount">The money amount.</param>
    public Money(decimal amount)
        : this(amount, CultureInfo.CurrentCulture.Name)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Money"/> class.
    /// </summary>
    /// <param name="amount">The money amount.</param>
    /// <param name="cultureInfo">The money culture.</param>
    public Money(decimal amount, CultureInfo cultureInfo)
        : this()
    {
        this.Amount = amount;
        this.cultureInfo = cultureInfo ?? throw new ArgumentNullException(nameof(cultureInfo));
        this.cultureName = cultureInfo.Name;
        this.regionInfo = new RegionInfo(CultureInfo.Name);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Money" /> class.
    /// </summary>
    /// <param name="amount">The money amount.</param>
    /// <param name="cultureName">The money culture name, which must be installed on the system.</param>
    [Newtonsoft.Json.JsonConstructor]
    [System.Text.Json.Serialization.JsonConstructor]
    public Money(decimal amount, string cultureName)
        : this()
    {
        if (string.IsNullOrWhiteSpace(cultureName))
        {
            throw new ArgumentException($"The '{nameof(cultureName)}' argument is required.", nameof(cultureName));
        }

        if (!Cultures.IsValidCultureName(cultureName))
        {
            throw new CultureNotFoundException($"The '{cultureName}' culture was not found.");
        }

        this.Amount = amount;
        this.cultureName = cultureName;
        this.cultureInfo = Cultures.GetCulture(cultureName);
        this.regionInfo = new RegionInfo(CultureInfo.Name);
    }

    /// <summary>
    /// Gets the money amount.
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// Gets the money's culture name.
    /// </summary>
    public string CultureName => cultureName ?? CultureInfo.CurrentCulture.Name;

    /// <summary>
    /// Gets the money currency symbol.
    /// </summary>
    public string CurrencySymbol => CultureInfo.NumberFormat.CurrencySymbol;

    /// <summary>
    /// Gets the money ISO currency code.
    /// </summary>
    public string ISOCurrencyCode => RegionInfo.ISOCurrencySymbol;

    /// <summary>
    /// Gets the culture info for the money's culture.
    /// </summary>
    private CultureInfo CultureInfo => cultureInfo ?? Cultures.GetCulture(CultureName);

    /// <summary>
    /// Gets the region info for the culture.
    /// </summary>
    private RegionInfo RegionInfo => regionInfo ?? new RegionInfo(CultureInfo.Name);

    /// <inheritdoc cref="Equals(Money,Money)"/>
    public static bool operator ==(Money left, Money right) => Equals(left, right);

    /// <summary>
    /// Returns a value indicating whether two instance of <see cref="Money"/> have different values.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns><c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Money left, Money right) => !Equals(left, right);

    /// <inheritdoc cref="GreaterThanOrEqual"/>
    public static bool operator >(Money left, Money right) => GreaterThanOrEqual(left, right);

    /// <inheritdoc cref="GreaterThan"/>
    public static bool operator >=(Money left, Money right) => GreaterThan(left, right);

    /// <inheritdoc cref="LessThan"/>
    public static bool operator <(Money left, Money right) => LessThan(left, right);

    /// <inheritdoc cref="LessThanOrEqual"/>
    public static bool operator <=(Money left, Money right) => LessThanOrEqual(left, right);

    /// <inheritdoc cref="Add(Money,Money)"/>
    public static Money operator +(Money left, Money right) => Add(left, right);

    /// <inheritdoc cref="Add(Money,decimal)"/>
    public static Money operator +(Money left, decimal right) => Add(left, right);

    /// <inheritdoc cref="Add(decimal,Money)"/>
    public static Money operator +(decimal left, Money right) => Add(left, right);

    /// <inheritdoc cref="Subtract(Money,Money)"/>
    public static Money operator -(Money left, Money right) => Subtract(left, right);

    /// <inheritdoc cref="Subtract(Money,decimal)"/>
    public static Money operator -(Money left, decimal right) => Subtract(left, right);

    /// <inheritdoc cref="Subtract(decimal,Money)"/>
    public static Money operator -(decimal left, Money right) => Subtract(left, right);

    /// <inheritdoc cref="Multiply(Money,Money)"/>
    public static Money operator *(Money left, Money right) => Multiply(left, right);

    /// <inheritdoc cref="Multiply(Money,decimal)"/>
    public static Money operator *(Money left, decimal right) => Multiply(left, right);

    /// <inheritdoc cref="Multiply(decimal,Money)"/>
    public static Money operator *(decimal left, Money right) => Multiply(left, right);

    /// <inheritdoc cref="Divide(Money,Money)"/>
    public static Money operator /(Money left, Money right) => Divide(left, right);

    /// <inheritdoc cref="Divide(Money,decimal)"/>
    public static Money operator /(Money left, decimal right) => Divide(left, right);

    /// <inheritdoc cref="Divide(decimal,Money)"/>
    public static Money operator /(decimal left, Money right) => Divide(left, right);

    /// <summary>
    /// Returns a value indicating whether two instance of <see cref="Money"/> represent the same value.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns><c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
    public static bool Equals(Money left, Money right)
    {
        // if the amount is 0, don't compare the currency code.
        return left.Amount.Equals(right.Amount) &&
               ((left.Amount == 0 && right.Amount == 0)
                || left.ISOCurrencyCode.Equals(right.ISOCurrencyCode, StringComparison.Ordinal));
    }

    /// <summary>
    /// Returns a value indicating whether a specified <see cref="Money"/> is less than another specified <see cref="Money"/>.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <exception cref="InvalidOperationException"><paramref name="left"/> and <paramref name="right"/> do not use the same culture.</exception>
    /// <returns><c>true</c> if <paramref name="left"/> is less than <paramref name="right"/>; otherwise, false.</returns>
    public static bool GreaterThan(Money left, Money right)
    {
        if (left.Amount != 0 && right.Amount != 0 && !left.ISOCurrencyCode.Equals(right.ISOCurrencyCode))
        {
            throw new InvalidOperationException("Money objects must have the same currency code.");
        }

        return left.Amount > right.Amount;
    }

    /// <summary>
    /// Returns a value indicating whether a specified <see cref="Money"/> is less than or equal to another specified <see cref="Money"/>.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <exception cref="InvalidOperationException"><paramref name="left"/> and <paramref name="right"/> do not use the same culture.</exception>
    /// <returns><c>true</c> if <paramref name="left"/> is less than or equal to <paramref name="right"/>; otherwise, false.</returns>
    public static bool GreaterThanOrEqual(Money left, Money right)
    {
        if (left.Amount != 0 && right.Amount != 0 && !left.ISOCurrencyCode.Equals(right.ISOCurrencyCode))
        {
            throw new InvalidOperationException("Money objects must have the same currency code.");
        }

        return left.Amount >= right.Amount;
    }

    /// <summary>
    /// Returns a value indicating whether a specified <see cref="Money"/> is greater than another specified <see cref="Money"/>.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <exception cref="InvalidOperationException"><paramref name="left"/> and <paramref name="right"/> do not use the same culture.</exception>
    /// <returns><c>true</c> if <paramref name="left"/> is greater than <paramref name="right"/>; otherwise, false.</returns>
    public static bool LessThan(Money left, Money right)
    {
        if (left.Amount != 0 && right.Amount != 0 && !left.ISOCurrencyCode.Equals(right.ISOCurrencyCode))
        {
            throw new InvalidOperationException("Money objects must have the same currency code.");
        }

        return left.Amount < right.Amount;
    }

    /// <summary>
    /// Returns a value indicating whether a specified <see cref="Money"/> is greater than or equal to another specified <see cref="Money"/>.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <exception cref="InvalidOperationException"><paramref name="left"/> and <paramref name="right"/> do not use the same culture.</exception>
    /// <returns><c>true</c> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>; otherwise, false.</returns>
    public static bool LessThanOrEqual(Money left, Money right)
    {
        if (left.Amount != 0 && right.Amount != 0 && !left.ISOCurrencyCode.Equals(right.ISOCurrencyCode))
        {
            throw new InvalidOperationException("Money objects must have the same currency code.");
        }

        return left.Amount <= right.Amount;
    }

    /// <summary>
    /// Adds two Money objects.
    /// </summary>
    /// <param name="left">The left side of the operation.</param>
    /// <param name="right">The right side of the operation.</param>
    /// <exception cref="InvalidOperationException"><paramref name="left"/> and <paramref name="right"/> do not use the same culture.</exception>
    /// <exception cref="OverflowException">The result of the operation is less than Decimal.MinValue or greater than Decimal.MaxValue.</exception>
    /// <returns>The result of the operator.</returns>
    public static Money Add(Money left, Money right)
    {
        var amount = left.Amount + right.Amount;

        if (left.Amount == 0)
        {
            return new Money(amount, right.CultureInfo);
        }

        if (right.Amount != 0 && !left.ISOCurrencyCode.Equals(right.ISOCurrencyCode))
        {
            throw new InvalidOperationException("Money objects must have the same currency code.");
        }

        return new Money(amount, left.CultureInfo);
    }

    /// <summary>
    /// Adds a Money and a decimal.
    /// </summary>
    /// <param name="left">The left side of the operation.</param>
    /// <param name="right">The right side of the operation.</param>
    /// <exception cref="InvalidOperationException"><paramref name="left"/> and <paramref name="right"/> do not use the same culture.</exception>
    /// <exception cref="OverflowException">The result of the operation is less than Decimal.MinValue or greater than Decimal.MaxValue.</exception>
    /// <returns>The result of the operator.</returns>
    public static Money Add(Money left, decimal right) => new(left.Amount + right, left.CultureInfo);

    /// <summary>
    /// Adds a Money and a decimal.
    /// </summary>
    /// <param name="left">The left side of the operation.</param>
    /// <param name="right">The right side of the operation.</param>
    /// <exception cref="OverflowException">The result of the operation is less than Decimal.MinValue or greater than Decimal.MaxValue.</exception>
    /// <returns>The result of the operator.</returns>
    public static Money Add(decimal left, Money right) => new(left + right.Amount, right.CultureInfo);

    /// <summary>
    /// Subtracts two Money objects.
    /// </summary>
    /// <param name="left">The left side of the operation.</param>
    /// <param name="right">The right side of the operation.</param>
    /// <exception cref="InvalidOperationException"><paramref name="left"/> and <paramref name="right"/> do not use the same culture.</exception>
    /// <exception cref="OverflowException">The result of the operation is less than Decimal.MinValue or greater than Decimal.MaxValue.</exception>
    /// <returns>The result of the operator.</returns>
    public static Money Subtract(Money left, Money right)
    {
        var amount = left.Amount - right.Amount;

        if (left.Amount == 0)
        {
            return new Money(amount, right.CultureInfo);
        }

        if (right.Amount != 0 && !left.ISOCurrencyCode.Equals(right.ISOCurrencyCode))
        {
            throw new InvalidOperationException("Money objects must have the same currency code.");
        }

        return new Money(amount, left.CultureInfo);
    }

    /// <summary>
    /// Subtracts a Money and a decimal.
    /// </summary>
    /// <param name="left">The left side of the operation.</param>
    /// <param name="right">The right side of the operation.</param>
    /// <exception cref="InvalidOperationException"><paramref name="left"/> and <paramref name="right"/> do not use the same culture.</exception>
    /// <exception cref="OverflowException">The result of the operation is less than Decimal.MinValue or greater than Decimal.MaxValue.</exception>
    /// <returns>The result of the operator.</returns>
    public static Money Subtract(Money left, decimal right) => new(left.Amount - right, left.CultureInfo);

    /// <summary>
    /// Subtracts a Money and a decimal.
    /// </summary>
    /// <param name="left">The left side of the operation.</param>
    /// <param name="right">The right side of the operation.</param>
    /// <exception cref="OverflowException">The result of the operation is less than Decimal.MinValue or greater than Decimal.MaxValue.</exception>
    /// <returns>The result of the operator.</returns>
    public static Money Subtract(decimal left, Money right) => new(left - right.Amount, right.CultureInfo);

    /// <summary>
    /// Multiplies two Money objects.
    /// </summary>
    /// <param name="left">The left side of the operation.</param>
    /// <param name="right">The right side of the operation.</param>
    /// <exception cref="InvalidOperationException"><paramref name="left"/> and <paramref name="right"/> do not use the same culture.</exception>
    /// <exception cref="OverflowException">The result of the operation is less than Decimal.MinValue or greater than Decimal.MaxValue.</exception>
    /// <returns>The result of the operator.</returns>
    public static Money Multiply(Money left, Money right)
    {
        var amount = left.Amount * right.Amount;

        if (left.Amount == 0)
        {
            return new Money(amount, right.CultureInfo);
        }

        if (right.Amount != 0 && !left.ISOCurrencyCode.Equals(right.ISOCurrencyCode))
        {
            throw new InvalidOperationException("Money objects must have the same currency code.");
        }

        return new Money(amount, left.CultureInfo);
    }

    /// <summary>
    /// Multiplies a Money and a decimal.
    /// </summary>
    /// <param name="left">The left side of the operation.</param>
    /// <param name="right">The right side of the operation.</param>
    /// <exception cref="InvalidOperationException"><paramref name="left"/> and <paramref name="right"/> do not use the same culture.</exception>
    /// <exception cref="OverflowException">The result of the operation is less than Decimal.MinValue or greater than Decimal.MaxValue.</exception>
    /// <returns>The result of the operator.</returns>
    public static Money Multiply(Money left, decimal right) => new(left.Amount * right, left.CultureInfo);

    /// <summary>
    /// Multiplies a Money and a decimal.
    /// </summary>
    /// <param name="left">The left side of the operation.</param>
    /// <param name="right">The right side of the operation.</param>
    /// <exception cref="OverflowException">The result of the operation is less than Decimal.MinValue or greater than Decimal.MaxValue.</exception>
    /// <returns>The result of the operator.</returns>
    public static Money Multiply(decimal left, Money right) => new(left * right.Amount, right.CultureInfo);

    /// <summary>
    /// Divides two Money objects.
    /// </summary>
    /// <param name="left">The left side of the operation.</param>
    /// <param name="right">The right side of the operation.</param>
    /// <exception cref="InvalidOperationException"><paramref name="left"/> and <paramref name="right"/> do not use the same culture.</exception>
    /// <exception cref="DivideByZeroException"><paramref name="right"/> is zero.</exception>
    /// <exception cref="OverflowException">The result of the operation is less than Decimal.MinValue or greater than Decimal.MaxValue.</exception>
    /// <returns>The result of the operator.</returns>
    public static Money Divide(Money left, Money right)
    {
        var amount = left.Amount / right.Amount;

        if (left.Amount == 0)
        {
            return new Money(amount, right.CultureInfo);
        }

        if (right.Amount != 0 && !left.ISOCurrencyCode.Equals(right.ISOCurrencyCode))
        {
            throw new InvalidOperationException("Money objects must have the same currency code.");
        }

        return new Money(amount, left.CultureInfo);
    }

    /// <summary>
    /// Divides a Money and a decimal.
    /// </summary>
    /// <param name="left">The left side of the operation.</param>
    /// <param name="right">The right side of the operation.</param>
    /// <exception cref="InvalidOperationException"><paramref name="left"/> and <paramref name="right"/> do not use the same culture.</exception>
    /// <exception cref="DivideByZeroException"><paramref name="right"/> is zero.</exception>
    /// <exception cref="OverflowException">The result of the operation is less than Decimal.MinValue or greater than Decimal.MaxValue.</exception>
    /// <returns>The result of the operator.</returns>
    public static Money Divide(Money left, decimal right) => new(left.Amount / right, left.CultureInfo);

    /// <summary>
    /// Divides a Money and a decimal.
    /// </summary>
    /// <param name="left">The left side of the operation.</param>
    /// <param name="right">The right side of the operation.</param>
    /// <exception cref="DivideByZeroException"><paramref name="right"/> is zero.</exception>
    /// <exception cref="OverflowException">The result of the operation is less than Decimal.MinValue or greater than Decimal.MaxValue.</exception>
    /// <returns>The result of the operator.</returns>
    public static Money Divide(decimal left, Money right) => new(left / right.Amount, right.CultureInfo);

    /// <summary>
    /// Returns a value indicating whether two instance of <see cref="Money"/> represent the same value.
    /// </summary>
    /// <param name="obj">The object to compare with this instance.</param>
    /// <returns><c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj) => obj is Money other && Equals(this, other);

    /// <summary>
    /// Returns a value indicating whether two instance of <see cref="Money"/> represent the same value.
    /// </summary>
    /// <param name="other">The object to compare with this instance.</param>
    /// <returns><c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
    public bool Equals(Money other) => Equals(this, other);

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data
    /// structures like a hash table.
    /// </returns>
    public override int GetHashCode() => Amount.GetHashCode() ^ ISOCurrencyCode.GetHashCode();

    /// <summary>
    /// Returns a culture aware <see cref="string"/> that represents this instance, minus the currency symbol.
    /// </summary>
    /// <returns>A culture aware <see cref="string"/> that represents this instance, minus the currency symbol.</returns>
    public override string ToString()
    {
        /*
         * We want to respect the set culture, but without a currency symbol so we clone the
         * NFI and drop the symbol. A trick from Jon Skeet.
         */

        var numberFormatInfo = CultureInfo.NumberFormat;

        numberFormatInfo = (NumberFormatInfo)numberFormatInfo.Clone();

        numberFormatInfo.CurrencySymbol = string.Empty;

        return string
            .Format(numberFormatInfo, "{0:c}", Amount)
            .Trim(); // Trim() because some currency symbols are at the end
    }

    /// <summary>
    /// Returns a culture aware <see cref="string"/> that represents this instance with the currency
    /// symbol included.
    /// </summary>
    /// <returns>
    /// A culture aware <see cref="string"/> that represents this instance with the currency symbol included.
    /// </returns>
    public string ToStringWithCurrencySymbol() => string.Format(CultureInfo, "{0:C}", Amount);
}
