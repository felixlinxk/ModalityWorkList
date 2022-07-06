using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModalityWorkList;

namespace Testing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Patient patient = new Patient("Animal^Owner", "66666666", "20220706", "200000", "US", "LXK")
            {
                PatientSex = "M",
                PatientBirthDate = "19950101",
                AccessionNumber = "20220706001",
                OperatorsName = "LXK",
                NameOfPhysiciansReadingStudy = "LXK",
                PatientWeight = 50,
                AdditionalPatientHistory = "Seizure",
                ScheduledProcedureStepDescription = "ABD",
                RequestedProcedureDescription = "ABD"
            };
            ModalityWorkList.ModalityWorklistProvider.CreateWorklistForUS(patient);
        }
    }
}
