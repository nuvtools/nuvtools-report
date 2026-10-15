using NUnit.Framework;
using QuestPDF.Infrastructure;

namespace NuvTools.Report.Pdf.Tests;

/// <summary>
/// QuestPDF refuses to render until the application declares its license. The library leaves that
/// choice to the application that uses it, so the tests declare it here, once, for every fixture.
/// </summary>
[SetUpFixture]
public class QuestPdfLicenseSetup
{
    [OneTimeSetUp]
    public void DeclareLicense() => QuestPDF.Settings.License = LicenseType.Community;
}
