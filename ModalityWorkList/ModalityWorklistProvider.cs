using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dicom;
using Renci.SshNet;

namespace ModalityWorkList
{
    public class ModalityWorklistProvider
    {
        public static void CreateWorklist(Patient patient)
        {
            string yyyyMMddHHmmss = DateTime.Now.ToString().Replace(@"/", "").Replace(":", "").Replace(" ", "");
            DicomUID SopClassUID = DicomUID.Generate();
            DicomUID SopInstanceUID = DicomUID.Generate();
            DicomDataset scheduledInfo = new DicomDataset();
            scheduledInfo.Add(DicomTag.ScheduledStationAETitle, patient.ScheduledStationAETitle);
            scheduledInfo.Add(DicomTag.ScheduledProcedureStepStartDate, patient.ScheduledProcedureStepStartDate);
            scheduledInfo.Add(DicomTag.ScheduledProcedureStepStartTime, patient.ScheduledProcedureStepStartTime);
            scheduledInfo.Add(DicomTag.Modality, patient.Modality);
            scheduledInfo.Add(DicomTag.ScheduledPerformingPhysicianName, patient.ScheduledPerformingPhysicianName);
            scheduledInfo.Add(DicomTag.ScheduledProcedureStepDescription, patient.ScheduledProcedureStepDescription ?? "UNKNOWN");
            scheduledInfo.Add(DicomTag.ScheduledStationName, patient.ScheduledStationName);
            scheduledInfo.Add(DicomTag.ScheduledProcedureStepLocation, patient.ScheduledProcedureStepLocation ?? "UNKNOWN");
            scheduledInfo.Add(DicomTag.ReferencedSOPClassUID, patient.ReferencedSOPClassUID ?? DicomUID.Generate());
            scheduledInfo.Add(DicomTag.ReferencedSOPInstanceUID, patient.ReferencedSOPInstanceUID ?? DicomUID.Generate());
            DicomSequence procedureStepSq = new DicomSequence(DicomTag.ScheduledProcedureStepSequence, scheduledInfo);
            DicomSequence procedureCodeSq = new DicomSequence(DicomTag.ProcedureCodeSequence, scheduledInfo);
            DicomSequence scheduledProtocolCodeSq = new DicomSequence(DicomTag.ScheduledProtocolCodeSequence, scheduledInfo);
            DicomDataset worklistDataSet = new DicomDataset();
            worklistDataSet.Add(DicomTag.ScheduledProcedureStepSequence, procedureStepSq.First());
            worklistDataSet.Add(DicomTag.ProcedureCodeSequence, procedureCodeSq.Items.First());
            worklistDataSet.Add(DicomTag.ScheduledProtocolCodeSequence, scheduledProtocolCodeSq.Items.First());
            worklistDataSet.Add(DicomTag.ReferencedDefinedProtocolSequence, patient.ReferencedDefinedProtocolSequence ?? scheduledInfo);
            worklistDataSet.Add(DicomTag.ReferencedPerformedProtocolSequence, patient.ReferencedPerformedProtocolSequence ?? scheduledInfo);
            worklistDataSet.Add(DicomTag.ScheduledProcedureStepID, Encoding.UTF8, patient.ScheduledProcedureStepID ?? "UNKNOWN");
            worklistDataSet.Add(DicomTag.RequestedProcedureID, Encoding.UTF8, patient.RequestedProcedureID ?? "UNKNOWN");
            worklistDataSet.Add(DicomTag.RequestedProcedureDescription, Encoding.UTF8, patient.RequestedProcedureDescription ?? "NONE");
            worklistDataSet.Add(DicomTag.RequestedProcedureCodeSequence, scheduledInfo);
            worklistDataSet.Add(DicomTag.PreMedication, Encoding.UTF8, patient.PreMedication ?? string.Empty);
            worklistDataSet.Add(DicomTag.RequestedContrastAgent, Encoding.UTF8, patient.RequestedContrastAgent ?? string.Empty);
            worklistDataSet.Add(DicomTag.ContainerIdentifier, Encoding.UTF8, patient.ContainerIdentifier ?? yyyyMMddHHmmss);
            worklistDataSet.Add(DicomTag.ContainerTypeCodeSequence, scheduledInfo);
            worklistDataSet.Add(DicomTag.SpecimenDescriptionSequence, scheduledInfo);
            worklistDataSet.Add(DicomTag.SpecimenIdentifier, yyyyMMddHHmmss);
            worklistDataSet.Add(DicomTag.SpecimenUID, DicomUID.Generate());

            worklistDataSet.Add(DicomTag.StudyInstanceUID, DicomUID.Generate());
            worklistDataSet.Add(DicomTag.StudyDate, DateTime.Now);
            worklistDataSet.Add(DicomTag.StudyTime, DateTime.Now);
            worklistDataSet.Add(DicomTag.ReferencedStudySequence, scheduledInfo);
            worklistDataSet.Add(DicomTag.ReferencedSOPClassUID, DicomUID.Generate());
            worklistDataSet.Add(DicomTag.ReferencedSOPInstanceUID, DicomUID.Generate());
            worklistDataSet.Add(DicomTag.RequestedProcedurePriority, Encoding.UTF8, patient.RequestedProcedurePriority ?? string.Empty);
            worklistDataSet.Add(DicomTag.PatientTransportArrangements, Encoding.UTF8, patient.PatientTransportArrangements ?? string.Empty);
            worklistDataSet.Add(DicomTag.SpecificCharacterSet, "ISO_IR 192");
            worklistDataSet.Add(DicomTag.SOPClassUID, SopClassUID);
            worklistDataSet.Add(DicomTag.SOPInstanceUID, SopInstanceUID);
            worklistDataSet.Add(DicomTag.PatientName, Encoding.UTF8, patient.PatientName);
            worklistDataSet.Add(DicomTag.PatientID, Encoding.UTF8, patient.PatientID);
            worklistDataSet.Add(DicomTag.PatientBirthDate, Encoding.UTF8, patient.PatientBirthDate ?? string.Empty);
            worklistDataSet.Add(DicomTag.PatientSex, Encoding.UTF8, patient.PatientSex ?? "O");
            worklistDataSet.Add(DicomTag.AccessionNumber, Encoding.UTF8, patient.AccessionNumber ?? string.Empty);
            worklistDataSet.Add(DicomTag.ReferringPhysicianName, Encoding.UTF8, patient.ReferringPhysicianName ?? string.Empty);
            worklistDataSet.Add(DicomTag.OperatorsName, Encoding.UTF8, patient.OperatorsName ?? string.Empty);
            worklistDataSet.Add(DicomTag.NameOfPhysiciansReadingStudy, Encoding.UTF8, patient.NameOfPhysiciansReadingStudy ?? string.Empty);
            worklistDataSet.Add(DicomTag.PatientWeight, Encoding.UTF8, patient.PatientWeight > 0 ? patient.PatientWeight : 0.0);
            worklistDataSet.Add(DicomTag.AdditionalPatientHistory, Encoding.UTF8, patient.AdditionalPatientHistory ?? String.Empty);
            worklistDataSet.Add(DicomTag.Allergies, Encoding.UTF8, patient.Allergies ?? String.Empty);
            worklistDataSet.Add(DicomTag.ValueType, "DATATIME");
            worklistDataSet.Add(DicomTag.DateTime, DateTime.Now);
            worklistDataSet.Add(DicomTag.Date, DateTime.Now);
            worklistDataSet.Add(DicomTag.Time, DateTime.Now);

            string tempFolder = Path.Combine(Path.GetTempPath(), nameof(ModalityWorkList));
            //string savePath = @"W:\" + patient.PatientID + "_" + yyyyMMddHHmmss + ".wl";
            string savePath = Path.Combine(tempFolder, patient.PatientID + "_" + yyyyMMddHHmmss + ".wl");
            Directory.CreateDirectory(tempFolder);
            DicomFile worklistFile = new DicomFile(worklistDataSet);
            worklistFile.Save(savePath);

            bool uploadSuccess = SftpUploadOneFile(
                "172.16.2.148",
                "pacs",
                "pacs123698745",
                savePath,
                "/etc/worklists/" + patient.PatientID + "_" + yyyyMMddHHmmss + ".wl");

            if (uploadSuccess != true)
            {
                throw new Exception("failed");
            }
        }

        public static void CreateWorklistForUS(Patient patient)
        {
            string yyyyMMddHHmmss = DateTime.Now.ToString().Replace(@"/", "").Replace(":", "").Replace(" ", "");
            DicomUID SopClassUID = DicomUID.Generate();
            DicomUID SopInstanceUID = DicomUID.Generate();
            DicomDataset worklistDataSet = new DicomDataset();
            worklistDataSet.Add(DicomTag.SOPClassUID, SopClassUID);
            worklistDataSet.Add(DicomTag.SOPInstanceUID, SopInstanceUID);
            worklistDataSet.Add(DicomTag.PatientName, Encoding.UTF8, patient.PatientName);
            worklistDataSet.Add(DicomTag.PatientID, Encoding.UTF8, patient.PatientID);
            worklistDataSet.Add(DicomTag.PatientBirthDate, Encoding.UTF8, patient.PatientBirthDate ?? string.Empty);
            worklistDataSet.Add(DicomTag.PatientSex, Encoding.UTF8, patient.PatientSex ?? "O");
            worklistDataSet.Add(DicomTag.AccessionNumber, Encoding.UTF8, patient.AccessionNumber ?? string.Empty);
            worklistDataSet.Add(DicomTag.ReferringPhysicianName, Encoding.UTF8, patient.ReferringPhysicianName ?? string.Empty);
            worklistDataSet.Add(DicomTag.OperatorsName, Encoding.UTF8, patient.OperatorsName ?? string.Empty);
            worklistDataSet.Add(DicomTag.NameOfPhysiciansReadingStudy, Encoding.UTF8, patient.NameOfPhysiciansReadingStudy ?? string.Empty);
            worklistDataSet.Add(DicomTag.PatientWeight, Encoding.UTF8, patient.PatientWeight > 0 ? patient.PatientWeight : 0.0);
            worklistDataSet.Add(DicomTag.AdditionalPatientHistory, Encoding.UTF8, patient.AdditionalPatientHistory ?? String.Empty);
            worklistDataSet.Add(DicomTag.Allergies, Encoding.UTF8, patient.Allergies ?? String.Empty);
            worklistDataSet.Add(DicomTag.ValueType, "DATATIME");
            worklistDataSet.Add(DicomTag.DateTime, DateTime.Now);
            worklistDataSet.Add(DicomTag.Date, DateTime.Now);
            worklistDataSet.Add(DicomTag.Time, DateTime.Now);

            string tempFolder = Path.Combine(Path.GetTempPath(), nameof(ModalityWorkList));
            string savePath = Path.Combine(tempFolder, patient.PatientID + "_" + yyyyMMddHHmmss + ".wl");
            Directory.CreateDirectory(tempFolder);
            DicomFile worklistFile = new DicomFile(worklistDataSet);
            worklistFile.Save(savePath);

            bool uploadSuccess = SftpUploadOneFile(
                "172.16.2.148",
                "pacs",
                "pacs123698745",
                savePath,
                "/etc/worklists/" + patient.PatientID + "_" + yyyyMMddHHmmss + ".wl");

            if (uploadSuccess != true)
            {
                throw new Exception("failed");
            }
        }

        private const int DefaultPort = 22;

        public static bool ConnectStatus(string host, string name, string password)
        {
            try
            {
                var sshClient = new SshClient(host, DefaultPort, name, password);
                sshClient.Connect();
                sshClient.Disconnect();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SendCommand(string host, string name, string password, string commandstr)
        {
            try
            {
                var sshClient = new SshClient(host, DefaultPort, name, password);
                sshClient.Connect();
                var cmd = sshClient.CreateCommand(commandstr);
                TimeSpan ts = new TimeSpan(0, 0, 0, 10);
                cmd.CommandTimeout = ts;
                var res = cmd.Execute();
                if (cmd.Error.Length > 0)
                {
                    return false;
                }
                sshClient.Disconnect();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SftpUploadOneFile(string host, string username, string password, string localPath, string remotePath)
        {
            try
            {
                var sftpClient = new SftpClient(host, DefaultPort, username, password);
                var filestream = File.OpenRead(localPath);
                sftpClient.Connect();
                sftpClient.UploadFile(filestream, remotePath, true, null);
                sftpClient.Disconnect();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public class Patient
    {
        public Patient(string patientName, string patientID, string scheduledProcedureStepStartDate, string scheduledProcedureStepStartTime, string modality, string referringPhysicianName)
        {
            this.PatientName = patientName;
            this.PatientID = patientID;
            this.ScheduledProcedureStepStartDate = scheduledProcedureStepStartDate;
            this.ScheduledProcedureStepStartTime = scheduledProcedureStepStartTime;
            this.Modality = modality;
            this.ReferringPhysicianName = referringPhysicianName;
            if (modality == "MR") { this.ScheduledStationAETitle = "GEHC"; this.ScheduledStationName = "GEHC"; };
            if (modality == "CT") { this.ScheduledStationAETitle = "ct99"; this.ScheduledStationName = "CT99"; };
            if (modality == "DX") { this.ScheduledStationAETitle = "TERRA_NETWORK"; this.ScheduledStationName = "TERRA_NETWORK"; };
            if (modality == "US") { this.ScheduledStationAETitle = "US"; this.ScheduledStationName = "US"; } ;
        }
        public string ScheduledStationAETitle { get; set; }
        public string ScheduledProcedureStepStartDate { get; set; }
        public string ScheduledProcedureStepStartTime { get; set; }
        public string Modality { get; set; }
        public string ScheduledProcedureStepDescription { get; set; }
        public string ScheduledPerformingPhysicianName { get; set; }
        public string ScheduledStationName { get; set; }
        public string ScheduledProcedureStepLocation { get; set; }
        public DicomDataset ReferencedDefinedProtocolSequence { get; set; }
        public DicomUID ReferencedSOPClassUID { get; set; }
        public DicomUID ReferencedSOPInstanceUID { get; set; }
        public DicomDataset ReferencedPerformedProtocolSequence { get; set; }
        public string RequestedProcedurePriority { get; set; }
        public string PatientTransportArrangements { get; set; }
        public string ScheduledProcedureStepID { get; set; }
        public string PreMedication { get; set; }
        public string RequestedContrastAgent { get; set; }
        public string ContainerIdentifier { get; set; }
        public string RequestedProcedureID { get; set; }
        public string RequestedProcedureDescription { get; set; }
        public string PatientName { get; set; }
        public string PatientDescription { get; set; }
        public string PatientID { get; set; }
        public string PatientSex { get; set; }
        public string PatientBirthDate { get; set; }
        public string AccessionNumber { get; set; }
        public string ReferringPhysicianName { get; set; }
        public string OperatorsName { get; set; }
        public string NameOfPhysiciansReadingStudy { get; set; }
        public double PatientWeight { get; set; }
        public string AdditionalPatientHistory { get; set; }
        public string Allergies { get; set; }
    }
}
