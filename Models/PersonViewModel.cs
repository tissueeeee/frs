using System.ComponentModel.DataAnnotations;

namespace FR_HKVision.Models
{

    public class PersonViewModel
    {
        public List<Person> Persons { get; set; }


    }

    public class Person
    {
        public int StudentSeq { get; set; }
        public string StudentName { get; set; }
        public string StudentNumber { get; set; }

        public string StudentPhoto { get; set; }
        public string ProgrammeName { get; set; }
        public string ProgrammeCode { get; set; }
        public string AttendanceTime { get; set; }
        public string AttendancePhoto { get; set; }

        public string ProfileStatus { get; set; }
        public string ProfileRecord { get; set; }

        public string StudentMessage { get; set; }


    }


}
