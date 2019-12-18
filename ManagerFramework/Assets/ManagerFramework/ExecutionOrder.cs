/*
* Developed by Gökhan KINAY.
* www.gokhankinay.com.tr
*
* Contact,
* info@gokhankinay.com.tr
*/

using System;

namespace ManagerActorFramework
{
    public class ExecutionOrder : Attribute
    {
        public int Order;

        public ExecutionOrder(int order)
        {
            Order = order;
        }
    }
}