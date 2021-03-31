using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using StandaloneSDKDemo.Helper;

namespace StandaloneSDKDemo
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            InputArguments arguments = new InputArguments(args);

            String inoutCSV = "a.csv";
            String ipAddress = "192.168.1.222";
            String port = "4370";
            String fileUser = "user.csv";
            String YearMonth = "";
            String deleteEmployee = "";
            String getFP = "";
            String FingerIdx = "";
            String emplID = "";
            String setFingerP = "";
            String inoutFP = "";

            if (args.Length == 0)
            {
                Console.Out.WriteLine("exe -inout name.csv -ip 192.168.1.225 -port 4370 -user user.csv -yearmonth 201905");
                Console.Out.WriteLine("exe -ip 192.168.1.225 -port 4370 -deleteEmployee 1234");
                Console.Out.WriteLine("exe -ip 192.168.1.225 -port 4370 -getFP 15 -FingerIdx 7 -inoutFP fp.txt");
                Console.Out.WriteLine("exe -ip 192.168.1.225 -port 4370 -setFingerP 15 -FingerIdx 7 -inoutFP fp15.txt");
                return;
            }
            
            inoutCSV = arguments["inout"];
            
            if (arguments["ip"] != null)
            {
                ipAddress = arguments["ip"];
            }
            if (arguments["port"] != null)
            {
                port = arguments["port"];
            }

            if (arguments["yearmonth"] != null)
            {
                YearMonth = arguments["yearmonth"];
            }

            if (arguments["deleteEmployee"] != null)
            {
                deleteEmployee = arguments["deleteEmployee"];
            }

            if (arguments["getFP"] != null)
            {
                getFP = arguments["getFP"];
            }
            if (arguments["setFingerP"] != null)
            {
                setFingerP = arguments["setFingerP"];
            }
            
            if (arguments["FingerIdx"] != null)
            {
                FingerIdx = arguments["FingerIdx"];
            }

            if (arguments["inoutFP"] != null)
            {
                inoutFP = arguments["inoutFP"];
            }
            
            fileUser = arguments["user"];
            
            CommandLine cmd = new CommandLine();
            cmd.YearMonth = YearMonth;

            Console.Out.WriteLine(cmd.getVersion());
            cmd.connectDevice(ipAddress, port, "0");


            //DataTable dt;
            //dt = cmd.readAttendantInOut("2018-02-17 00:00:00", "2019-04-18 00:00:00");
            //dt = cmd.readAllAttendantInOut();
            if (inoutCSV != null)
            {
                cmd.readAllAttendantInOut2CSV(inoutCSV);
            }

            if (fileUser != null)
            {
                cmd.readAllUsers2CSV(fileUser);
            }

            if (deleteEmployee != "")
            {
                cmd.deleteEmployeeId(deleteEmployee);
            }

            if (getFP != "" && inoutFP != "")
            {
                int iFingerIdx = 0;
                if (int.TryParse(FingerIdx, out iFingerIdx))
                {
                    String s = cmd.getFP(getFP, iFingerIdx, inoutFP);
                    Console.Out.WriteLine(s);
                }
            }
            else if (setFingerP != "" && inoutFP != "")
            {
                int iFingerIdx = 0;
                String sFP = cmd.getAllText(inoutFP);

                if (int.TryParse(FingerIdx, out iFingerIdx))
                {
                    bool b = cmd.setFP(setFingerP, iFingerIdx, sFP, 1);
                    if (b) Console.Out.WriteLine("Set Finger Print OK");
                }
            }

            
            cmd.CloseSDK();

            List<String> errs = cmd.getSystemOutput();

            foreach (String s in errs)
            {
                Console.Out.WriteLine(s);
            }
            //cmd.connectDevice();
        }
    }
}
