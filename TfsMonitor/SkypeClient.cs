using System;
using System.Collections.Generic;
using SKYPE4COMLib;

namespace TfsMonitor
{
    public class SkypeClient
    {
        private static SkypeClient _client;
        private readonly SKYPE4COMLib.Skype _OSkype;
        private string _DefaultChatGroupName;

        private SkypeClient()
        {
            _OSkype = new SKYPE4COMLib.Skype();
        }

        public static SkypeClient GetInstance()
        {
            return _client ?? (_client = new SkypeClient());
        }

        public string CreateChatGroup(string groupName, List<string> members)
        {
            UserCollection collection = new UserCollection();
            foreach (var member in members)
            {
                collection.Add(_OSkype.User[member]);
            }
            Chat chat = _OSkype.CreateChatMultiple(collection);
            chat.Topic = groupName;
            return chat.Name;
        }

        public bool SendMessageToChat(string chatName, string message)
        {
            try
            {
                Chat chat = _OSkype.Chat[chatName];
                chat.SendMessage(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void SetDefaultChatGroupName(string groupName)
        {
            _DefaultChatGroupName = groupName;
        }

        public bool SendMessageToChat(string message)
        {
            try
            {
                Chat chat = _OSkype.Chat[_DefaultChatGroupName];
                chat.SendMessage(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

}
