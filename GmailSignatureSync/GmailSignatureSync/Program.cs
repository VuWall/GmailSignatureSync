using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.GData.Apps;
using Google.GData.Apps.GoogleMailSettings;
using Google.GData.Client;
using Google.GData.Extensions;

namespace GmailSignatureSync
{
    class Program
    {
        private static List<User> Users = new List<User>();

        static void Main(string[] args)
        {
            var service = CreateGoogleMailService();

            PopulateUsersToUpdate();
            foreach (User user in Users)
            {
                var signature = CreateSignature(user);
                SendToGoogle(user, signature, service);
            }

            Console.WriteLine("Update Completed. Press enter to quit.");
            Console.ReadLine();
        }

        private static GoogleMailSettingsService CreateGoogleMailService()
        {
            // Get your service account email from Google Developer's Console
            // Read more: https://developers.google.com/identity/protocols/OAuth2ServiceAccount
            const string SERVICE_ACCT_EMAIL = "<YOUR SERVICE KEY>@developer.gserviceaccount.com";
            //Generate your .p12 key in the Google Developer Console and associate it with your project. 
            var certificate = new X509Certificate2("Key.p12", "notasecret", X509KeyStorageFlags.Exportable);

            var serviceAccountCredentialInitializer = new ServiceAccountCredential.Initializer(SERVICE_ACCT_EMAIL)
            {
                User = "oss@vuwall.com", // A user with administrator access. 
                Scopes = new[] { "https://apps-apis.google.com/a/feeds/emailsettings/2.0/" }
            }.FromCertificate(certificate);

            var credential = new ServiceAccountCredential(serviceAccountCredentialInitializer);
            if (!credential.RequestAccessTokenAsync(System.Threading.CancellationToken.None).Result)
                throw new InvalidOperationException("Access token failed.");

            var requestFactory = new GDataRequestFactory(null);
            requestFactory.CustomHeaders.Add("Authorization: Bearer " + credential.Token.AccessToken);

            // Replace the name of your domain and the Google Developer project you created...
            GoogleMailSettingsService service = new GoogleMailSettingsService("vuwall.com", "signatures");
            service.RequestFactory = requestFactory;

            return service;
        }

        private static void PopulateUsersToUpdate()
        {
            Users.Add(new User("charris", "Charles HARRIS", "Product Manager", "+1 555-444-3333", "+1 555-000-2222", "Skypster01"));
            Users.Add(new User("jmoriarty", "James MORIARTY", "Software Developer", "+1 555-444-3333", "+1 555-111-3333", "SupportSkype22"));
            Users.Add(new User("tbuchanan", "Thomas BUCHANAN", "Marketing Coordinator", "+1 555-444-3333", "+1 555-333-9999", "Chatterbox99"));
        }

        private static string CreateSignature(User user)
        {
            var header = @"<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=UTF-8' /></HEAD>";

            var body = string.Format("<BODY><P><STRONG>" +
                                     "<FONT style='FONT-SIZE: 14pt'>{0}</FONT><BR>" +
                                     "<FONT style='FONT-SIZE: 12pt'>{1}</FONT><BR>" +
                                     "<FONT style='FONT-SIZE: 10pt'>VuWall Technology, Inc.</FONT><BR>" +
                                     "</STRONG>" +
                                     "<STRONG>T:</STRONG> {2} {3} {4}<BR>", //T: ###-#### | C: ###-#### | Skype: <NAME>
                                     user.DisplayName,
                                     user.JobTitle,
                                     user.PhoneNumber,
                                     // Only add cell number / Skype name if one has been defined.
                                     String.IsNullOrWhiteSpace(user.CellNumber) ? "" : string.Format("| <STRONG>C:</STRONG> {0}", user.CellNumber),
                                     String.IsNullOrWhiteSpace(user.SkypeName) ? "" : string.Format("| <STRONG>Skype:</STRONG> {0}", user.SkypeName));

            var footer = @" <A title=VuWall href='http://www.vuwall.com'><IMG border=0 hspace=0 alt=VuWall src='http://vuwall.com/wp-content/uploads/2013/07/EmailLogoNoBorder.png'></A>&nbsp;
                            <A title=LinkedIn href='https://www.linkedin.com/company/vuwall'><IMG border=0 hspace=0 alt=LinkedIn src='http://vuwall.com/wp-content/uploads/2013/07/LIIcon.png'></A>&nbsp;
                            <A title=YouTube href='https://www.youtube.com/user/VuWall'><IMG border=0 hspace=0 alt=YouTube src='http://vuwall.com/wp-content/uploads/2013/07/YTIcon.png'></A>&nbsp;
                            <A title=Facebook href='https://www.facebook.com/vuwall'><IMG border=0 hspace=0 alt=Facebook src='http://vuwall.com/wp-content/uploads/2013/07/FBIcon.png'></A>&nbsp;
                            <A title=Twitter href='https://www.twitter.com/VuWall'><IMG border=0 hspace=0 alt=Twitter src='http://vuwall.com/wp-content/uploads/2013/07/TwitterIcon.png'></A>
                            </P></BODY></HTML>";

            var fullSignature = header + body + footer;
            return fullSignature;
        }

        private static void SendToGoogle(User user, string signature, GoogleMailSettingsService service)
        {
            try
            {
                Console.Write("Updating user: " + user.AccountName + "...");
                service.UpdateSignature(user.AccountName, signature);
                Console.WriteLine("Success.");
            }
            catch (GDataRequestException gdre)
            {
                Console.WriteLine("Could not update user: " + user.AccountName + " - Reason: " + gdre.ResponseString);
            }
        }
    }
}
