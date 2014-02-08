﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace CrmCodeGenerator.VSPackage.Model
{
    public class Settings : INotifyPropertyChanged
    {
        public Settings()
        {
            EntityList = new ObservableCollection<string>();
            EntitiesSelected = new ObservableCollection<string>();

            Dirty = false;
        }





        // boiler-plate
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        //protected bool SetField<T>(ref T field, T value, string propertyName)
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        
        private string _OutputPath;
        private string _Namespace;
        private string _EntitiesToIncludeString;
        private string _CrmOrg;
        private string _Password;
        private string _Username;
        private string _Domain;
        private string _CrmSdkUrl;
        private Context _Context;
        private string _Template;
        private string _T4Path;
        private string _ProjectName;
        public string ProjectName
        {
            get
            {
                return _ProjectName;
            }
            set
            {
                SetField(ref _ProjectName, value);
            }
        }
        public string T4Path
        {
            get
            {
                return _T4Path;
            }
            set
            {
                SetField(ref _T4Path, value);
            }
        }
        public string Template
        {
            get
            {
                return _Template;
            }
            set
            {
                SetField(ref _Template, value);
                NewTemplate = !System.IO.File.Exists(System.IO.Path.Combine(_Folder, _Template));
            }
        }
        private string _Folder = "";
        public string Folder
        {
            get
            {
                return _Folder;
            }
            set
            {
                SetField(ref _Folder, value);
            }
        }

        private bool _NewTemplate;
        public bool NewTemplate
        {
            get
            {
                return _NewTemplate;
            }
            set
            {
                SetField(ref _NewTemplate, value);
            }
        }
        public string OutputPath
        {
            get
            {
                return _OutputPath;
            }
            set
            {
                SetField(ref _OutputPath, value);
            }
        }

        public Context Context
        {
            get
            {
                return _Context;
            }
            set
            {
                SetField(ref _Context, value);
            }
        }

        // specify the connection details or specify the crmconnection property
        public string CrmSdkUrl
        {
            get
            {
                return _CrmSdkUrl;
            }
            set
            {
                SetField(ref _CrmSdkUrl, value);
            }
        }
        public string Domain
        {
            get
            {
                return _Domain;
            }
            set
            {
                SetField(ref _Domain, value);
            }
        }
        public string Username
        {
            get
            {
                return _Username;
            }
            set
            {
                SetField(ref _Username, value);
            }
        }
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                SetField(ref _Password, value);
            }
        }
        public string CrmOrg
        {
            get
            {
                return _CrmOrg;
            }
            set
            {
                SetField(ref _CrmOrg, value);
            }
        }

        private ObservableCollection<String> _OrgList = new ObservableCollection<String>();
        public ObservableCollection<String> OrgList
        {
            get
            {
                return _OrgList;
            }
            set
            {
                SetField(ref _OrgList, value);
            }
        }
        private ObservableCollection<String> _TemplateList = new ObservableCollection<String>();
        public ObservableCollection<String> TemplateList
        {
            get
            {
                return _TemplateList;
            }
            set
            {
                SetField(ref _TemplateList, value);
            }
        }


        public IOrganizationService CrmConnection { get; set; }

        //public string[] EntitiesToExclude { get; set; }
        //public string[] EntitiesToInclude { get; set; }

        public string EntitiesToIncludeString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var value in _EntitiesSelected)
                {
                    if(sb.Length != 0)
                        sb.Append(',');
                    sb.Append(value);
                }
                return sb.ToString();
                //return _EntitiesToIncludeString;
            }
            set
            {
                var newList = new ObservableCollection<string>();
                var split = value.Split(',').Select(p => p.Trim()).ToList(); 
                foreach (var s in split)
                {
                    newList.Add(s);
                    if (!_EntityList.Contains(s))
                        _EntityList.Add(s);
                }
                EntitiesSelected = newList;
                SetField(ref _EntitiesToIncludeString, value);
                OnPropertyChanged("EnableExclude");
            }
        }
        //public bool EnableExclude
        //{
        //    get
        //    {
        //        return String.IsNullOrWhiteSpace(_EntitiesToIncludeString);
        //    }
        //}
        //public string EntitiesToExcludeString
        //{
        //    get
        //    {
        //        return _EntitiesToExcludeString;
        //    }
        //    set
        //    {
        //        SetField(ref _EntitiesToExcludeString, value);
        //    }
        //}

        public ObservableCollection<string> _EntityList;
        public ObservableCollection<string> EntityList
        {
            get
            {
                return _EntityList;
            }
            set
            {
                SetField(ref _EntityList, value);
            }
        }

        public ObservableCollection<string> _EntitiesSelected;
        public ObservableCollection<string> EntitiesSelected
        {
            get
            {
                return _EntitiesSelected;
            }
            set
            {
                SetField(ref _EntitiesSelected, value);
            }
        }

        public string Namespace
        {
            get
            {
                return _Namespace;
            }
            set
            {
                SetField(ref _Namespace, value);
            }
        }

        public bool Dirty { get; set; }


    }
}