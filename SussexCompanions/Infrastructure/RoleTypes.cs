using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SussexCompanions.Infrastructure
{
    public class RoleTypes
    {
        public const string CUSTOMER = "Customer";
        public const string RECEPTIONIST = "Receptionist";
        public const string CLIENT_SERVICE_AGENT = "Client Service Agent";
        public const string FINANCE_MANAGER = "Finance Manager";

        public const int CUSTOMER_ID = 1;
        public const int RECEPTIONIST_ID = 2;
        public const int CLIENT_SERVICE_AGENT_ID = 3;
        public const int FINANCE_MANAGER_ID = 4;
    }
}