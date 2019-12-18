/*
* Developed by Gökhan KINAY.
* www.gokhankinay.com.tr
*
* Contact,
* info@gokhankinay.com.tr
*/

namespace ManagerActorFramework
{
    public delegate void Subscription<TManager>(object[] arguments) where TManager : Manager<TManager>;
}