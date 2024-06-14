using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace RadioFrimleyPark.Appz.Models
{

    public class CurrentNext
    {
        public Programmes programmes { set; get; }
    }

    public class Programmes
    {
        public ProgrammeInfo current { set; get; }
        public ProgrammeInfo next { set; get; }
    }

    public class ProgrammeInfo
    {
        public DateTime startTime { set; get; }
        public string name { set; get; }
        public string link { set; get; }
        public int type { set; get; }
    }
}
