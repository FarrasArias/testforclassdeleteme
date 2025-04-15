// ExampleEditModeTest.cs
using NUnit.Framework;

public class ExampleEditModeTest
{
    //NUnit is an open source unit test framework for .NET
    //TestRunner is a tool that discovers and runs tests 
    //UTF (unity test framework) is a unity built-in system for writing and running tests in the editor
    //NUnit uses attributes (metadata) to identify test-related elements
    [Test]
    public void Always_Passes()
    {
        Assert.IsTrue(true);
    }
}
