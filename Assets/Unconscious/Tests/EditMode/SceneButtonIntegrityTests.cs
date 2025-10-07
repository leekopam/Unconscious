using NUnit.Framework;
using System;
using System.Reflection;

public class SceneButtonIntegrityTests
{
    private static Type FindType(string typeName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = assembly.GetType(typeName, false);
            if (type != null)
            {
                return type;
            }
        }

        Assert.Fail($"Type not found: {typeName}");
        return null;
    }

    private static int GetSeatConstant(string name)
    {
        Type seatType = FindType("SeatIndex");
        FieldInfo field = seatType.GetField(name, BindingFlags.Public | BindingFlags.Static);
        Assert.IsNotNull(field, $"SeatIndex.{name} not found");
        return (int)field.GetValue(null);
    }

    private static bool InvokeSeatIsValid(int value)
    {
        Type seatType = FindType("SeatIndex");
        MethodInfo method = seatType.GetMethod("IsValid", BindingFlags.Public | BindingFlags.Static);
        Assert.IsNotNull(method, "SeatIndex.IsValid not found");
        return (bool)method.Invoke(null, new object[] { value });
    }

    private static string GetSceneConstant(string name)
    {
        Type sceneType = FindType("SceneNames");
        FieldInfo field = sceneType.GetField(name, BindingFlags.Public | BindingFlags.Static);
        Assert.IsNotNull(field, $"SceneNames.{name} not found");
        return (string)field.GetValue(null);
    }

    [Test]
    public void SeatIndex_Constants_AreStable()
    {
        int left = GetSeatConstant("Left");
        int middle = GetSeatConstant("Middle");
        int right = GetSeatConstant("Right");
        int count = GetSeatConstant("Count");
        int unknown = GetSeatConstant("Unknown");

        Assert.AreEqual(0, left);
        Assert.AreEqual(1, middle);
        Assert.AreEqual(2, right);
        Assert.AreEqual(3, count);

        Assert.IsTrue(InvokeSeatIsValid(left));
        Assert.IsTrue(InvokeSeatIsValid(middle));
        Assert.IsTrue(InvokeSeatIsValid(right));
        Assert.IsFalse(InvokeSeatIsValid(unknown));
        Assert.IsFalse(InvokeSeatIsValid(count));
    }

    [Test]
    public void SceneNames_Constants_AreStable()
    {
        Assert.AreEqual("Order", GetSceneConstant("Order"));
        Assert.AreEqual("Cocktail", GetSceneConstant("Cocktail"));
        Assert.AreEqual("Dessert", GetSceneConstant("Dessert"));
    }

    [Test]
    public void GameManager_CompatibilityMethod_StillExists()
    {
        Type gameManagerType = FindType("Game_Manager");
        MethodInfo changeScene = gameManagerType.GetMethod("ChangeScene", BindingFlags.Public | BindingFlags.Instance);
        MethodInfo changeSecene = gameManagerType.GetMethod("ChangeSecene", BindingFlags.Public | BindingFlags.Instance);

        Assert.IsNotNull(changeScene);
        Assert.IsNotNull(changeSecene);
    }
}
