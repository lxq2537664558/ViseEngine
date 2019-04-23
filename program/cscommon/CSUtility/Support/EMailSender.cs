using System.Net.Mail;

namespace CSUtility.Support
{

    public class EMailSender
    {
        public static bool SendMailUseGmail(string tarMail, string ccMail, string from, string body, string subject="硬件信息") 
        { 
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage(); 
            msg.To.Add(tarMail); 
            /* 
            * msg.To.Add("b@b.com"); 
            * msg.To.Add("b@b.com"); 
            * msg.To.Add("b@b.com");可以发送给多人 
            */
            if (!string.IsNullOrEmpty(ccMail))
                msg.CC.Add(tarMail); 
            /* 
            * msg.CC.Add("c@c.com"); 
            * msg.CC.Add("c@c.com");可以抄送给多人 
            */
            msg.From = new MailAddress("johnson3d@163.com", from, System.Text.Encoding.UTF8);
            
            /* 上面3个参数分别是发件人地址（可以随便写），发件人姓名，编码*/
            msg.Subject = subject;//邮件标题 
            msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码 
            msg.Body = body;//邮件内容 
            msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码 
            msg.IsBodyHtml = false;//是否是HTML邮件 
            msg.Priority = MailPriority.High;//邮件优先级 
            SmtpClient client = new SmtpClient(); 
            //client.Credentials = new System.Net.NetworkCredential("jonhsonchina@gmail.com", "123imgod"); 
            ////上述写你的GMail邮箱和密码 
            //client.Port = 587;//Gmail使用的端口 
            //client.Host = "smtp.gmail.com"; 

            //client.Credentials = new System.Net.NetworkCredential("18611126643@163.com", "61816498aa");
            //client.Credentials = new System.Net.NetworkCredential("etanggamebin@163.com", "123imgod");
            client.Credentials = new System.Net.NetworkCredential("johnson3d@163.com", "7824692");
            //上述写你的GMail邮箱和密码 
            client.Port = 25;//Gmail使用的端口 
            client.Host = "smtp.163.com"; 

            client.EnableSsl = true;//经过ssl加密 
            object userState = msg; 
            try 
            { 
                client.SendAsync(msg, userState); 
                return true;
            } 
            catch (System.Net.Mail.SmtpException) 
            { 
                return false;
            } 
        }
    }
}
