using MyNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetModels.Models
{
    public class Insured
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
    }
    public class PolicyWizard : Root
    {
        public string Policy { get; set; }
        public string PolicyName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string AgentCode { get; set; }
        public int id { get; set; }
        public string InsurerCode { get; set; }
        public string Comments { get; set; }
        public string PaymentType { get; set; }


    }

    public class Policy:Root
    {
        public string PolicyNumber { get; set; }
        public string PolicyName { get; set; }
        public string Manager { get; set; }
        public string StartDate { get; set; }
        public string ApplicableTo { get; set; }
        public string ResidentType { get; set; }
        public string elder { get; set; }
        public string PolicyType { get; set; }
        public string comments { get; set; }
        public string MinPremium { get; set; }
        public string SettlementDays { get; set; }
        public int id { get; set; }
    }

    public class Hospital:Root
    {
        public string HospitalName { get; set; }
        public decimal Percentage { get; set; } = 0;
        public string Address { get; set; }
        public string Representive { get; set; }
        public string EffectiveDate { get; set; }
        public string HospitalType { get; set; }
        public string Comments { get; set; }
        public string HospitalCode { get; set; }
        public string TerminationDate { get; set; }

        public int id { get; set; }

    }

    public class Customer:Root
    {
        public string CustomerCode { get; set; }
        public string Address { get; set; }
        public string Comments { get; set; }
        public string DOB { get; set; }
        public string DOJ { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string RiskPer { get; set; } = "0.00";
        public int id { get; set; }
    }
    public class Agent : Root
    {
        public string AgentCode { get; set; }
        public string Address { get; set; }
        public string Comments { get; set; }
        public string DOB { get; set; }
        public string DOJ { get; set; }
        public string TerminationDate { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string CommPer { get; set; } = "0.00";
        public int id { get; set; }
    }

    public class Transactions : Root
    {
        public string TransactionNumber { get; set; }
        public Decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public string RollbackTransaction { get; set; }
        public string EffectiveDate { get; set; }
        public string Policy { get; set; }
        public bool isprocessed { get; set; }
        public string isinternal { get; set; }
    }
    public class Run : Root
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

}
