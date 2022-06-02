using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metereologic
{
    public class Station
    {

        #region Properties

        private string id;
        private string name;
        private Dictionary<DateTime, Meteorology> meteorologyData = new Dictionary<DateTime, Meteorology>();

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Dictionary<DateTime, Meteorology> MeteorologyData
        {
            get { return meteorologyData; }
            set { meteorologyData = value; }
        }

        #endregion

        #region Constructors

        public Station()
        {
            this.id = string.Empty;
            this.name = string.Empty;
        }

        public Station(string id, string name) 
            : this()
        {
            this.id = id;
            this.name = name;
        }

        #endregion


        #region Methods

        public bool AddMeteorologicData(Meteorology meteorology, DateTime date)
        {
            if (HasMeteorologicDataForDate(date))
            {
                return false;
            }

            meteorologyData.Add(date, meteorology);
            return true;
        }

        private bool HasMeteorologicDataForDate(DateTime date)
        {
            return meteorologyData.ContainsKey(date);
        }

        public override string ToString()
        {
            string result = string.Empty;

            result += "Name: " + name + "\r\n";
            result += "ID: " + id + "\r\n";
            result += "Total metereologic records: " + meteorologyData.Count + "\r\n\r\n";

            foreach (KeyValuePair<DateTime, Meteorology> meteorology in meteorologyData)
            {
                result += "DateTime: " + meteorology.Key.ToString("yyyy-MM-dd HH:mm") + "\r\n";
                result += meteorology.Value.ToString() + "\r\n";
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            return Id.Equals(((Station)obj).Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #endregion
    }
}
