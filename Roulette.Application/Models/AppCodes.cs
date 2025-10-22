namespace Roulette.Application.Models
{
    public static class AppCodes
    {
        public static class System
        {
            public const string SUCCESS = "SUCCESS";                    // 200
            public const string CREATED = "CREATED";                    // 201
            public const string UPDATED = "UPDATED";                    // 200
            public const string DELETED = "DELETED";                    // 200
            public const string NOT_FOUND = "NOT_FOUND";                // 404
            public const string VALIDATION_ERROR = "VALIDATION_ERROR";  // 400
            public const string INTERNAL_ERROR = "INTERNAL_ERROR";      // 500
            public const string DATABASE_ERROR = "DATABASE_ERROR";      // 500
            public const string REDIS_ERROR = "REDIS_ERROR";            // 502
            public const string UNAUTHORIZED = "UNAUTHORIZED";          // 401
            public const string FORBIDDEN = "FORBIDDEN";                // 403
            public const string TIMEOUT = "TIMEOUT";                    // 504
            public const string CONFLICT = "CONFLICT";                  // 409
        }

        public static class Auth
        {
            public const string AUTH_SUCCESS = "AUTH_SUCCESS";                    // 200
            public const string AUTH_FAILED = "AUTH_FAILED";                      // 401
            public const string INVALID_TOKEN = "INVALID_TOKEN";                  // 401
            public const string INVALID_AUTH = "INVALID_AUTH";                    // 401
            public const string TOKEN_EXPIRED = "TOKEN_EXPIRED";                  // 401
            public const string TOKEN_REFRESHED = "TOKEN_REFRESHED";              // 200
            public const string ACCESS_DENIED = "ACCESS_DENIED";                  // 403
            public const string PASSWORD_MISMATCH = "PASSWORD_MISMATCH";          // 400
            public const string USER_LOCKED = "USER_LOCKED";                      // 423
            public const string USER_NOT_VERIFIED = "USER_NOT_VERIFIED";          // 403
            public const string SESSION_EXPIRED = "SESSION_EXPIRED";              // 440
            public const string AUTH_INTERNAL_ERROR = "AUTH_INTERNAL_ERROR";      // 500
        }

        public static class User
        {
            public const string USER_CREATED = "USER_CREATED";                    // 201
            public const string USER_UPDATED = "USER_UPDATED";                    // 200
            public const string USER_DELETED = "USER_DELETED";                    // 200
            public const string USER_NOT_FOUND = "USER_NOT_FOUND";                // 404
            public const string INVALID_CREDENTIALS = "INVALID_CREDENTIALS";      // 401
            public const string USER_ALREADY_EXISTS = "USER_ALREADY_EXISTS";      // 409
            public const string INSUFFICIENT_CREDIT = "INSUFFICIENT_CREDIT";      // 400
            public const string USER_VALID = "USER_VALID";                        // 200
            public const string USER_INVALID = "USER_INVALID";                    // 400
        }

        public static class Roulette
        {
            public const string ROULETTE_CREATED = "ROULETTE_CREATED";                  // 201
            public const string ROULETTE_OPENED = "ROULETTE_OPENED";                    // 200
            public const string ROULETTE_CLOSED = "ROULETTE_CLOSED";                    // 200
            public const string ROULETTE_LISTED = "ROULETTE_LISTED";                    // 200

            public const string ROULETTE_NOT_FOUND = "ROULETTE_NOT_FOUND";              // 404
            public const string ROULETTE_NOT_OPEN = "ROULETTE_NOT_OPEN";                // 400
            public const string ROULETTE_ALREADY_OPEN = "ROULETTE_ALREADY_OPEN";        // 409
            public const string ROULETTE_ALREADY_CLOSED = "ROULETTE_ALREADY_CLOSED";    // 409

            public const string ROULETTE_CREATION_ERROR = "ROULETTE_CREATION_ERROR";    // 500
            public const string ROULETTE_OPEN_ERROR = "ROULETTE_OPEN_ERROR";            // 500
            public const string ROULETTE_CLOSE_ERROR = "ROULETTE_CLOSE_ERROR";          // 500
        }

        public static class Bet
        {
            public const string BET_CREATED = "BET_CREATED";                      // 201
            public const string BET_VALID = "BET_VALID";                          // 200
            public const string BET_WINNER = "BET_WINNER";                        // 200
            public const string BET_LOSER = "BET_LOSER";                          // 200

            public const string INVALID_BET_TYPE = "INVALID_BET_TYPE";            // 400
            public const string INVALID_BET_AMOUNT = "INVALID_BET_AMOUNT";        // 400
            public const string INVALID_BET_NUMBER = "INVALID_BET_NUMBER";        // 400
            public const string INVALID_BET_COLOR = "INVALID_BET_COLOR";          // 400

            public const string BET_CREATION_ERROR = "BET_CREATION_ERROR";        // 500
            public const string BET_NOT_FOUND = "BET_NOT_FOUND";                  // 404
        }
    }
}
