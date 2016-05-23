///@brief Compares current installed patch of primavera to
///       latest version on Oracle website. Allows user to
///       be directed to page to download latest version
///       (optional feature)
///@version 2016.05.23

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using Logger;

namespace PrimaveraPatcher
{
    public partial class PrimaveraPatcher : Form
    {
        //Holds the current patch number
        protected decimal currentPatch;
        //Holds latest patch number
        protected decimal latestPatch;

        //All settings from app.config
        private static string aMaxPatchVer;
        private static string aUpdatePage;
        private static string aMailServer;
        private static string aFrom;
        private static string aTo;
        private static string aCC;
        private static string aSubject;
        private static string aDebug;

        //Log file
        private Log logFile;


        /// <summary>
        /// Class Constructor
        /// </summary>
        public PrimaveraPatcher()
        {
            InitializeComponent();
        }

        /// <summary>
        /// On load fetches the information for current patch and latest patch
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void PrimaveraPatcher_Load(object sender, EventArgs e)
        {
            if (GetConfigSettings(ref logFile)) {

                //Set up the log file
                logFile = new Log();

                //First log entry for when program started
                StringBuilder firstLog = new StringBuilder();

                logFile.AddToLog("Starting PrimaveraPatcher...");

                outputLabel.Text = "";  //Empties the output label
                updateLink.Text = "";   //Empties the link label

                try
                {
                    currentPatch = decimal.Parse(getCurrentPatch(ref logFile));   //Holds current used patch
                    latestPatch = decimal.Parse(getLatestPatch(ref logFile));     //Holds latest used patch

                    //If the current patch is less than the latest patch (using decimal for accuracy)
                    if (currentPatch < latestPatch)
                    {
                        updateAvailable();  //Update must be available
                    }
                    else if (currentPatch == latestPatch)
                    {
                        //Shows current patch number
                        outputLabel.Text = "Currently using patch #" + currentPatch.ToString();

                        //Updates closeButton to be appropriate
                        closeButton.Text = "Close updater";
                        closeButton.Visible = true;
                    }
                    else
                    {
                        //Not possible unless error
                        outputLabel.Text = "#DEBUG#\nSHOULD NEVER GET HERE!";
                        logFile.AddToLog("Current patch newer than latest",true);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    logFile.AddToLog(ex.Message, true);
                }

                //Only save and mail if debugging or if there is an error
                if (Convert.ToBoolean(aDebug) || logFile.GetLog().Contains("ERROR") )
                {
                    //Mail based on app settings
                    logFile.MailLog(aMailServer, aFrom, aTo, aSubject);
                    //Save log locally
                    logFile.SaveLog();
                }
            } else {
                //Show that we had an error with the config file
                MessageBox.Show("Error in getting config");
                logFile.SaveLog();
                this.Close();
            }
        }

        /// <summary>
        /// Searches Oracle database for latest patch for Primavera
        /// </summary>
        /// <returns>Returns string with patch number</returns>
        protected string getLatestPatch(ref Log logFile)
        {
            //Holds latest found patch
            string latestPatch = "";

            //Creates string that will hold the HTML file
            string pageHTML = "";
            //We will ignore the first result of our search because it is not the patch number
            bool ignoredFirst = false;

            //Create a web browser
            WebBrowser wBrowser = new WebBrowser();

            //Surpress script error
            wBrowser.ScriptErrorsSuppressed = true;

            //Navigate to the Oracle URL
            wBrowser.Navigate(aUpdatePage);

            //When the document is completely loaded we copy the HTML into pageHTML for parsing
            while (wBrowser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }

            //Save the entire webpage into pageHTML
            pageHTML = wBrowser.DocumentText;

            //we create a new list of decimals with all the patch numbers
            //COMMENTED OUT. Currently only taking the highest patch #
            //List<decimal> patches = new List<decimal>();

            //Split the HTML into an array of string based on spaces
            string[] tokens = pageHTML.Split(' ');

            //While we have less than the number of splits AND the latest Patch length is less than one (not found yet)
            for (int i = 0; i < tokens.Count() && latestPatch.Length < 1; i++)
            {
                //In case of errors we use try-catch
                try
                {
                    //First the token that starts with "Documentation" ignoring upper/lower case
                    if (tokens[i].StartsWith("Documentation", System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        //If this is NOT the first time we found a match AND this patch number is within the patch version
                        //allowed (ex: 13 max version would allow 13.4 but not 14.0 )
                        //Keep ignoredFirst so that we don't parse words as the first instance of documentation is instructions
                        if (ignoredFirst == true && decimal.Parse(tokens[i - 1].Substring(4)) < decimal.Parse(aMaxPatchVer) + 1m)
                        {
                            //We save the number as it is latest patch
                            latestPatch = tokens[i - 1].Substring(4);
                        }
                        else
                        {
                            //Else we ignore first occurance and set ignoredFirst to true
                            ignoredFirst = true;
                        }
                    }

                }
                catch (Exception ex)
                {
                    //Error handling
                    MessageBox.Show(ex.Message);
                    logFile.AddToLog(ex.Message, true);
                }
            }

            return latestPatch; //Return patch number
        }

        /// <summary>
        /// Gets current installed patch
        /// ***Needs actual way to get current patch***
        /// </summary>
        /// <returns>Returns string of patch</returns>
        protected string getCurrentPatch(ref Log logFile)
        {
            return "13.0"; //Return patch number
        }

        /// <summary>
        /// Get app data and return whether we were successful or not
        /// </summary>
        /// <param name="log">Log</param>
        /// <returns>True if successful</returns>
        private static bool GetConfigSettings(ref Log logFile)
        {
            //False until we get everything working
            bool retVal = false;

            // Get core settings. Any failure skips to return false.
            if (
                GetConfigSetting("maxUpdateVer", ref aMaxPatchVer, ref logFile) &&
                GetConfigSetting("updatePage", ref aUpdatePage, ref logFile) &&
                GetConfigSetting("mailserver", ref aMailServer, ref logFile) &&
                GetConfigSetting("from", ref aFrom, ref logFile) &&
                GetConfigSetting("to", ref aTo, ref logFile) &&
                GetConfigSetting("CC", ref aCC, ref logFile) &&
                GetConfigSetting("subject", ref aSubject, ref logFile) &
                GetConfigSetting("debugging", ref aDebug, ref logFile))
            {
                //Successfully got all app data
                retVal = true;
            }

            //Return result
            return retVal;
        }

        /// <summary>
        /// Gets the configuration settings from App.config
        /// </summary>
        /// <param name="aSettingKey">Key to lookup</param>
        /// <param name="aValue">Reference where we save data into</param>
        /// <param name="log">Log</param>
        /// <returns>True if successful</returns>
        private static bool GetConfigSetting(string aSettingKey, ref string aValue, ref Log logFile)
        {
            bool retVal = true; //Holds results. Assumes success

            //Attempt to get the data
            aValue = ConfigurationManager.AppSettings[aSettingKey];

            //If aValue is null (no data from App.config)
            if (aValue == null)
            {
                //Log the key lookup used
                logFile.AddToLog("Error retrieving core " + aSettingKey +
                                    " configuration setting; application cannot start.", true);
                //Could not retrieve info
                retVal = false;
            }

            //Return results
            return retVal;
        }

        /// <summary>
        /// There is a newer version of the software online
        /// </summary>
        private void updateAvailable()
        {
            //UPDATE NEEDED
            //Temporary Text
            outputLabel.Text = "Newer version #" + latestPatch.ToString() + " available";

            //Updates text with link to click
            updateLink.Text = "Click here for update page";
            updateLink.Links.Add(6, 4, aUpdatePage);

            //Updates closeButton to be appropriate
            closeButton.Text = "Update later";
            closeButton.Visible = true;
        }

        /// <summary>
        /// If user decides to click to update then we open it in browser
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Object send with link</param>
        private void updateLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        /// <summary>
        /// Close form
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
