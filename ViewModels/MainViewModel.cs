using HandyControl.Controls;
using Live2DCrack.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;



namespace Live2DCrack.ViewModels
{
    internal class MainViewModel : ObservableObject
    {
        private readonly SoftwareInfo softwareInfo = new();


        public string Path
        {
            get => softwareInfo.Path;
            set => SetProperty(softwareInfo.Path, value, softwareInfo, (obj, val) => obj.Path = val);
        }


        public string Version
        {
            get => softwareInfo.Version;
            set => SetProperty(softwareInfo.Version, value, softwareInfo, (obj, val) => obj.Version = val);
        }

        public ICommand WriteResourceCommand { get; }
        public ICommand OpenFolderSelectoryCommand { get; }
        public ICommand CopyDownloadUrlCommand { get; }

        public MainViewModel()
        {
            WriteResourceCommand = new RelayCommand(WriteResource);
            OpenFolderSelectoryCommand = new RelayCommand(OpenFolderSelector);
            CopyDownloadUrlCommand = new RelayCommand(CopyDownloadUrl);
            Path = Utils.GetLive2DInstallPath();
            if (string.Empty != Path)
            {
                if (Utils.GetLive2DVersion(softwareInfo.Path, out string version))
                {
                    Version = version;
                }

            }
            else
            {
                Path = "Failed to get path";
                Version = "Unkown version";
                Growl.Error("The software installation path was not obtained,make sure you downloaded you downloaded Live2D, turn of anti-virus or use administrator to run");
            }
        }

        public void WriteResource()
        {
            if (Directory.Exists(Path))
            {
                if (Utils.WriteResource(Path + @"\app\lib\rlm1221.jar"))
                {
                    Growl.Success("Success");
                }
                else
                {
                    Growl.Error("Failed,please turn off anti-virus or use administrator to run");
                }
            }
            else
            {
                Growl.Error("The software installation directory was not obtained");
            }
        }

        private void OpenFolderSelector()
        {
            using (System.Windows.Forms.FolderBrowserDialog dialog = new())
            {
                dialog.Description = "Choose Live 2D installation path";
                dialog.AutoUpgradeEnabled = true;
                var result = dialog.ShowDialog();
                if (System.Windows.Forms.DialogResult.OK == result)
                {
                    if (Directory.Exists(dialog.SelectedPath))
                    {
                        if (Utils.GetLive2DVersion(dialog.SelectedPath, out string version))
                        {
                            Path = dialog.SelectedPath;
                            Version = version;
                        }
                        else
                        {
                            Growl.Error("The Live2D version has not been obtained. It may be that the selected path is incorrect.");
                        }

                    }

                }
            }

        }

        public void CopyDownloadUrl()
        {
            try
            {
                Clipboard.SetDataObject($"https://www.live2d.com/en/cubism/download/editor_dl/?url=https%3A%2F%2Fcubism.live2d.com%2Feditor%2Fbin%2FLive2D_Cubism_Setup_4.1.03_1_en.exe");
                Growl.Success("Copy success,use your browser to download");
            }
            catch (Exception e)
            {
                Growl.Success($"Copy failed{e.Message}");
            }

        }

    }
}
