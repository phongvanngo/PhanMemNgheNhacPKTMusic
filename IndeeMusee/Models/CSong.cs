using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndeeMusee.Models
{
    public class CSong
    {
        private string path;
        private string name;
        private string singer;
        public string Path { get => path; set => path = value; }
        public string Name { get => name; set => name = value; }
        public string Singer { get => singer; set => singer = value; }

        public CSong(string path, string name, string singer)
        {
            Path = path;
            Name = name;
            Singer = singer;
        }
    }
}
