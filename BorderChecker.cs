using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCustomGridLibrary
{
    class BorderChecker
    {
        public bool IsRightLimit(double sizeWantToUse, double fullSize)
        {
            return sizeWantToUse > fullSize;
        }

        public bool IsLeftLimit(double sizeWantToUse, double fullSize)
        {
            return sizeWantToUse > fullSize;
        }

        public bool IsTopLimit(double sizeWantToUse, double fullSize)
        {
            return sizeWantToUse > fullSize;
        }
    }
}
