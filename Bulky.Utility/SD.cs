using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Utility
{
    public static class SD
    {
        public const string Role_User_Cust = "Customer";
        public const string Role_User_Comp = "Company";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";

        public const string statusPending = "Pending";
        public const string statusApproved = "Approved";
        public const string statusInProcess = "InProcess";
        public const string statusShipped = "Shipped";
        public const string statusCancelled = "Cancelled";
        public const string statusRefunded = "Refunded";

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayedPayment = "Delayed Payment";
        public const string PaymentStatusRejected = "Rejected";

        public const string SessionShoppingCart = "SessionShoppingCart";
    }
}
