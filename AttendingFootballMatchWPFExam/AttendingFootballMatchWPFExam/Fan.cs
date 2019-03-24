using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendingFootballMatchWPFExam
{
    public class Fan
    {
        private bool _hasTicket;
        private int _numberOfSector;
        private int _numberOfPlace;
        private bool _tookThePlace;

        public bool Hasticket { get => _hasTicket; }   
        public bool TookThePlace { get => _tookThePlace; set => _tookThePlace = value; }
        public int NumberOfSector { get => _numberOfSector; set => _numberOfSector = value; }
        public int NumberOfPlace { get => _numberOfPlace; set => _numberOfPlace = value; }

        public Fan(bool a_has_ticket)
        {
            _hasTicket = a_has_ticket;
            NumberOfSector = 0;
            NumberOfPlace = 0;
            _tookThePlace = false;
        }
    }
}
