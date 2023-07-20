using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using RedLine.Domain;
using Xunit;

namespace RedLine.UnitTests.Domain.MoneyTests
{
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Makes the code more clear.")]
    public class ArithmeticOperatorsShould
    {
        public static IEnumerable<object[]> OperatorData = new List<object[]>
        {
            new object[] { 199.99M, 7.99M },
            new object[] { 199.9955M, 7.3333M },
        };

        [Theory]
        [MemberData(nameof(OperatorData))]
        public void Add(decimal amount1, decimal amount2)
        {
            var expected = new Money(amount1 + amount2);
            var actual = new Money(amount1) + new Money(amount2);

            actual.Equals(expected).Should().BeTrue();

            actual = amount1 + new Money(amount2);
            actual.Equals(expected).Should().BeTrue();

            actual = new Money(amount1) + amount2;
            actual.Equals(expected).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(OperatorData))]
        public void Subtract(decimal amount1, decimal amount2)
        {
            var expected = new Money(amount1 - amount2);
            var actual = new Money(amount1) - new Money(amount2);

            actual.Equals(expected).Should().BeTrue();

            actual = amount1 - new Money(amount2);
            actual.Equals(expected).Should().BeTrue();

            actual = new Money(amount1) - amount2;
            actual.Equals(expected).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(OperatorData))]
        public void Multiply(decimal amount1, decimal amount2)
        {
            var expected = new Money(amount1 * amount2);
            var actual = new Money(amount1) * new Money(amount2);

            actual.Equals(expected).Should().BeTrue();

            actual = amount1 * new Money(amount2);
            actual.Equals(expected).Should().BeTrue();
            actual.CultureName.Should().Be("en-US");

            actual = new Money(amount1) * amount2;
            actual.Equals(expected).Should().BeTrue();
            actual.CultureName.Should().Be("en-US");
        }

        [Theory]
        [MemberData(nameof(OperatorData))]
        public void Divide(decimal amount1, decimal amount2)
        {
            var expected = new Money(amount1 / amount2);
            var actual = new Money(amount1) / new Money(amount2);

            actual.Equals(expected).Should().BeTrue();

            actual = amount1 / new Money(amount2);
            actual.Equals(expected).Should().BeTrue();
            actual.CultureName.Should().Be("en-US");

            actual = new Money(amount1) / amount2;
            actual.Equals(expected).Should().BeTrue();
            actual.CultureName.Should().Be("en-US");
        }

        public static IEnumerable<object[]> HandleDefaultAndZeroData = new List<object[]>
        {
            new object[] { 12.345M, default(Money) },
            new object[] { 12.345M, Money.Zero },
            new object[] { 56.789M, default(Money) },
            new object[] { 56.789M, Money.Zero },
        };

        [Theory]
        [MemberData(nameof(HandleDefaultAndZeroData))]
        public void AddWithZero(decimal amount, Money zero)
        {
            var money = new Money(amount);

            (money + zero).Amount.Should().Be(amount);
            (zero + money).Amount.Should().Be(amount);
            (amount + zero).Amount.Should().Be(amount);
            (zero + amount).Amount.Should().Be(amount);

            Money.Add(money, zero).Amount.Should().Be(amount);
            Money.Add(zero, money).Amount.Should().Be(amount);
            Money.Add(amount, zero).Amount.Should().Be(amount);
            Money.Add(zero, amount).Amount.Should().Be(amount);
        }

        [Theory]
        [MemberData(nameof(HandleDefaultAndZeroData))]
        public void SubtractWithZero(decimal amount, Money zero)
        {
            var money = new Money(amount);

            (money - zero).Amount.Should().Be(amount);
            (zero - money).Amount.Should().Be(-amount);
            (amount - zero).Amount.Should().Be(amount);
            (zero - amount).Amount.Should().Be(-amount);

            Money.Subtract(money, zero).Amount.Should().Be(amount);
            Money.Subtract(zero, money).Amount.Should().Be(-amount);
            Money.Subtract(amount, zero).Amount.Should().Be(amount);
            Money.Subtract(zero, amount).Amount.Should().Be(-amount);
        }

        [Theory]
        [MemberData(nameof(HandleDefaultAndZeroData))]
        public void MultiplyWithZero(decimal amount, Money zero)
        {
            var money = new Money(amount);

            (money * zero).Amount.Should().Be(0);
            (zero * money).Amount.Should().Be(0);
            (amount * zero).Amount.Should().Be(0);
            (zero * amount).Amount.Should().Be(0);

            Money.Multiply(money, zero).Amount.Should().Be(0);
            Money.Multiply(zero, money).Amount.Should().Be(0);
            Money.Multiply(amount, zero).Amount.Should().Be(0);
            Money.Multiply(zero, amount).Amount.Should().Be(0);
        }

        [Theory]
        [MemberData(nameof(HandleDefaultAndZeroData))]
        public void DivideWithZero(decimal amount, Money zero)
        {
            var money = new Money(amount);

            var action = () => money / zero;
            action.Should().Throw<DivideByZeroException>();
            (zero / money).Amount.Should().Be(0);
            action = () => amount / zero;
            action.Should().Throw<DivideByZeroException>();
            (zero / amount).Amount.Should().Be(0);

            action = () => Money.Divide(money, zero);
            action.Should().Throw<DivideByZeroException>();
            Money.Divide(zero, money).Amount.Should().Be(0);
            action = () => Money.Divide(amount, zero);
            action.Should().Throw<DivideByZeroException>();
            Money.Divide(zero, amount).Amount.Should().Be(0);
        }

        [Fact]
        public void ThrowWhenCultureIsDifferent()
        {
            var money1 = new Money(100, "en-US");
            var money2 = new Money(10, "en-CA");

            // Add
            Func<Money> action = () => money1 + money2;
            action.Should().Throw<InvalidOperationException>().And.Message.Contains("must use the same culture");

            // Subtract
            action = () => money1 - money2;
            action.Should().Throw<InvalidOperationException>().And.Message.Contains("must use the same culture");

            // Multiply
            action = () => money1 * money2;
            action.Should().Throw<InvalidOperationException>().And.Message.Contains("must use the same culture");

            // Divide
            action = () => money1 / money2;
            action.Should().Throw<InvalidOperationException>().And.Message.Contains("must use the same culture");
        }
    }
}
