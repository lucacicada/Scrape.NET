namespace Scrape.NET.Tests;

[TestFixture]
public class QueryBuilderTests
{
    [Test]
    public void ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new QueryBuilder((string)null!));
        Assert.Throws<ArgumentNullException>(() => new QueryBuilder((Uri)null!));
    }

    [Test]
    [TestCase("%$")]
    [TestCase("^^")]
    [Parallelizable(ParallelScope.All)]
    public void ShouldThrowUriFormatExceptionOnInvalidUrl(string uri)
    {
        Assert.Throws<UriFormatException>(() => new QueryBuilder(uri));
        Assert.Throws<UriFormatException>(() => new QueryBuilder(new Uri(uri, UriKind.Relative)));
        Assert.Throws<UriFormatException>(() => new QueryBuilder(new Uri(uri, UriKind.Absolute)));
    }

    [Test]
    [TestCase("")]
    [TestCase("/")]
    [TestCase("/relative/url")]
    [TestCase("relative")]
    [TestCase("relative/url")]
    [Parallelizable(ParallelScope.All)]
    public void ShouldThrowUriFormatExceptionOnRelativeUrl(string uri)
    {
        Assert.Throws<UriFormatException>(() => new QueryBuilder(uri));
        Assert.Throws<UriFormatException>(() => new QueryBuilder(new Uri(uri, UriKind.Relative)));
        Assert.Throws<UriFormatException>(() => new QueryBuilder(new Uri(uri, UriKind.Absolute)));
    }

    [Test]
    [TestCase("http://example", "a", "1", "http://example/?a=1")]
    [TestCase("http://example/", "a", "1", "http://example/?a=1")]
    [TestCase("http://example/path", "a", "1", "http://example/path?a=1")]
    [TestCase("http://example/path/", "a", "1", "http://example/path/?a=1")]
    // ""
    [TestCase("http://example", "a", "", "http://example/?a=")]
    [TestCase("http://example/", "a", "", "http://example/?a=")]
    [TestCase("http://example/path", "a", "", "http://example/path?a=")]
    [TestCase("http://example/path/", "a", "", "http://example/path/?a=")]
    // null
    [TestCase("http://example", "a", null, "http://example/?a=")]
    [TestCase("http://example/", "a", null, "http://example/?a=")]
    [TestCase("http://example/path", "a", null, "http://example/path?a=")]
    [TestCase("http://example/path/", "a", null, "http://example/path/?a=")]
    // escaped
    [TestCase("http://example", "a", "<>", "http://example/?a=%3c%3e")]
    [Parallelizable(ParallelScope.All)]
    public void ShouldWork(string uri, string key, string? value, string result)
    {
        Assert.That(new QueryBuilder(uri).Set(key, value).Uri.AbsoluteUri, Is.EqualTo(result));
    }
}
