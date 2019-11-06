using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenPop.Mime;
using OpenPop.Pop3;
using System.IO;

namespace OraMailLoad
{
    class MailWorker 
    {
        private Pop3Client client;
        Logger logger = new Logger();
        public void Connect(string server, int port, string user, string password)
        {
            try
            {
                client = new Pop3Client();
                client.Connect(server, port, false);
                client.Authenticate(user, password);
                logger.LogMessage("Успешно подключено к "+ server);
            }
            catch (Exception ex)
            {
                logger.LogMessage("Ошибка подключения", ex.Message);
            }
        }
        public void Disconnect()
         {
            if (client.Connected)
            {
                try
                {
                    client.Disconnect();
                    logger.LogMessage("Отключено от сервера");
                }
                catch (Exception ex)
                {
                    logger.LogMessage("Не удается отключиться  от сервера", ex.Message);
                }
                
            }
            
        }

        public void DeleteMessage(int msgId)
        {
            client.DeleteMessage(msgId);
        }
       
        public List<Pop3Mail> GetMail()
        {
            int messageCount = client.GetMessageCount();
            var allMessages = new List<Pop3Mail>(messageCount);
            for (int i = messageCount; i > 0; i--)
            {
                allMessages.Add(new Pop3Mail() { MessageNumber = i, Message = client.GetMessage(i) });
            }
            return allMessages;
        }

        public List<string> GetAttachments(Message message)
        {
            var getAttachments = new List<string>();
            var attachments = message.FindAllAttachments();
            var attachmentdirectory = @"c:\temp\mail\attachments";

            Directory.CreateDirectory(attachmentdirectory);

            foreach (var att in attachments)
            {
                string filename = string.Format(@"{0}{1}_{2}{3}", attachmentdirectory, Path.GetFileNameWithoutExtension(att.FileName), DateTime.Now.ToString("MMddyyyyhhmmss"), Path.GetExtension(att.FileName));
                att.Save(new FileInfo(filename));

                getAttachments.Add(filename);
            }

            return getAttachments;
        }
    }

    public class Pop3Mail
    {
        public int MessageNumber { get; set; }
        public Message Message { get; set; }
    }
}
