using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Station
    {
        private int id;
        private string name;

        public Station(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public int getId()
        {
            return id;
        }
        public string getName() {
            return name;
        }

        public static bool operator== (Station a, Station b)
        {
            return a.id == b.id;
        }

        public static bool operator !=(Station a, Station b)
        {
            return !(a == b);
        }

        public string toString()
        {
            return "Name: " + name + "\nID: " + id;
        }
    }
}
