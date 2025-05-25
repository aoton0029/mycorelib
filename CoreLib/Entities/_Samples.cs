using CoreLib.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Entities
{
    /// <summary>
    /// ユーザーエンティティ
    /// </summary>
    public class User : FullFeaturedEntityBase<Guid>
    {
        public string Username { get; private set; }
        public string PasswordHash { get; private set; }
        public string Email { get; private set; }
        public string? FirstName { get; private set; }
        public string? LastName { get; private set; }
        public UserRole Role { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public int LoginAttempts { get; private set; }
        public bool IsLocked { get; private set; }
        public DateTime? LockoutEnd { get; private set; }
        public virtual ICollection<UserPermission> Permissions { get; private set; }

        // EF Coreのためのプライベートコンストラクタ
        private User()
        {
            Username = string.Empty;
            PasswordHash = string.Empty;
            Email = string.Empty;
            Permissions = new List<UserPermission>();
        }

        public User(string username, string passwordHash, string email, UserRole role = UserRole.User)
        {
            Username = username;
            PasswordHash = passwordHash;
            Email = email;
            Role = role;
            Permissions = new List<UserPermission>();
        }

        // ビジネスルールを実装するメソッド
        public void UpdateProfile(string? firstName, string? lastName)
        {
            FirstName = firstName;
            LastName = lastName;
            this.MarkAsUpdated();
        }

        public void ChangeEmail(string newEmail)
        {
            Email = newEmail;
            this.MarkAsUpdated();
        }

        public void RecordLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            LoginAttempts = 0;
            IsLocked = false;
            LockoutEnd = null;
            this.MarkAsUpdated();
        }

        public void RecordFailedLoginAttempt(int maxAttempts, int lockoutMinutes)
        {
            LoginAttempts++;

            if (LoginAttempts >= maxAttempts)
            {
                IsLocked = true;
                LockoutEnd = DateTime.UtcNow.AddMinutes(lockoutMinutes);
            }

            this.MarkAsUpdated();
        }

        public bool CanLogin()
        {
            if (!IsLocked) return true;

            if (LockoutEnd.HasValue && LockoutEnd.Value <= DateTime.UtcNow)
            {
                IsLocked = false;
                LockoutEnd = null;
                return true;
            }

            return false;
        }

        public void ChangeRole(UserRole newRole)
        {
            Role = newRole;
            this.MarkAsUpdated();
        }

        public void AddPermission(string permissionName, string grantedBy)
        {
            var permission = new UserPermission(Id, permissionName, grantedBy);
            Permissions.Add(permission);
        }

        public void RemovePermission(string permissionName)
        {
            var permission = Permissions.FirstOrDefault(p => p.PermissionName == permissionName);
            if (permission != null)
            {
                Permissions.Remove(permission);
            }
        }
    }

    /// <summary>
    /// ユーザー権限エンティティ
    /// </summary>
    public class UserPermission : GuidEntityBase
    {
        public Guid UserId { get; private set; }
        public string PermissionName { get; private set; }
        public virtual User User { get; private set; } = null!;

        // EF Coreのためのプライベートコンストラクタ
        private UserPermission()
        {
            PermissionName = string.Empty;
        }

        public UserPermission(Guid userId, string permissionName, string grantedBy)
        {
            UserId = userId;
            PermissionName = permissionName;
            CreatedBy = grantedBy;
        }
    }

    /// <summary>
    /// 顧客エンティティ
    /// </summary>
    public class Customer : GuidEntityBase
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public EmailAddress? Email { get; private set; }
        public PhoneNumber? Phone { get; private set; }
        public Address? Address { get; private set; }
        public virtual ICollection<Order> Orders { get; private set; }

        // EF Coreのためのプライベートコンストラクタ
        private Customer()
        {
            Code = string.Empty;
            Name = string.Empty;
            Orders = new List<Order>();
        }

        public Customer(string code, string name)
        {
            Code = code;
            Name = name;
            Orders = new List<Order>();
        }

        public void UpdateContactInfo(EmailAddress? email, PhoneNumber? phone)
        {
            Email = email;
            Phone = phone;
            this.MarkAsUpdated();
        }

        public void UpdateAddress(Address address)
        {
            Address = address;
            this.MarkAsUpdated();
        }
    }

    /// <summary>
    /// 注文エンティティ
    /// </summary>
    public class Order : GuidEntityBase
    {
        public string OrderNumber { get; private set; }
        public Guid CustomerId { get; private set; }
        public DateTime OrderDate { get; private set; }
        public OrderStatus Status { get; private set; }
        public Money TotalAmount { get; private set; }
        public virtual Customer Customer { get; private set; } = null!;
        public virtual ICollection<OrderItem> Items { get; private set; }

        // EF Coreのためのプライベートコンストラクタ
        private Order()
        {
            OrderNumber = string.Empty;
            TotalAmount = Money.FromYen(0);
            Items = new List<OrderItem>();
        }

        public Order(string orderNumber, Guid customerId, DateTime orderDate)
        {
            OrderNumber = orderNumber;
            CustomerId = customerId;
            OrderDate = orderDate;
            Status = OrderStatus.New;
            TotalAmount = Money.FromYen(0);
            Items = new List<OrderItem>();
        }

        public void AddItem(Guid productId, string productName, int quantity, Money unitPrice)
        {
            var item = new OrderItem(Id, productId, productName, quantity, unitPrice);
            Items.Add(item);
            RecalculateTotal();
            this.MarkAsUpdated();
        }

        public void RemoveItem(Guid itemId)
        {
            var item = Items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                Items.Remove(item);
                RecalculateTotal();
                this.MarkAsUpdated();
            }
        }

        public void UpdateItemQuantity(Guid itemId, int newQuantity)
        {
            var item = Items.FirstOrDefault(i => i.Id == itemId);
            if (item != null && newQuantity > 0)
            {
                item.UpdateQuantity(newQuantity);
                RecalculateTotal();
                this.MarkAsUpdated();
            }
        }

        private void RecalculateTotal()
        {
            decimal total = 0;
            foreach (var item in Items)
            {
                total += item.TotalPrice.Amount;
            }
            TotalAmount = Money.FromYen(total);
        }

        public void ChangeStatus(OrderStatus newStatus, string? updatedBy = null)
        {
            Status = newStatus;
            this.MarkAsUpdated(updatedBy);
        }
    }

    /// <summary>
    /// 注文項目エンティティ
    /// </summary>
    public class OrderItem : GuidEntityBase
    {
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public Money UnitPrice { get; private set; }
        public Money TotalPrice { get; private set; }
        public virtual Order Order { get; private set; } = null!;

        // EF Coreのためのプライベートコンストラクタ
        private OrderItem()
        {
            ProductName = string.Empty;
            UnitPrice = Money.FromYen(0);
            TotalPrice = Money.FromYen(0);
        }

        public OrderItem(Guid orderId, Guid productId, string productName, int quantity, Money unitPrice)
        {
            OrderId = orderId;
            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
            TotalPrice = new Money(unitPrice.Amount * quantity, unitPrice.Currency);
        }

        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero");

            Quantity = newQuantity;
            TotalPrice = new Money(UnitPrice.Amount * Quantity, UnitPrice.Currency);
            this.MarkAsUpdated();
        }
    }

    /// <summary>
    /// 注文状態を表す列挙型
    /// </summary>
    public enum OrderStatus
    {
        New = 0,
        Processing = 1,
        Shipped = 2,
        Delivered = 3,
        Cancelled = 4,
        Returned = 5
    }
}
