using System;
using System.Collections.Generic;
using System.Linq;
using TfsMonitor;

namespace SkypeGroupGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please input the group topic:");
            var topic = Console.ReadLine();
            Console.WriteLine("Please input the skype account you want to add into the group:");
            var member = Console.ReadLine();
            var oSkype = SkypeClient.GetInstance();
            var names = member.Split(',');
            var memberList = names.ToList();
            var groupName = oSkype.CreateChatGroup(topic, memberList);
            Console.WriteLine(groupName);
            Console.ReadKey();
        }
    }
}
