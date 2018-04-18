using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spreadsheet
{
    
    partial class AboutBox1 : Form
    {
        public AboutBox1()
        {
            InitializeComponent();
            Versionnumber versionnumber = Versionnumber.Instance;

            this.Text = String.Format("About {0}", AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
            this.labelCopyright.Text = AssemblyCopyright;
            this.labelCompanyName.Text = AssemblyCompany;
            this.textBoxDescription.Text = AssemblyDescription;
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                
                return "The Excel Killer";
            }
        }

        public string AssemblyVersion
        {
            get
            {
                Versionnumber versionnumber = Versionnumber.Instance;
                return versionnumber.version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                
                return "This Product is intended to be the superior to Microsofts less superior prototype of this software. \n " +
                    "You are about to experience magic during the use of the best program ever.";
            }
        }

        public string AssemblyProduct
        {
            get
            {
                
                return "CptS 321 SpreadSheet";
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                
                return "";
            }
        }

        public string AssemblyCompany
        {
            get
            {
                
                return "ChristopherYoung.Inc";
            }
        }
        #endregion

        private void labelProductName_Click(object sender, EventArgs e)
        {

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
    public class Versionnumber
    {
        private static Versionnumber instance;

        private Versionnumber() { }
        double _version;
        public double version
        {
            get
            {
                return _version;
            }
            set
            {
                if (_version != value && _version < value)
                {
                    _version = value;
                }
            }
        }
        public static Versionnumber Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Versionnumber();
                }
                return instance;
            }
        }
    }
}
