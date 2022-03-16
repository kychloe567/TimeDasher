using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Models
{
    public class DrawPriority : IComparable
    {
        public enum PriorityType
        {
            Default, Top, Bottom, Custom
        };

        private DrawPriority()
        {
            Type = PriorityType.Default;
        }

        public PriorityType Type { get; set; }
        private int CustomPriority { get; set; }


        public static DrawPriority Top
        {
            get { return new DrawPriority() { Type = PriorityType.Top }; }
        }
        public static DrawPriority Bottom
        {
            get { return new DrawPriority() { Type = PriorityType.Bottom }; }
        }

        public static DrawPriority Custom(int custom)
        {
            return new DrawPriority() { Type = PriorityType.Custom, CustomPriority = custom };
        }

        public static DrawPriority Default
        {
            get { return new DrawPriority(); }
        }

        /// <summary>
        /// Bottom < Default < Custom < Top
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj != null && obj is DrawPriority dp)
            {
                if (dp.Type == PriorityType.Top)
                {
                    if (Type == PriorityType.Top)
                        return 0;
                    else
                        return -1;
                }
                else if (dp.Type == PriorityType.Bottom)
                {
                    if (Type == PriorityType.Bottom)
                        return 0;
                    else
                        return 1;
                }
                else if (dp.Type == PriorityType.Custom)
                {
                    if (Type == PriorityType.Top)
                        return 1;
                    else if (Type == PriorityType.Custom)
                    {
                        if (CustomPriority > dp.CustomPriority)
                            return 1;
                        else if (CustomPriority < dp.CustomPriority)
                            return -1;
                        else
                            return 0;
                    }
                    else
                        return -1;
                }
                else
                {
                    if (Type == PriorityType.Default)
                        return 0;
                    else if (Type == PriorityType.Bottom)
                        return -1;
                    else
                        return 1;
                }
            }
            else
                return 1;
        }
    }
}
