using System;

using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using HtmlAgilityPack;

internal class MailBox
{
    internal void SendMail()
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Baga", "@gmail.com"));
        email.To.Add(new MailboxAddress("Bagylan Kadirbay", "@gmail.com"));

        email.Subject = "SMTP C# sending email";
        email.Body = GenerateMessageBody();

        using (var smtp = new SmtpClient())
        {
            smtp.Connect("smtp.gmail.com", 587);

            smtp.Authenticate("@gmail.com", "*********");


            smtp.Send(email);
            smtp.Disconnect(true);
        }

    }

    private MimeEntity GenerateMessageBody()
    {
        HtmlWeb web = new HtmlWeb();
        DateTime date = DateTime.Today;
        string month = Convert.ToString(date.Month);
        string day = Convert.ToString(date.Day);
        string year = Convert.ToString(date.Year);
        string url = "https://www.basketball-reference.com/boxscores/?month="+month+"&day="+day+"&year="+year;
        HtmlDocument document = web.Load(url);

        //load liga
        var liga = document.DocumentNode.SelectSingleNode("//th[@class=' poptip sort_default_asc left']");
        var eastern = liga.InnerText.Trim();

        //load top
        List< List<string> > data = document.DocumentNode.SelectSingleNode("//table[@class='suppress_all sortable stats_table']")
                    .Descendants("tr")
                    .Where(tr => tr.Elements("td").Count()>1)
                    .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                    .ToList();


        TextPart text = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = "<b> Top team at the "+ eastern + " liga is "+ data[0] + " </b>"
        };
        return text;
    }
}