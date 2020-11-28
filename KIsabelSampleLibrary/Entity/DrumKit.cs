using System;
using System.Collections.Generic;
using System.Text;

namespace KIsabelSampleLibrary.Entity
{
    public class DrumKit
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string IdName { 
            get
            {
                return Id + " - " + Name;
            }

            
        }

        public string Slot0 { get; set; }
        public string Slot1 { get; set; }
        public string Slot2 { get; set; }
        public string Slot3 { get; set; }
        public string Slot4 { get; set; }
        public string Slot5 { get; set; }
        public string Slot6 { get; set; }
        public string Slot7 { get; set; }
        public string Slot8 { get; set; }
        public string Slot9 { get; set; }
        public string Slot10 { get; set; }
        public string Slot11 { get; set; }
        public string Slot12 { get; set; }
        public string Slot13 { get; set; }
        public string Slot14 { get; set; }
        public string Slot15 { get; set; }
    }
}
