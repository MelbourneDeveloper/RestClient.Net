namespace Outcome.Tests;

[TestClass]
public class UnitTests
{
    [TestMethod]
    public void Unit_Value_IsSingleton()
    {
        var unit1 = Unit.Value;
        var unit2 = Unit.Value;
        Assert.AreSame(unit1, unit2);
    }

    [TestMethod]
    public void Unit_Equality_WorksCorrectly()
    {
        var unit1 = Unit.Value;
        var unit2 = Unit.Value;
        Assert.AreEqual(unit1, unit2);
        Assert.IsTrue(unit1 == unit2);
    }

    [TestMethod]
    public void Unit_GetHashCode_IsConsistent()
    {
        var unit1 = Unit.Value;
        var unit2 = Unit.Value;
        Assert.AreEqual(unit1.GetHashCode(), unit2.GetHashCode());
    }

    [TestMethod]
    public void Unit_ToString_Works()
    {
        var unit = Unit.Value;
        var str = unit.ToString();
        Assert.IsNotNull(str);
    }

    [TestMethod]
    public void Unit_CanBeUsedInResult()
    {
        var result = new Result<Unit, string>.Ok<Unit, string>(Unit.Value);
        Assert.IsTrue(result.IsOk);
        var value = result.Match(static u => u, static _ => Unit.Value);
        Assert.AreSame(Unit.Value, value);
    }

    [TestMethod]
    public void Unit_CanBeUsedAsErrorType()
    {
        var result = Result<string, Unit>.Failure(Unit.Value);
        Assert.IsTrue(result.IsError);
        var error = !result;
        Assert.AreSame(Unit.Value, error);
    }
}
