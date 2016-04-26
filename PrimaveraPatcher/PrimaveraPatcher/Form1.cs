///@brief Compares current installed patch of primavera to
///       latest version on Oracle website. Allows user to
///       be directed to page to download latest version
///       (optional feature)
///@version 16.04.26

using System;
using System.Linq;
using System.Windows.Forms;

namespace PrimaveraPatcher
{
    public partial class PrimaveraPatcher : Form
    {
        //Holds the current patch number
        protected decimal currentPatch;
        //Holds latest patch number
        protected decimal latestPatch;
        //Holds max patch version accepted
        private readonly decimal maxPatchVer = 13.0m;
        //String to the update page
        private readonly string updatePage =
            "http://www.oracle.com/technetwork/apps-tech/primaverapcm-087479.html";


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
            outputLabel.Text = "";  //Empties the output label
            updateLink.Text = "";   //Empties the link label

            try
            {
                currentPatch = decimal.Parse(getCurrentPatch());   //Holds current used patch
                latestPatch = decimal.Parse(getLatestPatch());     //Holds latest used patch

                //If the current patch is less than the latest patch (using decimal for accuracy)
                if (currentPatch < latestPatch)
                {
                    updateAvailable();  //Update must be available
                }
                else if (currentPatch == latestPatch)
                {
                    outputLabel.Text = "Currently using patch #" + currentPatch.ToString();

                    //Updates closeButton to be appropriate
                    closeButton.Text = "Close updater";
                    closeButton.Visible = true;
                }
                else
                {
                    outputLabel.Text = "#DEBUG#\nSHOULD NEVER GET HERE!";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Searches Oracle database for latest patch for Primavera
        /// </summary>
        /// <returns>Returns string with patch number</returns>
        protected string getLatestPatch()
        {
            //Holds latest found patch
            string latestPatch = "";
            
            //Creates string that will hold the HTML file
            string pageHTML = "";
            //We will ignore the first result of our search because it is not the patch number
            bool ignoredFirst = false;

            //Create a web browser
            WebBrowser wBrowser = new WebBrowser();
            //Navigate to the Oracle URL
            wBrowser.Navigate(updatePage);

            //When the document is completely loaded we copy the HTML into pageHTML for parsing
            while ( wBrowser.ReadyState != WebBrowserReadyState.Complete )
            {
                Application.DoEvents();
            }

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
                        if (ignoredFirst == true && decimal.Parse(tokens[i-1].Substring(4)) < maxPatchVer + 1 )
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
                }
            }
            
            return latestPatch; //Return patch number
        }

        /// <summary>
        /// Gets current installed patch
        /// </summary>
        /// <returns>Returns string of patch</returns>
        protected string getCurrentPatch()
        {
            return "13.0"; //Return patch number
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
            updateLink.Links.Add(6, 4, updatePage);

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
