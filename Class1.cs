using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;

namespace Scan_log_lib
{

    //Интерфейс для класс IMyClass
    [Guid("7213FA50-F0D5-4030-9274-29C3DA5EE3DC")]
    public interface IMyClass
    {        
        [DispId(1)]
        List<string> FindStreamReader(string file);             
        [DispId(2)]
        string Help { get; }        
        [DispId(3)]
        string RegexString { get; set; }        
        [DispId(4)]
        long IDBlock { get; set; }        
        [DispId(5)]
        long TimeWork { get; }
        [DispId(5)]
        List<string> ListLine { get;}

        //[DispId(3)]
        //string FindRegByte(string file);  
    }

    //Интерфейс для событий 
    [Guid("A3210251-3879-4EE8-B94E-3266A6934CC2"),
    InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IMyEvents
    
    { 
    }

    [Guid("E43C7DB7-C22E-45E0-BE1E-67B9CB733138"),
    ClassInterface(ClassInterfaceType.None),
    ComSourceInterfaces(typeof(IMyEvents))]
    [ComVisible(true)]


    //regasm Scan_log_lib.dll /tlb:Scan_log_lib.tlb регистрация описания открытых типов

    public class MyClass : IMyClass
    {
        //field
        private string regStr = string.Empty; 
        private long Iblock;
        private long timeWork;        

        public MyClass(){}        
        //property
        public string Help// Справочник
        {
            get
            {
                return "************************************\r\n"+
                       "RegexString(свойство {get,set}) - искомое выражение\r\n" +
                       "IBlock(свойство {get,set}) - ID блока найденной строки\r\n" +
                       "ListLine(свойство {get}) - Массив найденых строк\r\n" +
                       "TimeWork(свойство {get}) - вернет затраченное время работы метода\r\n" +
                       "FindStreamReader(file List<string>) - метод поиска выражений. Возвращает найденную строку. Может использовать свойство PosLine. Не работает с занятым файлом!\r\n" +
                       "************************************";
            }
        }
        public string RegexString// Регулярное выражение
        {
            get
            {
                return regStr;
            }
            set
            {
                regStr = value;
            }
        }
        public long IDBlock// Позиция в файле
        {
            get
            {
                return Iblock;
            }
            set
            {
                Iblock = value;
            }
        }        
        public long TimeWork// Возвращает затраченое время работы метода
        {
            get
            {
                return timeWork;
            }
            protected set 
            {
                timeWork = value;
            }
        }
        public List<string> ListLine { protected set; get; }//Массив найденых строк


        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        public List<string> FindStreamReader(string fScan)
        {            
            sw.Start();
            Regex reg = new Regex(RegexString);
            string Curline = string.Empty;
            ListLine = new List<string>();
            using (FileStream fs = new FileStream(fScan, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader StrRead = new StreamReader(fs))
                {
                    if (IDBlock > 0)
                    {
                        if (IDBlock > 1024)
                            StrRead.BaseStream.Position = IDBlock - 1024;//Position set!!!
                        while ((Curline = StrRead.ReadLine()) != null)
                        {
                            Match match = reg.Match(Curline);
                            if (match.Success)
                            {
                                ListLine.Add(Curline);
                                IDBlock = StrRead.BaseStream.Position;
                            }
                        }
                    }
                    else
                    {
                        while ((Curline = StrRead.ReadLine()) != null)
                        {
                            Match match = reg.Match(Curline);
                            if (match.Success)
                            {
                                ListLine.Add(Curline);
                                IDBlock = StrRead.BaseStream.Position;
                            }
                        }
                    }
                }
            }
            sw.Stop();
            TimeWork = sw.ElapsedMilliseconds;
            return ListLine;
        }        
    }   
}
