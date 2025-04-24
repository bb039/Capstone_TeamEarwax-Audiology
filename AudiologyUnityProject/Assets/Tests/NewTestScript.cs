using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NewTestScript
{

    [UnityTest]
    public IEnumerator CheckHeadTypeSetting()
    {

        string head = "Right";
        PlayerPrefs.SetString("headType", "Right");

        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        Assert.AreEqual(PlayerPrefs.GetString("headType"), head);


        yield return null;
    }

    [UnityTest]
    public IEnumerator CheckCerumenAmountSetting()
    {

        int cerumen = 15;

        PlayerPrefs.SetInt("cerumenAmount", 15);

        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        Assert.AreEqual(PlayerPrefs.GetInt("cerumenAmount"), cerumen);


        yield return null;
    }
}
