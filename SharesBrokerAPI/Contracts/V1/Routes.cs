namespace SharesBrokerAPI.Contracts.V1
{
    public static class Routes
    {
        public const string Root = "api";

        public const string Version = "v1";

        public const string Base = Root + "/" + Version;


        public static class SharesRoutes
        {
            public const string GetAll = Base + "/shares";

            public const string Get = Base + "/shares/{companySymbol}";
        }

        public static class UserRoutes
        {
            public const string GetAll = Base + "/users";

            public const string Get = Base + "/users/{username}";

            public const string Create = Base + "/users";

            public const string Delete = Base + "/users/{username}";
            
            public const string Update = Base + "/users/{username}";
        }

        public static class PurchaseRoutes
        {
            public const string GetAll = Base + "/purchases";

            public const string Get = Base + "/purchases/{id}";

            public const string Create = Base + "/purchases";
        }

        public static class SalesRoutes
        {
            public const string GetAll = Base + "/sales";

            public const string Get = Base + "/sales/{id}";

            public const string Create = Base + "/sales";
        }

        public static class UserStockRoutes
        {
            public const string Get = Base + "/usershares/{id}";
            
            public const string GetAll = Base + "/usershares";
        }

    }
}