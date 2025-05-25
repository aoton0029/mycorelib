using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Entities
{
    /// <summary>
    /// 値オブジェクトの基底クラス
    /// </summary>
    public abstract class ValueObject
    {
        /// <summary>
        /// 等値比較のための値を取得
        /// </summary>
        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            var other = (ValueObject)obj;

            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }

        public static bool operator ==(ValueObject? left, ValueObject? right)
        {
            return EqualOperator(left, right);
        }

        public static bool operator !=(ValueObject? left, ValueObject? right)
        {
            return !EqualOperator(left, right);
        }

        private static bool EqualOperator(ValueObject? left, ValueObject? right)
        {
            if (left is null ^ right is null)
                return false;

            return left is null || left.Equals(right);
        }
    }

    /// <summary>
    /// 住所を表す値オブジェクト
    /// </summary>
    public class Address : ValueObject
    {
        public string PostalCode { get; }
        public string Prefecture { get; }
        public string City { get; }
        public string Street { get; }
        public string? Building { get; }

        private Address() { }  // EF Core用

        public Address(string postalCode, string prefecture, string city, string street, string? building = null)
        {
            PostalCode = postalCode;
            Prefecture = prefecture;
            City = city;
            Street = street;
            Building = building;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return PostalCode;
            yield return Prefecture;
            yield return City;
            yield return Street;

            if (Building != null)
                yield return Building;
        }

        public override string ToString()
        {
            return $"{PostalCode} {Prefecture}{City}{Street} {Building}".Trim();
        }
    }

    /// <summary>
    /// 貨幣を表す値オブジェクト
    /// </summary>
    public class Money : ValueObject
    {
        public decimal Amount { get; }
        public string Currency { get; }

        private Money() { }  // EF Core用

        public Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public static Money FromYen(decimal amount)
        {
            return new Money(amount, "JPY");
        }

        public static Money FromDollar(decimal amount)
        {
            return new Money(amount, "USD");
        }

        public static Money FromEuro(decimal amount)
        {
            return new Money(amount, "EUR");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        public Money Add(Money other)
        {
            if (other.Currency != Currency)
                throw new InvalidOperationException("Cannot add money with different currencies");

            return new Money(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            if (other.Currency != Currency)
                throw new InvalidOperationException("Cannot subtract money with different currencies");

            return new Money(Amount - other.Amount, Currency);
        }

        public override string ToString()
        {
            return $"{Amount:N2} {Currency}";
        }
    }

    /// <summary>
    /// 日付範囲を表す値オブジェクト
    /// </summary>
    public class DateRange : ValueObject
    {
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }

        private DateRange() { }  // EF Core用

        public DateRange(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("End date must be greater than or equal to start date");

            StartDate = startDate;
            EndDate = endDate;
        }

        public bool Contains(DateTime date)
        {
            return date >= StartDate && date <= EndDate;
        }

        public bool Overlaps(DateRange other)
        {
            return StartDate <= other.EndDate && EndDate >= other.StartDate;
        }

        public int GetDays()
        {
            return (EndDate - StartDate).Days + 1;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StartDate;
            yield return EndDate;
        }

        public override string ToString()
        {
            return $"{StartDate:yyyy/MM/dd} - {EndDate:yyyy/MM/dd}";
        }
    }

    /// <summary>
    /// メールアドレスを表す値オブジェクト
    /// </summary>
    public class EmailAddress : ValueObject
    {
        public string Value { get; }

        private EmailAddress() { }  // EF Core用

        public EmailAddress(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty");

            // 基本的な検証
            if (!email.Contains('@') || !email.Contains('.'))
                throw new ArgumentException("Invalid email format");

            Value = email;
        }

        public string GetDomain()
        {
            return Value.Split('@')[1];
        }

        public string GetLocalPart()
        {
            return Value.Split('@')[0];
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value.ToLowerInvariant();
        }

        public override string ToString()
        {
            return Value;
        }
    }

    /// <summary>
    /// 電話番号を表す値オブジェクト
    /// </summary>
    public class PhoneNumber : ValueObject
    {
        public string Value { get; }

        private PhoneNumber() { }  // EF Core用

        public PhoneNumber(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Phone number cannot be empty");

            // 基本的な検証 - 数字、ハイフン、括弧、+のみ許可
            if (!System.Text.RegularExpressions.Regex.IsMatch(number, @"^[0-9\-\(\)\+\s]+$"))
                throw new ArgumentException("Invalid phone number format");

            Value = number;
        }

        public string GetFormattedNumber()
        {
            // 日本の電話番号フォーマットの例（03-1234-5678など）
            // 実際のフォーマットはビジネスルールに合わせて実装
            return Value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // 比較時は空白、ハイフンなどを無視
            yield return new string(Value.Where(char.IsDigit).ToArray());
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
