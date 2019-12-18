/*
* TRIFLES GAMES
* www.triflesgames.com
*
* Developed by Gökhan KINAY.
* www.gokhankinay.com.tr
*
* Contact,
* info@triflesgames.com
* info@gokhankinay.com.tr
*/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ManagerActorFramework
{
    [InitializeOnLoad]
    public class ExecutionOrderManager
    {
        static ExecutionOrderManager()
        {
            foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts())
            {
                if (monoScript.GetClass() != null)
                {
                    foreach (var a in Attribute.GetCustomAttributes(monoScript.GetClass(), typeof(ExecutionOrder)))
                    {
                        var currentOrder = MonoImporter.GetExecutionOrder(monoScript);
                        var newOrder = ((ExecutionOrder)a).Order;
                        if (currentOrder != newOrder)
                        {
                            MonoImporter.SetExecutionOrder(monoScript, newOrder);
                        }
                    }
                }
            }
        }
    }
}