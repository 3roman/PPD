using Microsoft.Win32;
using Mvvm;
using Mvvm.Commands;
using PipePressureDrop.Model;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PipePressureDrop.Utility;

namespace PipePressureDrop.ViewModel
{
    class ViewModelMain : BindableBase
    {
        public Pipeline Pipe { get; set; } = new Pipeline();
        public ObservableCollection<Report> MyReport { get; set; } = new ObservableCollection<Report>();
        public ICommand CalculateCommand { get; }
        public ICommand ExportReportCommand { get; }
        public ICommand HelpCommand { get; }


        public ViewModelMain()
        {
            CalculateCommand = new DelegateCommand(Calculate);
            ExportReportCommand = new DelegateCommand(ExportReport, CanExportReport);
            HelpCommand = new DelegateCommand(Help);
        }

        private void Calculate()
        {
            Pipe.CalculatePipePressureDrop();
            GenerateReport();
        }

        public void ExportReport()
        {
            var sfd = new SaveFileDialog
            {
                DefaultExt = @"xlsx",
                Filter = @"Excel文件|*.xlsx",
                FileName = "MyReport",
            };
            if (true == sfd.ShowDialog())
            {
                try
                {
                    ExcelExporter.ListToExcel(sfd.FileName, MyReport);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        private bool CanExportReport()
        {
            return null != MyReport;
        }

        private void Help()
        {
            System.Diagnostics.Process.Start("iexplore.exe",
                "https://github.com/lim42snec/PipePressureDrop/wiki");
        }

        private void GenerateReport()
        {
            MyReport.Clear();
            MyReport.Add(new Report { Item = "***Design Input***", Value = "", Unit = "" });
            MyReport.Add(new Report {Item="MassFlow", Value=Pipe.MassFlow.ToString(), Unit="kg/s" });
            MyReport.Add(new Report {Item="Density", Value=Pipe.Density.ToString(), Unit="kg/m³" });
            MyReport.Add(new Report {Item="DynamicViscosity", Value=Pipe.Viscosity.ToString(), Unit="Pa.s" });
            MyReport.Add(new Report {Item="InnerDiameter", Value=Pipe.InnerDiameter.ToString(), Unit="m" });
            MyReport.Add(new Report {Item="AbsoluteRoughness", Value=Pipe.AbsoluteRoughness.ToString(), Unit="m" });
            MyReport.Add(new Report {Item="MarginFactor", Value=Pipe.MarginFactor.ToString(), Unit="" });
            MyReport.Add(new Report {Item="PipeLength", Value=Pipe.PipeLength.ToString(), Unit="m" });
            MyReport.Add(new Report {Item="ElevationChange", Value=Pipe.ElevationChange.ToString(), Unit="m" });
            MyReport.Add(new Report {Item="ElbowAndTee", Value=Pipe.ElbowAndTee.ToString(), Unit="pcs." });
            MyReport.Add(new Report {Item="GlobeValve", Value=Pipe.GlobeValve.ToString(), Unit="pcs." });
            MyReport.Add(new Report {Item="CheckValve", Value=Pipe.CheckValve.ToString(), Unit="pcs." });
            MyReport.Add(new Report {Item="", Value="", Unit="" });

            MyReport.Add(new Report {Item="***Report***", Value="", Unit="" });
            MyReport.Add(new Report {Item="PressureDrop", Value=Math.Round(Pipe.FrictionResistance, 3).ToString(), Unit="kPa" });
            MyReport.Add(new Report {Item="Velocity", Value=Math.Round(Pipe.Velocity, 1).ToString(), Unit="m/s" });
            MyReport.Add(new Report {Item="ReynoldsNumber", Value=Math.Round(Pipe.ReynoldsNumber).ToString(), Unit="" });
            MyReport.Add(new Report {Item="FrictionFactor", Value=Math.Round(Pipe.FrictionFactor, 3).ToString(), Unit="" });
            MyReport.Add(new Report {Item="EquivalentLength", Value=Math.Round(Pipe.FittingEquivalentLength, 1).ToString(), Unit="m" });
            MyReport.Add(new Report {Item="TotalLength", Value=Math.Round(Pipe.FittingEquivalentLength+Pipe.PipeLength, 1).ToString(), Unit="m" });
           
        }
    }
}