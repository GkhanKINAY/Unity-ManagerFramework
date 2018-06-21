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