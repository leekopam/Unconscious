using NUnit.Framework;
using System;
using System.Reflection;

public class RewardDataTests
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

    private static object GetRewardDataInstance()
    {
        Type rewardDataType = FindType("RewardData");
        PropertyInfo instanceProperty = rewardDataType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
        Assert.IsNotNull(instanceProperty, "RewardData.Instance not found");

        object instance = instanceProperty.GetValue(null);
        Assert.IsNotNull(instance, "RewardData.Instance is null");
        return instance;
    }

    [SetUp]
    public void SetUp()
    {
        object instance = GetRewardDataInstance();
        PropertyInfo rewardGold = instance.GetType().GetProperty("Reward_Gold", BindingFlags.Public | BindingFlags.Instance);
        Assert.IsNotNull(rewardGold, "RewardData.Reward_Gold property not found");
        rewardGold.SetValue(instance, 0);
    }

    [Test]
    public void RewardGold_Setter_ClampsToZero()
    {
        object instance = GetRewardDataInstance();
        PropertyInfo rewardGold = instance.GetType().GetProperty("Reward_Gold", BindingFlags.Public | BindingFlags.Instance);
        Assert.IsNotNull(rewardGold, "RewardData.Reward_Gold property not found");

        rewardGold.SetValue(instance, -10);
        int value = (int)rewardGold.GetValue(instance);

        Assert.AreEqual(0, value);
    }

    [Test]
    public void AddGold_AddsExpectedAmount()
    {
        object instance = GetRewardDataInstance();
        MethodInfo addGold = instance.GetType().GetMethod("AddGold", BindingFlags.Public | BindingFlags.Instance);
        PropertyInfo rewardGold = instance.GetType().GetProperty("Reward_Gold", BindingFlags.Public | BindingFlags.Instance);

        Assert.IsNotNull(addGold, "RewardData.AddGold not found");
        Assert.IsNotNull(rewardGold, "RewardData.Reward_Gold property not found");

        addGold.Invoke(instance, new object[] { 15 });
        int value = (int)rewardGold.GetValue(instance);

        Assert.AreEqual(15, value);
    }
}
