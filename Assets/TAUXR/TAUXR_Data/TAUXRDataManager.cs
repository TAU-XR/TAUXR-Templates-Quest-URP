using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region Analytics Data Classes
public interface AnalyticsDataClass
{
    string TableName { get; }
}

public class AnalyticsLogLine : AnalyticsDataClass
{
    public string TableName => "TAUXR_Logs";
    public float LogTime;
    public string LogText;

    public AnalyticsLogLine(string line)
    {
        LogTime = Time.time;
        LogText = line;
    }
}

// Declare here new AnalyticsDataClasses for every table file output you desire.

#endregion

public class TAUXRDataManager : TAUXRSingleton<TAUXRDataManager>
{
    // updated from TAUXRPlayer
    bool ExportEyeTracking = false;
    bool ExportFaceTracking = false;

    // automatically switched to true if not in editor.
    public bool ShouldExport = false;


    AnalyticsWriter analyticsWriter;
    DataContinuousWriter continuousWriter;
    DataExporterFaceExpression faceExpressionWriter;

    #region Analytics Data Classes
    // declare pointers for all experience-specific analytics classes
    public AnalyticsLogLine LogLine;

    // write additional events here..


    #endregion

    #region Project Specific Analytics Reporters
    // Write here all the functions you'll want to use to report relevant data.

    // log a new string line with the time logged to TAUXR_Logs file.
    public void LogLineToFile(string line)
    {
        // creates a new instante of AnalyticsLogLine data class. In it's constructor, it gets the line and automatically assign Time.time to the log time.
        LogLine = new AnalyticsLogLine(line);

        // tells the alnalytics writer to write a new line in file.
        WriteAnalyticsToFile(LogLine);
    }

    #endregion

    private void WriteAnalyticsToFile(AnalyticsDataClass analyticsDataClass)
    {
        if (!ShouldExport) return;

        analyticsWriter.WriteAnalyticsDataFile(analyticsDataClass);
    }

    void Start()
    {
        Init();
    }

    private void Init()
    {
        ShouldExport = ShouldExportData();
        if (!ShouldExport) return;

        ExportEyeTracking = TAUXRPlayer.Instance.IsEyeTrackingEnabled;
        ExportFaceTracking = TAUXRPlayer.Instance.IsFaceTrackingEnabled;

        analyticsWriter = new AnalyticsWriter();

        // for now, instead of making the whole interface in the datamanager, it will split between the different scripts.
        continuousWriter = GetComponent<DataContinuousWriter>();
        continuousWriter.Init(ExportEyeTracking);

        if (ExportFaceTracking)
        {
            faceExpressionWriter = GetComponent<DataExporterFaceExpression>();
            faceExpressionWriter.Init();
        }
    }

    // default data export on false in editor. always export on build.
    private bool ShouldExportData()
    {
        if (!ShouldExport)
        {
            // if app runs on build- always export.
            if (!Application.isEditor)
            {
                return true;
            }
            else
            {
                Debug.Log("Data Manager won't export data because it is running in editor. To export, manually enable ShouldExport");
                return false;
            }
        }
        else
        {
            return true;
        }
    }

    void FixedUpdate()
    {
        if (!ShouldExport) return;

        continuousWriter.RecordContinuousData();

        if (ExportFaceTracking)
        {
            faceExpressionWriter.CollectWriteDataToFile();
        }
    }

    private void OnApplicationQuit()
    {
        if (!ShouldExport) return;

        analyticsWriter.Close();
        continuousWriter.Close();
        faceExpressionWriter.Close();
    }

}
