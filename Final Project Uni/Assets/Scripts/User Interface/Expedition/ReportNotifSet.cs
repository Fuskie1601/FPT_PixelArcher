using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportNotifSet : MonoBehaviour
{
    public ReportElement report;

    public void NotifSetter()
    {
        ExpeditionReport.Instance.ReportElements.Add(report);
    }
}
