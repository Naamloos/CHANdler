using SetupUtility.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SetupUtility
{
    public class Program
    {
        private static string Currentdir { get; set; }
        public static async Task Main(string[] args)
        {
            #region Directory Stuff
            Currentdir = Directory.GetCurrentDirectory();
            Currentdir = Path.GetFullPath(Path.Combine(Currentdir, "..", "..", "..", "..", "Chandler"));
            //currentdir = $"{currentdir.Substring(0, currentdir.IndexOf("SetupUtility"))}\\Chandler";

            var resfolderpath = Path.Combine(Currentdir, "res");
            var conffolderpath = Path.Combine(Currentdir, "Data");
            var conffilepath = Path.Combine(Currentdir, "Data", "config.json");

            if (!Directory.Exists(resfolderpath))
                Directory.CreateDirectory(resfolderpath);

            if (!Directory.Exists(conffolderpath))
                Directory.CreateDirectory(conffolderpath);

            if (File.Exists(conffilepath))
            {
                Console.WriteLine("Configuration File already exists, you can now run the main Chandler program");
                Console.ReadKey();
                Environment.Exit(0);
            }
            #endregion
            else
            {
                #region Configuration
                #region Site Configuration
                var siteconf = new SiteConfig();
                Console.WriteLine("---Site Configuration---");
                Console.Write("Please Enter a name for the site: ");
                siteconf.SiteName = Console.ReadLine();

                Console.Write("\nPlease Enter the source of the site logo: ");
                var logo = Console.ReadLine();
                string ex = await CheckAndSaveFileAsync(logo, "logo");
                siteconf.SiteLogo = $"/res/logo.{ex}";

                Console.Write("\nPlease Enter the source of the site favicon: ");
                var fav = Console.ReadLine();
                await CheckAndSaveFileAsync(fav, "favicon");
                siteconf.SiteLogo = $"/res/logo.jpg";

                Console.Write("\nPlease Enter a default password for all site threads (must be at least 8 characters): ");
                var defpass = Console.ReadLine();
                while (defpass.Length < 8)
                {
                    Console.WriteLine("\nThe password must be at least 8 characters long");
                    Console.Write("Please Enter a default password for all site threads: ");
                    defpass = Console.ReadLine();
                }
                siteconf.DefaultPassword = defpass;

                Console.Write("\nPlease Enter the base url of the server: ");
                var baseurl = Console.ReadLine();
                while (!Uri.IsWellFormedUriString(baseurl, UriKind.Absolute))
                {
                    Console.WriteLine("\nPlease enter a valid url");
                    Console.Write("\nPlease Enter the base url of the server: ");
                    baseurl = Console.ReadLine();
                }
                siteconf.BaseUrl = baseurl;
                #endregion
                Console.Clear();
                #region Database Setup
                Console.WriteLine("---Database Setup---");
                Console.Write("Please enter the connection string for the database: ");
                var constr = Console.ReadLine();

                Console.WriteLine($"0 = PostgreSQL, 1 = SQLite, 2 = In Memory, 3 = SQL Server");
                Console.Write("\nPlease enter the database provider: ");
                var dbprovstr = Console.ReadLine();
                while (!Enum.TryParse(typeof(DatabaseProvider), dbprovstr, out _))
                {
                    Console.Write("\nPlease enter a valid database provider: ");
                    dbprovstr = Console.ReadLine();
                }
                var dbprov = (DatabaseProvider)Enum.Parse(typeof(DatabaseProvider), dbprovstr);
                #endregion
                Console.Clear();
                #region Default Admins
                var defadmin = new List<DefaultAdmin>();
                Console.WriteLine("---Default Admins---");
                Console.Write("Would you like to add default admin accounts? (y/n): ");
                var ans = Console.ReadLine().ToLower();
                CheckAnswer(ans);
                if (ans == "y")
                {
                    while (true)
                    {
                        var currdefadmin = new DefaultAdmin();
                        Console.Write("\nPlease enter a Username for the admin: ");
                        currdefadmin.Username = Console.ReadLine();
                        Console.Write("\nPlease enter an Email for the admin: ");
                        currdefadmin.Email = Console.ReadLine();
                        Console.Write("\nPlease enter a Password for the admin: ");
                        currdefadmin.Password = Console.ReadLine();
                        defadmin.Add(currdefadmin);
                        Console.Write("\nWould you like to add another account? (y/n): ");
                        var anst = Console.ReadLine().ToLower();
                        CheckAnswer(anst);
                        if (anst == "n") break;
                    }
                }
                #endregion
                Console.Clear();
                #region Discord OAuth
                var dauth = new DiscordOAuthSettings()
                {
                    RedirectUri = siteconf.BaseUrl
                };
                Console.WriteLine("---Discord OAuth---");
                Console.Write("Would you like to add signing in with discord? (y/n): ");
                var dans = Console.ReadLine().ToLower();
                CheckAnswer(dans);
                if (dans == "y")
                {
                    Console.Write("\nPlease Enter the Client Id: ");
                    var clieid = Console.ReadLine();
                    while (!ulong.TryParse(clieid, out _))
                    {
                        Console.Write("\nPlease Enter a valid Id: ");
                        clieid = Console.ReadLine();
                    }
                    dauth.ClientId = ulong.Parse(clieid);

                    Console.Write("\nPlease Enter the Client Secret: ");
                    dauth.ClientSecret = Console.ReadLine();
                }
                else dauth = null;
                #endregion
                Console.Clear();
                #endregion

                var buff = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new ServerConfig()
                {
                    ConnectionString = constr,
                    DefaultAdminUsers = defadmin,
                    DiscordOAuthSettings = dauth,
                    Provider = dbprov,
                    SiteConfig = siteconf
                }, Formatting.Indented));
                var file = File.Create(conffilepath, buff.Length, FileOptions.None);
                await file.WriteAsync(buff, 0, buff.Length);
                Console.WriteLine("Configuration File created, you can now run the main Chandler program");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        public static void CheckAnswer(string input)
        {
            while (input != "y" && input != "n")
            {
                Console.Write("\nPlease answer with 'y' for Yes, and 'n' for No: ");
                input = Console.ReadLine().ToLower();
            }
        }

        public static async Task<string> CheckAndSaveFileAsync(string input, string filename)
        {
            while (true)
            {
                string extension = "jpg";
                if (input.Length > 3) extension = input.Substring(input.Length - 3, 3);
                var resfilepath = Path.Combine(Currentdir, "res", $"{filename}.{extension}");

                if (Uri.IsWellFormedUriString(input, UriKind.Absolute))
                {
                    Console.WriteLine("Saving file");
                    using var http = new HttpClient();
                    using var stream = await http.GetStreamAsync(input);
                    using var fs = new FileStream(resfilepath, FileMode.Create);
                    await stream.CopyToAsync(fs);
                    return extension;
                }
                else if (File.Exists(input))
                {
                    Console.WriteLine("Saving file");
                    var fs = new FileStream(input, FileMode.Open);
                    var nfs = File.Create(resfilepath, (int)fs.Length, FileOptions.None);
                    await fs.CopyToAsync(nfs);
                    return extension;
                }
                else
                {
                    Console.Write("\nPlease enter a valid source: ");
                    input = Console.ReadLine();
                }
            }
        }
    }
}
