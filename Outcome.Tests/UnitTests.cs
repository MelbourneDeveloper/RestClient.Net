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

        Assert.IsInstanceOfType<Unit>(unit1);
        Assert.AreSame(unit1, Unit.Value);
        Assert.IsFalse(unit1.Equals(null));
        OutcomeAssertions.Success(new Result<Unit, string>.Ok<Unit, string>(unit1), unit2);
    }

    [TestMethod]
    public void Unit_Equality_WorksCorrectly()
    {
        var unit1 = Unit.Value;
        var unit2 = Unit.Value;
        Assert.AreEqual(unit1, unit2);
        Assert.IsTrue(unit1 == unit2);

        Assert.IsTrue(unit2.Equals(unit1));
        Assert.IsFalse(unit1 != unit2);
        Assert.IsFalse(unit1.Equals(null));
        var values = new HashSet<Unit> { unit1, unit2, Unit.Value };
        Assert.AreEqual(1, values.Count);
        Assert.IsTrue(values.Contains(Unit.Value));
        OutcomeAssertions.Success(new Result<Unit, string>.Ok<Unit, string>(unit1), unit2);
    }

    [TestMethod]
    public void Unit_GetHashCode_IsConsistent()
    {
        var unit1 = Unit.Value;
        var unit2 = Unit.Value;
        Assert.AreEqual(unit1.GetHashCode(), unit2.GetHashCode());

        var dictionary = new Dictionary<Unit, string> { [unit1] = "first" };
        Assert.AreEqual("first", dictionary[unit2]);
        dictionary[Unit.Value] = "updated";
        Assert.AreEqual(1, dictionary.Count);
        Assert.AreEqual("updated", dictionary[unit1]);
        Assert.AreSame(Unit.Value, unit2);
        OutcomeAssertions.Success(new Result<Unit, string>.Ok<Unit, string>(unit1), unit2);
    }

    [TestMethod]
    public void Unit_ToString_Works()
    {
        var unit = Unit.Value;
        var str = unit.ToString();
        Assert.IsNotNull(str);

        Assert.AreEqual("Unit { }", str);
        Assert.AreEqual(str, Unit.Value.ToString());
        Assert.AreSame(unit, Unit.Value);
        OutcomeAssertions.Success(new Result<Unit, string>.Ok<Unit, string>(unit), Unit.Value);
    }

    [TestMethod]
    public void Unit_CanBeUsedInResult()
    {
        var result = new Result<Unit, string>.Ok<Unit, string>(Unit.Value);
        Assert.IsTrue(result.IsOk);
        var value = result.Match(static u => u, static _ => Unit.Value);
        Assert.AreSame(Unit.Value, value);

        OutcomeAssertions.Success(result, Unit.Value);
        var mapped = result.Map(static _ => "completed");
        OutcomeAssertions.Success(mapped, "completed");
        Assert.AreSame(Unit.Value, +result);
    }

    [TestMethod]
    public void Unit_CanBeUsedAsErrorType()
    {
        var result = Result<string, Unit>.Failure(Unit.Value);
        Assert.IsTrue(result.IsError);
        var error = !result;
        Assert.AreSame(Unit.Value, error);

        OutcomeAssertions.Error(result, Unit.Value);
        var mapped = result.MapError(static _ => "failed");
        OutcomeAssertions.Error(mapped, "failed");
        Assert.AreSame(Unit.Value, !result);
    }
}
