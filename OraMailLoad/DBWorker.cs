using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devart.Data.Oracle;
using Devart.Common;
using System.Data;

namespace OraMailLoad
{

    static class DBContext
    {
        static Logger logger = new Logger();

        public static string connectionString = "user id=orc;password=empty;server=10.3.0.108;direct=True;sid=xe;service name=xe";
        public static OracleConnection Connect(string connectionString)
        {
            
            OracleConnection connection = new OracleConnection(connectionString);
            try
            {
                connection.Open();
                logger.LogMessage("Подключено к базе данных");
            }
            catch (Exception ex)
            {
                logger.LogMessage("Ошибка подключения к базе данных", ex.Message);
            }

            return connection;
        }

        public static OracleConnection conn = Connect(connectionString);
    }


    class DBWorker
    {
        static Logger logger = new Logger();


        static string mainInsert = "INSERT INTO mailfrompol(mailfrom, mailto, subject, datemail, text, seq) VALUES(:mailfrom,:mailto,:subject,:datemail,:text, main_seq.nextval) returning seq into :seq";
        static string fileInsert = "INSERT INTO mailfrompol_file (seq_main,namefile,fileblob,seq) VALUES (:seq_main,:namefile,:fileblob, file_seq.nextval)";

        OracleConnection conn = DBContext.conn;

        private void FitMainParam(OracleCommand cmd, Mail mail)
        {
            cmd.Connection = conn;
            cmd.Parameters.Add("mailfrom", OracleDbType.VarChar, 500).Value = mail.MailFrom;
            cmd.Parameters.Add("mailto", OracleDbType.VarChar, 700).Value = mail.MailTo;
            cmd.Parameters.Add("subject", OracleDbType.VarChar, 250).Value = mail.Subject;
            cmd.Parameters.Add("datemail", OracleDbType.Date).Value = mail.DateMail;
            cmd.Parameters.Add("text", OracleDbType.Clob).Value = mail.Text;
            cmd.Parameters.Add("seq", OracleDbType.Number, 20).Direction = ParameterDirection.Output;
        }

        private void FitFileParam(OracleCommand cmd, decimal seq,  MailFile mf)
        {
          cmd.Connection = conn;
          cmd.Parameters.Add("seq_main", OracleDbType.Number, 20).Value = seq;
          cmd.Parameters.Add("namefile", OracleDbType.VarChar, 300).Value = mf.FileName;
          cmd.Parameters.Add("fileblob", OracleDbType.Blob).Value = mf.FileBlob;
        }


        public bool PutMail(Mail mail)
        {
            OracleCommand commandMain = new OracleCommand(mainInsert);
            
            FitMainParam(commandMain, mail);
            try
            {
                commandMain.ExecuteNonQuery();
                logger.LogMessage("Вставлено в основную таблицу");

                // If succrssfully inserted into main table, then we continue this job and putting files into another table.

                OracleCommand commandFile = new OracleCommand(fileInsert);
              

                foreach (var f in mail.Attach)
                {

                    FitFileParam(commandFile, (decimal)commandMain.Parameters["seq"].Value, f);

                    try
                    {
                        commandFile.ExecuteNonQuery();
                        logger.LogMessage("Вставлено таблицу с файлами " + f.FileName);
                    }
                    catch (Exception ex)
                    {
                        logger.LogMessage("Ошибка вставки в таблицу с файлами ", ex.Message);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogMessage("Ошибка вставки в основную таблицу ", ex.Message);
                return false;
            }

            return true;
        }


    }
}
