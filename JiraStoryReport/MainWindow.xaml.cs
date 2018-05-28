using System;
using System.Collections;
using System.ComponentModel;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;

namespace JiraStoryReport
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CookieContainer cookies;
        private string JSESSIONID_NAME;
        private string JSESSIONID_VALUE;
        private string domainname;
        private ArrayList stories;
        private BackgroundWorker worker;

        public MainWindow()
        {
            InitializeComponent();
            this.cookies = new CookieContainer();
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            this.UsernameTextBox.Text = userName;
            this.domainname = "GER.CORP.INTEL.COM";

            this.worker = new BackgroundWorker();

            this.worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            this.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                this.DoWorkButton.Content = "Task Cancelled.";
            }
            else if (e.Error != null)
            {
                this.DoWorkButton.Content = "Error while performing background operation.";
            }
            else
            {
                this.DoWorkButton.Content = "Task Completed...";
            }

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.StatusBar.Content = "Done ";
            }));


            this.UpdateHtmlViewer();
            this.DoWorkButton.IsEnabled = true;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this.Login() == false)
            {
                MessageBox.Show("Unable to login check your credentials");
                return;
            }

            var users = this.GetUsers();

            if (users.Count == 0)
            {
                MessageBox.Show("Unable to build a list of users to query, check groups, names and idsids");
                return;
            }

            this.BuildStoriesList(users);
        }

        private bool Login()
        {
            try
            {
                var username = "";

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    username = this.UsernameTextBox.Text;
                }));

                System.Threading.Thread.Sleep(500);


                if (username.Contains("\\"))
                {
                    var split = username.Split('\\');
                    username = split[1];
                }

                var password = this.PasswordTextBox.Password.ToString();
                string postData = "{ \"username\": \"" + username + "\", \"password\": \"" + password + "\" }";

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.StatusBar.Content = "Attempting to login...";
                }));

                Uri loginUri = new Uri("https://jira01.devtools.intel.com/rest/auth/1/session");
                HttpWebRequest requestLogin = (HttpWebRequest)WebRequest.Create(loginUri);
                requestLogin.Method = "POST";
                byte[] data = Encoding.ASCII.GetBytes(postData);
                requestLogin.ContentType = "application/json";
                requestLogin.ContentLength = data.Length;
                requestLogin.CookieContainer = this.cookies;
                requestLogin.AllowAutoRedirect = true;

                Stream requestStream = requestLogin.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();

                HttpWebResponse loginHttpWebResponse = (HttpWebResponse)requestLogin.GetResponse();
                Console.WriteLine(((HttpWebResponse)loginHttpWebResponse).StatusDescription);
                Stream dataStream = loginHttpWebResponse.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                Console.WriteLine(responseFromServer);
                reader.Close();
                loginHttpWebResponse.Close();

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.StatusBar.Content = "Reading login response...";
                }));

                Regex r1 = new Regex("\"name\":\"(.*?)\",\"value\":\"(.*?)\"", RegexOptions.IgnoreCase);
                Match m1 = r1.Match(responseFromServer);

                if (m1.Success)
                {
                    this.JSESSIONID_NAME = m1.Groups[1].Value;
                    this.JSESSIONID_VALUE = m1.Groups[2].Value;
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.StatusBar.Content = e.Message;
                }));
                return false;
            }
        }

        private ArrayList getGroupMembers(string group)
        {
            ArrayList samaccoutnames = new ArrayList();

            try
            {
                var groupName = group;

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.StatusBar.Content = "Getting groups users: " + group;
                }));

                PrincipalContext ctx = new PrincipalContext(ContextType.Domain, this.domainname);
                GroupPrincipal grp = GroupPrincipal.FindByIdentity(ctx, IdentityType.Name, groupName);

                if (grp != null)
                {
                    foreach (Principal p in grp.GetMembers(true))
                    {
                        samaccoutnames.Add(p.SamAccountName);
                    }

                    grp.Dispose();
                    ctx.Dispose();
                }
            }
            catch (Exception e)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.StatusBar.Content = e.Message;
                }));
            }

            return samaccoutnames;
        }

        private string getUserIdSIDbyName(string user)
        {
            try
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.StatusBar.Content = "Getting user idsid: " + user;
                }));

                PrincipalContext ctx = new PrincipalContext(ContextType.Domain, this.domainname);
                UserPrincipal upr = UserPrincipal.FindByIdentity(ctx, IdentityType.Name, user);

                if (upr != null)
                {
                    return upr.SamAccountName;
                }
            }
            catch (Exception e)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.StatusBar.Content = e.Message;
                }));
            }

            return null;
        }

        private ArrayList GetUsers()
        {
            ArrayList samaccoutnames = new ArrayList();

            try
            {    
                var groups = new TextRange(this.GroupTextBox.Document.ContentStart, this.GroupTextBox.Document.ContentEnd).Text.Trim();

                if (groups.Length > 0)
                {
                    string[] names;
                    if (groups.Contains(System.Environment.NewLine))
                    {
                        names = groups.Split(System.Environment.NewLine.ToCharArray());
                    }
                    else
                    {
                        names = new string[1] { groups };
                    }

                    foreach (string x in names)
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            this.StatusBar.Content = "Getting group members: " + x;
                        }));

                        var sids = this.getGroupMembers(x);
                        foreach (string y in sids)
                        {
                            if (samaccoutnames.Contains(y) == false)
                            {
                                samaccoutnames.Add(y);
                            }
                        }
                    }
                }

                var sams = new TextRange(this.IDSIDTextBox.Document.ContentStart, this.IDSIDTextBox.Document.ContentEnd).Text.Trim();

                if (sams.Length > 0)
                {
                    string[] names;
                    if (sams.Contains(System.Environment.NewLine))
                    {
                        names = sams.Split(System.Environment.NewLine.ToCharArray());
                    }
                    else
                    {
                        names = new string[1] { sams };
                    }

                    foreach (string x in names)
                    {
                        if (samaccoutnames.Contains(x) == false)
                        {
                            samaccoutnames.Add(x);
                        }
                    }
                }

                var users = new TextRange(this.NameTextBox.Document.ContentStart, this.NameTextBox.Document.ContentEnd).Text.Trim();

                if (users.Length > 0)
                {
                    string[] names;
                    if (users.Contains(System.Environment.NewLine))
                    {
                        names = users.Split(System.Environment.NewLine.ToCharArray());
                    }
                    else
                    {
                        names = new string[1] { users };
                    }

                    foreach (string x in names)
                    {
                        var sid = this.getUserIdSIDbyName(x);
                        if (sid != null)
                        {
                            if (samaccoutnames.Contains(sid) == false)
                            {
                                samaccoutnames.Add(sid);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.StatusBar.Content = e.Message;
                }));
            }

            return samaccoutnames;
        }

        private void BuildStoriesList(ArrayList users)
        {
            foreach (var user in users)
            {
                try
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.StatusBar.Content = "Getting user stories: " + user;
                    }));

                    Uri apiquery = new Uri("https://jira01.devtools.intel.com/rest/api/2/search?jql=assignee=" + user + "%20and%20Sprint%20in%20openSprints()%20and%20Status%20!=%20Closed");
                    HttpWebRequest apirequest = (HttpWebRequest)WebRequest.Create(apiquery);
                    apirequest.Headers.Add("cookie", this.JSESSIONID_NAME + "=" + this.JSESSIONID_VALUE);
                    apirequest.ContentType = "application/json";
                    apirequest.CookieContainer = this.cookies;
                    apirequest.AllowAutoRedirect = true;

                    HttpWebResponse apiHttpWebResponse = (HttpWebResponse)apirequest.GetResponse();
                    Console.WriteLine(((HttpWebResponse)apiHttpWebResponse).StatusDescription);
                    Stream dataStream = apiHttpWebResponse.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    Console.WriteLine(responseFromServer);
                    reader.Close();
                    apiHttpWebResponse.Close();

                    MatchCollection matches = Regex.Matches(responseFromServer, "key\":\\s*\"(.*?)\".*?null,\"summary\":\"(.*?)\".*?customfield_11204\":(.*?),", RegexOptions.IgnoreCase);

                    // Report on each match.
                    foreach (Match match in matches)
                    {
                        GroupCollection groups = match.Groups;
                        this.stories.Add(new string[] { user.ToString(), groups[1].Value, groups[2].Value, groups[3].Value });
                        Console.WriteLine("{0} : {1} : {2}", groups[1].Value, groups[2].Value, groups[3].Value);
                    }
                }
                catch (Exception e)
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.StatusBar.Content = e.Message;
                    }));
                }
            }
        }

        private void UpdateHtmlViewer()
        {
            string body = "<!DOCTYPE html><html><head><style>table, th, td { border:1px solid #CCCCCC; }</style></head><body><table width='900'><tr><th width='100'>User</th><th width='120'>Jira #</th><th width='550'>Title</th><th width='80'>Points</th><th width='*'>Availability</th></tr>" + Environment.NewLine;

            var c_user = "";
            var c_count = 1;
            var rows = "";

            foreach (var story in this.stories)
            {
                string[] storydetails = (string[])story;

                if (c_user.CompareTo(storydetails[0]) != 0)
                {
                    rows = rows.Replace("SPAN", c_count.ToString());
                    body += rows;
                    //rows = "<tr><td colspan='3' style='background-color: #cccccc'>&nbsp;</td></tr>" + Environment.NewLine;
                    rows = "<tr><td rowspan='SPAN'>" + storydetails[0] + "</td><td>" + storydetails[1] + "</td><td>" + storydetails[2] + "</td><td>" + storydetails[3] + "</td><td rowspan='SPAN'>&nbsp;</td></tr>" + Environment.NewLine;
                    c_count = 1;
                    c_user = storydetails[0];
                    continue;
                }

                rows += "<tr><td>" + storydetails[1] + "</td><td>" + storydetails[2] + "</td><td>" + storydetails[3] + "</td></tr>" + Environment.NewLine;
                c_count++;
            }

            rows = rows.Replace("SPAN", c_count.ToString());
            body += rows;

            body += "</body></html>" + Environment.NewLine;

            Console.WriteLine(body);

            this.WebBrowser.NavigateToString(body);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.stories = new ArrayList();
            this.DoWorkButton.IsEnabled = false;
            this.worker.RunWorkerAsync();
        }
    }
}
