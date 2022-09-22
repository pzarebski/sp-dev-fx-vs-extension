﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Framework.VSIX.Resources;

namespace Framework.VSIX
{
	public partial class NewProjectForm : Form
	{
		private bool formCancel = false;
		private bool commandValid = false;
		private string commandString = String.Empty;

		public NewProjectForm()
		{
			InitializeComponent();
		}

		public void Initialize()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			if (Package.GetGlobalService(typeof(IUIHostLocale)) is IUIHostLocale hostLocale)
			{
				var dlgFont = new UIDLGLOGFONT[] { new UIDLGLOGFONT() };
				hostLocale.GetDialogFont(dlgFont);
				this.Font = Font.FromLogFont(dlgFont[0]);
			}

			// Form settings
			this.Name = Global.Form_Project_Name;
			this.Text = Global.Form_Project_Title;
			this.Icon = Global.Extension;

			// Tab settings
			this.tabProps.Text = Global.Form_PropertyTab_Title;
			this.tabAdv.Text = Global.Form_AdvancedTab_Title;

			// Solution name
			lblSolutionName.Text = Global.Form_SolutionName;
			txtSolutionName.Enabled = false;
			txtSolutionName.TextChanged += SolutionName_TextChanged;

			// Framework
			lblFramework.Text = Global.Form_Framework;
			Dictionary<string, string> cboFrameworkSource = new Dictionary<string, string>
			{
				{ "none", "none" },
				{ "react", "react" },
				{ "knockout", "knockout" }
			};
			cboFramework.DataSource = new BindingSource(cboFrameworkSource, null);
            cboFramework.SelectedIndex = 0;
			cboFramework.DisplayMember = "Value";
			cboFramework.ValueMember = "Key";
			cboFramework.SelectedIndexChanged += Framework_SelectedIndexChanged;

			// Environment
			lblEnvironment.Text = Global.Form_Prompt_Environment_Label;
			Dictionary<string, string> cboEnvironmentSource = new Dictionary<string, string>
			{
				{ "spo", Global.Form_Prompt_Environment_Option1 },
				{ "onprem", Global.Form_Prompt_Environment_Option2 },
                { "onprem19", Global.Form_Prompt_Environment_Option3 }
			};
			cboEnvironment.DataSource = new BindingSource(cboEnvironmentSource, null);
            cboEnvironment.SelectedIndex = 0;
			cboEnvironment.DisplayMember = "Value";
			cboEnvironment.ValueMember = "Key";
			cboEnvironment.SelectedIndexChanged += Environment_SelectedIndexChanged;


			// Component Name
			lblComponentName.Text = Global.Form_ComponentName;
			txtComponentName.TextChanged += ComponentName_TextChanged;

			// Component Description
			lblComponentDescription.Text = Global.Form_ComponentDescription;
			txtComponentDescription.TextChanged += ComponentDescription_TextChanged;

			// Component Type
			lblComponentType.Text = Global.Form_ComponentType;
			Dictionary<string, string> cboComponentTypeSource = new Dictionary<string, string>
			{
				{ "webpart", Global.Form_Prompt_ComponentType_WebPart },
				{ "extension", Global.Form_Prompt_ComponentType_Extension }
			};
			cboComponentType.DataSource = new BindingSource(cboComponentTypeSource, null);
            cboComponentType.SelectedIndex = 0;
			cboComponentType.DisplayMember = "Value";
			cboComponentType.ValueMember = "Key";
			cboComponentType.SelectedIndexChanged += ComponentType_SelectedIndexChanged;

			// Extension type
			lblExtensionType.Visible = false;
			lblExtensionType.Text = Global.Form_ExtensionType;
			cboExtensionType.Visible = false;
			Dictionary<string, string> cboExtensionTypeSource = new Dictionary<string, string>
			{
				{ "ApplicationCustomizer", Global.Form_Prompt_ComponentType_Extension_1 },
				{ "FieldCustomizer", Global.Form_Prompt_ComponentType_Extension_2 },
				{ "ListViewCommandSet", Global.Form_Prompt_ComponentType_Extension_3 }
			};
			cboExtensionType.DataSource = new BindingSource(cboExtensionTypeSource, null);
            cboExtensionType.DisplayMember = "Value";
			cboExtensionType.ValueMember = "Key";
			cboExtensionType.SelectedIndexChanged += ExtensionType_SelectedIndexChanged;

            // Package Manager
            lblPackageManager.Visible = true;
            lblPackageManager.Text = Global.Form_Prompt_PackageManager_Label;
            cboPackageManager.Visible = true;
            Dictionary<string, string> cboPackageManagerSource = new Dictionary<string, string>
            {
                { "npm", "NPM" },
                { "pnpm", "PNPM" },
                { "yarn", "Yarn" }
            };
            cboPackageManager.DataSource = new BindingSource(cboPackageManagerSource, null);
            cboPackageManager.SelectedIndex = 0;
            cboPackageManager.DisplayMember = "Value";
            cboPackageManager.ValueMember = "Key";
            cboPackageManager.SelectedIndexChanged += cboPackageManager_SelectedIndexChanged;

            // Skip Feature Deployment
            cbxSkipFeatureDeployment.Text = Global.Form_Prompt_SkipFeature_Label;
			cbxSkipFeatureDeployment.Checked = false;
			cbxSkipFeatureDeployment.AutoSize = true;
			cbxSkipFeatureDeployment.CheckedChanged += SkipFeatureDeployment_CheckedChanged;
            lblSkipFeatureDeploymentInfo.Text = Global.Form_Label_SkipFeatureDeployment_Info;

			// Skip NPM Install
			cbxSkipInstall.Text = Global.Form_Prompt_SkipInstall_Label;
			cbxSkipInstall.Checked = false;
			cbxSkipInstall.AutoSize = true;
			cbxSkipInstall.CheckedChanged += SkipInstall_CheckedChanged;

            //Domain Isolation
            cbxDomainIsolated.Text = Global.Form_Prompt_Domain_Isolated_Label;
            cbxDomainIsolated.Checked = false;
            cbxDomainIsolated.AutoSize = false;
            cbxDomainIsolated.CheckedChanged += cbxDomainIsolated_CheckedChanged;
            lblIsDomainIsolatedInfo.Text = Global.Form_Label_IsDomainIsolated_Info;

            // Beta Features
            cbxPlusBeta.Text = Global.Form_Prompt_PlusBeta_Label;
            cbxPlusBeta.Checked = false;
            cbxPlusBeta.AutoSize = true;
            cbxSkipInstall.CheckedChanged += CbxSkipInstall_CheckedChanged;

			// Command string
			lblCommandString.Text = Global.Form_CommandString;
			lblCommandDescription.Text = Global.Form_AdvancedTab_CommandDescription;

			// Show window
			cbxShowWindow.Text = Global.Form_ShowCommandWindow;

			// buttons
			btnGenerate.Text = Global.Form_ButtonGenerate;
            btnGenerate.Enabled = commandValid;
            if (commandValid == true)
            {
                btnGenerate.ForeColor = Color.White;
                btnGenerate.BackColor = Color.Green;
            }
			btnCancel.Text = Global.Form_ButtonCancel;
            btnCancel.ForeColor = Color.White;
            btnCancel.BackColor = Color.DarkRed;

			// Footer
			lblFooter.Text = Global.Form_Footer_GeneratorText;
            lblYeomanVersion.Text = Global.Footer_Yeoman_Version;

		}

        #region Control Event Handlers

        private void SkipFeatureDeployment_CheckedChanged(object sender, EventArgs e)
		{
            if (cbxSkipFeatureDeployment.Checked)
            {
                lblSkipFeatureDeploymentInfo.Visible = false;
            }
            else
            {
                lblSkipFeatureDeploymentInfo.Visible = true;
            }

            SetCommandText();
			SetSubmitState();
		}

		private void Environment_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Environment=="onprem")
			{
				cboComponentType.SelectedValue = "webpart";
				cboComponentType.Enabled = false;
			}
			else
			{
				cboComponentType.Enabled = true;
			}
			SetCommandText();
			SetSubmitState();
		}

		private void ExtensionType_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetCommandText();
			SetSubmitState();
		}

		private void ComponentType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (ComponentType == "extension")
			{
				lblExtensionType.Visible = true;
				cboExtensionType.Visible = true;
                cbxDomainIsolated.Visible = false;
                lblIsDomainIsolatedInfo.Visible = false;
			}
			else
			{
				lblExtensionType.Visible = false;
				cboExtensionType.Visible = false;
                cbxDomainIsolated.Visible = true;
                lblIsDomainIsolatedInfo.Visible = true;
			}
			SetCommandText();
			SetSubmitState();
		}

		private void SkipInstall_CheckedChanged(object sender, EventArgs e)
		{
			SetCommandText();
			SetSubmitState();
		}

        private void CbxSkipInstall_CheckedChanged(object sender, EventArgs e)
        {
            SetCommandText();
            SetSubmitState();
        }

        private void ComponentDescription_TextChanged(object sender, EventArgs e)
		{
			SetCommandText();
			SetSubmitState();
		}

		private void ComponentName_TextChanged(object sender, EventArgs e)
		{
			SetCommandText();
			SetSubmitState();
		}

		private void Framework_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetCommandText();
			SetSubmitState();
		}

		private void SolutionName_TextChanged(object sender, EventArgs e)
		{
			SolutionName = txtSolutionName.Text;
			SetCommandText();
			SetSubmitState();
		}

        private void cboPackageManager_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetCommandText();
            SetSubmitState();
        }

        private void cbxDomainIsolated_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxDomainIsolated.Checked)
            {
                cboComponentType.SelectedValue = "webpart";
                cboComponentType.Enabled = false;
                lblIsDomainIsolatedInfo.Visible = false;
            }
            else
            {
                cboComponentType.Enabled = true;
                lblIsDomainIsolatedInfo.Visible = true;
            }
            SetCommandText();
            SetSubmitState();
        }

        private void SetCommandText()
		{
			commandValid = Utility.SetProjectCommand(SolutionName, Framework, ComponentName,
																				ComponentDescription, ComponentType, ExtensionType,
																				Environment, SkipFeatureDeployment, SkipInstall, PlusBeta, PackageManager, DomainIsolated, out commandString);

			txtCommandString.Text = commandString;

            if (commandValid == true)
            {
                btnGenerate.BackColor = Color.Green;
                btnGenerate.ForeColor = Color.White;
            }
		}

		protected void SetSubmitState()
		{
			btnGenerate.Enabled = commandValid;
            if (commandValid ==  true)
            {
                btnGenerate.BackColor = Color.Green;
                btnGenerate.ForeColor = Color.White;
            }
		}

		private void Generate_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void Cancel_Click(object sender, EventArgs e)
		{
			formCancel = true;
			this.Close();
		}

		#endregion


		#region Properties

		public string SolutionName
		{
			get { return txtSolutionName.Text; }
			set { txtSolutionName.Text = value; }
		}
		public string Framework
		{
			get { return ((KeyValuePair<string, string>)cboFramework.SelectedItem).Key; }
			private set { }
		}
		public string Environment
		{
			get { return ((KeyValuePair<string, string>)cboEnvironment.SelectedItem).Key; }
			private set { }
		}
		public string ComponentName
		{
			get { return txtComponentName.Text; }
			private set { }
		}
		public string ComponentDescription
		{
			get { return txtComponentDescription.Text; }
			private set { }
		}
		public string ComponentType
		{
			get { return ((KeyValuePair<string, string>)cboComponentType.SelectedItem).Key; }
			private set { }
		}
		public string ExtensionType
		{
			get { return (ComponentType == "extension") ? ((KeyValuePair<string, string>)cboExtensionType.SelectedItem).Key : String.Empty; }
			private set { }
		}
		public bool SkipFeatureDeployment
		{
			get { return cbxSkipFeatureDeployment.Checked; }
			private set { }
		}
		public bool SkipInstall
		{
			get { return cbxSkipInstall.Checked; }
			private set { }
		}

        public bool PlusBeta
        {
            get { return cbxPlusBeta.Checked; }
            private set { }
        }

		public bool ShowWindow
		{
			get { return cbxShowWindow.Checked; }
			private set { }
		}
		public string CommandString
		{
			get { return commandString; }
			private set { }
		}
		public bool FormCancel
		{
			get { return formCancel; }
			private set { }
		}

        public string PackageManager
        {
            get { return ((KeyValuePair<string, string>)cboPackageManager.SelectedItem).Key; }
            private set { }
        }

        public bool DomainIsolated
        {
            get { return cbxDomainIsolated.Checked; }
            private set { }
        }

        #endregion


    }
}
