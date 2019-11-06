using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OraMailLoad
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger logger = new Logger();
            logger.LogMessage("Hallo");
            MailWorker mw = new MailWorker();

            mw.Connect("10.3.1.1", 110, "vc-mlv", "vc2015");

            List<Pop3Mail> list  =  mw.GetMail();
            List<Mail> frompol = new List<Mail>();
            // перебираем полученные письма
            foreach (var l in list)
            {
            // выбираем вложения и запихиваем в список
                List<string> attach = mw.GetAttachments(l.Message);
                List<MailFile> polfile = new List<MailFile>();// создаем новый список вложений для каждого письма
                if (attach.Any())
                {
                    foreach (var a in attach)
                    {
                        polfile.Add(new MailFile(a, GetFile(a)));
                    }
                }
                Mail письмо = new Mail(MailFrom: l.Message.Headers.From.Address,
                                       MailTo: string.Join(",", l.Message.Headers.To),
                                       Subject: l.Message.Headers.Subject,
                                       DateMail: l.Message.Headers.DateSent,
                                       Text: l.Message.MessagePart.MessageParts[0].GetBodyAsText(),
                                       Attach: polfile);
                
                frompol.Add(письмо);
            }

               DBWorker dB = new DBWorker();

            // dB.Connect(dB.connectionString);

            

               foreach (var f in frompol)
               {
                   dB.PutMail(f);

               }
                mw.Disconnect(); // отключаемся от почты
        }
        static byte[] GetFile(string filePath)
        {
            FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);
            byte[] file = reader.ReadBytes((int)stream.Length);
            reader.Close();
            stream.Close();
            File.Delete(filePath);
            return file;
        }
    }
}
