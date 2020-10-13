using System.Threading.Tasks;
using Gimme.Commands;
using McMaster.Extensions.CommandLineUtils;

namespace Gimme
{
    public class Program
    {
        static async Task Main(string[] args)
        {
           var app = new CommandLineApplication<GimmeCommand>();
           app.Conventions.UseDefaultConventions();
           await app.ExecuteAsync(args);
        }
    }
}
