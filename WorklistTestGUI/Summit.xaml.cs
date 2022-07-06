using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ModalityWorkList;

namespace WorklistTestGUI
{
    /// <summary>
    /// Summit.xaml 的交互逻辑
    /// </summary>
    public partial class Summit : Window
    {

        public string SummitPatientName { get; set; }
        public string SummitPatientID { get; set; }
        public string SummitAccessionNumber { get; set; }
        public string SummitDescription { get; set; }
        public string SummitPatientBirthday { get; set; }
        public double SummitPatientWeight { get; set; }
        public string SummitPatientSex { get; set; }
        public string SummitRefferringPhysicianName { get; set; }
        public string SummitOperatorsName { get; set; }
        public string SummitNameOfPhysiciansReadingStudy { get; set; }
        public string SummitHistory { get; set; }
        public DateTime SummitScheduleDate { get; set; } = DateTime.Now;
        public string SummitScheduleTimeHour { get; set; } = DateTime.Now.Hour.ToString().PadLeft(2, '0');
        public string SummitScheduleTimeMinute { get; set; } = DateTime.Now.Minute.ToString().PadLeft(2, '0');
        public List<string> SummitModalities { get; set; } = new List<string> { };

        public Summit()
        {
            InitializeComponent();
            DatePicker.DisplayDate = DateTime.Today;
            DatePicker.SelectedDate = DateTime.Today;
            ScheduleTime_Hour.Text = SummitScheduleTimeHour;
            ScheduleTime_Min.Text = SummitScheduleTimeMinute;
        }


        private void Summit_Click(object sender, RoutedEventArgs e)
        {
            Modalities_Check(sender, e);
            if (InputName.Text == string.Empty || InputID.Text == string.Empty || InputReferringPhysicianName.Text == String.Empty || SummitModalities.Count == 0)
            {
                MessageBox.Show("姓名、病历号、预约日期、预约时间、影像设备、处方医生为必填项！", "提示", MessageBoxButton.OK);
            }
            else
            {
                string yyyymmdd = SummitScheduleDate.Year.ToString() + SummitScheduleDate.Month.ToString().PadLeft(2, '0') + SummitScheduleDate.Day.ToString().PadLeft(2, '0');
                string hhmmss = SummitScheduleTimeHour + SummitScheduleTimeMinute + "00";
                foreach (string Modality in SummitModalities)
                {
                    Patient patient = new Patient(SummitPatientName, SummitPatientID, yyyymmdd, hhmmss, Modality, SummitRefferringPhysicianName)
                    {
                        AccessionNumber = SummitAccessionNumber,
                        ScheduledProcedureStepDescription = SummitDescription,
                        RequestedProcedureDescription = SummitDescription,
                        PatientBirthDate = SummitPatientBirthday,
                        PatientWeight = SummitPatientWeight,
                        PatientSex = SummitPatientSex,
                        OperatorsName = SummitOperatorsName,
                        AdditionalPatientHistory = SummitHistory
                    };
                    //if (Modality == "US") ModalityWorklistProvider.CreateWorklistForUS(patient);
                    //if (Modality == "MR" || Modality == "CT") ModalityWorklistProvider.CreateWorklistForMRCT(patient);
                    //if (Modality == "DX") ModalityWorklistProvider.CreateWorklistForDX(patient);
                }
                this.Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Modalities_Check(object sender, RoutedEventArgs e)
        {
            SummitModalities.Clear();
            foreach (CheckBox box in ModalitiesSelection.Children)
            {
                if (box.IsChecked == true)
                {
                    SummitModalities.Add(box.Name);
                }
            }
        }

        private void Date_Click(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(Today)) { SummitScheduleDate = DateTime.Now; return; };
            if (sender.Equals(Tomorrow)) { SummitScheduleDate = DateTime.Now.AddDays(1); return; };
            if (sender.Equals(TodayPlus2)) { SummitScheduleDate = DateTime.Now.AddDays(2); return; };
            if (sender.Equals(OtherDate) && DatePicker.SelectedDate != null)
            {
                SummitScheduleDate = DatePicker.SelectedDate ?? DateTime.Now; return;
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DatePicker.SelectedDate < DateTime.Today)
            {
                DatePicker.SelectedDate = DateTime.Today;
                DatePicker.DisplayDate = DateTime.Today;
            }
            SummitScheduleDate = DatePicker.SelectedDate ?? DateTime.Today;
            OtherDate.IsChecked = true;

        }
    }
}
