namespace RestClient.Net.CsTest.Models;

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CA1812 // Avoid uninstantiated internal classes - This is instantiated via deserialization
// Leaving this rule turned on because we want to design for AOT, and not reflection
public sealed record PostResponse(int id);
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
#pragma warning restore IDE1006 // Naming Styles
