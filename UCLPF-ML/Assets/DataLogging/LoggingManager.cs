using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggingManager : MonoBehaviour
{
    static List<LoggerInterface> loggerList;

    public static void RegisterLogger(LoggerInterface logger)
    {
        if (loggerList == null) { loggerList = new List<LoggerInterface>(); }
        loggerList.Add(logger);
    }

    public static void DeRegisterLogger(LoggerInterface logger)
    {
        logger.Save();
        loggerList.Remove(logger);
    }

    private void FixedUpdate()
    {

        SaveAllData();
    }

    void SaveAllData()
    {
        if(loggerList == null) {return; }
        foreach (LoggerInterface logger in loggerList)
            logger?.AddData();
    }

    private void OnDestroy()
    {
        //loggerList = new();
    }

    public void Clear()
    {
        loggerList = new();
    }

}
