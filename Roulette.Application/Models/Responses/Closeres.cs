using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roulette.Application.Models.Responses
{
    public record Closeres(string hola)
    {
        public static Closeres Success(string hola) =>
            new(hola);

        public static Closeres Fail(string message) => 
            new(message);
    }
}
