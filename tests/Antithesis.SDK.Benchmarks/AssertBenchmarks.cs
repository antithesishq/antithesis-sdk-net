namespace Antithesis.SDK;

using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

[Config(typeof(AssertBenchmarks.Config))]
[DisassemblyDiagnoser(maxDepth: 0, printInstructionAddresses: true, printSource: true)]
[HideColumns(Column.Job, Column.RatioSD)]
[MemoryDiagnoser]
public class AssertBenchmarks
{
    public class Config : ManualConfig
	{
		public Config()
		{
            var localSinkEnvironmentVariable =
                new EnvironmentVariable(EnvironmentVariableNames.LocalSinkOutputFilePath, Path.GetTempFileName());

			AddJob(Job.Default.AsBaseline());
            AddJob(Job.Default.WithEnvironmentVariable(localSinkEnvironmentVariable));
		}
	}

    public const string IdIsTheMessage = nameof(AlwaysInTightLoop);

    [Benchmark]
    public long AlwaysInTightLoop()
    {
        long sum = 0;

        for (int i = 0; i < 1_000; i++)
        {
            sum += i;
            Assert.Always(true, IdIsTheMessage);
        }

        return sum;
    }   
}