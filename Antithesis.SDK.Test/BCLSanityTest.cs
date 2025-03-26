namespace Antithesis.SDK;

using System.IO;

public class BCLSanityTest
{
    [Fact]
    public void PathGetDirectoryName()
    {
        XAssert.Equal("/foo/bar", Path.GetDirectoryName("/foo/bar/"));
        XAssert.Equal("/foo", Path.GetDirectoryName("/foo/bar"));
    }
}