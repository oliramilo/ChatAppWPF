using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;
using System.IO;
using ChatDLL;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace FileServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class FileHandler : FileServerInterface
    {

        // File data is stored in a List of FileMessage instances.
        private List<FileMessage> files;
        public FileHandler() 
        {
            files = new List<FileMessage>();
        }

        public FileMessage getFile(string chatroom, string fileName)
        {
            foreach (FileMessage fm in files)
            {
                if(fm.getChatRoomName().Equals(chatroom) && fm.getFileName().Equals(fileName))
                {
                    return fm;
                }
            }
            return null;
        }

        public List<FileMessage> getFilesForChatRoom(string chatRoomName)
        {
            Console.WriteLine("Request to get files for chatroom: " + chatRoomName);
            List<FileMessage> chatRoomFiles = new List<FileMessage>();
            foreach (FileMessage fm in files)
            {
                if (fm.getChatRoomName().Equals(chatRoomName))
                {
                    chatRoomFiles.Add(fm);
                }
            }
            return chatRoomFiles;
        }

        public void uploadFile(FileMessage fileMessage)
        {
            Console.WriteLine("Request to add file: " + fileMessage.getFileName() + ". sender: " + fileMessage.getSender());
            files.Add(fileMessage);
        }

        public int getChatRoomFileCount(string chatroom)
        {
            int count = 0;
            foreach (FileMessage fm in files)
            {
                if (fm.getChatRoomName().Equals(chatroom))
                {
                    count = count + 1;
                }
            }
            return count;
        }

        public List<FileMessage> test()
        {
            List<FileMessage> fakeMessages = new List<FileMessage>();
            byte[] data = new byte[1024];
            FileMessage message = new FileMessage("supermman2017","chatroom", "word.txt", ".txt", data);
            FileMessage message2 = new FileMessage("supermman2017", "chatroom", "test.txt", ".txt", data);
            fakeMessages.Add(message);
            fakeMessages.Add(message2);
            return fakeMessages;
        }
    }
}
