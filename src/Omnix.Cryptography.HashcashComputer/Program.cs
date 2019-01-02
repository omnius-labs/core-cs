using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

namespace Omnix.Cryptography.HashcashComputer
{
    class Program
    {
        static int Main(string[] args)
        {
            var app = new CommandLineApplication(throwOnUnexpectedArg: false);

            app.Name = "HashcashComputer";
            app.HelpOption("-h|--help");

            app.Command("compute", computeCommand =>
            {
                computeCommand.Description = "Compute hashcash.";
                computeCommand.HelpOption("-h|--help");

                var typeOption = computeCommand.Option("--type", "Hashcash type.", CommandOptionType.SingleValue);
                var limitOption = computeCommand.Option("--limit", "Hashcash limit.", CommandOptionType.SingleValue);
                var timeoutOption = computeCommand.Option("--timeout", "*/s.", CommandOptionType.SingleValue);
                var valueOption = computeCommand.Option("--value", "Value to be computed.", CommandOptionType.SingleValue);
                var bindOption = computeCommand.Option("--bind", "Process id.", CommandOptionType.SingleValue);

                computeCommand.OnExecute(() =>
                {
                    if (typeOption.Value() == null || limitOption.Value() == null || timeoutOption.Value() == null || valueOption.Value() == null || bindOption.Value() == null)
                    {
                        computeCommand.ShowHelp();
                        return 1;
                    }

                    if (typeOption.Value() == "OmniHashcash_Sha2_256")
                    {
                        var limit = int.Parse(limitOption.Value() ?? "256");
                        var timeout = TimeSpan.FromSeconds(int.Parse(timeoutOption.Value()));
                        var value = BytesConvert.FromHexString(valueOption.Value());
                        var processId = int.Parse(bindOption.Value());

                        using (var tokenSource = new CancellationTokenSource())
                        {
                            var task = Task.Factory.StartNew(() =>
                            {
                                return OmniHashcash_Sha256.Compute(value, limit, timeout, tokenSource.Token);
                            }, TaskCreationOptions.LongRunning);

                            using (var process = Process.GetProcessById(processId))
                            {
                                while (!task.IsCompleted)
                                {
                                    if (process.HasExited) return 1;
                                    Thread.Sleep(1000 * 3);
                                }
                            }

                            Console.WriteLine(BytesConvert.ToHexString(task.Result));
                        }
                    }

                    return 0;
                });
            });

            return app.Execute(args);
        }
    }
}
