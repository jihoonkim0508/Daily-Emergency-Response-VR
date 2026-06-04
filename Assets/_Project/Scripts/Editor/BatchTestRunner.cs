using System;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

public static class BatchTestRunner
{
    public static void RunEditMode()
    {
        Run(TestMode.EditMode, "TestResults-EditMode.xml");
    }

    public static void RunPlayMode()
    {
        Run(TestMode.PlayMode, "TestResults-PlayMode.xml");
    }

    static void Run(TestMode testMode, string resultPath)
    {
        Debug.Log($"BatchTestRunner invoked for {testMode}.");
        var api = ScriptableObject.CreateInstance<TestRunnerApi>();
        api.RegisterCallbacks(new ExitOnRunFinished(resultPath));
        var settings = new ExecutionSettings(new Filter { testMode = testMode })
        {
            runSynchronously = testMode == TestMode.EditMode
        };
        var jobId = api.Execute(settings);
        Debug.Log($"BatchTestRunner scheduled job {jobId} for {testMode}.");
    }

    sealed class ExitOnRunFinished : ICallbacks
    {
        readonly string m_ResultPath;

        public ExitOnRunFinished(string resultPath)
        {
            m_ResultPath = resultPath;
        }

        public void RunStarted(ITestAdaptor testsToRun)
        {
            Debug.Log($"Batch test run started: {testsToRun.FullName}");
        }

        public void RunFinished(ITestResultAdaptor result)
        {
            TestRunnerApi.SaveResultToFile(result, m_ResultPath);
            Debug.Log($"Batch test run finished: passed={result.PassCount}, failed={result.FailCount}, skipped={result.SkipCount}, inconclusive={result.InconclusiveCount}");
            EditorApplication.Exit(result.FailCount > 0 ? 1 : 0);
        }

        public void TestStarted(ITestAdaptor test)
        {
        }

        public void TestFinished(ITestResultAdaptor result)
        {
            if (result.TestStatus == TestStatus.Failed)
                Debug.LogError($"{result.FullName}: {result.Message}{Environment.NewLine}{result.StackTrace}");
        }
    }
}
