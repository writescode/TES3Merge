using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using TES3Merge.Util;
using static TES3Merge.Tests.Utility;

namespace TES3Merge.Tests.Installation;

[TestClass, TestCategory("Installation")]
public class OpenMW
{
    protected IHost _host;
    protected Microsoft.Extensions.Logging.ILogger _logger;

    public OpenMWInstallation? Install;

    public OpenMW()
    {
        // Setup logging.
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        var hostBuilder = Host.CreateDefaultBuilder().UseSerilog();
        _host = hostBuilder.Build();
        _logger = _host.Services.GetRequiredService<ILogger<Morrowind>>();

        if (Directory.Exists(Properties.Resources.OpenMWInstallDirectory))
        {
            Install = new OpenMWInstallation(Properties.Resources.OpenMWInstallDirectory);
        }
    }

    [TestMethod]
    public void InstallationFound()
    {
        if (Directory.Exists(Properties.Resources.OpenMWInstallDirectory))
        {
            Assert.IsNotNull(Install);
        }
    }

    [TestMethod]
    public void InstallationPathValid()
    {
        if (Install is null)
        {
            Assert.Inconclusive();
            return;
        }

        _logger.LogInformation("Installation Directory: {path}", Install.RootDirectory);
        var cfgPath = Path.Combine(Install.RootDirectory, "openmw.cfg");
        Assert.IsTrue(File.Exists(cfgPath));
    }

    [TestMethod]
    public void ArchivesFound()
    {
        if (Install is null)
        {
            Assert.Inconclusive();
            return;
        }

        _logger.LogInformation("Archives: {list}", Install.Archives);
        Assert.IsTrue(Install.Archives.Contains("Morrowind.bsa"));
        Assert.IsTrue(Install.Archives.Contains("Tribunal.bsa"));
        Assert.IsTrue(Install.Archives.Contains("Bloodmoon.bsa"));
    }

    [TestMethod]
    public void ArchivesAreInOrder()
    {
        if (Install is null)
        {
            Assert.Inconclusive();
            return;
        }

        _logger.LogInformation("Archives: {list}", Install.Archives);
        Assert.AreEqual("Morrowind.bsa", Install.Archives[0]);
        Assert.AreEqual("Tribunal.bsa", Install.Archives[1]);
        Assert.AreEqual("Bloodmoon.bsa", Install.Archives[2]);
    }

    [TestMethod]
    public void GameFilesFound()
    {
        if (Install is null)
        {
            Assert.Inconclusive();
            return;
        }

        _logger.LogInformation("Game Files: {list}", Install.GameFiles);
        Assert.IsTrue(Install.GameFiles.Contains("Morrowind.esm"));
        Assert.IsTrue(Install.GameFiles.Contains("Tribunal.esm"));
        Assert.IsTrue(Install.GameFiles.Contains("Bloodmoon.esm"));
    }

    [TestMethod]
    public void GameFilesFoundAreInOrder()
    {
        if (Install is null)
        {
            Assert.Inconclusive();
            return;
        }

        _logger.LogInformation("Game Files: {list}", Install.GameFiles);
        Assert.AreEqual("Morrowind.esm", Install.GameFiles[0]);
        Assert.AreEqual("Tribunal.esm", Install.GameFiles[1]);
        Assert.AreEqual("Bloodmoon.esm", Install.GameFiles[2]);
    }

    [TestMethod]
    public void DataFilesFound()
    {
        if (Install is null)
        {
            Assert.Inconclusive();
            return;
        }

        Assert.IsNotNull(Install.GetDataFile("Morrowind.esm"));
        Assert.IsNotNull(Install.GetDataFile("Morrowind.bsa"));
    }
}
