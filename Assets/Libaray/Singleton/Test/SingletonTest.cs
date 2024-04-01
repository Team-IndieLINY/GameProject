using System.Collections;
using System.Collections.Generic;
using IndieLINY.Singleton;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[Singleton(ESingletonType.Global)]
public class TestMonoSingleton : MonoBehaviourSingleton<TestMonoSingleton>
{
    private List<int> _list;

    public bool HasNumber(int value)
    {
        return _list.Contains(value);
    }

    public void SetNumber(int value)
    {
        if (HasNumber(value) == false)
        {
            _list.Add(value);
        }
    }
    public override void PostInitialize()
    {
        _list = new List<int>();
    }

    public override void PostRelease()
    {
        _list.Clear();
    }
}

[Singleton(ESingletonType.Global)]
public class TestGeneralSingleton : IGeneralSingleton
{
    private List<int> _list;

    public bool HasNumber(int value)
    {
        return _list.Contains(value);
    }

    public void SetNumber(int value)
    {
        if (HasNumber(value) == false)
        {
            _list.Add(value);
        }
    }

    public void Initialize()
    {
        _list = new List<int>();
    }

    public void Release()
    {
        _list.Clear();
    }
}

public class TestFailSingleton : IGeneralSingleton
{
    public void Initialize()
    {
    }

    public void Release()
    {
    }
}

public class SingletonTest
{
    [Test]
    public void TestMono()
    {
        Singleton.GetSingleton<TestMonoSingleton>().SetNumber(1);
        Singleton.GetSingleton<TestMonoSingleton>().SetNumber(2);

        var mgr = Singleton.GetSingleton<TestMonoSingleton>();

        Debug.Log(mgr.HasNumber(1) == true);
        Debug.Log(mgr.HasNumber(3) == false);
    }

    [Test]
    public void TestGeneral()
    {
        Singleton.GetSingleton<TestGeneralSingleton>().SetNumber(1);
        Singleton.GetSingleton<TestGeneralSingleton>().SetNumber(2);

        var mgr = Singleton.GetSingleton<TestGeneralSingleton>();

        Debug.Log(mgr.HasNumber(1) == true);
        Debug.Log(mgr.HasNumber(3) == false);
    }

    [Test]
    public void TestFail()
    {
        try
        {
            Singleton.GetSingleton<TestFailSingleton>();
        }
        catch
        {
            return;
        }

        Debug.Assert(false);
    }

    [UnityTest]
    public IEnumerator TestToLoadSceneScopeSingleton()
    {
        SceneManager.LoadScene("ScopeTest");
        yield return null;

        var singleton = Singleton.GetSingleton<ScopeSingleton>();

        var scopedSingleton = singleton.GetScopeSingleton<TestScopeSingleton>();
        Debug.Assert(scopedSingleton.number == 1, scopedSingleton.number);


        SceneManager.LoadScene("ScopeTest_Sub1");
        yield return null;
        scopedSingleton = singleton.GetScopeSingleton<TestScopeSingleton>();
        Debug.Assert(scopedSingleton.number == 2, scopedSingleton.number);

        SceneManager.LoadScene("ScopeTest_Sub2", LoadSceneMode.Additive);
        yield return null;
        scopedSingleton = singleton.GetScopeSingleton<TestScopeSingleton>();
        Debug.Assert(scopedSingleton.number == 2, scopedSingleton.number);
    }

    [UnityTest]
    public IEnumerator TestToLoadSceneScopeSingletonCallback()
    {
        int count = 0;
        void Run(ISingleton s)
        {
            Debug.Log(s);
            count++;
        }
        
        SceneManager.LoadScene("ScopeTest");
        yield return null;

        var singleton = Singleton.GetSingleton<ScopeSingleton>();
        singleton.RegisterScopeSingletonChanged<TestScopeSingleton>(Run);

        SceneManager.LoadScene("ScopeTest_Sub1");
        yield return null;
        
        singleton.UnRegisterScopeSingletonChanged<TestScopeSingleton>(Run);
        SceneManager.LoadScene("ScopeTest_Sub2");
        yield return null;
        Debug.Assert(count == 1);
    }
}