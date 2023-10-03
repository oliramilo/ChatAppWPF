using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ChatDLL
{
    [DataContract]
    public class FileMessage : Message
    {
        [DataMember]
        private string fileName;
        [DataMember]
        private string fileExtension;
        [DataMember]
        private string chatRoom;
        [DataMember]
        private byte[] bytes;

        public FileMessage(string sender, string chatroom, string fileName, string fileExtension, byte[] data) : base (sender)
        {
            this.fileName = fileName;
            this.fileExtension = fileExtension;
            this.bytes = data;
            this.chatRoom = chatroom;
        }
        public string getFileName()
        {
            return fileName;
        }

        public string getChatRoomName()
        {
            return chatRoom;
        }

        public byte[] getFileData()
        {
            return bytes;
        }

        public string getFileExtension()
        {
            return fileExtension;
        }
        
    }
}
