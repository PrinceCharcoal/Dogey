using Discord.Commands;
using System.Threading.Tasks;

namespace Dogey.Modules.Eval
{
    [RequireOwner]
    [Group("eval"), Name("Eval")]
    public class EvalModule : DogeyModuleBase
    {
        private readonly RoslynService _roslyn;

        public EvalModule(RoslynService roslyn)
        {
            _roslyn = roslyn;
        }
        
        [Command(RunMode = RunMode.Async)]
        public async Task EvalAsync([Remainder]string code)
        {
            var result = await _roslyn.EvalAsync(Context, code);
            await ReplyAsync(result);
        }
    }
}
