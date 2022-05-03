using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    /// <summary>
    /// CREATOR: Steve C
    /// Created: 2022/04/20
    /// Data Object class for an Mtr Hop
    /// </summary>
    public class MtrHop
    {
        public int MtrHopID { get; set; }
        public decimal PacketLoss { get; set; }
        public byte PacketsSent { get; set; }
        public decimal LastPingMS { get; set; }
        public decimal AvgPingMS { get; set; }
        public decimal BestPingMS { get; set; }
        public decimal WorstPingMS { get; set; }
        public decimal StDev { get; set; }
        public string? Host { get; set; }
        public byte HopNum { get; set; }


        override
        public string ToString()
        {
            
            string hopNumAndHost = ensureAlignedToString();
            return hopNumAndHost + $"Packet Loss:{PacketLoss}%\tAverage:{AvgPingMS}\tWorst:{WorstPingMS}";
        }

        private string ensureAlignedToString()
        {
            string hopNum = HopNum.ToString();
            if (hopNum.Length == 1)
            {
                hopNum += "   ";
            }
            else
            {
                hopNum += "  ";
            }
            string hopNumAndHost = hopNum + Host + "\n    ";
            
            return hopNumAndHost;
        }
    }
}